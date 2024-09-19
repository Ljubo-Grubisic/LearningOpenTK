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
using Assimp.Configs;

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
            // back face
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom-left
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f, // top-right
            0.5f, -0.5f, -0.5f, 1.0f, 0.0f, // bottom-right
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f, // top-right
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, // bottom-left
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, // top-left
            // front face
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom-left
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f, // bottom-right
            0.5f, 0.5f, 0.5f, 1.0f, 1.0f, // top-right
            0.5f, 0.5f, 0.5f, 1.0f, 1.0f, // top-right
            -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, // top-left
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom-left
            // left face
            -0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // top-right
            -0.5f, 0.5f, -0.5f, 1.0f, 1.0f, // top-left
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // bottom-left
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // bottom-left
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom-right
            -0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // top-right
            // right face
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // top-left
            0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // bottom-right
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f, // top-right
            0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // bottom-right
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // top-left
            0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom-left
            // bottom face
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // top-right
            0.5f, -0.5f, -0.5f, 1.0f, 1.0f, // top-left
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f, // bottom-left
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f, // bottom-left
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, // bottom-right
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f, // top-right
            // top face
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, // top-left
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // bottom-right
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f, // top-right
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, // bottom-right
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, // top-left
            -0.5f, 0.5f, 0.5f, 0.0f, 0.0f // bottom-left
        };
        float[] planeVertices = {
            // positions          // texture Coords (note we set these higher than 1 (together with GL_REPEAT as texture wrapping mode). this will cause the floor texture to repeat)
             5.0f, -0.5f,  5.0f,  2.0f, 0.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
            -5.0f, -0.5f,  5.0f,  0.0f, 0.0f,

             5.0f, -0.5f, -5.0f,  2.0f, 2.0f,
            -5.0f, -0.5f, -5.0f,  0.0f, 2.0f,
             5.0f, -0.5f,  5.0f,  2.0f, 0.0f
        };
        float[] quadVertices = {
            // positions // texCoords
            -1.0f, 1.0f, 0.0f, 1.0f,
            -1.0f, -1.0f, 0.0f, 0.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            -1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, -1.0f, 1.0f, 0.0f,
            1.0f, 1.0f, 1.0f, 1.0f
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
        private Shader FrameBufferShader;

        private int cubeVao, cubeVbo, plainVao, plainVbo, vegetationVao, vegetationVbo, quadVao, quadVbo, fbo, rbo, texColorBuffer;

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
            fbo = GL.GenFramebuffer();
            rbo = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, Size.X, Size.Y);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
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
            this.FrameBufferShader = new Shader("Shaders/FrameBuffer/frameBufferVertexShader.glsl", "Shaders/FrameBuffer/frameBufferFragmentShader.glsl");
        }
       
        protected override void OnLoadTextures()
        {
            BrickTexture = Texture.LoadFromFile("Resources/Textures/container2.png");
            PlankTexture = Texture.LoadFromFile("Resources/Textures/container.png");
            GrassTexture = Texture.LoadFromFile("Resources/Textures/grass.png", TextureWrapMode.ClampToBorder);
            WindowTexture = Texture.LoadFromFile("Resources/Textures/window.png", TextureWrapMode.ClampToBorder);

            texColorBuffer = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, texColorBuffer);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Size.X, Size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texColorBuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, rbo);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete)
                Console.WriteLine("Framebuffer error");

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
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

            quadVao = GL.GenVertexArray();
            quadVbo = GL.GenBuffer();

            GL.BindVertexArray(quadVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, quadVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * quadVertices.Length, quadVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(FrameBufferShader.GetAttribLocation("aPos"));
            GL.VertexAttribPointer(FrameBufferShader.GetAttribLocation("aPos"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);
            GL.EnableVertexAttribArray(FrameBufferShader.GetAttribLocation("aTexCoords"));
            GL.VertexAttribPointer(FrameBufferShader.GetAttribLocation("aTexCoords"), 2, VertexAttribPointerType.Float, false, 4 * sizeof(float), 2 * sizeof(float));
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
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            Shader.Use();
            Shader.SetMatrix("view", view);
            Shader.SetMatrix("projection", projection);

            GL.BindVertexArray(plainVao);
            GL.BindTexture(TextureTarget.Texture2D, PlankTexture.Handle);
            Shader.SetMatrix("model", Matrix4.Identity);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);

            GL.Enable(EnableCap.CullFace);

            DrawTwoContainers();

            GL.Disable(EnableCap.CullFace);

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


            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            FrameBufferShader.Use();
            
            
            GL.BindVertexArray(quadVao);
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, this.texColorBuffer);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            GL.BindVertexArray(0);
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
