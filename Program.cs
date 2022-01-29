using System;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

using Fwork;

using ImGuiNET;

namespace DataVisualisation3D_OpenTK_
{
	static class Program
	{
		static private GameWindow window;
		static private Dear_ImGui_Sample.ImGuiController imguiController;
		static void Main(string[] args)
		{
			GameWindowSettings gameWindowSettings = new GameWindowSettings();
			gameWindowSettings.RenderFrequency = 0;
			gameWindowSettings.UpdateFrequency = 60d;

			NativeWindowSettings nativeWindowSettings = new NativeWindowSettings();
			nativeWindowSettings.API = OpenTK.Windowing.Common.ContextAPI.OpenGL;
			nativeWindowSettings.APIVersion = Version.Parse("3.3");
			nativeWindowSettings.Size = new Vector2i(800, 600);
			nativeWindowSettings.StartFocused = true;
			nativeWindowSettings.Title = "Data Visualisation 3D";


			window = new GameWindow(gameWindowSettings, nativeWindowSettings);
			window.VSync = VSyncMode.Off;
			//------------------------------


			HeightMap hmap = new HeightMap();
			//------------------------------

			Camera camera = new Camera();
			camera.Mode = Camera.CameraMode.Orbiting;

			

			ShaderProgram shaderProgram = new ShaderProgram();

			

			window.Load += () =>
			{
				imguiController = new Dear_ImGui_Sample.ImGuiController(window.Bounds.Size.X, window.Bounds.Size.Y);

				GL.Enable(EnableCap.DepthTest);


				shaderProgram.Load("3.3.heightmap.vert", "3.3.heightmap.frag");

				GL.Uniform1(GL.GetUniformLocation(shaderProgram.Id, "texture0"), 0);
			};

			window.Resize += (ResizeEventArgs args) =>
			{
				imguiController.WindowResized(window.Bounds.Size.X, window.Bounds.Size.Y);
				GL.Viewport(0, 0, window.Size.X, window.Size.Y);
			};

			window.UpdateFrame += (FrameEventArgs args) =>
			{
				camera.Update(window.KeyboardState);


			};


			window.RenderFrame += (FrameEventArgs args) =>
			{
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				imguiController.Update(window, (float)args.Time);


				GL.UseProgram(shaderProgram.Id);
				camera.Apply();

				hmap.Draw();


				//ImGui.ShowDemoWindow();

				ImGui.Begin("Camera control");
				//////////////////////////// Camera controll buttons
				///
				ImGui.PushButtonRepeat(true);
				ImGui.SameLine(35f);
				if (ImGui.ArrowButton("forward", ImGuiDir.Up)) camera.MoveForward();
				ImGui.SameLine(100f);
				if (ImGui.ArrowButton("up", ImGuiDir.Up)) camera.MoveUp();
				if (ImGui.ArrowButton("left", ImGuiDir.Left)) camera.MoveLeft();
				ImGui.SameLine();
				if (ImGui.ArrowButton("backward", ImGuiDir.Down)) camera.MoveBackward();
				ImGui.SameLine();
				if (ImGui.ArrowButton("right", ImGuiDir.Right)) camera.MoveRight();
				ImGui.SameLine(100f);
				if (ImGui.ArrowButton("down", ImGuiDir.Down)) camera.MoveDown();
				////////////////////////////
				ImGui.End();

				

				imguiController.Render();
				GL.Enable(EnableCap.DepthTest);

				window.SwapBuffers();
			};

			window.KeyDown += (KeyboardKeyEventArgs args) =>
			{
				imguiController.PressChar((char)args.Key);
			};

			window.Unload += () =>
			{
				GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
				GL.BindVertexArray(0);
				GL.UseProgram(0);

				// Delete all the resources.
				hmap.Terminate();

				GL.DeleteProgram(shaderProgram.Id);
			};

			window.Run();

		}		
	}
}

