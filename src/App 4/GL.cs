using Silk.NET.OpenGL;
using System;
using System.Runtime.InteropServices;

namespace App_4
{
    class GL
    {
        unsafe static GL()
        {
            _debugProcCallback = OpenGLDebug;
            _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);

            GLApi = Silk.NET.OpenGL.GL.GetApi();

            GL.GLApi.DebugMessageCallback(_debugProcCallback, null);
            GL.GLApi.Enable(GLEnum.DebugOutput);
            GL.GLApi.Enable(GLEnum.DebugOutputSynchronous);
        }


        private static DebugProc _debugProcCallback;
        private static GCHandle _debugProcCallbackHandle; // so that the .NET GC doesn't fuck you over

        private static DebugProc OpenGLDebug = (source, type, id, severity, length, message, userParam) =>
        {
            string msg = Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine("Debug message: " + msg);
            if (msg.Contains("error"))
            {
                //GL_OBJECT_TYPE_ARB, 
                GL.GLApi.GetProgram(Program.Handle, GLEnum.LinkStatus, out int oh);
                throw new Exception(msg);
            }
        };

        public static Silk.NET.OpenGL.GL GLApi { get; }
    }
}
