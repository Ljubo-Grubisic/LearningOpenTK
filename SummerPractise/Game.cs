using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using SummerPractise.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using SummerPractise.Texturing;
using System.ComponentModel;
using System.Xml.Linq;

namespace SummerPractise
{
    internal class Game : GameWindow
    {
        internal Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private int vao;

        private Camera Camera;
        private Shader Shader;
        private Texture ContainerTexture;
        private Texture SmileTexture;

        private Matrix4 Model;
        private Matrix4 View;
        private Matrix4 Projecton;

        private float[] VertexData =
        {
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
            0.5f, -0.5f, -0.5f, 1.0f, 0.0f,
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 1.0f,
            -0.5f, 0.5f, 0.5f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            -0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 1.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f,
            0.5f, 0.5f, -0.5f, 1.0f, 1.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, 0.0f, 0.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f
        };

        private Vector3[] CubePositions =
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(2.0f, 5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3( 2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f, 3.0f, -7.5f),
            new Vector3( 1.3f, -2.0f, -2.5f),
            new Vector3( 1.5f, 2.0f, -2.5f),
            new Vector3( 1.5f, 0.2f, -1.5f),
            new Vector3(-1.3f, 1.0f, -1.5f),
        };
      
        protected unsafe override void OnLoad()
        { 
            base.OnLoad();

            GLFW.SetInputMode(this.WindowPtr, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);

            GL.Enable(EnableCap.DepthTest);

            Camera = new Camera(new Vector3(0, 0, 3), (float)this.Size.X / (float)this.Size.Y, 45);

            Shader = new Shader("../../../Shadering/Shaders/VertexShader.glsl", "../../../Shadering/Shaders/FragmentShader.glsl");
            Shader.SetInt("texture1", 1);
            Shader.SetInt("texture2", 2);

            ContainerTexture = Texture.LoadFromFile("../../../Texturing/Textures/Imgs/container.png");
            SmileTexture = Texture.LoadFromFile("../../../Texturing/Textures/Imgs/awesomeface.png");

            vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            // Add data to the vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.Length * sizeof(float), VertexData, BufferUsageHint.StaticDraw);

            // Tell the vao how to interpet the vertex data
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);
            GL.EnableVertexAttribArray(0);

            // Tell the vao how to interpet the texture data
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            GL.BindVertexArray(0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Camera.UpdateKeys(MouseState, KeyboardState, (float)args.Time);

            Projecton = Camera.GetProjectionMatrix();
            View = Camera.GetViewMatrix();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            Clear();

            Shader.SetMatrix("view", View);
            Shader.SetMatrix("projection", Projecton);

            Shader.Use();

            ContainerTexture.Use(TextureUnit.Texture1);
            SmileTexture.Use(TextureUnit.Texture2);

            GL.BindVertexArray(vao);

            for (int i = 0; i < CubePositions.Length; i++)
            {
                Model = Matrix4.Identity * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(50.0f * i * (float)GLFW.GetTime())) * Matrix4.CreateTranslation(CubePositions[i]);

                Shader.SetMatrix("model", Model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
  
            GL.BindVertexArray(0);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
            Camera.AspectRatio = (float)Size.X / (float)Size.Y;
        }

        private void Clear()
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
