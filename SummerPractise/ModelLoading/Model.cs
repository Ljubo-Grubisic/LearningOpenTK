using Assimp;
using Assimp.Configs;
using Assimp.Unmanaged;
using Lighting.Shadering;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SummerPractise.ModelLoading
{
    internal class Model
    {
        private List<Mesh> meshes;
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
            importer.SetConfig(new TransformUVConfig(UVTransformFlags.Rotation));
            Scene scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

            if (scene.SceneFlags == SceneFlags.Incomplete || scene.RootNode == null || scene == null)
            {
                Console.WriteLine("Error");
            }

            ProcessNode(scene.RootNode, scene);
        }
        
        private void ProcessNode(Node node, Scene scene)
        {
            for (int i = 0; i < node.MeshCount; i++)
            {
                Assimp.Mesh mesh = scene.Meshes[node.MeshIndices[i]];
                meshes.Add(ProcessMesh(mesh, scene));
            }

            for (int i = 0; i < node.ChildCount; i++)
            {
                ProcessNode(node.Children[i], scene);
            }
        }

        private Mesh ProcessMesh(Assimp.Mesh mesh, Scene scene)
        {
            List<Vertex> vertices = new List<Vertex>();
            List<int> indices = new List<int>();
            List<Texture> textures = new List<Texture>();

            for (int i = 0; i < mesh.VertexCount; i++)
            {
                Vertex vertex = new Vertex { };

                {
                    Vector3 vector = new Vector3();
                    vector.X = mesh.Vertices[i].X;
                    vector.Y = mesh.Vertices[i].Y;
                    vector.Z = mesh.Vertices[i].Z;
                    vertex.Position = vector;

                    vector.X = mesh.Normals[i].X;
                    vector.Y = mesh.Normals[i].Y;
                    vector.Z = mesh.Normals[i].Z;
                    vertex.Normal = vector;
                }

                if (mesh.TextureCoordinateChannels[0] != null)
                {
                    Vector2 vector = new Vector2();

                    vector.X = mesh.TextureCoordinateChannels[0][i].X;
                    vector.Y = mesh.TextureCoordinateChannels[0][i].Y;
                    vertex.TexCoords = vector;
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
                material.GetMaterialTexture(type, i, out textureSlot);
                bool skip = false;
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
                    Texture texture = new Texture();
                    texture.id = TextureHelper.LoadFromFile(directory + textureSlot.FilePath);
                    texture.type = typeName;
                    texture.path = textureSlot.FilePath;
                    textures.Add(texture);
                }
            }
            return textures;
        }
    }
}
