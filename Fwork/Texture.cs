using System.Collections.Generic;

using OpenTK.Graphics.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Fwork
{
	public class Texture
	{
		private int id;
		public string Name { get; set; }
		public Texture(string path)
		{
			id = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, id);


			List<byte> colorData = new List<byte>();
			Image<Rgba32> image = Image.Load<Rgba32>(path);
			for (int y = image.Height - 1; y >= 0; y--)
			{
				for (int x = image.Width - 1; x >= 0; x--)
				{
					colorData.Add(image[x, y].R);
					colorData.Add(image[x, y].G);
					colorData.Add(image[x, y].B);
					colorData.Add(image[x, y].A);
				}
			}

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height,
				0, PixelFormat.Rgba, PixelType.UnsignedByte, colorData.ToArray());

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		public void Use(TextureUnit unit)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, id);
		}
	}
}
