using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Lighting.Rendering.Display;

namespace Lighting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var window = DisplayManager.CreateWindow("Pong", 1280, 720);
            window.Run();
        }
    }
}
