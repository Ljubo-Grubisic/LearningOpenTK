using Lighting.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SummerPractise.ModelLoading
{
    internal class Mesh
    {
        internal List<Vertex> verticies;
        internal List<int> indicies;
        internal List<Texture> textures;

        private int VAO, VBO, EBO;
        
        internal Mesh(List<Vertex> verticies, List<int> indicies, List<Texture> textures)
        {
            this.verticies = verticies;
            this.indicies = indicies;
            this.textures = textures;

            setupMesh();
        }

        internal void Draw(Shader shader)
        {
            int diffuseNumber = 1, specularNumber = 1;

            for (int i = 0; i < textures.Count; i++)
            {
                GL.ActiveTexture((TextureUnit)i);

                string number = "";
                string name = textures[i].type;
                if (name == "texture_diffuse")
                {
                    number = diffuseNumber.ToString();
                    diffuseNumber++;
                }
                else if (name == "texture_specular")
                {
                    number = specularNumber.ToString();
                    specularNumber++;
                }

                shader.SetFloat("material." + name + number, i);
                GL.BindTexture(TextureTarget.Texture2D, textures[i].id);
            }
            GL.ActiveTexture(TextureUnit.Texture0);

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, indicies.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private unsafe void setupMesh()
        {
            this.VAO = GL.GenVertexArray();
            this.VBO = GL.GenBuffer();
            this.EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticies.Count * sizeof(Vertex), verticies.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicies.Count * sizeof(uint), indicies.ToArray(), BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("Position"));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>("Position").ToInt32() + Marshal.OffsetOf<Vertex>("Normal").ToInt32());
        
            GL.BindVertexArray(0);
        }
    }

    internal struct Vertex
    {
        internal Vector3 Position;
        internal Vector3 Normal;
        internal Vector2 TexCoords;
    }

    internal struct Texture
    {
        internal int id;
        internal string type;
        internal string path;
    }
}
