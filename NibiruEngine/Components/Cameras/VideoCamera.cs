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
	public class VideoCamera : DrawableGameComponent, IEngineCamera
	{
		public enum Types
		{
			First,
			Third,
			Watch,
			Fly,
		};

		public const float DefaultField = 90.0f;
		public const float DefaultNear = 0.1f;
		public const float DefaultFar = 1000.0f;

		public const float DefaultThirdMinimum = DefaultNear + 1.0f;
		public const float DefaultThirdMaximum = DefaultFar * 0.5f;

		public const float DefaultThirdOffset = DefaultThirdMinimum +
			(DefaultThirdMaximum - DefaultThirdMinimum) * 0.25f;

		private static Vector3 WorldX = new Vector3(1.0f, 0.0f, 0.0f);
		private static Vector3 WorldY = new Vector3(0.0f, 1.0f, 0.0f);
		private static Vector3 WorldZ = new Vector3(0.0f, 0.0f, 1.0f);

		private Types type;

		private bool preferTargetY;

		private float field;
		private float aspect;
		private float near;
		private float far;
		private float totalPitch;
		private float thirdMinimum;
		private float thirdMaximum;
		private float thirdOffset;
		private float firstOffset;

		private Vector3 lens;
		private Vector3 target;
		private Vector3 targetY;
		private Vector3 direction;

		private Vector3 x;
		private Vector3 y;
		private Vector3 z;

		private Quaternion orientation;
		private Matrix view;
		private Matrix projection;

		private Quaternion savedOrientation;
		private Vector3 savedLens;
		private float savedTotalPitch;

		/// <summary>
		/// Property to get and set the camera orientation.
		/// </summary>
		public Quaternion Orientation { get { return orientation; } set { ChangeOrientation(value); } }

		/// <summary>
		/// Property to get and set the camera position.
		/// </summary>
		public Vector3 Position { get { return lens; } set { lens = value; UpdateView(); } }

		/// <summary>
		/// Property to get the perspective projection matrix.
		/// </summary>
		public Matrix Projection { get { return projection; } }

		/// <summary>
		/// Property to get the viewing direction vector.
		/// </summary>
		public Vector3 Direction { get { return direction; } }

		/// <summary>
		/// Property to get the view matrix.
		/// </summary>
		public Matrix View { get { return view; } }

		/// <summary>
		/// Property to get the concatenated view-projection matrix.
		/// </summary>
		public Matrix ViewProjection { get { return view * projection; } }

		/// <summary>
		/// Property to get the camera's local X axis.
		/// </summary>
		public Vector3 X { get { return x; } }

		/// <summary>
		/// Property to get the camera's local Y axis.
		/// </summary>
		public Vector3 Y { get { return y; } }

		/// <summary>
		/// Property to get the camera's local Z axis.
		/// </summary>
		public Vector3 Z { get { return z; } }

		public bool PreferTargetY { get { return preferTargetY; } set { preferTargetY = value; } }

		/// <summary>
		/// Property to get and set the max orbit zoom distance.
		/// </summary>
		public float ThirdMaximum { get { return thirdMaximum; } set { thirdMaximum = value; } }

		/// <summary>
		/// Property to get and set the min orbit zoom distance.
		/// </summary>
		public float ThirdMinimum { get { return thirdMinimum; } set { thirdMinimum = value; } }

		/// <summary>
		/// Property to get and set the distance from the target when orbiting.
		/// </summary>
		public float ThirdOffset { get { return thirdOffset; } set { thirdOffset = value; } }

		/// <summary>
		/// Property to get and set the camera orbit target position.
		/// </summary>
		public Vector3 ThirdTarget { get { return target; } set { target = value; } }

		/// <summary>
		/// The current type this camera implements for viewing.
		/// </summary>
		public Types CurrentType { get { return type; } set { type = value; } }

		public float Near { get { return near; } set { near = value; } }

		public float Far { get { return far; } set { far = value; } }

		/// <summary>
		/// The resources that will be used to load the camera from the content pipeline.
		/// </summary>
		public string Resource { get { return String.Empty; } }

		/// <summary>
		/// Gives the ability to mark an object to be destroyed on the next update call.
		/// </summary>
		public bool Destroy { get; set; }

		/// <summary>
		/// Mark this video camera's resources to stay persistent after destruction.
		/// </summary>
		public bool Persist { get; set; }

		public bool Loaded { get; internal set; }

		public VideoCamera(GameEngine game) : base (game)
		{
			type = Types.Fly;
			preferTargetY = true;

			this.field = DefaultField;
			this.near = DefaultNear;
			this.far = DefaultFar;

			this.totalPitch = 0.0f;
			this.thirdMinimum = DefaultThirdMinimum;
			this.thirdMaximum = DefaultThirdMaximum ;
			this.thirdOffset = DefaultThirdOffset;
			this.firstOffset = 0.0f;

			this.lens = Vector3.Zero;
			this.target = Vector3.Zero;
			this.targetY = Vector3.UnitY;
			x = Vector3.UnitX;
			y = Vector3.UnitY;
			z = Vector3.UnitZ;

			orientation = Quaternion.Identity;
			view = Matrix.Identity;

			savedLens = lens;
			savedOrientation = orientation;
			savedTotalPitch = totalPitch;

			Loaded = false;
		}

		public void Load(ContentCache cache)
		{
			Loaded = true;
			// Video cameras dont need to load anything.
		}

		public void Unload(ContentCache cache)
		{
			Loaded = false;
			// Video cameras dont need to load anything.
		}

		public virtual bool CollidesWith(ICollidable collider)
		{
			// TODO: Code this allow a video camera to collide with another 3D entity.
			return false;
		}

		public void LookAt(Vector3 target)
		{
			LookAt(lens, target, y);
		}

		public void LookAt(Vector3 lens, Vector3 target, Vector3 up)
		{
			this.lens = lens;
			this.target = target;

			z = lens - target;
			z.Normalize();

			direction.X = -z.X;
			direction.Y = -z.Y;
			direction.Z = -z.Z;

			Vector3.Cross(ref up, ref z, out x);
			x.Normalize();

			Vector3.Cross(ref z, ref x, out y);
			y.Normalize();
			x.Normalize();

			view.M11 = x.X;
			view.M21 = x.Y;
			view.M31 = x.Z;
			Vector3.Dot(ref x, ref lens, out view.M41);
			view.M41 = -view.M41;

			view.M12 = y.X;
			view.M22 = y.Y;
			view.M32 = y.Z;
			Vector3.Dot(ref y, ref lens, out view.M42);
			view.M42 = -view.M42;

			view.M13 = z.X;
			view.M23 = z.Y;
			view.M33 = z.Z;
			Vector3.Dot(ref z, ref lens, out view.M43);
			view.M43 = -view.M43;

			view.M14 = 0.0f;
			view.M24 = 0.0f;
			view.M34 = 0.0f;
			view.M44 = 1.0f;

			totalPitch = MathHelper.ToDegrees((float)Math.Asin(view.M23));
			Quaternion.CreateFromRotationMatrix(ref view, out orientation);
		}

		public virtual void Move(float dx, float dy, float dz)
		{
			// The position is always relative to the object being targeted.
			if (type == Types.Third)
				return;

			Vector3 forwards;

			if (type == Types.First)
				forwards = Vector3.Normalize(Vector3.Cross(WorldY, x));
			else
				forwards = direction;

			lens += x * dx;
			lens += WorldY * dy;
			lens += direction * dz;

			Position = lens;
		}

		public virtual void Move(Vector3 direction, Vector3 distance)
		{
			// The position is always relative to the object being targeted.
			if (type == Types.Third)
				return;
			
			lens.X += direction.X * distance.X;
			lens.Y += direction.Y * distance.Y;
			lens.Z += direction.Z * distance.Z;

			UpdateView();
		}

		protected virtual void UpdateView()
		{
			Matrix.CreateFromQuaternion(ref orientation, out view);

			x.X = view.M11;
			x.Y = view.M21;
			x.Z = view.M31;

			y.X = view.M12;
			y.Y = view.M22;
			y.Z = view.M32;

			z.X = view.M13;
			z.Y = view.M23;
			z.Z = view.M33;

			if (type == Types.Third)
			{
				// Calculate the new camera position based on the current
				// orientation. The camera must always maintain the same
				// distance from the target. Use the current offset vector
				// to determine the correct distance from the target.

				lens = target + z * thirdOffset;
			}

			view.M41 = -Vector3.Dot(y, lens);
			view.M42 = -Vector3.Dot(y, lens);
			view.M43 = -Vector3.Dot(z, lens);

			direction.X = -z.X;
			direction.Y = -z.Y;
			direction.Z = -z.Z;
		}

		public void Perspective(float field, float aspect, float near, float far)
		{
			this.field = field;
			this.aspect = aspect;
			this.near = near;
			this.far = far;

			float aspectInv = 1.0f / aspect;
			float e = 1.0f / (float)Math.Tan(MathHelper.ToRadians(field) / 2.0f);
			float fovy = 2.0f * (float)Math.Atan(aspectInv / e);
			float xScale = 1.0f / (float)Math.Tan(0.5f * fovy);
			float yScale = xScale / aspectInv;

			projection.M11 = xScale;
			projection.M12 = 0.0f;
			projection.M13 = 0.0f;
			projection.M14 = 0.0f;

			projection.M21 = 0.0f;
			projection.M22 = yScale;
			projection.M23 = 0.0f;
			projection.M24 = 0.0f;

			projection.M31 = 0.0f;
			projection.M32 = 0.0f;
			projection.M33 = (far + near) / (near - far);
			projection.M34 = -1.0f;

			projection.M41 = 0.0f;
			projection.M42 = 0.0f;
			projection.M43 = (2.0f * far * near) / (near - far);
			projection.M44 = 0.0f;
		}

		public void Rotate(float yaw, float pitch, float roll)
		{
			yaw = -yaw;
			pitch = -pitch;
			roll = -roll;

			switch (type)
			{
				case Types.First:
				case Types.Watch:
					RotateFirst(yaw, pitch);
					break;

				case Types.Fly:
					RotateFly(yaw, pitch, roll);
					break;

				case Types.Third:
					RotateThird(yaw, pitch, roll);
					break;

				default:
					break;
			}

			UpdateView();
		}

		public void UndoRoll()
		{
			if (type == Types.Third)
				LookAt(lens, target, targetY);
			else
				LookAt(lens, lens + Direction, WorldY);
		}

		public void Zoom(float zoom, float min, float max)
		{
			if (type == Types.Third)
			{
				Vector3 offset = lens - target;

				this.thirdOffset = offset.Length();
				offset.Normalize();
				this.thirdOffset += zoom;
				this.thirdOffset = Math.Min(Math.Max(this.thirdOffset, min), max);
				offset *= this.thirdOffset;
				lens = offset + target;
				UpdateView();
			}
			else
			{
				zoom = Math.Min(Math.Max(zoom, min), max);
				Perspective(zoom, aspect, near, far);
			}
		}

		private void ChangeType(Types newType)
		{
			Types previousType = type;

			if (previousType == newType)
				return;

			type = newType;

			switch (newType)
			{
				case Types.First:
					switch (previousType)
					{
						case Types.Fly:
						case Types.Watch:
							lens.Y = firstOffset;
							UpdateView();
							break;

						case Types.Third:
							lens.X = savedLens.X;
							lens.Z = savedLens.Z;
							lens.Y = firstOffset;
							orientation = savedOrientation;
							totalPitch = savedTotalPitch;
							UpdateView();
							break;

						default:
							break;
					}

					UndoRoll();
					break;

				case Types.Watch:
					switch (previousType)
					{
						case Types.Fly:
							UpdateView();
							break;

						case Types.Third:
							lens = savedLens;
							orientation = savedOrientation;
							totalPitch = savedTotalPitch;
							UpdateView();
							break;

						default:
							break;
					}

					UndoRoll();
					break;

				case Types.Fly:
					if (previousType == Types.Third)
					{
						lens = savedLens;
						orientation = savedOrientation;
						totalPitch = savedTotalPitch;
						UpdateView();
					}
					else
					{
						savedLens = lens;
						UpdateView();
					}
					break;

				case Types.Third:
					if (previousType == Types.First)
						firstOffset = lens.Y;

					savedLens = lens;
					savedOrientation = orientation;
					savedTotalPitch = totalPitch;

					targetY = y;

					Vector3 newEye = lens + z * thirdOffset;

					LookAt(newEye, lens, targetY);
					break;

				default:
					break;
			}
		}

		private void ChangeOrientation(Quaternion newOrientation)
		{
			Matrix m = Matrix.CreateFromQuaternion(newOrientation);

			// Store the pitch for this new orientation.
			// First person and spectator behaviors limit pitching to
			// 90 degrees straight up and down.

			float pitch = (float)Math.Asin(m.M23);

			totalPitch = MathHelper.ToDegrees(pitch);

			// First person and spectator behaviors don't allow rolling.
			// Negate any rolling that might be encoded in the new orientation.

			orientation = newOrientation;

			if (type == Types.First || type == Types.Watch)
				LookAt(lens, lens + Vector3.Negate(z), WorldY);

			UpdateView();
		}

		private void RotateFirst(float yaw, float pitch)
		{
			totalPitch += pitch;

			if (totalPitch > 90.0f)
			{
				pitch = 90.0f - (totalPitch - pitch);
				totalPitch = 90.0f;
			}

			if (totalPitch < -90.0f)
			{
				pitch = -90.0f - (totalPitch - pitch);
				totalPitch = -90.0f;
			}

			float yawRadians = MathHelper.ToRadians(yaw);
			float pitchRadians = MathHelper.ToRadians(pitch);
			Quaternion rotation = Quaternion.Identity;

			// Rotate the camera about the world Y axis.
			if (yawRadians != 0.0f)
			{
				Quaternion.CreateFromAxisAngle(ref WorldY, yawRadians, out rotation);
				Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
			}

			// Rotate the camera about its local X axis.
			if (pitchRadians != 0.0f)
			{
				Quaternion.CreateFromAxisAngle(ref WorldX, pitchRadians, out rotation);
				Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
			}
		}

		private void RotateFly(float yaw, float pitch, float roll)
		{
			totalPitch += pitch;

			if (totalPitch > 360.0f)
				totalPitch -= 360.0f;

			if (totalPitch < -360.0f)
				totalPitch += 360.0f;

			float yawRadians = MathHelper.ToRadians(yaw);
			float pitchRadians = MathHelper.ToRadians(pitch);
			float rollRadians = MathHelper.ToRadians(roll);

			Quaternion rotation = Quaternion.CreateFromYawPitchRoll(yawRadians, pitchRadians, rollRadians);
			Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
		}

		private void RotateThird(float yaw, float pitch, float roll)
		{
			float yawRadians = MathHelper.ToRadians(yaw);
			float pitchRadians = MathHelper.ToRadians(pitch);

			if (preferTargetY)
			{
				Quaternion rotation = Quaternion.Identity;

				if (yawRadians != 0.0f)
				{
					Quaternion.CreateFromAxisAngle(ref targetY, yawRadians, out rotation);
					Quaternion.Concatenate(ref rotation, ref orientation, out orientation);
				}

				if (pitchRadians != 0.0f)
				{
					Quaternion.CreateFromAxisAngle(ref WorldX, pitchRadians, out rotation);
					Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
				}
			}
			else
			{
				float rollRadians = MathHelper.ToRadians(roll);
				Quaternion rotation = Quaternion.CreateFromYawPitchRoll(yawRadians, pitchRadians, rollRadians);
				Quaternion.Concatenate(ref orientation, ref rotation, out orientation);
			}
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}
	}
}
