using SummerPractice.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SummerPractice.ModelLoading
{
    internal class Mesh
    {
        internal List<Vertex> Vertices;
        internal List<int> Indices;
        internal List<Texture> Textures;

        private int VAO, VBO, EBO;
        
        internal Mesh(List<Vertex> vertices, List<int> indices, List<Texture> textures)
        {
            this.Vertices = vertices;
            this.Indices = indices;
            this.Textures = textures;

            SetupMesh();
        }

        internal void Draw(Shader shader)
        {
            int diffuseNumber = 1, specularNumber = 1;

            for (int i = 0; i < Textures.Count; i++)
            {
                GL.ActiveTexture((TextureUnit)i);

                string number = "";
                string name = Textures[i].type;
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
            
                shader.SetInt("material." + name + number, i);
                GL.BindTexture(TextureTarget.Texture2D, Textures[i].id);
            }

            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        private unsafe void SetupMesh()
        {
            this.VAO = GL.GenVertexArray();
            this.VBO = GL.GenBuffer();
            this.EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Count * sizeof(Vertex), Vertices.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Count * sizeof(int), Indices.ToArray(), BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)));
            
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));
            
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(Vertex), Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoords)));

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
