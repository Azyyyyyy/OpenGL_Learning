using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;

namespace App_1
{
    class Program
    {
        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static CancellationToken _token => _tokenSource.Token;
        private static IWindow window;
        
        static async Task Main(string[] args)
        {
            window = Window.Create(WindowOptions.Default);
            window.Size = new Size(10, 10);
            window.WindowBorder = WindowBorder.Fixed;
            window.Title = "Wew";

            window.Render += d => render(d);
            window.Closing += async () =>
            {
                window.Close();
                while (window.IsVisible)
                {
                    await Task.Delay(1);
                }

                _tokenSource.Cancel();
            };
            
            Task.Run(() => window.Run());
            
            window.GetInput().Keyboards[0].KeyDown += (keyboard, key) => { Console.WriteLine($"Down: {key}"); };
            window.GetInput().Keyboards[0].KeyUp += (keyboard, key) => { Console.WriteLine($"Up: {key}"); };

            await Task.Delay(-1, _token);
        }

        private static async Task render(double deltaTime)
        {
            window.Size = Size.Add(window.Size, new Size(1,1));
        }
    }
}
