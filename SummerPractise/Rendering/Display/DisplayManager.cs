using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lighting.Rendering.Display
{
    public static class DisplayManager
    {
        static public unsafe Game CreateWindow(string title, int width, int height)
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;

            nws.APIVersion = Version.Parse("4.1.0");
            nws.Size = new Vector2i(width, height);
            nws.Title = title;
            //nws.MaximumSize = new Vector2i(width, height);
            //nws.MinimumSize = new Vector2i(width, height);
            nws.StartFocused = true;
            nws.StartVisible = true;

            Monitor* monitor = GLFW.GetPrimaryMonitor();
            int x, y, monitorWidth, monitorHeight;
            GLFW.GetMonitorWorkarea(monitor, out x, out y, out monitorWidth, out monitorHeight);
            x = (monitorWidth - width) / 2;
            y = (monitorHeight - height) / 2;

            nws.Location = new Vector2i(x, y);

            Game Window = new Game(gws, nws);
            return Window;
        }
    }
}
