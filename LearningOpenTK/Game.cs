using LearningOpenTK.Shadering;
using LearningOpenTK.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace LearningOpenTK
{
    public class Game : GameWindow
    {
        private Shader shader;

        private Texture texture1;
        private Texture texture2;

        private int vao;
        private int vbo;
        private int ebo;

        private Matrix4 view;
        private Matrix4 projection;

        private readonly float cameraSpeed = 5f;
        private Vector3 cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
        private Vector3 cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        private Vector3 cameraUp = new Vector3(0.0f, 1.0f, 0.0f);
        private Vector3 cameraDirection;
        private float cameraYaw = -90;
        private float cameraPitch = 0;

        private Vector3[] cubePositions =
        {
            new Vector3( 0.0f, 0.0f, 0.0f),
            new Vector3( 2.0f, 5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3( 2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f, 3.0f, -7.5f),
            new Vector3( 1.3f, -2.0f, -2.5f),
            new Vector3( 1.5f, 2.0f, -2.5f),
            new Vector3( 1.5f, 0.2f, -1.5f),
            new Vector3(-1.3f, 1.0f, -1.5f)
        };

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override unsafe void OnLoad()
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);

            shader = new Shader("../../../Shadering/Shaders/Shader1/vertexShader.glsl", "../../../Shadering/Shaders/Shader1/fragmentShader.glsl");
            shader.SetInt("texture1", 1);
            shader.SetInt("texture2", 2);

            texture1 = Texture.LoadFromFile("../../../Texturing/Textures/Imgs/container.png");
            texture2 = Texture.LoadFromFile("../../../Texturing/Textures/Imgs/awesomeface.png");

            float[] vertexData =
            {
                // Front 
                0.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left
                1.0f, 0.0f, 0.0f,       1.0f, 0.0f,  // Bottom right
                1.0f, 1.0f, 0.0f,       1.0f, 1.0f,  // Top right

                1.0f, 1.0f, 0.0f,       1.0f, 1.0f,  // Top right
                0.0f, 1.0f, 0.0f,       0.0f, 1.0f,  // Top left
                0.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left


                // Back
                0.0f, 0.0f, 1.0f,       0.0f, 0.0f,  // Bottom left
                1.0f, 0.0f, 1.0f,       1.0f, 0.0f,  // Bottom right
                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right

                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right
                0.0f, 1.0f, 1.0f,       0.0f, 1.0f,  // Top left
                0.0f, 0.0f, 1.0f,       0.0f, 0.0f,  // Bottom left


                // Bottom
                0.0f, 0.0f, 1.0f,       0.0f, 0.0f,  // Bottom left
                1.0f, 0.0f, 1.0f,       1.0f, 0.0f,  // Bottom right
                1.0f, 0.0f, 0.0f,       1.0f, 1.0f,  // Top right

                1.0f, 0.0f, 0.0f,       1.0f, 1.0f,  // Top right
                0.0f, 0.0f, 0.0f,       0.0f, 1.0f,  // Top left
                0.0f, 0.0f, 1.0f,       0.0f, 0.0f,  // Bottom left


                // Top
                0.0f, 1.0f, 0.0f,       0.0f, 0.0f,  // Bottom left
                1.0f, 1.0f, 0.0f,       1.0f, 0.0f,  // Bottom right
                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right

                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right
                0.0f, 1.0f, 1.0f,       0.0f, 1.0f,  // Top left
                0.0f, 1.0f, 0.0f,       0.0f, 0.0f,  // Bottom left


                // Left
                0.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left
                0.0f, 0.0f, 1.0f,       1.0f, 0.0f,  // Bottom right
                0.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right

                0.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right
                0.0f, 1.0f, 0.0f,       0.0f, 1.0f,  // Top left
                0.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left


                // Right
                1.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left
                1.0f, 0.0f, 1.0f,       1.0f, 0.0f,  // Bottom right
                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right

                1.0f, 1.0f, 1.0f,       1.0f, 1.0f,  // Top right
                1.0f, 1.0f, 0.0f,       0.0f, 1.0f,  // Top left
                1.0f, 0.0f, 0.0f,       0.0f, 0.0f,  // Bottom left
            };

            for (int i = 0; i < vertexData.Length; i++)
            {
                if ((i - 3) % 5 != 0 && (i - 4) % 5 != 0)
                {
                    vertexData[i] = vertexData[i] - 0.5f;
                }
            }

            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexData.Length, vertexData, BufferUsageHint.StreamDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
           
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);
            GLFW.SetInputMode(this.WindowPtr, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Clear();

            cameraDirection.X = MathF.Cos(MathHelper.DegreesToRadians(cameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(cameraPitch));
            cameraDirection.Y = MathF.Sin(MathHelper.DegreesToRadians(cameraPitch));
            cameraDirection.Z = MathF.Sin(MathHelper.DegreesToRadians(cameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(cameraPitch));


            view = Matrix4.LookAt(cameraPos, cameraPos + cameraFront, cameraUp);

            shader.SetMatrix("view", view);
            shader.SetMatrix("projection", projection);

            texture1.Use(TextureUnit.Texture1);
            texture2.Use(TextureUnit.Texture2);

            GL.BindVertexArray(vao);

            for (int i = 0; i < cubePositions.Length; i++)
            {
                Matrix4 model = Matrix4.Identity * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(50.0f*i*(float)GLFW.GetTime())) * Matrix4.CreateTranslation(cubePositions[i]);

                shader.SetMatrix("model", model);
                
                shader.Use();

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            UpdateKeys();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private void Clear()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private void UpdateKeys()
        {
            if (IsKeyDown(Keys.W))
            {
                cameraPos += cameraFront * cameraSpeed * (float)UpdateTime;
            }
            if(IsKeyDown(Keys.S))
            {
                cameraPos -= cameraFront * cameraSpeed * (float)UpdateTime;
            }
            if (IsKeyDown(Keys.A))
            {
                cameraPos -= Vector3.Cross(cameraFront, cameraUp) * cameraSpeed * (float)UpdateTime;
            }
            if (IsKeyDown(Keys.D))
            {
                cameraPos += Vector3.Cross(cameraFront, cameraUp) * cameraSpeed * (float)UpdateTime;
            }
        }
    }
}
