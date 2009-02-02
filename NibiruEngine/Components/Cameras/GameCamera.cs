using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nibiru.Interfaces;

namespace Nibiru.Cameras
{
	public class GameCamera : VideoCamera
	{
		private GamePlayer player;

		// Max yaw/pitch variables
		private float totalRoll = MathHelper.Pi; // 180 degrees
		private float currentRoll = 0;
		private float totalPitch = MathHelper.PiOver4; // 90 degrees
		private float currentPitch = 0;
		private float totalYaw = MathHelper.Pi; // 360 degrees
		private float currentYaw = 0;

		private Vector3 angle;

		private float turnSpeed = 90f;

		public bool UseKeyboard { get; set; }
		public bool UseMouse { get; set; }

		public bool AllowMove { get; set; }
		public bool AllowRoll { get; set; }
		public bool AllowPitch { get; set; }
		public bool AllowYaw { get; set; }

		public float TotalRoll { get { return totalRoll; } set { totalRoll = value; } }

		public float TotalPitch { get { return totalPitch; } set { totalPitch = value; } }

		public float TotalYaw { get { return totalYaw; } set { totalYaw = value; } }

		/// <summary>
		/// The player that is controlling this game camera.
		/// </summary>
		public GamePlayer Player { get { return player; } set { player = value; } }

		/// <summary>
		/// Constructor for a game camera that can be controlled by a game player.
		/// </summary>
		/// <param name="game"></param>
		/// <param name="index"></param>
		/// <param name="position"></param>
		/// <param name="target"></param>
		/// <param name="up"></param>
		/// <param name="near"></param>
		/// <param name="far"></param>
		/// <param name="speed"></param>
		public GameCamera(GameEngine game, PlayerIndex index, Vector3 position, Vector3 target, Vector3 up, int near, int far, float speed)
			: base(game, position, target, up, near, far, speed)
		{
			this.player = game.Players.Get(index);

			AllowPitch = true;
			AllowYaw = true;
			AllowRoll = false;
			AllowMove = false;

			UseKeyboard = false;
			UseMouse = true;
		}

		public override void Update(GameTime gameTime)
		{
			int center = Game.Window.ClientBounds.Width / 2;
			float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			Vector3 forward;
			Vector3 left;

			/*if (player.GamePadState.IsConnected)
			{
				angle.X -= player.GamePadState.ThumbSticks.Right.Y * turnSpeed * 0.001f;
				angle.Y += player.GamePadState.ThumbSticks.Right.X * turnSpeed * 0.001f;

				forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y)));
				left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

				position -= forward * Speed * player.GamePadState.ThumbSticks.Left.Y * delta;
				position += left * Speed * player.GamePadState.ThumbSticks.Left.X * delta;

				view = Matrix.Identity;
				view *= Matrix.CreateTranslation(-position);
				view *= Matrix.CreateRotationZ(angle.Z);
				view *= Matrix.CreateRotationY(angle.Y);
				view *= Matrix.CreateRotationX(angle.X);
			}
			else
			{*/
				KeyboardState keyboard = Keyboard.GetState();
				MouseState mouse = Mouse.GetState();
				
				int centerX = center;
				int centerY = center;

				Mouse.SetPosition(centerX, centerY);

				angle.X += MathHelper.ToRadians((mouse.Y - centerY) * turnSpeed * 0.01f); // pitch
				angle.Y += MathHelper.ToRadians((mouse.X - centerX) * turnSpeed * 0.01f); // yaw

				forward = Vector3.Normalize(new Vector3((float)Math.Sin(-angle.Y), (float)Math.Sin(angle.X), (float)Math.Cos(-angle.Y)));
				left = Vector3.Normalize(new Vector3((float)Math.Cos(angle.Y), 0f, (float)Math.Sin(angle.Y)));

				if (keyboard.IsKeyDown(Keys.W))
					position -= forward * Speed * delta;

				if (keyboard.IsKeyDown(Keys.S))
					position += forward * Speed * delta;

				if (keyboard.IsKeyDown(Keys.A))
					position -= left * Speed * delta;

				if (keyboard.IsKeyDown(Keys.D))
					position += left * Speed * delta;
				
				if (keyboard.IsKeyDown(Keys.Z))
					position += Vector3.Down * Speed * delta;
				
				if (keyboard.IsKeyDown(Keys.X))
					position += Vector3.Up * Speed * delta;
				
				view = Matrix.Identity;
				view *= Matrix.CreateTranslation(-position);
				view *= Matrix.CreateRotationZ(angle.Z);
				view *= Matrix.CreateRotationY(angle.Y);
				view *= Matrix.CreateRotationX(angle.X);
			//}
			/*
			if (UseKeyboard)
			{
				if (AllowMove)
				{
					// Move forward/backward
					if (player.IsKeyDown(Keys.W))
						MoveForward();
					if (player.IsKeyDown(Keys.S))
						MoveBackward();

					// Move side to side
					if (player.IsKeyDown(Keys.A))
						MoveLeft();
					if (player.IsKeyDown(Keys.D))
						MoveRight();
				}

				if (AllowYaw)
				{
					// Look from side to side
					if (player.IsKeyDown(Keys.E))
						LookLeft();
					if (player.IsKeyDown(Keys.Q))
						LookRight();
				}

				if (AllowPitch)
				{
					// Look up and down
					if (player.IsKeyDown(Keys.R))
						LookUp();
					if (player.IsKeyDown(Keys.F))
						LookDown();
				}

				if (AllowRoll)
				{
					// Roll to the left and right
					if (player.IsKeyDown(Keys.Z))
						RollLeft();
					if (player.IsKeyDown(Keys.C))
						RollRight();
				}
			}

			if (UseMouse)
			{
				// Check to see if the mouse button is currently right clicked.
				if (player.IsRightButtonPressed())
				{
					// Yaw rotation
					if (AllowYaw)
					{
						float yawAngle = (-MathHelper.PiOver4 / 150) * (player.MouseMovementX());
						//if (Math.Abs(currentYaw + yawAngle) < totalYaw)
						//{
						Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Up, yawAngle));
						currentYaw += yawAngle;
						//}
						Console.WriteLine("Current yaw: " + currentYaw);
					}

					// Pitch rotation
					if (AllowPitch)
					{
						//float testPitch = MathHelper.ToRadians(1);
						//Console.WriteLine("TestPitch: " + testPitch);

						//float pitchAngle = 0.0001f * (player.MouseMovementY());

						float pitchAngle = (MathHelper.PiOver4 / 150) * (player.MouseMovementY());
						//if (Math.Abs(currentPitch + pitchAngle) < totalPitch)
						//{
						if (pitchAngle != 0)
							Console.WriteLine("Pitch Angle: " + pitchAngle);

						Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Cross(Up, Direction), pitchAngle));
						currentPitch += pitchAngle;
						//}
					}
				}
			}*/

			base.Update(gameTime);
		}
	}
}
