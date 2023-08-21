using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lighting.Rendering
{
    internal static class DisplayManager
    {
        internal static unsafe Game CreateGame(int width, int height, string title)
        {
            GameWindowSettings gws = new GameWindowSettings();
            NativeWindowSettings nws = new NativeWindowSettings();

            nws.APIVersion = Version.Parse("4.1.0");
            nws.Size = new Vector2i(width, height);
            nws.WindowState = WindowState.Normal;
            nws.Title = title;

            Monitor* monitor = GLFW.GetPrimaryMonitor();
            int x, y, monitorWidth, monitorHeight;
            GLFW.GetMonitorWorkarea(monitor, out x, out y, out monitorWidth, out monitorHeight);
            x = (monitorWidth - width) / 2;
            y = (monitorHeight - height) / 2;

            nws.Location = new Vector2i(x, y);

            return new Game(gws, nws);
        }
    }
}
