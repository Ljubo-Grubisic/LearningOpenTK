using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;
using Lighting.Shadering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using AssimpMesh = Assimp.Mesh;

namespace SummerPractise.ModelLoading
{
    public static class Extensions
    {
        public static Vector2 ConvertAssimpVector2(this Vector3D AssimpVector)
        {
            // Reinterpret the assimp vector into an OpenTK vector.
            return Unsafe.As<Vector3D, Vector2>(ref AssimpVector);
        }

        public static Vector3 ConvertAssimpVector3(this Vector3D AssimpVector)
        {
            // Reinterpret the assimp vector into an OpenTK vector.
            return Unsafe.As<Vector3D, Vector3>(ref AssimpVector);
        }

        public static Matrix4 ConvertAssimpMatrix4(this Matrix4x4 AssimpMatrix)
        {
            // Take the column-major assimp matrix and convert it to a row-major OpenTK matrix.
            return Matrix4.Transpose(Unsafe.As<Matrix4x4, Matrix4>(ref AssimpMatrix));
        }
    }

    internal class Model
    {
        private List<Mesh> meshes = new List<Mesh>();
        private string directory;

        internal Model(string path)
        {
            LoadModel(path);
        }

        internal void Draw(Shader shader)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].Draw(shader);
            }
        }

        private void LoadModel(string path)
        {
            AssimpContext importer = new AssimpContext();

            LogStream logstream = new LogStream(delegate (String msg, String userData)
            {
                Console.WriteLine(msg);
            });
            logstream.Attach();

            Scene scene = importer.ImportFile(path, PostProcessSteps.Triangulate);

            if (scene == null || scene.SceneFlags.HasFlag(SceneFlags.Incomplete) || scene.RootNode == null)
            {
                Console.WriteLine("Unable to load model from: " + path);
                return;
            }

            this.directory = path.Remove(path.LastIndexOf('/') + 1);

            ProcessNode(scene.RootNode, scene, Matrix4.Identity);

            importer.Dispose();
        }
        
        private void ProcessNode(Node node, Scene scene, Matrix4 parentTransform)
        {
            Matrix4 transform = node.Transform.ConvertAssimpMatrix4() * parentTransform;

            for (int i = 0; i < node.MeshCount; i++)
            {
                AssimpMesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene, transform));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene, transform);
            }
        }

        private Mesh ProcessMesh(AssimpMesh mesh, Scene scene, Matrix4 transform)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();

            Matrix4 inverseTransform = Matrix4.Invert(transform);

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex();

                // Positions
                Vector3 position = mesh.Vertices[i].ConvertAssimpVector3();
                Vector3 transformedPosition = Vector3.TransformPosition(position, transform);
                vertex.Position = transformedPosition;

                // Normals
                if (mesh.HasNormals)
                {
                    Vector3 normal = mesh.Normals[i].ConvertAssimpVector3();
                    Vector3 transformedNormal = Vector3.TransformNormalInverse(normal, inverseTransform);
                    vertex.Normal = transformedNormal;
                }

                // Texture coordinates
                if (mesh.TextureCoordinateChannels[0] != null)
                {
                    Vector2 texCoords = mesh.TextureCoordinateChannels[0][i].ConvertAssimpVector2();

                    vertex.TexCoords = texCoords;
                }
                else
                    vertex.TexCoords = new Vector2();

                vertices.Add(vertex); 
            }

            for (int i = 0; i < mesh.FaceCount; i++)
            {
                Face face = mesh.Faces[i];
                for (int j = 0; j < face.IndexCount; j++)
                {
                    indices.Add(face.Indices[j]);
                }
            }

            if (mesh.MaterialIndex >= 0)
            {
                Material material = scene.Materials[mesh.MaterialIndex];
                List<Texture> diffuseMaps = LoadMaterialTextures(material, TextureType.Diffuse, "texture_diffuse");
                textures.InsertRange(textures.Count, diffuseMaps);
                List<Texture> specularMaps = LoadMaterialTextures(material, TextureType.Specular, "texture_specular");
                textures.InsertRange(textures.Count, specularMaps);
            }

            return new Mesh(vertices, indices, textures);
        }

        private List<Texture> LoadMaterialTextures(Material material, TextureType type, string typeName)
        {
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < material.GetMaterialTextureCount(type); i++)
            {
                TextureSlot textureSlot = new TextureSlot();
                bool skip = !material.GetMaterialTexture(type, i, out textureSlot);
                for (int j = 0; j < textures.Count; j++)
                {
                    if (textures[j].path == textureSlot.FilePath)
                    {
                        textures.Add(textures[j]);
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    Texture texture = new Texture()
                    {
                        id = TextureHelper.LoadFromFile(directory + textureSlot.FilePath),
                        type = typeName,
                        path = textureSlot.FilePath
                    };
                    textures.Add(texture);
                }
            }

            return textures;
        }
    }
}
