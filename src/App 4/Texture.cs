using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;

namespace App_4
{
    class Texture
    {
        public uint Handle { get; } = GL.GLApi.GenTexture();

        public byte[] Img { get; private set; }

        public unsafe static Texture Load(string path)
        {
            Image<Rgba32> image = Image.Load(path);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            //Get an array of the pixels, in ImageSharp's internal format.
            Rgba32[] tempPixels = image.GetPixelSpan().ToArray();

            //Convert ImageSharp's format into a byte array, so we can use it with OpenGL.
            List<byte> pixels = new List<byte>();

            foreach (Rgba32 p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            var texture = new Texture
            {
                Img = pixels.ToArray()
            };
            GL.GLApi.BindTexture(GLEnum.Texture2D, texture.Handle);

            fixed (void* _pixels = texture.Img)
            {
                GL.GLApi.TexImage2D(GLEnum.Texture2D, 0, (int)GLEnum.Rgba, (uint)image.Width, (uint)image.Height, 0, GLEnum.Rgba, GLEnum.UnsignedByte, _pixels);
            }
            GL.GLApi.GenerateMipmap(GLEnum.Texture2D);
            return texture;
        }
    }
}
