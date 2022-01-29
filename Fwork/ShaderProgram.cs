using System;

using OpenTK.Graphics.OpenGL;

namespace Fwork
{
	class ShaderProgram
	{
		public int Id { get; set; }

		public void Load(string vertShaderPath, string fragShaderPath)
		{
			Id = GL.CreateProgram();

			Shader vertShader = new Shader(),
				fragShader = new Shader();


			vertShader.Load(vertShaderPath, ShaderType.VertexShader);
			fragShader.Load(fragShaderPath, ShaderType.FragmentShader);

			GL.AttachShader(Id, vertShader.Id);
			GL.AttachShader(Id, fragShader.Id);
			GL.LinkProgram(Id);
			GL.DetachShader(Id, vertShader.Id);
			GL.DetachShader(Id, fragShader.Id);
			GL.DeleteShader(vertShader.Id);
			GL.DeleteShader(fragShader.Id);

			string infoLog = GL.GetProgramInfoLog(Id);
			if (!string.IsNullOrEmpty(infoLog))
				throw new Exception(infoLog);
		}
	}

}
