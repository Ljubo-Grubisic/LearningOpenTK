using System;
using Lighting.Rendering;

namespace Lighting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = DisplayManager.CreateGame(1280, 720, "Practise");
            game.Run();
        }
    }
}
