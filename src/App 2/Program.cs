using System;
using System.Drawing;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using System.IO;
using System.Text;

namespace App_2
{
    class Program
    {
        private static GL GL = GL.GetApi();
        private static readonly float[] Vertices = 
        {
            //X      Y      Z
            -0.5f, -0.5f, 0.0f, //Bottom-left vertex
            0.5f, -0.5f, 0.0f, //Bottom-right vertex
            0.0f,  0.5f, 0.0f  //Top vertex
        };
        private static uint VertexBufferObject;
        private static uint Handle;

        private static uint VertexArrayObject;

        static void Main()
        {
            var window = Window.Create(WindowOptions.Default);
            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Resize += Window_Resize;
            window.Closing += Window_Closing;
            window.Run();
        }

        private static void Window_Closing()
        {
            GL.BindBuffer(GLEnum.ArrayBuffer, 0);
            //Note we are crashing here and I don't know why
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteProgram(Handle);
        }

        private static void Window_Resize(Size obj)
        {
            GL.Viewport(obj);
        }


        private unsafe static void Window_Load()
        {
            string vertexShaderSource;
            using (StreamReader reader = new StreamReader("shader.vert", Encoding.UTF8))
            {
                vertexShaderSource = reader.ReadToEnd();
            }

            string fragmentShaderSource;
            using (StreamReader reader = new StreamReader("shader.frag", Encoding.UTF8))
            {
                fragmentShaderSource = reader.ReadToEnd();
            }

            // Get Vertex Shader
            var vertexShader = GL.CreateShader(GLEnum.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);

            GL.CompileShader(vertexShader);
            string infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLogVert))
            {
                Console.WriteLine(infoLogVert);
            }

            // Get Fragment Shader
            var fragmentShader = GL.CreateShader(GLEnum.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.CompileShader(fragmentShader);
            string infoLogFrag = GL.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLogFrag))
            {
                Console.WriteLine(infoLogFrag);
            }

            // Make program, attach, link and cleanup
            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            // Change backgroud colour
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Get vertex buffer
            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(GLEnum.ArrayBuffer, VertexBufferObject);
            fixed (void* vertices = Vertices)
            {
                GL.BufferData(GLEnum.ArrayBuffer, (uint)Vertices.Length * sizeof(float), vertices, GLEnum.StaticDraw);
            }

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(GLEnum.ArrayBuffer, VertexBufferObject);
        }

        private static void Window_Update(double obj)
        {
            GL.Clear((uint)GLEnum.ColorBufferBit);

            GL.UseProgram(Handle);
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(GLEnum.Triangles, 0, 3);
        }
    }
}
