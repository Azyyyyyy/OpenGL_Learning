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
        {  
            // note that we start from 0!
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        };
        private static uint VertexArrayObject;
        private static uint Handle;
        private static uint ElementBufferObject;

        static void Main()
        {
            var window = Window.Create(WindowOptions.Default);
            window.Size = new Size(500, 250);
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

            var vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
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
            GL.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        }
    }
}
