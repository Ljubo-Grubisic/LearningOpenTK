using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using Lighting.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using Lighting.Texturing;
using System.ComponentModel;
using System.Xml.Linq;
using Texture = Lighting.Texturing.Texture;

namespace Lighting
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
            new Vector3( 0.0f,  0.0f,  0.0f),
            new Vector3( 2.0f,  5.0f, -15.0f),
            new Vector3(-1.5f, -2.2f, -2.5f),
            new Vector3(-3.8f, -2.0f, -12.3f),
            new Vector3( 2.4f, -0.4f, -3.5f),
            new Vector3(-1.7f,  3.0f, -7.5f),
            new Vector3( 1.3f, -2.0f, -2.5f),
            new Vector3( 1.5f,  2.0f, -2.5f),
            new Vector3( 1.5f,  0.2f, -1.5f),
            new Vector3(-1.3f,  1.0f, -1.5f),
        };

        Vector3[] PointLightPositions = {
            new Vector3( 0.7f, 0.2f, 2.0f),
            new Vector3( 2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3( 0.0f, 0.0f, -3.0f)
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
            LightingShader.SetFloat("material.shininess", 32.0f);
            
            // Spot light
            LightingShader.SetVec3("spotLight.position", Camera.Position);
            LightingShader.SetVec3("spotLight.direction", Camera.Front);
            LightingShader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(10.0f)));
            LightingShader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));
            
            LightingShader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
            LightingShader.SetVec3("spotLight.diffuse", 1.0f, 1.0f, 1.0f);
            LightingShader.SetVec3("spotLight.specular", 1.0f, 1.0f, 1.0f);

            LightingShader.SetFloat("spotLight.constant", 1.0f);
            LightingShader.SetFloat("spotLight.linear", 0.09f);
            LightingShader.SetFloat("spotLight.quadratic", 0.032f);

            // Direction light
            LightingShader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);

            LightingShader.SetVec3("dirLight.ambient", 0.05f, 0.05f, 0.05f);
            LightingShader.SetVec3("dirLight.diffuse", 0.4f, 0.4f, 0.4f);
            LightingShader.SetVec3("dirLight.specular", 0.5f, 0.5f, 0.5f);

            // Point light
            for (int i = 0; i < 4; i++)
            {
                LightingShader.SetVec3("pointLights[" + i + "].position", PointLightPositions[i]);

                LightingShader.SetVec3("pointLights[" + i + "].ambient", PointLightPositions[i] * 0.1f);
                LightingShader.SetVec3("pointLights[" + i + "].diffuse", PointLightPositions[i]);
                LightingShader.SetVec3("pointLights[" + i + "].specular", PointLightPositions[i]);

                LightingShader.SetFloat("pointLights[" + i + "].constant", 0.5f);
                LightingShader.SetFloat("pointLights[" + i + "].linear", 0.045f);
                LightingShader.SetFloat("pointLights[" + i + "].quadratic", 0.016f);
            }

            LightingShader.SetVec3("viewPos", Camera.Position);
            
            Model = Matrix4.Identity;

            LightingShader.SetMatrix("view", View);
            LightingShader.SetMatrix("projection", Projecton);
            

            for (int i = 0; i < 10; i++)
            {
                float angle = 20 * i;
                Matrix4 rotation = Matrix4.CreateRotationX(angle * 1.0f);
                rotation *= Matrix4.CreateRotationY(angle * 0.3f);
                rotation *= Matrix4.CreateRotationZ(angle * 0.5f);

                Model = Matrix4.Identity * rotation * Matrix4.CreateTranslation(CubePositions[i]);

                LightingShader.SetMatrix("model", Model);

                GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            }
  
            GL.BindVertexArray(0);


            LampShader.Use();

            GL.BindVertexArray(vaoLamp);

            LampShader.SetMatrix("view", View);
            LampShader.SetMatrix("projection", Projecton);

            for (int i = 0; i < PointLightPositions.Length; i++)
            {
                Model = Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(PointLightPositions[i]);

                LampShader.SetMatrix("model", Model);
                LampShader.SetVec3("color", PointLightPositions[i]);

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
            GL.ClearColor(0.02f, 0.02f, 0.02f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
