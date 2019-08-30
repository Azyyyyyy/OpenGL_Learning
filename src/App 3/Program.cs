using System;
using System.Drawing;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using System.IO;
using System.Text;

namespace App_3
{
    class Program
    {
        private static GL GL = GL.GetApi();
        private static float[] Vertices = 
        {
            0.5f,  0.5f, 0.0f,  // top right
             0.5f, -0.5f, 0.0f,  // bottom right
            -0.5f, -0.5f, 0.0f,  // bottom left
            -0.5f,  0.5f, 0.0f   // top left
        };
        private static uint[] Indices =
        {  // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        private static uint VertexArrayObject;
        private static uint Handle;
        private static uint ElementBufferObject;

        static void Main()
        {
            var window = Window.Create(WindowOptions.Default);
            window.Load += Window_Load;
            window.Resize += Window_Resize;
            window.Render += Window_Render;
            window.Run();
        }

        private static void Window_Resize(Size obj)
        {
            GL.Viewport(obj);
        }

        private unsafe static void Window_Render(double obj)
        {
            GL.Clear((uint)GLEnum.ColorBufferBit);
            GL.UseProgram(Handle);
            GL.BindVertexArray(VertexArrayObject);
            //GL.DrawArrays(GLEnum.Triangles, 0, 3);
            fixed (void* _ind = Indices)
            {
                GL.DrawElements(GLEnum.Triangles, (uint)Indices.Length, GLEnum.UnsignedInt, _ind);
            }
        }

        private unsafe static void Window_Load()
        {
            string VertexShaderSource;

            using (StreamReader reader = new StreamReader("shader.vert", Encoding.UTF8))
            {
                VertexShaderSource = reader.ReadToEnd();
            }

            string FragmentShaderSource;

            using (StreamReader reader = new StreamReader("shader.frag", Encoding.UTF8))
            {
                FragmentShaderSource = reader.ReadToEnd();
            }

            var VertexShader = GL.CreateShader(GLEnum.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);

            var FragmentShader = GL.CreateShader(GLEnum.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);

            GL.CompileShader(VertexShader);
            var vertexCompileError = GL.GetShaderInfoLog(VertexShader);
            if (!string.IsNullOrWhiteSpace(vertexCompileError))
            {
                Console.WriteLine($"Ah shit here we go again:\r\n{vertexCompileError}");
            }

            GL.CompileShader(FragmentShader);
            var fragmentCompileError = GL.GetShaderInfoLog(FragmentShader);
            if (!string.IsNullOrWhiteSpace(fragmentCompileError))
            {
                Console.WriteLine($"Ah shit here we go again:\r\n{fragmentCompileError}");
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, VertexShader);
            GL.AttachShader(Handle, FragmentShader);

            GL.LinkProgram(Handle);

            GL.DetachShader(Handle, VertexShader);
            GL.DetachShader(Handle, FragmentShader);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            GL.ClearColor(Color.BurlyWood);
            var _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(GLEnum.ArrayBuffer, _vertexBufferObject);
            fixed (void* vertices = Vertices)
            {
                GL.BufferData(GLEnum.ArrayBuffer, (uint)Vertices.Length * sizeof(float), vertices, GLEnum.StaticDraw);
            }

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(GLEnum.ElementArrayBuffer, ElementBufferObject);
            fixed (void* _ind = Indices)
            {
                GL.BufferData(GLEnum.ElementArrayBuffer, (uint)(Indices.Length * sizeof(uint)), _ind, GLEnum.StaticDraw);
            }

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(GLEnum.ArrayBuffer, _vertexBufferObject);

            //GL.LinkProgram(Handle)
        }
    }
}
