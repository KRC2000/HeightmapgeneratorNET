using System;
using System.Collections.Generic;
using System.Text;

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Fwork
{
	class Camera
	{
		public enum CameraMode
		{
			Orbiting, Free
		}

		private Matrix4 projectionMat = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), 800 / 600, 0.01f, 100f);
		private Matrix4 viewMat;


		private Vector3 position = new Vector3(0f, 0f, 5f);
		public CameraMode Mode { get; set; } = CameraMode.Free;
		public Vector3 Right { get; private set; } = new Vector3(1f, 0f, 0f);
		public Vector3 Front { get; private set; } = new Vector3(0f, 0f, -1f);
		public Vector3 Position
		{
			get { return position; }
			set
			{
				position = value;
				RecalculateViewMatrix();
			}
		}

		private float rotation = 0;
		public float Rotation
		{
			get { return rotation; }
			private set { rotation = value; RecalculateViewMatrix(); }
		}

		private float toTargetDistance = 5f;
		public float ToTargetDistance
		{
			get { return toTargetDistance; }
			set { toTargetDistance = value; RecalculateViewMatrix(); }
		}
		public Vector3 Target { get; private set; } = new Vector3(0f, 0f, 0f);

		public Camera()
		{
			viewMat = Matrix4.LookAt(new Vector3((float)MathHelper.Sin(Position.X), Position.Y, Position.Z), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
		}

		public void Update(KeyboardState keyboardState)
		{
			if (Mode == CameraMode.Orbiting)
			{
				if (keyboardState.IsKeyDown(Keys.Space)) Position = new Vector3(Position.X, Position.Y + 0.05f, Position.Z);
				if (keyboardState.IsKeyDown(Keys.LeftControl)) Position = new Vector3(Position.X, Position.Y - 0.05f, Position.Z);
				if (keyboardState.IsKeyDown(Keys.A)) Rotation -= 0.01f;
				if (keyboardState.IsKeyDown(Keys.D)) Rotation += 0.01f;
				if (keyboardState.IsKeyDown(Keys.S)) ToTargetDistance += 0.1f;
				if (keyboardState.IsKeyDown(Keys.W)) if (ToTargetDistance > 0.1f) ToTargetDistance -= 0.1f;

			}
			else if (Mode == CameraMode.Free)
			{
				if (keyboardState.IsKeyDown(Keys.W)) Position += Front / 100;
				if (keyboardState.IsKeyDown(Keys.A)) Position -= Right / 100;
				if (keyboardState.IsKeyDown(Keys.S)) Position -= Front / 100;
				if (keyboardState.IsKeyDown(Keys.D)) Position += Right / 100;
			}
		}

		public void MoveForward()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					if (ToTargetDistance > 0.1f) ToTargetDistance -= 0.1f;
					break;
				case CameraMode.Free:
					Position += Front / 100;
					break;
				default:
					break;
			}
		}

		public void MoveBackward()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					ToTargetDistance += 0.1f;
					break;
				case CameraMode.Free:
					Position -= Front / 100;
					break;
				default:
					break;
			}
		}

		public void MoveLeft()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					Rotation -= 0.01f;
					break;
				case CameraMode.Free:
					Position -= Right / 100;
					break;
				default:
					break;
			}
		}

		public void MoveRight()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					Rotation += 0.01f;
					break;
				case CameraMode.Free:
					Position += Right / 100;
					break;
				default:
					break;
			}
		}

		public void MoveUp()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					Position = new Vector3(Position.X, Position.Y + 0.05f, Position.Z);
					break;
				case CameraMode.Free:
					break;
				default:
					break;
			}
		}

		public void MoveDown()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					Position = new Vector3(Position.X, Position.Y - 0.05f, Position.Z);
					break;
				case CameraMode.Free:
					break;
				default:
					break;
			}
		}

		public void Apply()
		{

			GL.UniformMatrix4(GL.GetUniformLocation(GL.GetInteger(GetPName.CurrentProgram), "projection_mat4"), false, ref projectionMat);
			GL.UniformMatrix4(GL.GetUniformLocation(GL.GetInteger(GetPName.CurrentProgram), "view_mat4"), false, ref viewMat);
		}

		public void RecalculateViewMatrix()
		{
			switch (Mode)
			{
				case CameraMode.Orbiting:
					viewMat = Matrix4.LookAt(new Vector3((float)MathHelper.Sin(Rotation) * ToTargetDistance, Position.Y, (float)MathHelper.Cos(Rotation) * ToTargetDistance), Target, new Vector3(0, 1, 0));
					break;
				case CameraMode.Free:
					viewMat = Matrix4.LookAt(Position, Target, new Vector3(0, 1, 0));
					break;
				default:
					break;
			}
		}
	}
}
