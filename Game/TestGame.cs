using System;
using System.Collections.Generic;
using System.Text;
using GameEngine;
using GameEngine.Rendering;
using GameEngine.ModelLoading;
using GameEngine.Shadering;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using AbstractGame = GameEngine.MainLooping.Game;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;

namespace Game
{
    public class TestGame : AbstractGame
    {
        float[] squareVertices =
        {
            -0.5f, -0.5f, -0.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.0f,  0.0f, 0.0f,
        };
        float[] cubeVertices = {
            // positions          // texture Coords
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };
        float[] planeVertices = {
            // positions          // texture Coords (note we set these higher than 1 (together with GL_REPEAT as texture wrapping mode). this will cause the floor texture to repeat)
             5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
            -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,

             5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
             5.0f, -0.5f, -5.0f,  2.0f, 2.0f
        };

        Vector3[] VegetationPositions =
        {
            new Vector3(-1.5f, 0.0f, -0.48f),
            new Vector3( 1.5f, 0.0f, 0.51f),
            new Vector3( 0.0f, 0.0f, 0.7f),
            new Vector3(-0.3f, 0.0f, -2.3f),
            new Vector3( 0.5f, 0.0f, -0.6f)
        };
        

        private Shader Shader;
        private Shader OutlineShader;
        private Shader LightShader;

        private int cubeVao, cubeVbo, plainVao, plainVbo, vegetationVao, vegetationVbo;

        private Texture BrickTexture;
        private Texture PlankTexture;
        private Texture GrassTexture;
        private Texture WindowTexture;

        private bool WireFrameMode = false;

        public TestGame(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnInit()
        {
        }

        protected override Camera OnCreateCamera()
        {
            return new Camera(new Vector3(0f, 0f, 5f), this.Size.X / this.Size.Y);
        }

        protected override void OnLoadShaders()
        {
            this.Shader = new Shader("Shaders/vertexShader.glsl", "Shaders/simpleFragmentShader.glsl");
            this.LightShader = new Shader("Shaders/vertexShader.glsl", "Shaders/lightFragmentShader.glsl");
            this.OutlineShader = new Shader("Shaders/vertexShader.glsl", "Shaders/outlineFragmentShader.glsl");
        }
       
        protected override void OnLoadTextures()
        {
            BrickTexture = Texture.LoadFromFile("Resources/Textures/container2.png");
            PlankTexture = Texture.LoadFromFile("Resources/Textures/container.png");
            GrassTexture = Texture.LoadFromFile("Resources/Textures/grass.png", TextureWrapMode.ClampToBorder);
            WindowTexture = Texture.LoadFromFile("Resources/Textures/window.png", TextureWrapMode.ClampToBorder);
        }

        protected override void OnLoadModels()
        {
            cubeVao = GL.GenVertexArray();
            cubeVbo = GL.GenBuffer();

            GL.BindVertexArray(cubeVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, cubeVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * cubeVertices.Length, cubeVertices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aPos"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aPos"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aTexCoords"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);

            plainVao = GL.GenVertexArray();
            plainVbo = GL.GenBuffer();

            GL.BindVertexArray(plainVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, plainVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * planeVertices.Length, planeVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aPos"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aPos"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aTexCoords"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);

            vegetationVao = GL.GenVertexArray();
            vegetationVbo = GL.GenBuffer();

            GL.BindVertexArray(vegetationVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vegetationVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * squareVertices.Length, squareVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aPos"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aPos"), 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(Shader.GetAttribLocation("aTexCoords"));
            GL.VertexAttribPointer(Shader.GetAttribLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.BindVertexArray(0);
        }

        protected override void OnUpdate(FrameEventArgs args)
        {
            if (KeyboardManager.OnKeyPressed(Keys.P))
            {
                if (WireFrameMode)
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                    WireFrameMode = false;
                }
                else
                {
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    WireFrameMode = true;
                }
            }
        }

        protected override void OnRender(FrameEventArgs args, Matrix4 view, Matrix4 projection)
        {
            Shader.Use();
            Shader.SetMatrix("view", view);
            Shader.SetMatrix("projection", projection);

            GL.BindVertexArray(plainVao);
            GL.BindTexture(TextureTarget.Texture2D, PlankTexture.Handle);
            Shader.SetMatrix("model", Matrix4.Identity);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            DrawTwoContainers();


            SortedList<float, Vector3> sortedWindows = new SortedList<float, Vector3>();
            for (int i = 0; i < VegetationPositions.Length; i++)
            {
                sortedWindows.Add(Vector3.Distance(Camera.Position, VegetationPositions[i]), VegetationPositions[i]);
            }

            GL.BindVertexArray(vegetationVao);
            GL.BindTexture(TextureTarget.Texture2D, WindowTexture.Handle);
            for (int i = 0; i < sortedWindows.Count; i++)
            {
                Matrix4 model = Matrix4.Identity;
                model *= Matrix4.CreateTranslation(sortedWindows.Values[sortedWindows.Count - 1 - i]);
                Shader.SetMatrix("model", model);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }


        }

        protected override void OnWindowResize(ResizeEventArgs args)
        {
        }

        private void DrawTwoContainers()
        {
            Matrix4 model = Matrix4.Identity;

            GL.BindVertexArray(cubeVao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, BrickTexture.Handle);
            model *= Matrix4.CreateTranslation(-1.0f, 0.1f, -1.0f);
            Shader.SetMatrix("model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation(2.0f, 0.1f, 0.0f);
            Shader.SetMatrix("model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void DrawTwoContainersOutline()
        {
            Matrix4 model = Matrix4.Identity;

            GL.BindVertexArray(cubeVao);
            model *= Matrix4.CreateScale(1.05f);
            model *= Matrix4.CreateTranslation(-1.0f, 0.1f, -1.0f);
            OutlineShader.SetMatrix("model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            model = Matrix4.Identity;
            model *= Matrix4.CreateScale(1.05f);
            model *= Matrix4.CreateTranslation(2.0f, 0.1f, 0.0f);
            OutlineShader.SetMatrix("model", model);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
        }
    }
}
