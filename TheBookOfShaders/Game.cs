using TheBookOfShaders.Shadering;
using TheBookOfShaders.Texturing;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TheBookOfShaders
{
    public class Game : GameWindow
    {
        private Shader Shader;
        private int vaoObject;

        private float[] vertexData =
        {
            1.0f, 1.0f,
            1.0f, -1.0f,
            -1.0f, 1.0f,

            -1.0f, 1.0f,
            -1.0f, -1.0f,
            1.0f, -1.0f
        };

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override unsafe void OnLoad()
        {
            base.OnLoad();

            
            GL.Enable(EnableCap.DepthTest);

            Shader = new Shader("../../../Shadering/Shaders/vertexShader.glsl", "../../../Shadering/Shaders/fragmentShader.glsl");

            int vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * vertexData.Length, vertexData, BufferUsageHint.StreamDraw);
            
            vaoObject = GL.GenVertexArray();
            GL.BindVertexArray(vaoObject);
            {
                int location = Shader.GetAttribLocation("aPos");
                GL.VertexAttribPointer(location, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);
                GL.EnableVertexAttribArray(location);
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Clear();

            GL.BindVertexArray(vaoObject);
            Shader.Use();

            Shader.SetVec2("u_resolution", this.Size);
            Shader.SetVec2("u_mouse", this.MousePosition);
            Shader.SetFloat("u_time", (float)GLFW.GetTime());

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        private void Clear()
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
