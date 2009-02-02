/* Nibiru Engine - XNA 3.0 2D/3D Game Library
 * 
 * Copyright (C) 2009 Philippe Durand <draekz@gmail.com>
 * This file is part of Nibiru Engine.
 * 
 * License: GNU General Public License (GPL) v3
 * 
 * Nibiru Engine is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Nibiru Engine is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Nibiru Engine.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nibiru.Interfaces;

namespace Nibiru.Cameras
{
	/// <summary>
	/// Video cameras allow 3D objects to be rendered to the screen.
	/// </summary>
	public class VideoCamera : DrawableGameComponent, IEngineCamera
	{
		internal Vector3 position;
		internal Vector3 direction;
		internal Vector3 up;
		internal int farPlane;
		internal int nearPlane;

		/// <summary>
		/// The coordinates vector of the camera's position.
		/// </summary>
		public Vector3 Position { get { return position; } set { position = value; } }

		/// <summary>
		/// The direction vector the camera is pointing.
		/// </summary>
		public Vector3 Direction { get { return direction; } set { direction = value; } }

		/// <summary>
		/// The upward vector of the Y axis.
		/// </summary>
		public Vector3 Up { get { return up; } set { up = value; } }

		/// <summary>
		/// The near plane distance used for clipping objects too close to the camera.
		/// </summary>
		public int NearPlane { get { return nearPlane; } set { nearPlane = value; } }

		/// <summary>
		/// The far plane distance used for clipping objects too far from the camera.
		/// </summary>
		public int FarPlane { get { return farPlane; } set { farPlane = value; } }

		/// <summary>
		/// The speed at which the camera moves.
		/// </summary>
		public float Speed { get; protected set; }

		/// <summary>
		/// The projection matrix of the camera.
		/// </summary>
		public Matrix Projection { get; protected set; }

		public bool Loaded { get; set; }

		/// <summary>
		/// The view matrix of the camera. This is calculated in realtime.
		/// </summary>
		internal Matrix view;
		public Matrix View
		{
			get
			{
				return view;
			}
		}

		/// <summary>
		/// Constructor for a new video camera.
		/// </summary>
		/// <param name="game"></param>
		/// <param name="position">The coordinates vector of the camera's position.</param>
		/// <param name="target">The target vector of the camera's direction.</param>
		/// <param name="up">The upward vector of the Y axis.</param>
		/// <param name="nearPlane">The near plane distance used for clipping objects too close to the camera.</param>
		/// <param name="farPlane">The far plane distance used for clipping objects too far from the camera.</param>
		/// <param name="speed">The speed at which the camera moves.</param>
		public VideoCamera(GameEngine game, Vector3 position, Vector3 target, Vector3 up, int nearPlane, int farPlane, float speed)
			: base(game)
		{
			this.Position = position;
			this.Direction = target - position;
			this.Direction.Normalize();
			this.up = up;
			this.nearPlane = nearPlane;
			this.farPlane = farPlane;
			this.Speed = speed;

			CreateLookAt();

			// Create the projection of the camera, using a 45 degree viewing angle.
			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
				(float)Game.Window.ClientBounds.Width /
				(float)Game.Window.ClientBounds.Height, nearPlane, farPlane);

			Loaded = false;
		}

		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
			// TODO: Add your initialization code here
			Mouse.SetPosition(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 2);

			base.Initialize();
		}

		public void Load(ContentCache cache)
		{
			Loaded = true;
		}

		public void Unload(ContentCache cache)
		{
			Loaded = false;
		}

		public bool CollidesWith(ICollidable collider)
		{
			return false;
		}

		/// <summary>
		/// Moves the camera forward on its current position vector.
		/// </summary>
		public virtual void MoveForward()
		{
			Position = Position + Direction * Speed;
		}

		/// <summary>
		/// Moves the camera backwards on its current position vector.
		/// </summary>
		public virtual void MoveBackward()
		{
			Position = Position - Direction * Speed;
		}

		/// <summary>
		/// Move the camera to the left.
		/// </summary>
		public virtual void MoveLeft()
		{
			Position = Position + Vector3.Cross(Up, Direction) * Speed;
		}

		/// <summary>
		/// Move the camera to the right.
		/// </summary>
		public virtual void MoveRight()
		{
			Position = Position - Vector3.Cross(Up, Direction) * Speed;
		}

		/// <summary>
		/// Yaw the camera to the left.
		/// </summary>
		public virtual void LookLeft()
		{
			Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Up, (-MathHelper.PiOver4) * Speed));
		}

		/// <summary>
		/// Yaw the camera to the right.
		/// </summary>
		public virtual void LookRight()
		{
			Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Up, (MathHelper.PiOver4) * Speed));
		}

		/// <summary>
		/// Pitch the camera upwards.
		/// </summary>
		public virtual void LookUp()
		{
			Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Cross(Up, Direction), (-MathHelper.PiOver4 / 100) * Speed));
		}

		/// <summary>
		/// Pitch the camera downwards.
		/// </summary>
		public virtual void LookDown()
		{
			Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Vector3.Cross(Up, Direction), (MathHelper.PiOver4 / 100) * Speed));
		}

		/// <summary>
		/// Roll the camera to the left.
		/// </summary>
		public virtual void RollLeft()
		{
			Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(Direction, MathHelper.PiOver4 / 45));
		}

		/// <summary>
		/// Roll the camera to the right.
		/// </summary>
		public virtual void RollRight()
		{
			Up = Vector3.Transform(Up, Matrix.CreateFromAxisAngle(Direction, -MathHelper.PiOver4 / 45));
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			CreateLookAt();

			base.Update(gameTime);
		}

		public void CreateLookAt()
		{
			view = Matrix.CreateLookAt(Position, Position + Direction, Up);
		}
	}
}
