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
using Texture = SummerPractise.Texturing.Texture;

namespace SummerPractise
{
    internal class Game : GameWindow
    {
        internal Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private int vaoLight;
        private int vaoLamp;

        private Camera Camera;
        private Shader LightingShader;
        private Shader LampShader;

        private Texture ContainerTexture;
        private Texture ContainerSpecularTexture;

        private Matrix4 Model;
        private Matrix4 View;
        private Matrix4 Projecton;

        private float[] VertexData =
        {
            // positions // normals // texture coords
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f,
            0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
            0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f,
            -0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 1.0f,
            0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
            0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 1.0f,
            0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f,
            -0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f
        };

        private Vector3 LightPosition = new Vector3(1.2f, 1.0f, 2.0f);
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

            Camera = new Camera(new Vector3(0, 0, 4), (float)this.Size.X / (float)this.Size.Y, 45);

            ContainerTexture = Texture.LoadFromFile("Resources/Textures/container2.png");
            ContainerSpecularTexture = Texture.LoadFromFile("Resources/Textures/container2_specular.png");
                    
            LightingShader = new Shader("Shadering/Shaders/VertexShader.glsl", "Shadering/Shaders/FragmentShader.glsl");
            LampShader = new Shader("Shadering/Shaders/VertexShader.glsl", "Shadering/Shaders/Lighting/LightingFragmentShader.glsl");

            vaoLight = GL.GenVertexArray();
            vaoLamp = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            // Add data to vbo
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, VertexData.Length * sizeof(float), VertexData, BufferUsageHint.StaticDraw);


            GL.BindVertexArray(vaoLight);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            {
                int location = LightingShader.GetAttribLocation("aPos");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                GL.EnableVertexAttribArray(location);
            }
            {
                int location = LightingShader.GetAttribLocation("aNormal");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 3);
                GL.EnableVertexAttribArray(location);
            }
            {
                int location = LightingShader.GetAttribLocation("aTexCoords");
                GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);
                GL.EnableVertexAttribArray(location);
            }


            GL.BindVertexArray(vaoLamp);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            {
                int location = LightingShader.GetAttribLocation("aPos");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                GL.EnableVertexAttribArray(location);
            }

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

            LightingShader.Use();

            GL.BindVertexArray(vaoLight);

            ContainerTexture.Use(TextureUnit.Texture0);
            ContainerSpecularTexture.Use(TextureUnit.Texture1);

            LightingShader.SetInt("material.diffuse", 0);
            LightingShader.SetInt("material.specular", 1);

            LightingShader.SetVec3("material.specular", 0.5f, 0.5f, 0.5f);
            LightingShader.SetFloat("material.shininess", 32.0f);

            
            LightingShader.SetVec3("light.ambient", 0.2f, 0.2f, 0.2f);
            LightingShader.SetVec3("light.diffuse", 0.5f, 0.5f, 0.5f);
            LightingShader.SetVec3("light.specular", 1.0f, 1.0f, 1.0f);
            LightingShader.SetVec3("light.position", LightPosition);

            LightingShader.SetVec3("viewPos", Camera.Position);
            
            Model = Matrix4.Identity;

            LightingShader.SetMatrix("view", View);
            LightingShader.SetMatrix("projection", Projecton);
            LightingShader.SetMatrix("model", Model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
  
            GL.BindVertexArray(0);


            LampShader.Use();

            GL.BindVertexArray(vaoLamp);

            Model = Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(LightPosition);

            LampShader.SetMatrix("view", View);
            LampShader.SetMatrix("projection", Projecton);
            LampShader.SetMatrix("model", Model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

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
