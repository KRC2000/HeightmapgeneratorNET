using System;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

using ImGuiNET;

using Fwork;

namespace DataVisualisation3D_OpenTK_
{
	class HeightMap
	{
		private int vao, vbo, ebo;

		public List<List<float>> Lods { get; private set; }
		public List<List<uint>> Indices { get; private set; }
		public string[] GetLodsNames()
		{
			string[] lods = new string[Lods.Count];
			for (int i = 0; i < lods.Length; i++)
				lods[i] = $"Lod {i}: {Lods[i].Count/3} vertices";
			return lods;
		}
		public List<Texture> Textures { get; private set; }
		public List<string> GetTexturesNames() 
		{
			List<string> names = new List<string>();
			foreach (Texture t in Textures) { names.Add(t.Name); }
			return names;
		}

		public int vertexFrequency = 10;
		public bool wireframeMode = false;


		// variables used by ui
		public int currentTexture = 0;
		public int lod = 0;

		public HeightMap() 
		{
			GenLods(5, 50, 5);
			LoadTextures("Heightmaps");

			vao = GL.GenVertexArray();
			vbo = GL.GenBuffer();
			ebo = GL.GenBuffer();

			GL.BindVertexArray(vao);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

			GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * Lods[Lods.Count - 1].Count, IntPtr.Zero, BufferUsageHint.DynamicDraw);
			GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * Lods[lod].Count , Lods[lod].ToArray());
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * Indices[Indices.Count - 1].Count, IntPtr.Zero, BufferUsageHint.DynamicDraw);
			GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(uint) * Indices[lod].Count, Indices[lod].ToArray());
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
			
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
			GL.BindVertexArray(0);
		}

		public void UpdateLod()
		{
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

			GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, sizeof(float) * Lods[lod].Count, Lods[lod].ToArray());
			GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, sizeof(uint) * Indices[lod].Count, Indices[lod].ToArray());

			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		private List<float> GeneratePlane(uint vertexFrequency, out List<uint> indices)
		{
			if (vertexFrequency > 500) vertexFrequency = 500;

			List<float> vertices = new List<float>();
			indices = new List<uint>();

			// Grid side lenght
			float gridSide = 2f / vertexFrequency;
			
			for (uint y = 0; y < vertexFrequency+1; y++)
			{
				for (uint x = 0; x < vertexFrequency+1; x++)
				{
					vertices.Add(-1 + x * gridSide); 	// X
					vertices.Add(0); 					// Y
					vertices.Add(-1 + y * gridSide); 	// Z
				}
			}

			for (uint y = 0; y < vertexFrequency; y++)
			{
				for (uint x = 0; x < vertexFrequency; x++)
				{
					// x = 0, y = 1
					// x = 1, y = 0
					indices.Add(x + y * (vertexFrequency+1));
					indices.Add(x + y * (vertexFrequency+1) + 1);
					indices.Add(x + y * (vertexFrequency+1) + (vertexFrequency+1));

					indices.Add(x + y * (vertexFrequency+1) + 1);
					indices.Add(x + y * (vertexFrequency+1) + (vertexFrequency+1));
					indices.Add(x + y * (vertexFrequency+1) + (vertexFrequency+1) + 1);
				}
			}
			return vertices;
		}

		public void Draw()
		{
			if (wireframeMode) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

			if (Textures != null && Textures.Count > 0) Textures[currentTexture].Use(TextureUnit.Texture0);
			GL.BindVertexArray(vao);
			GL.DrawElements(PrimitiveType.Triangles, Indices[lod].Count, DrawElementsType.UnsignedInt, Indices[lod].ToArray());

			if (wireframeMode) GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

			DrawUI();
		}

		public void DrawUI()
		{
			ImGui.Begin("Heightmap settings");
				//////////////////////////// Texture selection
				///
				ImGui.Combo("Texture", ref currentTexture , GetTexturesNames().ToArray(), Textures.Count);
				////////////////////////////

				//////////////////////////// Wireframe mode switch
				///
				ImGui.Checkbox("Wireframe mode", ref wireframeMode);
				////////////////////////////

				if (ImGui.Combo("Level of detail", ref lod, GetLodsNames(), Lods.Count)) UpdateLod();

			ImGui.End();
		}
		public void Terminate()
		{
			GL.DeleteBuffer(vbo);
			GL.DeleteBuffer(ebo);
			GL.DeleteVertexArray(vao);
		}

		private void LoadTextures(string folderName)
		{
			Textures = new List<Texture>();
			List<string> texturePaths = new List<string>();

			texturePaths.AddRange(Directory.GetFiles(folderName, "*.png"));
			texturePaths.AddRange(Directory.GetFiles(folderName, "*.jpeg"));
			texturePaths.AddRange(Directory.GetFiles(folderName, "*.bmp"));
			texturePaths.AddRange(Directory.GetFiles(folderName, "*.gif"));
			texturePaths.AddRange(Directory.GetFiles(folderName, "*.tga"));
			texturePaths.AddRange(Directory.GetFiles(folderName, "*.jpg"));

			foreach (string s in texturePaths)
			{
				Textures.Add(new Texture(s) { Name = s });
			}
		}

		private void GenLods(int minVertFreq, int maxVertFreq, int steps)
		{
			Lods = new List<List<float>>();
			Indices = new List<List<uint>>();
			int stepFreq = (maxVertFreq - minVertFreq) / steps;

			for (int i = 0; i < steps; i++)
			{
				Lods.Add(new List<float>());
				List<uint> temp;
				Lods[Lods.Count - 1] = GeneratePlane((uint)(minVertFreq + i * stepFreq), out temp);
				Indices.Add(temp);
			}
		}
	}
}
