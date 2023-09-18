using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SummerPractice.Rendering;

namespace SummerPractice
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var window = DisplayManager.CreateWindow("Model", 1280, 720);
            window.Run();
        }
    }
}
