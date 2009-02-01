using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nibiru.Interfaces;

namespace Nibiru.Cameras
{
	public class GameCamera : GameComponent, IEngineCamera
	{
		public enum Actions
		{
			FlyYawLeftPrimary,
			FlyYawLeftAlternate,
			FlyYawRightPrimary,
			FlyYawRightAlternate,

			MoveForwardsPrimary,
			MoveForwardsAlternate,
			MoveBackwardsPrimary,
			MoveBackwardsAlternate,

			MoveDownPrimary,
			MoveDownAlternate,
			MoveUpPrimary,
			MoveUpAlternate,

			ThirdRollLeftPrimary,
			ThirdRollLeftAlternate,
			ThirdRollRightPrimary,
			ThirdRollRightAlternate,

			PitchUpPrimary,
			PitchUpAlternate,
			PitchDownPrimary,
			PitchDownAlternate,

			YawLeftPrimary,
			YawLeftAlternate,
			YawRightPrimary,
			YawRightAlternate,

			RollLeftPrimary,
			RollLeftAlternate,
			RollRightPrimary,
			RollRightAlternate,

			StrafeRightPrimary,
			StrafeRightAlternate,
			StrafeLeftPrimary,
			StrafeLeftAlternate
		};

		private const float DEFAULT_ACCELERATION_X = 8.0f;
		private const float DEFAULT_ACCELERATION_Y = 8.0f;
		private const float DEFAULT_ACCELERATION_Z = 8.0f;
		private const float DEFAULT_MOUSE_SMOOTHING_SENSITIVITY = 0.5f;
		private const float DEFAULT_SPEED_Fly_YAW = 100.0f;
		private const float DEFAULT_SPEED_MOUSE_WHEEL = 1.0f;
		private const float DEFAULT_SPEED_Third_ROLL = 100.0f;
		private const float DEFAULT_SPEED_ROTATION = 0.3f;
		private const float DEFAULT_VELOCITY_X = 1.0f;
		private const float DEFAULT_VELOCITY_Y = 1.0f;
		private const float DEFAULT_VELOCITY_Z = 1.0f;

		private const int MouseSmoothingCacheSize = 10;

		private VideoCamera camera;
		private bool movingAlongPosX;
		private bool movingAlongNegX;
		private bool movingAlongPosY;
		private bool movingAlongNegY;
		private bool movingAlongPosZ;
		private bool movingAlongNegZ;
		private int savedMousePosX;
		private int savedMousePosY;
		private int mouseIndex;
		private float rotationSpeed;
		private float thirdRollSpeed;
		private float flyYawSpeed;
		private float mouseSmoothingSensitivity;
		private float mouseWheelSpeed;
		private Vector3 acceleration;
		private Vector3 currentVelocity;
		private Vector3 velocity;
		private Vector2[] mouseMovement;
		private Vector2[] mouseSmoothingCache;
		private Vector2 smoothedMouseMovement;
		private MouseState currentMouseState;
		private MouseState previousMouseState;
		private KeyboardState currentKeyboardState;
		private Dictionary<Actions, Keys> actionKeys;

		/// <summary>
		/// The resources that will be used to load the camera from the content pipeline.
		/// </summary>
		public string Resource { get { return String.Empty; } }

		public bool Loaded { get; internal set; }

		/// <summary>
		/// Property to get and set the camera's acceleration.
		/// </summary>
		public Vector3 Acceleration
		{
			get { return acceleration; }
			set { acceleration = value; }
		}

		/// <summary>
		/// Property to get and set the camera's behavior.
		/// </summary>
		public VideoCamera.Types CurrentType
		{
			get { return camera.CurrentType; }
			set { camera.CurrentType = value; }
		}

		/// <summary>
		/// Property to get the camera's current velocity.
		/// </summary>
		public Vector3 CurrentVelocity
		{
			get { return currentVelocity; }
		}

		/// <summary>
		/// Property to get and set the Fly behavior's yaw speed.
		/// </summary>
		public float FlyYawSpeed
		{
			get { return flyYawSpeed; }
			set { flyYawSpeed = value; }
		}

		/// <summary>
		/// Property to get and set the sensitivity value used to smooth
		/// mouse movement.
		/// </summary>
		public float MouseSmoothingSensitivity
		{
			get { return mouseSmoothingSensitivity; }
			set { mouseSmoothingSensitivity = value; }
		}

		/// <summary>
		/// Property to get and set the speed of the mouse wheel.
		/// This is used to zoom in and out when the camera is Thirding.
		/// </summary>
		public float MouseWheelSpeed
		{
			get { return mouseWheelSpeed; }
			set { mouseWheelSpeed = value; }
		}

		/// <summary>
		/// Property to get and set the max Third zoom distance.
		/// </summary>
		public float ThirdMaximum
		{
			get { return camera.ThirdMaximum; }
			set { camera.ThirdMaximum = value; }
		}

		/// <summary>
		/// Property to get and set the min Third zoom distance.
		/// </summary>
		public float ThirdMinimum
		{
			get { return camera.ThirdMinimum; }
			set { camera.ThirdMinimum = value; }
		}

		/// <summary>
		/// Property to get and set the distance from the target when Thirding.
		/// </summary>
		public float ThirdOffset
		{
			get { return camera.ThirdOffset; }
			set { camera.ThirdOffset = value; }
		}

		/// <summary>
		/// Property to get and set the Third behavior's rolling speed.
		/// This only applies when PreferTargetYAxisThirding is set to false.
		/// Thirding with PreferTargetYAxisThirding set to true will ignore
		/// any camera rolling.
		/// </summary>
		public float ThirdRollSpeed
		{
			get { return thirdRollSpeed; }
			set { thirdRollSpeed = value; }
		}

		/// <summary>
		/// Property to get and set the camera Third target position.
		/// </summary>
		public Vector3 ThirdTarget
		{
			get { return camera.ThirdTarget; }
			set { camera.ThirdTarget = value; }
		}

		/// <summary>
		/// Property to get and set the camera orientation.
		/// </summary>
		public Quaternion Orientation
		{
			get { return camera.Orientation; }
			set { camera.Orientation = value; }
		}

		/// <summary>
		/// Property to get and set the camera position.
		/// </summary>
		public Vector3 Position
		{
			get { return camera.Position; }
			set { camera.Position = value; }
		}

		/// <summary>
		/// Property to get and set the flag to force the camera
		/// to Third around the Third target's Y axis rather than the camera's
		/// local Y axis.
		/// </summary>
		public bool PreferTargetY
		{
			get { return camera.PreferTargetY; }
			set { camera.PreferTargetY = value; }
		}

		/// <summary>
		/// Property to get the perspective projection matrix.
		/// </summary>
		public Matrix Projection
		{
			get { return camera.Projection; }
		}

		/// <summary>
		/// Property to get and set the mouse rotation speed.
		/// </summary>
		public float RotationSpeed
		{
			get { return rotationSpeed; }
			set { rotationSpeed = value; }
		}

		/// <summary>
		/// Property to get and set the camera's velocity.
		/// </summary>
		public Vector3 Velocity
		{
			get { return velocity; }
			set { velocity = value; }
		}

		/// <summary>
		/// Property to get the viewing direction vector.
		/// </summary>
		public Vector3 Direction
		{
			get { return camera.Direction; }
		}

		/// <summary>
		/// Property to get the view matrix.
		/// </summary>
		public Matrix View
		{
			get { return camera.View; }
		}

		/// <summary>
		/// Property to get the concatenated view-projection matrix.
		/// </summary>
		public Matrix ViewProjection
		{
			get { return camera.ViewProjection; }
		}

		/// <summary>
		/// Property to get the camera's local X axis.
		/// </summary>
		public Vector3 X { get { return camera.X; } }

		/// <summary>
		/// Property to get the camera's local Y axis.
		/// </summary>
		public Vector3 Y { get { return camera.Y; } }

		/// <summary>
		/// Property to get the camera's local Z axis.
		/// </summary>
		public Vector3 Z { get { return camera.Z; } }
		
		public float Near { get { return camera.Near; } set { camera.Near = value; } }

		public float Far { get { return camera.Far; } set { camera.Far = value; } }

		/// <summary>
		/// Mark this video camera's resources to stay persistent after destruction.
		/// </summary>
		public bool Persist { get; set; }

		/// <summary>
		/// Constructs a new instance of the CameraComponent class. The
		/// camera will have a Watch behavior, and will be initially
		/// positioned at the world origin looking down the world negative
		/// z axis. An initial perspective projection matrix is created
		/// as well as setting up initial key bindings to the actions.
		/// </summary>
		public GameCamera(GameEngine game)
			: base(game)
		{
			camera = new VideoCamera(game);
			camera.CurrentType = VideoCamera.Types.Watch;

			movingAlongPosX = false;
			movingAlongNegX = false;
			movingAlongPosY = false;
			movingAlongNegY = false;
			movingAlongPosZ = false;
			movingAlongNegZ = false;

			savedMousePosX = -1;
			savedMousePosY = -1;

			rotationSpeed = DEFAULT_SPEED_ROTATION;
			ThirdRollSpeed = DEFAULT_SPEED_Third_ROLL;
			FlyYawSpeed = DEFAULT_SPEED_Fly_YAW;
			mouseWheelSpeed = DEFAULT_SPEED_MOUSE_WHEEL;
			mouseSmoothingSensitivity = DEFAULT_MOUSE_SMOOTHING_SENSITIVITY;
			acceleration = new Vector3(DEFAULT_ACCELERATION_X, DEFAULT_ACCELERATION_Y, DEFAULT_ACCELERATION_Z);
			velocity = new Vector3(DEFAULT_VELOCITY_X, DEFAULT_VELOCITY_Y, DEFAULT_VELOCITY_Z);
			mouseSmoothingCache = new Vector2[MouseSmoothingCacheSize];

			mouseIndex = 0;
			mouseMovement = new Vector2[2];
			mouseMovement[0].X = 0.0f;
			mouseMovement[0].Y = 0.0f;
			mouseMovement[1].X = 0.0f;
			mouseMovement[1].Y = 0.0f;

			Rectangle clientBounds = game.Window.ClientBounds;
			float aspect = (float)clientBounds.Width / (float)clientBounds.Height;

			Perspective(VideoCamera.DefaultField, aspect, VideoCamera.DefaultNear, VideoCamera.DefaultFar);

			actionKeys = new Dictionary<Actions, Keys>();

			actionKeys.Add(Actions.FlyYawLeftPrimary, Keys.Left);
			actionKeys.Add(Actions.FlyYawLeftAlternate, Keys.A);
			actionKeys.Add(Actions.FlyYawRightPrimary, Keys.Right);
			actionKeys.Add(Actions.FlyYawRightAlternate, Keys.D);
			actionKeys.Add(Actions.MoveForwardsPrimary, Keys.Up);
			actionKeys.Add(Actions.MoveForwardsAlternate, Keys.W);
			actionKeys.Add(Actions.MoveBackwardsPrimary, Keys.Down);
			actionKeys.Add(Actions.MoveBackwardsAlternate, Keys.S);
			actionKeys.Add(Actions.MoveDownPrimary, Keys.Q);
			actionKeys.Add(Actions.MoveDownAlternate, Keys.PageDown);
			actionKeys.Add(Actions.MoveUpPrimary, Keys.E);
			actionKeys.Add(Actions.MoveUpAlternate, Keys.PageUp);
			actionKeys.Add(Actions.ThirdRollLeftPrimary, Keys.Left);
			actionKeys.Add(Actions.ThirdRollLeftAlternate, Keys.A);
			actionKeys.Add(Actions.ThirdRollRightPrimary, Keys.Right);
			actionKeys.Add(Actions.ThirdRollRightAlternate, Keys.D);
			actionKeys.Add(Actions.StrafeRightPrimary, Keys.Right);
			actionKeys.Add(Actions.StrafeRightAlternate, Keys.D);
			actionKeys.Add(Actions.StrafeLeftPrimary, Keys.Left);
			actionKeys.Add(Actions.StrafeLeftAlternate, Keys.A);

			Game.Activated += new EventHandler(HandleGameActivatedEvent);
			Game.Deactivated += new EventHandler(HandleGameDeactivatedEvent);

			UpdateOrder = 1;
		}

		/// <summary>
		/// Initializes the CameraComponent class. This method repositions the
		/// mouse to the center of the game window.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();

			Rectangle clientBounds = Game.Window.ClientBounds;
			Mouse.SetPosition(clientBounds.Width / 2, clientBounds.Height / 2);

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

		/// <summary>
		/// Builds a look at style viewing matrix.
		/// </summary>
		/// <param name="target">The target position to look at.</param>
		public void LookAt(Vector3 target)
		{
			camera.LookAt(target);
		}

		/// <summary>
		/// Builds a look at style viewing matrix.
		/// </summary>
		/// <param name="eye">The camera position.</param>
		/// <param name="target">The target position to look at.</param>
		/// <param name="up">The up direction.</param>
		public void LookAt(Vector3 eye, Vector3 target, Vector3 up)
		{
			camera.LookAt(eye, target, up);
		}

		/// <summary>
		/// Binds an action to a keyboard key.
		/// </summary>
		/// <param name="action">The action to bind.</param>
		/// <param name="key">The key to map the action to.</param>
		public void MapActionToKey(Actions action, Keys key)
		{
			actionKeys[action] = key;
		}

		/// <summary>
		/// Moves the camera by dx world units to the left or right; dy
		/// world units upwards or downwards; and dz world units forwards
		/// or backwards.
		/// </summary>
		/// <param name="dx">Distance to move left or right.</param>
		/// <param name="dy">Distance to move up or down.</param>
		/// <param name="dz">Distance to move forwards or backwards.</param>
		public void Move(float dx, float dy, float dz)
		{
			camera.Move(dx, dy, dz);
		}

		/// <summary>
		/// Moves the camera the specified distance in the specified direction.
		/// </summary>
		/// <param name="direction">Direction to move.</param>
		/// <param name="distance">How far to move.</param>
		public void Move(Vector3 direction, Vector3 distance)
		{
			camera.Move(direction, distance);
		}

		/// <summary>
		/// Builds a perspective projection matrix based on a horizontal field
		/// of view.
		/// </summary>
		/// <param name="fovx">Horizontal field of view in degrees.</param>
		/// <param name="aspect">The viewport's aspect ratio.</param>
		/// <param name="znear">The distance to the near clip plane.</param>
		/// <param name="zfar">The distance to the far clip plane.</param>
		public void Perspective(float fovx, float aspect, float znear, float zfar)
		{
			camera.Perspective(fovx, aspect, znear, zfar);
		}

		/// <summary>
		/// Rotates the camera. Positive angles specify counter clockwise
		/// rotations when looking down the axis of rotation towards the
		/// origin.
		/// </summary>
		/// <param name="headingDegrees">Y axis rotation in degrees.</param>
		/// <param name="pitchDegrees">X axis rotation in degrees.</param>
		/// <param name="rollDegrees">Z axis rotation in degrees.</param>
		public void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			camera.Rotate(headingDegrees, pitchDegrees, rollDegrees);
		}

		/// <summary>
		/// Updates the state of the CameraComponent class.
		/// </summary>
		/// <param name="gameTime">Time elapsed since the last call to Update.</param>
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			UpdateInput();
			UpdateCamera(gameTime);
		}

		/// <summary>
		/// Undo any camera rolling by leveling the camera. When the camera is
		/// Thirding this method will cause the camera to become level with the
		/// Third target.
		/// </summary>
		public void UndoRoll()
		{
			camera.UndoRoll();
		}

		/// <summary>
		/// Zooms the camera. This method functions differently depending on
		/// the camera's current behavior. When the camera is Thirding this
		/// method will move the camera closer to or further away from the
		/// Third target. For the other camera behaviors this method will
		/// change the camera's horizontal field of view.
		/// </summary>
		///
		/// <param name="zoom">
		/// When Thirding this parameter is how far to move the camera.
		/// For the other behaviors this parameter is the new horizontal
		/// field of view.
		/// </param>
		/// 
		/// <param name="minZoom">
		/// When Thirding this parameter is the min allowed zoom distance to
		/// the Third target. For the other behaviors this parameter is the
		/// min allowed horizontal field of view.
		/// </param>
		/// 
		/// <param name="maxZoom">
		/// When Thirding this parameter is the max allowed zoom distance to
		/// the Third target. For the other behaviors this parameter is the max
		/// allowed horizontal field of view.
		/// </param>
		public void Zoom(float zoom, float minZoom, float maxZoom)
		{
			camera.Zoom(zoom, minZoom, maxZoom);
		}

		/// <summary>
		/// Determines which way to move the camera based on player input.
		/// The returned values are in the range [-1,1].
		/// </summary>
		/// <param name="direction">The movement direction.</param>
		private void GetMovementDirection(out Vector3 direction)
		{
			direction.X = 0.0f;
			direction.Y = 0.0f;
			direction.Z = 0.0f;

			if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsPrimary]) ||
				currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveForwardsAlternate]))
			{
				if (!movingAlongNegZ)
				{
					movingAlongNegZ = true;
					currentVelocity.Z = 0.0f;
				}

				direction.Z += 1.0f;
			}
			else
			{
				movingAlongNegZ = false;
			}

			if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsPrimary]) ||
				currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveBackwardsAlternate]))
			{
				if (!movingAlongPosZ)
				{
					movingAlongPosZ = true;
					currentVelocity.Z = 0.0f;
				}

				direction.Z -= 1.0f;
			}
			else
			{
				movingAlongPosZ = false;
			}

			if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpPrimary]) ||
				currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveUpAlternate]))
			{
				if (!movingAlongPosY)
				{
					movingAlongPosY = true;
					currentVelocity.Y = 0.0f;
				}

				direction.Y += 1.0f;
			}
			else
			{
				movingAlongPosY = false;
			}

			if (currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownPrimary]) ||
				currentKeyboardState.IsKeyDown(actionKeys[Actions.MoveDownAlternate]))
			{
				if (!movingAlongNegY)
				{
					movingAlongNegY = true;
					currentVelocity.Y = 0.0f;
				}

				direction.Y -= 1.0f;
			}
			else
			{
				movingAlongNegY = false;
			}

			switch (CurrentType)
			{
				case VideoCamera.Types.First:
				case VideoCamera.Types.Watch:
					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeRightAlternate]))
					{
						if (!movingAlongPosX)
						{
							movingAlongPosX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X += 1.0f;
					}
					else
					{
						movingAlongPosX = false;
					}

					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.StrafeLeftAlternate]))
					{
						if (!movingAlongNegX)
						{
							movingAlongNegX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X -= 1.0f;
					}
					else
					{
						movingAlongNegX = false;
					}

					break;

				case VideoCamera.Types.Fly:
					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlyYawLeftPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.FlyYawLeftAlternate]))
					{
						if (!movingAlongPosX)
						{
							movingAlongPosX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X += 1.0f;
					}
					else
					{
						movingAlongPosX = false;
					}

					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.FlyYawRightPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.FlyYawRightAlternate]))
					{
						if (!movingAlongNegX)
						{
							movingAlongNegX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X -= 1.0f;
					}
					else
					{
						movingAlongNegX = false;
					}
					break;

				case VideoCamera.Types.Third:
					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.ThirdRollLeftPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.ThirdRollLeftAlternate]))
					{
						if (!movingAlongPosX)
						{
							movingAlongPosX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X += 1.0f;
					}
					else
					{
						movingAlongPosX = false;
					}

					if (currentKeyboardState.IsKeyDown(actionKeys[Actions.ThirdRollRightPrimary]) ||
						currentKeyboardState.IsKeyDown(actionKeys[Actions.ThirdRollRightAlternate]))
					{
						if (!movingAlongNegX)
						{
							movingAlongNegX = true;
							currentVelocity.X = 0.0f;
						}

						direction.X -= 1.0f;
					}
					else
					{
						movingAlongNegX = false;
					}
					break;

				default:
					break;
			}
		}

		/// <summary>
		/// Determines which way the mouse wheel has been rolled.
		/// The returned value is in the range [-1,1].
		/// </summary>
		/// <returns>
		/// A positive value indicates that the mouse wheel has been rolled
		/// towards the player. A negative value indicates that the mouse
		/// wheel has been rolled away from the player.
		/// </returns>
		private float GetMouseWheelDirection()
		{
			int currentWheelValue = currentMouseState.ScrollWheelValue;
			int previousWheelValue = previousMouseState.ScrollWheelValue;

			if (currentWheelValue > previousWheelValue)
				return -1.0f;
			else if (currentWheelValue < previousWheelValue)
				return 1.0f;
			else
				return 0.0f;
		}

		/// <summary>
		/// Event handler for when the game window acquires input focus.
		/// </summary>
		/// <param name="sender">Ignored.</param>
		/// <param name="e">Ignored.</param>
		private void HandleGameActivatedEvent(object sender, EventArgs e)
		{
			if (savedMousePosX >= 0 && savedMousePosY >= 0)
				Mouse.SetPosition(savedMousePosX, savedMousePosY);
		}

		/// <summary>
		/// Event hander for when the game window loses input focus.
		/// </summary>
		/// <param name="sender">Ignored.</param>
		/// <param name="e">Ignored.</param>
		private void HandleGameDeactivatedEvent(object sender, EventArgs e)
		{
			MouseState state = Mouse.GetState();

			savedMousePosX = state.X;
			savedMousePosY = state.Y;
		}

		/// <summary>
		/// Filters the mouse movement based on a weighted sum of mouse
		/// movement from previous frames.
		/// <para>
		/// For further details see:
		///  Nettle, Paul "Smooth Mouse Filtering", flipCode's Ask Midnight column.
		///  http://www.flipcode.com/cgi-bin/fcarticles.cgi?show=64462
		/// </para>
		/// </summary>
		/// <param name="x">Horizontal mouse distance from window center.</param>
		/// <param name="y">Vertical mouse distance from window center.</param>
		private void PerformMouseFiltering(float x, float y)
		{
			// Shuffle all the entries in the cache.
			// Newer entries at the front. Older entries towards the back.
			for (int i = mouseSmoothingCache.Length - 1; i > 0; --i)
			{
				mouseSmoothingCache[i].X = mouseSmoothingCache[i - 1].X;
				mouseSmoothingCache[i].Y = mouseSmoothingCache[i - 1].Y;
			}

			// Store the current mouse movement entry at the front of cache.
			mouseSmoothingCache[0].X = x;
			mouseSmoothingCache[0].Y = y;

			float averageX = 0.0f;
			float averageY = 0.0f;
			float averageTotal = 0.0f;
			float currentWeight = 1.0f;

			// Filter the mouse movement with the rest of the cache entries.
			// Use a weighted average where newer entries have more effect than
			// older entries (towards the back of the cache).
			for (int i = 0; i < mouseSmoothingCache.Length; ++i)
			{
				averageX += mouseSmoothingCache[i].X * currentWeight;
				averageY += mouseSmoothingCache[i].Y * currentWeight;
				averageTotal += 1.0f * currentWeight;
				currentWeight *= mouseSmoothingSensitivity;
			}

			// Calculate the new smoothed mouse movement.
			smoothedMouseMovement.X = averageX / averageTotal;
			smoothedMouseMovement.Y = averageY / averageTotal;
		}

		/// <summary>
		/// Averages the mouse movement over a couple of frames to smooth out
		/// the mouse movement.
		/// </summary>
		/// <param name="x">Horizontal mouse distance from window center.</param>
		/// <param name="y">Vertical mouse distance from window center.</param>
		private void PerformMouseSmoothing(float x, float y)
		{
			mouseMovement[mouseIndex].X = x;
			mouseMovement[mouseIndex].Y = y;

			smoothedMouseMovement.X = (mouseMovement[0].X + mouseMovement[1].X) * 0.5f;
			smoothedMouseMovement.Y = (mouseMovement[0].Y + mouseMovement[1].Y) * 0.5f;

			mouseIndex ^= 1;
			mouseMovement[mouseIndex].X = 0.0f;
			mouseMovement[mouseIndex].Y = 0.0f;
		}

		/// <summary>
		/// Dampens the rotation by applying the rotation speed to it.
		/// </summary>
		/// <param name="headingDegrees">Y axis rotation in degrees.</param>
		/// <param name="pitchDegrees">X axis rotation in degrees.</param>
		/// <param name="rollDegrees">Z axis rotation in degrees.</param>
		private void RotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
		{
			headingDegrees *= rotationSpeed;
			pitchDegrees *= rotationSpeed;
			rollDegrees *= rotationSpeed;

			Rotate(headingDegrees, pitchDegrees, rollDegrees);
		}

		/// <summary>
		/// Gathers and updates input from all supported input devices for use
		/// by the CameraComponent class.
		/// </summary>
		private void UpdateInput()
		{
			currentKeyboardState = Keyboard.GetState();

			previousMouseState = currentMouseState;
			currentMouseState = Mouse.GetState();

			Rectangle clientBounds = Game.Window.ClientBounds;

			int centerX = clientBounds.Width / 2;
			int centerY = clientBounds.Height / 2;
			int deltaX = centerX - currentMouseState.X;
			int deltaY = centerY - currentMouseState.Y;

			Mouse.SetPosition(centerX, centerY);

			PerformMouseFiltering((float)deltaX, (float)deltaY);
			PerformMouseSmoothing(smoothedMouseMovement.X, smoothedMouseMovement.Y);
		}

		/// <summary>
		/// Updates the camera's velocity based on the supplied movement
		/// direction and the elapsed time (since this method was last
		/// called). The movement direction is the in the range [-1,1].
		/// </summary>
		/// <param name="direction">Direction moved.</param>
		/// <param name="elapsedTimeSec">Elapsed game time.</param>
		private void UpdateVelocity(ref Vector3 direction, float elapsedTimeSec)
		{
			if (direction.X != 0.0f)
			{
				// Camera is moving along the x axis.
				// Linearly accelerate up to the camera's max speed.

				currentVelocity.X += direction.X * acceleration.X * elapsedTimeSec;

				if (currentVelocity.X > velocity.X)
					currentVelocity.X = velocity.X;
				else if (currentVelocity.X < -velocity.X)
					currentVelocity.X = -velocity.X;
			}
			else
			{
				// Camera is no longer moving along the x axis.
				// Linearly decelerate back to stationary state.

				if (currentVelocity.X > 0.0f)
				{
					if ((currentVelocity.X -= acceleration.X * elapsedTimeSec) < 0.0f)
						currentVelocity.X = 0.0f;
				}
				else
				{
					if ((currentVelocity.X += acceleration.X * elapsedTimeSec) > 0.0f)
						currentVelocity.X = 0.0f;
				}
			}

			if (direction.Y != 0.0f)
			{
				// Camera is moving along the y axis.
				// Linearly accelerate up to the camera's max speed.

				currentVelocity.Y += direction.Y * acceleration.Y * elapsedTimeSec;

				if (currentVelocity.Y > velocity.Y)
					currentVelocity.Y = velocity.Y;
				else if (currentVelocity.Y < -velocity.Y)
					currentVelocity.Y = -velocity.Y;
			}
			else
			{
				// Camera is no longer moving along the y axis.
				// Linearly decelerate back to stationary state.

				if (currentVelocity.Y > 0.0f)
				{
					if ((currentVelocity.Y -= acceleration.Y * elapsedTimeSec) < 0.0f)
						currentVelocity.Y = 0.0f;
				}
				else
				{
					if ((currentVelocity.Y += acceleration.Y * elapsedTimeSec) > 0.0f)
						currentVelocity.Y = 0.0f;
				}
			}

			if (direction.Z != 0.0f)
			{
				// Camera is moving along the z axis.
				// Linearly accelerate up to the camera's max speed.

				currentVelocity.Z += direction.Z * acceleration.Z * elapsedTimeSec;

				if (currentVelocity.Z > velocity.Z)
					currentVelocity.Z = velocity.Z;
				else if (currentVelocity.Z < -velocity.Z)
					currentVelocity.Z = -velocity.Z;
			}
			else
			{
				// Camera is no longer moving along the z axis.
				// Linearly decelerate back to stationary state.

				if (currentVelocity.Z > 0.0f)
				{
					if ((currentVelocity.Z -= acceleration.Z * elapsedTimeSec) < 0.0f)
						currentVelocity.Z = 0.0f;
				}
				else
				{
					if ((currentVelocity.Z += acceleration.Z * elapsedTimeSec) > 0.0f)
						currentVelocity.Z = 0.0f;
				}
			}
		}

		/// <summary>
		/// Moves the camera based on player input.
		/// </summary>
		/// <param name="direction">Direction moved.</param>
		/// <param name="elapsedTimeSec">Elapsed game time.</param>
		private void UpdatePosition(ref Vector3 direction, float elapsedTimeSec)
		{
			if (currentVelocity.LengthSquared() != 0.0f)
			{
				// Only move the camera if the velocity vector is not of zero
				// length. Doing this guards against the camera slowly creeping
				// around due to floating point rounding errors.

				Vector3 displacement = (currentVelocity * elapsedTimeSec) +
					(0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);

				// Floating point rounding errors will slowly accumulate and
				// cause the camera to move along each axis. To prevent any
				// unintended movement the displacement vector is clamped to
				// zero for each direction that the camera isn't moving in.
				// Note that the UpdateVelocity() method will slowly decelerate
				// the camera's velocity back to a stationary state when the
				// camera is no longer moving along that direction. To account
				// for this the camera's current velocity is also checked.

				if (direction.X == 0.0f && (float)Math.Abs(currentVelocity.X) < 1e-6f)
					displacement.X = 0.0f;

				if (direction.Y == 0.0f && (float)Math.Abs(currentVelocity.Y) < 1e-6f)
					displacement.Y = 0.0f;

				if (direction.Z == 0.0f && (float)Math.Abs(currentVelocity.Z) < 1e-6f)
					displacement.Z = 0.0f;

				Move(displacement.X, displacement.Y, displacement.Z);
			}

			// Continuously update the camera's velocity vector even if the
			// camera hasn't moved during this call. When the camera is no
			// longer being moved the camera is decelerating back to its
			// stationary state.

			UpdateVelocity(ref direction, elapsedTimeSec);
		}

		/// <summary>
		/// Updates the state of the camera based on player input.
		/// </summary>
		/// <param name="gameTime">Elapsed game time.</param>
		private void UpdateCamera(GameTime gameTime)
		{
			float elapsedTimeSec = 0.0f;

			if (Game.IsFixedTimeStep)
				elapsedTimeSec = (float)gameTime.ElapsedGameTime.TotalSeconds;
			else
				elapsedTimeSec = (float)gameTime.ElapsedRealTime.TotalSeconds;

			Vector3 direction = new Vector3();

			GetMovementDirection(out direction);

			float dx = 0.0f;
			float dy = 0.0f;
			float dz = 0.0f;

			switch (camera.CurrentType)
			{
				case VideoCamera.Types.First:
				case VideoCamera.Types.Watch:
					dx = smoothedMouseMovement.X;
					dy = smoothedMouseMovement.Y;

					RotateSmoothly(dx, dy, 0.0f);
					UpdatePosition(ref direction, elapsedTimeSec);
					break;

				case VideoCamera.Types.Fly:
					dy = -smoothedMouseMovement.Y;
					dz = smoothedMouseMovement.X;

					RotateSmoothly(0.0f, dy, dz);

					if ((dx = direction.X * FlyYawSpeed * elapsedTimeSec) != 0.0f)
						camera.Rotate(dx, 0.0f, 0.0f);

					direction.X = 0.0f; // ignore yaw motion when updating camera's velocity
					UpdatePosition(ref direction, elapsedTimeSec);
					break;

				case VideoCamera.Types.Third:
					dx = -smoothedMouseMovement.X;
					dy = -smoothedMouseMovement.Y;

					RotateSmoothly(dx, dy, 0.0f);

					if (!camera.PreferTargetY)
					{
						if ((dz = direction.X * ThirdRollSpeed * elapsedTimeSec) != 0.0f)
							camera.Rotate(0.0f, 0.0f, dz);
					}

					if ((dz = GetMouseWheelDirection() * mouseWheelSpeed) != 0.0f)
						camera.Zoom(dz, camera.ThirdMinimum, camera.ThirdMaximum);

					break;

				default:
					break;
			}
		}
	}
}
