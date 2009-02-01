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
	public class AutomatedSprite : VideoSprite
	{
		/// <summary>
		/// Constructor for a automated animated sprite.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		/// <param name="frameWidth"></param>
		/// <param name="frameHeight"></param>
		/// <param name="frame"></param>
		/// <param name="sheetX"></param>
		/// <param name="sheetY"></param>
		public AutomatedSprite(string resource, Vector2 position, Color color, int frameWidth, int frameHeight, int frame, int sheetX, int sheetY)
			: base(resource, position, color, frameWidth, frameHeight, frame, sheetX, sheetY)
		{

		}

		/// <summary>
		/// Constructor for a automated static sprite.
		/// </summary>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public AutomatedSprite(string resource, Vector2 position, Color color)
			: base(resource, position, color)
		{

		}

		/// <summary>
		/// Updates the controlled sprite's position based on speed and direction.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			int width = 0;
			int height = 0;

			if (IsAnimated)
			{
				width = FrameWidth;
				height = FrameHeight;
			}
			else
			{
				width = Texture.Width;
				height = Texture.Height;
			}

			// This is where we actually move the sprite position.
			SetPosition(Position.X + Speed.X, Position.Y + Speed.Y);

			float speedX = Speed.X, speedY = Speed.Y;

			// This is logic checks to act when sprite comes across certain conditions.
			if (Position.X > Manager.Game.Window.ClientBounds.Width - width || Position.X < 0)
				speedX = Speed.X * -1;
			if (Position.Y > Manager.Game.Window.ClientBounds.Height - height || Position.Y < 0)
				speedY = Speed.Y * -1;

			SetSpeed(speedX, speedY);

			base.Update(gameTime);
		}
	}
}
