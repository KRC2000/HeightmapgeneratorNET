using System;
using System.IO;

using OpenTK.Graphics.OpenGL;

namespace Fwork
{
	class Shader
	{
		public int Id { get; set; }

		public void Load(string path, ShaderType shaderType)
		{
			Id = GL.CreateShader(shaderType);
			GL.ShaderSource(Id, File.ReadAllText(path));
			GL.CompileShader(Id);

			string infoLog = GL.GetShaderInfoLog(Id);
			if (!string.IsNullOrEmpty(infoLog))
				throw new Exception(infoLog);
		}
	}
}
