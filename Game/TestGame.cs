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
using OpenTK.Graphics.OpenGL;

namespace Game
{
    public class TestGame : AbstractGame
    {
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

        

        private Shader Shader;
        private Shader OutlineShader;
        private Shader LightShader;

        private int cubeVao, cubeVbo, plainVao, plainVbo;

        private Texture Brick;
        private Texture Plank;

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
            Brick = Texture.LoadFromFile("Resources/Textures/brickTexture.png");
            Plank = Texture.LoadFromFile("Resources/Textures/container.png");
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
            GL.Enable(EnableCap.DepthTest);
            GL.StencilOp(fail: StencilOp.Keep, zfail: StencilOp.Keep, zpass: StencilOp.Replace);
            

            GL.StencilMask(0x00);

            Shader.Use();
            Shader.SetMatrix("view", view);
            Shader.SetMatrix("projection", projection);

            GL.BindVertexArray(plainVao);
            GL.BindTexture(TextureTarget.Texture2D, Plank.Handle);
            Shader.SetMatrix("model", Matrix4.Identity);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.StencilMask(0xFF);

            DrawTwoContainers();

            
            GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
            GL.StencilMask(0x00);
            GL.Disable(EnableCap.DepthTest);

            OutlineShader.Use();

            OutlineShader.SetMatrix("view", view);
            OutlineShader.SetMatrix("projection", projection);

            DrawTwoContainersOutline();

            GL.StencilMask(0xFF);
            GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
            GL.Enable(EnableCap.DepthTest);

           
        }

        protected override void OnWindowResize(ResizeEventArgs args)
        {
        }

        private void DrawTwoContainers()
        {
            Matrix4 model = Matrix4.Identity;

            GL.BindVertexArray(cubeVao);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Brick.Handle);
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
