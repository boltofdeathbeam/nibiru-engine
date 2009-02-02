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
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Interfaces;

namespace Nibiru.Sprites
{
	/// <summary>
	/// A video sprite is an abstract 2D object that provides animation, movement and pixel collision capabilities.
	/// </summary>
	public class VideoSprite : IEngineSprite
	{
		private SpriteManager manager = null;
		private int timeSinceLastFrame = 0;
		private Point frameSize = Point.Zero;
		private Point sheetSize = Point.Zero;
		private bool paused = false;
		private int collisionRectOffset = 5;
		public Color[] pixelData;

		private Texture2D texture = null;
		private string resource;
		private Vector2 position = Vector2.Zero;
		private Vector2 speed = Vector2.Zero;
		private Color color = Color.White;
		private float rotation = 0.0f;
		private Vector2 origin = Vector2.Zero;
		private SpriteEffects effect = SpriteEffects.None;
		private int depth = 0;
		
		private int scale = 1;
		private int millisecondsPerFrame = 60;
		private Point currentFrame = Point.Zero;
		
		/// <summary>
		/// The sprite manager that will be updating and drawing this sprite.
		/// </summary>
		public IEngineManager Manager { get { return manager; } set { manager = value as SpriteManager; } }

		/// <summary>
		/// The texture that will be used for the single sprite or animation sprite sheet.
		/// </summary>
		public Texture2D Texture
		{
			get { return texture; }
			set
			{
				texture = value;

				if (texture != null)
				{
					pixelData = new Color[texture.Width * texture.Height];
					texture.GetData<Color>(pixelData);
				}
			}
		}

		/// <summary>
		/// The resources that will be used to load the sprite from the content pipeline.
		/// </summary>
		//public string Resource { get { return resource; } }

		/// <summary>
		/// The current position of the sprite on screen.
		/// </summary>
		public Vector2 Position { get { return position; } }

		/// <summary>
		/// The speed at which the sprite moves about the screen, measured in pixels per update.
		/// </summary>
		public Vector2 Speed { get { return speed; } set { speed = value; } }

		/// <summary>
		/// The tint color the sprite will be drawn on the screen with. By default it is White.
		/// </summary>
		public Color Color { get { return color; } set { color = value; } }

		/// <summary>
		/// The rotation value the sprite will be drawn with.
		/// </summary>
		public float Rotation { get { return rotation; } set { rotation = value; } }

		/// <summary>
		/// The layer depth value of the sprite, which determines the order it is drawn in.
		/// </summary>
		public int Depth { get { return depth; } set { depth = value; } }

		/// <summary>
		/// The scale used to draw the sprite in a certain size. By default it is 1.
		/// </summary>
		public int Scale { get { return scale; } set { scale = value; } }

		/// <summary>
		/// This value per frame will allow changing of the animation speed.
		/// </summary>
		public int MillisecondsPerFrame { get { return millisecondsPerFrame; } set { millisecondsPerFrame = value; } }

		/// <summary>
		/// The width value of a single frame within a sprite sheet.
		/// </summary>
		public int FrameWidth { get { return frameSize.X; } }

		/// <summary>
		/// The height value of a single frame within a sprite sheet.
		/// </summary>
		public int FrameHeight { get { return frameSize.Y; } }

		/// <summary>
		/// The number of columns in the sprite sheet.
		/// </summary>
		public int SheetX { get { return sheetSize.X; } }

		/// <summary>
		/// The number of rows in the sprite sheet.
		/// </summary>
		public int SheetY { get { return sheetSize.Y; } }

		/// <summary>
		/// The current frame coordinates (x,y) on the current sprite sheet used to display a specific frame in the animation.
		/// </summary>
		public Point CurrentFrame { get { return currentFrame; } }

		/// <summary>
		/// Used to determine whether this sprite contains the necessary details to perform animation.
		/// </summary>
		public bool IsAnimated { get; private set; }

		/// <summary>
		/// Gives the ability to mark an object to be destroyed on the next update call.
		/// </summary>
		public bool Destroy { get; set; }

		/// <summary>
		/// Mark this video sprite's resources to stay persistent after destruction.
		/// </summary>
		public bool Persist { get; set; }

		public bool Loaded { get; internal set; }

		/// <summary>
		/// The bound box rectangle used to assist in pixel based 2D collision detection.
		/// </summary>
		public Rectangle BoundBox
		{
			get
			{
				if (IsAnimated)
					return new Rectangle((int)position.X + collisionRectOffset, (int)position.Y + collisionRectOffset, FrameWidth - (collisionRectOffset * 2), FrameHeight - (collisionRectOffset * 2));
				else
					return new Rectangle((int)position.X + collisionRectOffset, (int)position.Y + collisionRectOffset, texture.Width - (collisionRectOffset * 2), texture.Height - (collisionRectOffset * 2));
			}
		}

		private List<CollisionEventData> collisions = new List<CollisionEventData>();

		/// <summary>
		/// Constructor for new video sprite used to display an animation.
		/// </summary>
		/// <param name="resource">The resource path that will be used to load the texture from the content pipeline.</param>
		/// <param name="position">The current position of the sprite on screen.</param>
		/// <param name="color">The tint color the sprite will be drawn on the screen with. By default it is White.</param>
		/// <param name="frameWidth">The width value of a single frame within a sprite sheet.</param>
		/// <param name="frameHeight">The height value of a single frame within a sprite sheet.</param>
		/// <param name="frame">The current frame coordinates (x,y) on the current sprite sheet used to display a specific frame in the animation.</param>
		/// <param name="sheetX">The number of columns in the sprite sheet.</param>
		/// <param name="sheetY">The number of rows in the sprite sheet.</param>
		public VideoSprite(string resource, Vector2 position, Color color, int frameWidth, int frameHeight, int frame, int sheetX, int sheetY)
		{
			this.resource = resource;
			this.position = position;
			this.color = color;
			frameSize = new Point(frameWidth, frameHeight);
			currentFrame = new Point(frame, frame);
			sheetSize = new Point(sheetX, sheetY);

			IsAnimated = true;
			Loaded = false;
		}

		/// <summary>
		/// Constructor for a new video sprite that has a single frame.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public VideoSprite(string resource, Vector2 position, Color color)
		{
			this.resource = resource;
			this.position = position;
			this.color = color;

			IsAnimated = false;
			Loaded = false;
		}

		/// <summary>
		/// Constructor used to quickly create a basic non-animated video sprite.
		/// </summary>
		/// <param name="resource"></param>
		public VideoSprite(string resource)
			: this(resource, Vector2.Zero, Color.White)
		{
			Loaded = false;
		}

		public void Load(ContentCache cache)
		{
			Log.Write(this, "Loading texture resource for video sprite.");

			cache.Load(resource, out texture);

			pixelData = new Color[texture.Width * texture.Height];
			texture.GetData<Color>(pixelData);

			Loaded = true;
		}

		public void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading texture resource for video sprite.");

			cache.Unload(resource, out texture);

			pixelData = null;

			Loaded = false;
		}

		/// <summary>
		/// Updates the position of the sprite on the screen.
		/// </summary>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		public void SetPosition(float x, float y)
		{
			this.position.X = x;
			this.position.Y = y;
		}

		public void SetSpeed(float x, float y)
		{
			this.speed.X = x;
			this.speed.Y = y;
		}

		/// <summary>
		/// Registers a collision between two video models, resulting in the delegate specified being called.
		/// </summary>
		/// <param name="one"></param>
		/// <param name="two"></param>
		/// <param name="collision"></param>
		public void RegisterCollision(VideoSprite collider, bool repeat, CollisionEvent collision)
		{
			collisions.Add(new CollisionEventData(this, collider, repeat, collision));
		}

		public bool CollidesWith(ICollidable collider)
		{
			int top = Math.Max(this.BoundBox.Top, (collider as VideoSprite).BoundBox.Top);
			int bottom = Math.Min(this.BoundBox.Bottom, (collider as VideoSprite).BoundBox.Bottom);
			int left = Math.Max(this.BoundBox.Left, (collider as VideoSprite).BoundBox.Left);
			int right = Math.Min(this.BoundBox.Right, (collider as VideoSprite).BoundBox.Right);

			for (int y = top; y < bottom; y++)
			{
				for (int x = left; x < right; x++)
				{
					Color colA = pixelData[(y - BoundBox.Top) * BoundBox.Width + (x - BoundBox.Left)];
					Color colB = (collider as VideoSprite).pixelData[(y - (collider as VideoSprite).BoundBox.Top) *
						(collider as VideoSprite).BoundBox.Width + (x - (collider as VideoSprite).BoundBox.Left)];

					if (colA.A != 0 && colB.A != 0)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Sets the current frame coordinates to be displayed from the sprite sheet.
		/// </summary>
		/// <param name="frame"></param>
		public void SetCurrentFrame(int frameX, int frameY)
		{
			this.currentFrame.X = frameX;
			this.currentFrame.Y = frameY;
		}

		/// <summary>
		/// Causes the animation to pause at the current frame.
		/// </summary>
		public void Pause()
		{
			this.paused = true;
		}

		/// <summary>
		/// Causes the animation to resumes from the current frame.
		/// </summary>
		public void Resume()
		{
			this.paused = false;
		}

		/// <summary>
		/// Updates the animation frame if this is a animated video sprite. Override this to add functionality.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update(GameTime gameTime)
		{
			if (IsAnimated)
			{
				if (!paused)
				{
					timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
					if (timeSinceLastFrame > millisecondsPerFrame)
					{
						timeSinceLastFrame -= millisecondsPerFrame;
						++currentFrame.X;
						if (currentFrame.X >= sheetSize.X)
						{
							currentFrame.X = 0;
							++currentFrame.Y;
							if (currentFrame.Y >= sheetSize.Y)
								currentFrame.Y = 0;
						}
					}
				}
			}

			List<CollisionEventData> toRemove = new List<CollisionEventData>();

			// Now check for collisions
			foreach (CollisionEventData collision in collisions)
			{
				if ((collision.Collidee as VideoSprite).BoundBox.Intersects((collision.Collider as VideoSprite).BoundBox))
				{
					// The bound boxes DO collide, now lets go further and do a pixel check
					if ((collision.Collidee as VideoSprite).CollidesWith((collision.Collider as VideoSprite)))
					{
						collision.Event(this, new CollisionEventArgs(collision.Collidee, collision.Collider, collision.Repeat));

						if (!collision.Repeat)
							toRemove.Add(collision);
					}
				}
			}

			foreach (CollisionEventData collision in toRemove)
				collisions.Remove(collision);
		}

		/// <summary>
		/// Draws the sprite using the sprite manager's batch.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Draw(GameTime gameTime)
		{
			if (manager == null)
			{
				Log.Write(this, "Unable to draw sprite, not added to sprite manager.", LogLevels.Warning);

				return;
			}

			if (IsAnimated)
			{
				manager.SpriteBatch.Draw(texture, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y),
					color, rotation, origin, scale, effect, depth);
			}
			else
			{
				manager.SpriteBatch.Draw(texture, position, null, color, rotation, origin, scale, effect, depth);
			}
		}
	}
}
