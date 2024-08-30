using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    internal static class Program
    {
        internal static TestGame TestGame { get; private set; }

        internal static void Main(string[] args)
        {
            TestGame.Window window = TestGame.CreateWindow("TestGame", 1280, 720);
            TestGame = new TestGame(window.GameWindowSettings, window.NativeWindowSettings);
            TestGame.Run();
        }
    }
}
