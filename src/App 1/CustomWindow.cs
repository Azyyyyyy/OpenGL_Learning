using Silk.NET.Windowing.Common;
using System;
using Silk.NET.Windowing.Desktop;

namespace App_1
{
    class CustomWindow : GlfwWindow
    {
        public CustomWindow(WindowOptions options)
            : base(options)
        {
            DateTime = DateTime.Now;
        }

        public DateTime DateTime;
    }
}
