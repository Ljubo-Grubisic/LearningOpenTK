using Lighting.Shadering;
using Lighting.Texturing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace Lighting
{
    public class Game : GameWindow
    {
        private Camera Camera;

        private Shader Shader;
        private Shader LightShader;

        private int vaoObject;
        private int vaoLight;

        private Vector3[] CubePositions =
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

        Vector3[] PointLightPositions = {
            new Vector3( 0.7f, 0.2f, 2.0f),
            new Vector3( 2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3( 0.0f, 0.0f, -3.0f)
        };

        float[] vertexData = {
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


        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override unsafe void OnLoad()
        {
            base.OnLoad();

            GLFW.SetInputMode(this.WindowPtr, CursorStateAttribute.Cursor, CursorModeValue.CursorDisabled);
            
            GL.Enable(EnableCap.DepthTest);

            Camera = new Camera(new Vector3(0, 0, 4), (float)this.Size.X / (float)this.Size.Y, 45);

            Shader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/fragmentShader.glsl");
            LightShader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/lightFragmentShader.glsl");


            int vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexData.Length, vertexData, BufferUsageHint.StreamDraw);
            
            vaoObject = GL.GenVertexArray();
            GL.BindVertexArray(vaoObject);
            {
                int location = Shader.GetAttribLocation("aPos");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                GL.EnableVertexAttribArray(location);
            }
            {
                int location = Shader.GetAttribLocation("aNormal");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 3);
                GL.EnableVertexAttribArray(location);
            }
            {
                int location = Shader.GetAttribLocation("aTexCoords");
                GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, sizeof(float) * 8, sizeof(float) * 6);
                GL.EnableVertexAttribArray(location);
            }

            vaoLight = GL.GenVertexArray();
            GL.BindVertexArray(vaoLight);
            {
                int location = Shader.GetAttribLocation("aPos");
                GL.VertexAttribPointer(location, 3, VertexAttribPointerType.Float, false, sizeof(float) * 8, 0);
                GL.EnableVertexAttribArray(location);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Camera.UpdateKeys(MouseState, KeyboardState, (float)args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Clear();

            {
                GL.BindVertexArray(vaoObject);
                Shader.Use();

                Shader.SetMatrix("view", Camera.GetViewMatrix());
                Shader.SetMatrix("projection", Camera.GetProjectionMatrix());
                
                Shader.SetInt("material.diffuse", 0);
                Shader.SetInt("material.specular", 1);
                Shader.SetFloat("material.shininess", 32.0f);

                // Point light  
                for (int i = 0; i < PointLightPositions.Length; i++)
                {
                    Shader.SetVec3("pointLights[" + i +"].ambient", 0.2f, 0.2f, 0.2f);
                    Shader.SetVec3("pointLights[" + i +"].diffuse", 0.5f, 0.5f, 0.5f);
                    Shader.SetVec3("pointLights[" + i +"].specular", 1.0f, 1.0f, 1.0f);

                    Shader.SetVec3("pointLights[" + i + "].position", PointLightPositions[i]);

                    Shader.SetFloat("pointLights[" + i + "].constant", 2.0f);
                    Shader.SetFloat("pointLights[" + i + "].linear", 0.18f);
                    Shader.SetFloat("pointLights[" + i + "].quadratic", 0.064f);
                }

                // Directional light
                Shader.SetVec3("dirLight.ambient", 0.1f, 0.1f, 0.1f);
                Shader.SetVec3("dirLight.diffuse", 0.25f, 0.25f, 0.25f);
                Shader.SetVec3("dirLight.specular", 0.75f, 0.75f, 0.75f);
                                
                Shader.SetVec3("dirLight.direction", -0.2f, -1.0f, -0.3f);

                // Spot Light
                Shader.SetVec3("spotLight.position", Camera.Position);
                Shader.SetVec3("spotLight.direction", Camera.Front);
                Shader.SetFloat("spotLight.cutOff", MathF.Cos(MathHelper.DegreesToRadians(10.0f)));
                Shader.SetFloat("spotLight.outerCutOff", MathF.Cos(MathHelper.DegreesToRadians(12.5f)));

                Shader.SetVec3("spotLight.ambient", 0.0f, 0.0f, 0.0f);
                Shader.SetVec3("spotLight.diffuse", 1.0f, 1.0f, 1.0f);
                Shader.SetVec3("spotLight.specular", 1.0f, 1.0f, 1.0f);

                Shader.SetFloat("spotLight.constant", 1.0f);
                Shader.SetFloat("spotLight.linear", 0.09f);
                Shader.SetFloat("spotLight.quadratic", 0.032f);


                Shader.SetVec3("viewPos", Camera.Position);

                Random random = new Random();

                for (int i = 0; i < CubePositions.Length; i++)
                {
                    float angle = 20 * i;
                    Matrix4 rotation = Matrix4.CreateRotationX(angle * 1.0f);
                    rotation *= Matrix4.CreateRotationY(angle * 0.3f);
                    rotation *= Matrix4.CreateRotationZ(angle * 0.5f);

                    Matrix4 model = rotation * Matrix4.CreateTranslation(CubePositions[i]);

                    Shader.SetMatrix("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }

            {
                GL.BindVertexArray(vaoLight);
                LightShader.Use();

                LightShader.SetMatrix("view", Camera.GetViewMatrix());
                LightShader.SetMatrix("projection", Camera.GetProjectionMatrix());
                
                for (int i = 0; i < PointLightPositions.Length; i++)
                {
                    Matrix4 model = Matrix4.Identity * Matrix4.CreateScale(0.2f) * Matrix4.CreateTranslation(PointLightPositions[i]);

                    LightShader.SetMatrix("model", model);

                    GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
                }
            }
            
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
