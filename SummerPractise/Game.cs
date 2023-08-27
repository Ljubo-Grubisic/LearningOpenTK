using Lighting.Shadering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SummerPractise.ModelLoading;
using Texture = SummerPractise.Texturing.Texture;
using System;

namespace Lighting
{
    public class Game : GameWindow
    {
        private Camera Camera;

        private Shader Shader;
        private Shader LightShader;

        private Model Model;
        private Texture Texture;

        Vector3[] PointLightPositions = {
            new Vector3( 0.7f, 0.2f, 2.0f),
            new Vector3( 2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3( 0.0f, 0.0f, -3.0f)
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

            Shader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/fragmentShader2.glsl");
            LightShader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/lightFragmentShader.glsl");

            Model = new Model("Resources/Objects/mig_fbx/mig23mld.FBX");
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

            Shader.Use();

            Shader.SetMatrix("view", Camera.GetViewMatrix());
            Shader.SetMatrix("projection", Camera.GetProjectionMatrix());
            Shader.SetMatrix("model", Matrix4.Identity * Matrix4.CreateScale(1.0f / 1.0f));


            Model.Draw(Shader);
            
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
