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
	public class VideoFont : IEngineSprite
	{
		private string resource;
		private SpriteManager manager = null;
		private SpriteFont font = null;
		private string text = "";
		private Vector2 position = Vector2.Zero;
		private int depth = 0;
		private float rotation = 0.0f;
		private Color color = Color.White;
		private int scale = 1;

		/// <summary>
		/// The resource path that will be used to load the texture from the content pipeline.
		/// </summary>
		//public string Resource { get { return resource; } }

		public bool Loaded { get; internal set; }

		/// <summary>
		/// The sprite manager that will be updating and drawing this sprite.
		/// </summary>
		public IEngineManager Manager { get { return manager; } set { manager = value as SpriteManager; } }

		/// <summary>
		/// The internal sprite font used to render this video font object.
		/// </summary>
		public SpriteFont Font { get { return font; } set { font = value; } }

		/// <summary>
		/// The text that will be displayed by this video font object.
		/// </summary>
		public string Text { get { return text; } set { text = value; } }

		/// <summary>
		/// The current position of the sprite on screen.
		/// </summary>
		public Vector2 Position { get { return position; } }

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
		/// Gives the ability to mark an object to be destroyed on the next update call.
		/// </summary>
		public bool Destroy { get; set; }

		/// <summary>
		/// Mark this video font's resources to stay persistent after destruction.
		/// </summary>
		public bool Persist { get; set; }

		/// <summary>
		/// Constructor for a new video font sprite which displays text to the screen.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public VideoFont(string text, string resource, Vector2 position, Color color)
		{
			this.text = text;
			this.resource = resource;
			this.position = position;
			this.color = color;

			Loaded = false;
		}

		/// <summary>
		/// Constructor used to quickly create a font sprite at (0,0) with no tint.
		/// </summary>
		/// <param name="resource"></param>
		public VideoFont(string text, string resource)
			: this(text, resource, Vector2.Zero, Color.White)
		{
			Loaded = false;
		}

		public void Load(ContentCache cache)
		{
			cache.Load(resource, out font);

			Loaded = true;
		}

		public void Unload(ContentCache cache)
		{
			cache.Unload(resource, out font);

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

		public bool CollidesWith(ICollidable collider)
		{
			// TODO: Code this
			return false;
		}

		/// <summary>
		/// Allows objects that derive from VideoFont to perform update calls in the game loop.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update(GameTime gameTime)
		{

		}

		/// <summary>
		/// Draws the video font to the screen uding the manager's sprite batch.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Draw(GameTime gameTime)
		{
			if (manager == null)
			{
				Log.Write(this, "Unable to draw sprite, not added to sprite manager.", LogLevels.Warning);

				return;
			}

			manager.SpriteBatch.DrawString(font, text, position, color, rotation, Vector2.Zero, scale, SpriteEffects.None, depth);
		}
	}
}
