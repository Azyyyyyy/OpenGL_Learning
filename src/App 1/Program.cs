using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing.Common;
using System.Threading;

namespace App_1
{
    static class Program
    {
        private static readonly List<CustomWindow> Windows = new List<CustomWindow>();
        private static readonly GL GL = GL.GetApi();
        private static readonly Random Random = new Random();

        private static readonly CancellationTokenSource TokenSource = new CancellationTokenSource();
        private static CancellationToken Token => TokenSource.Token;

        private static async Task Main()
        {
            MakeWindow();

            await Task.Delay(-1, Token);
        }

        private static void Window_Move(Point obj, int windowInt)
        {
            Console.WriteLine($"Move: {obj.X}, {obj.Y} (Window {windowInt})");
        }

        private static void Window_Update(CustomWindow customWindow, double deltaTime, int windowInt)
        {
            // Change the screen colour
            if (DateTime.Now.Subtract(customWindow.DateTime).TotalMilliseconds >= 500)
            {
                GL.ClearColor(Color.FromArgb(10, Random.Next(255), Random.Next(255), Random.Next(255)));
                customWindow.DateTime = DateTime.Now;
            }
            Console.WriteLine($"Update: {deltaTime} (Window {windowInt})");
        }

        private static void Window_Load(IWindow window, int windowInt)
        {
            var keyboard = window.GetInput().Keyboards[0];
            keyboard.KeyDown += (_, key) => { Console.WriteLine($"Down: {key} (Window {windowInt})"); };
            keyboard.KeyUp += (_, key) => { Console.WriteLine($"Up: {key} (Window {windowInt})"); };

            GL.ClearColor(Color.Chocolate);
        }

        private static void Window_Render(double deltaTime, int windowInt)
        {
            GL.Clear((uint)GLEnum.ColorBufferBit);
            Console.WriteLine($"Render: {deltaTime} (Window {windowInt})");

            //window.Size = Size.Add(window.Size, new Size(1,1));
        }

        public static IWindow MakeWindow(bool runWindow = true)
        {
            if (Windows.Count == 16)
            {
                return null;
            }

            var window = new CustomWindow(WindowOptions.Default)
            {
                Size = new Size(500, 250),
                WindowBorder = WindowBorder.Resizable,
                Title = $"Wew {Windows.Count + 1}"
            };

            window.Load += async () => 
            {
                Window_Load(window, Windows.IndexOf(window) + 1);
                await Task.Delay(5000);
                MakeWindow();
            };
            window.Update += (d) => Window_Update(window, d, Windows.IndexOf(window) + 1);
            window.Render += (d) => Window_Render(d, Windows.IndexOf(window) + 1);
            window.Resize += (s) => Window_Resize(s, Windows.IndexOf(window) + 1);
            window.Closing += () => Window_Closing(window, Windows.IndexOf(window) + 1);
            window.Move += (p) => Window_Move(p, Windows.IndexOf(window) + 1);

            if (runWindow)
            {
                _ = Task.Run(() => window.Run()).ConfigureAwait(false);
            }
            Windows.Add(window);
            return window;
        }

        private static void Window_Closing(CustomWindow window, int windowInt)
        {
            Windows.Remove(window);
            if (Windows.Count == 0)
            {
                TokenSource.Cancel();
            }
            Console.WriteLine($"Window {windowInt} is closing");
        }

        private static void Window_Resize(Size obj, int windowInt)
        {
            Console.WriteLine($"Resize: {obj.Width}, {obj.Height} (Window {windowInt})");
        }
    }
}
