using SummerPractice.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SummerPractice.ModelLoading;
using System;

namespace SummerPractice
{
    public class Game : GameWindow
    {
        private Camera Camera;

        private Shader Shader;
        private Shader LightShader;

        private Model Model;
        private Model LightBulb;

        private bool WireFrameMode = false;

        Vector3[] PointLightPositions = {
            new Vector3( 0.7f, 0.2f, 2.0f),
            new Vector3( 2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3( 0.0f, 0.0f, -3.0f)
        };

        private Vector3[] BackPackPositions =
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

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override unsafe void OnLoad()
        {
            base.OnLoad();

            this.CursorState = CursorState.Grabbed;
            
            GL.Enable(EnableCap.DepthTest);

            Camera = new Camera(new Vector3(0, 0, 4), (float)this.Size.X / (float)this.Size.Y, 45);

            Shader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/fragmentShader.glsl");
            LightShader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/lightFragmentShader.glsl");

            //Model = new Model("Resources/Objects/backpack_gltf/scene.gltf");
            Model = new Model("Resources/Objects/mig_gltf/scene.gltf");
            LightBulb = new Model("Resources/Objects/light_bulb/source/light_bulb.obj");
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (this.IsKeyPressed(Keys.P))
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

            Camera.UpdateKeys(MouseState, KeyboardState, (float)args.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Clear();

            Shader.Use();

            Shader.SetMatrix("view", Camera.GetViewMatrix());
            Shader.SetMatrix("projection", Camera.GetProjectionMatrix());

            Shader.SetFloat("material.shininess", 32);

            // Point light  
            for (int i = 0; i < PointLightPositions.Length; i++)
            {
                Shader.SetVec3("pointLights[" + i + "].ambient", 0.2f, 0.2f, 0.2f);
                Shader.SetVec3("pointLights[" + i + "].diffuse", 0.5f, 0.5f, 0.5f);
                Shader.SetVec3("pointLights[" + i + "].specular", 1.0f, 1.0f, 1.0f);
                
                Shader.SetVec3("pointLights[" + i + "].position", PointLightPositions[i]);
                
                Shader.SetFloat("pointLights[" + i + "].constant", 2.0f);
                Shader.SetFloat("pointLights[" + i + "].linear", 0.18f);
                Shader.SetFloat("pointLights[" + i + "].quadratic", 0.064f);
            }

            // Directional light
            Shader.SetVec3("dirLight.ambient", 0.1f, 0.1f, 0.1f);
            Shader.SetVec3("dirLight.diffuse", 0.10f, 0.10f, 0.10f);
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

            for (int i = 0; i < 1; i++)
            {
                Shader.SetMatrix("model", Matrix4.Identity * Matrix4.CreateScale(2.0f / 1.0f) * Matrix4.CreateTranslation(BackPackPositions[i]));

                //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

                Model.Draw(Shader);
            }

            LightShader.Use();

            LightShader.SetMatrix("view", Camera.GetViewMatrix());
            LightShader.SetMatrix("projection", Camera.GetProjectionMatrix());

            for (int i = 0; i < PointLightPositions.Length; i++)
            {
                LightShader.SetMatrix("model", Matrix4.CreateScale(1.0f / 10.0f) * Matrix4.CreateTranslation(PointLightPositions[i]));

                LightBulb.Draw(LightShader);
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
