using System;
using System.Drawing;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Common;
using System.IO;
using System.Text;

namespace App_4
{
    class Program
    {
        private static float[] Vertices =
        {
            //Position          Texture coordinates
            0.5f, 0.5f, 0.0f,   1.0f, 1.0f, // top right
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f,  0.0f, 1.0f  // top left
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
            GL.GLApi.Viewport(obj);
        }

        private unsafe static void Window_Render(double obj)
        {
            GL.GLApi.Clear((uint)GLEnum.ColorBufferBit);
            GL.GLApi.UseProgram(Handle);
            GL.GLApi.BindVertexArray(VertexArrayObject);
            //GL.GLApi.DrawArrays(GLEnum.Triangles, 0, 3);
            fixed (void* _ind = Indices)
            {
                GL.GLApi.DrawElements(GLEnum.Triangles, (uint)Indices.Length, GLEnum.UnsignedInt, _ind);
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
            var vertexShader = GL.GLApi.CreateShader(GLEnum.VertexShader);
            GL.GLApi.ShaderSource(vertexShader, vertexShaderSource);

            GL.GLApi.CompileShader(vertexShader);
            string infoLogVert = GL.GLApi.GetShaderInfoLog(vertexShader);
            if (!string.IsNullOrWhiteSpace(infoLogVert))
            {
                Console.WriteLine(infoLogVert);
            }

            // Get Fragment Shader
            var fragmentShader = GL.GLApi.CreateShader(GLEnum.FragmentShader);
            GL.GLApi.ShaderSource(fragmentShader, fragmentShaderSource);

            GL.GLApi.CompileShader(fragmentShader);
            string infoLogFrag = GL.GLApi.GetShaderInfoLog(fragmentShader);
            if (!string.IsNullOrWhiteSpace(infoLogFrag))
            {
                Console.WriteLine(infoLogFrag);
            }

            // Make program, attach, link and cleanup
            Handle = GL.GLApi.CreateProgram();

            GL.GLApi.AttachShader(Handle, vertexShader);
            GL.GLApi.AttachShader(Handle, fragmentShader);

            GL.GLApi.LinkProgram(Handle);

            GL.GLApi.DetachShader(Handle, vertexShader);
            GL.GLApi.DetachShader(Handle, fragmentShader);
            GL.GLApi.DeleteShader(fragmentShader);
            GL.GLApi.DeleteShader(vertexShader);

            // Change backgroud colour
            GL.GLApi.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            var vertexBufferObject = GL.GLApi.GenBuffer();
            GL.GLApi.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
            fixed (void* vertices = Vertices)
            {
                GL.GLApi.BufferData(GLEnum.ArrayBuffer, (uint)Vertices.Length * sizeof(float), vertices, GLEnum.StaticDraw);
            }

            ElementBufferObject = GL.GLApi.GenBuffer();
            GL.GLApi.BindBuffer(GLEnum.ElementArrayBuffer, ElementBufferObject);
            fixed (void* _ind = Indices)
            {
                GL.GLApi.BufferData(GLEnum.ElementArrayBuffer, (uint)(Indices.Length * sizeof(uint)), _ind, GLEnum.StaticDraw);
            }

            VertexArrayObject = GL.GLApi.GenVertexArray();
            GL.GLApi.BindVertexArray(VertexArrayObject);

            GL.GLApi.VertexAttribPointer(0, 3, GLEnum.Float, false, 5 * sizeof(float), 0);
            GL.GLApi.EnableVertexAttribArray(0);
            GL.GLApi.VertexAttribPointer(0, 2, GLEnum.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            //GL.GLApi.VertexAttribPointer(0, 3, GLEnum.Float, false, 3 * sizeof(float), 0);
            GL.GLApi.BindBuffer(GLEnum.ArrayBuffer, vertexBufferObject);
        }
    }
}
