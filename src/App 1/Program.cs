using System;
using System.Drawing;
using System.Threading.Tasks;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

namespace App_1
{
    static class Program
    {
        private static IWindow window;
        private static readonly GL GL = GL.GetApi();
        private static readonly Random Random = new Random();

        private static async Task Main()
        {
            (await MakeWindow(false)).Run();
        }

        private static void Window_Move(Point obj)
        {
            Console.WriteLine($"Move: {obj.X}, {obj.Y}");
        }

        private static void Window_Update(double deltaTime)
        {
            // Change the screen colour
            GL.ClearColor(Color.FromArgb(10, Random.Next(255), Random.Next(255), Random.Next(255)));
            Console.WriteLine($"Update: {deltaTime}");
        }

        private static void Window_Load(IWindow window)
        {
            var keyboard = window.GetInput().Keyboards[0];
            keyboard.KeyDown += (_, key) => { Console.WriteLine($"Down: {key}"); };
            keyboard.KeyUp += (_, key) => { Console.WriteLine($"Up: {key}"); };

            GL.ClearColor(Color.Chocolate);
        }

        private static void Window_Render(double deltaTime)
        {
            GL.Clear((uint)GLEnum.ColorBufferBit);
            Console.WriteLine($"Render: {deltaTime}");

            //window.Size = Size.Add(window.Size, new Size(1,1));
        }

        public static async Task<IWindow> MakeWindow(bool runWindow = true)
        {
            //new WindowOptions(true, false, new Point(0, 0), new Size(250, 500), 67, 67, GraphicsAPI.Default, "Wew", WindowState.Normal, WindowBorder.Resizable, VSyncMode.Adaptive, 30)
            window = Window.Create(WindowOptions.Default);
            window.Size = new Size(500, 250);
            window.WindowBorder = WindowBorder.Resizable;
            window.Title = "Wew";

            window.Load += async () => 
            {
                Window_Load(window);
                await Task.Delay(5000);
                MakeWindow();
            };
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.Move += Window_Move;

            if (runWindow)
            {
                _ = Task.Run(() => window.Run()).ConfigureAwait(false);
            }

            return window;
        }
    }
}
