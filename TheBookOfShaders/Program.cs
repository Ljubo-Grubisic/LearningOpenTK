global using OpenTK.Graphics.OpenGL4;
global using OpenTK.Mathematics;
using TheBookOfShaders.Rendering.Display;

namespace TheBookOfShaders
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var window = DisplayManager.CreateWindow("TheBookOfShaders", 760, 760);
            window.Run();
        }
    }
}
