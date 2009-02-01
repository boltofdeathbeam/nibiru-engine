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
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Interfaces;

namespace Nibiru.Sprites
{
	/// <summary>
	/// Video sprite that can be controlled by a player's input.
	/// </summary>
	public class ControlledSprite : VideoSprite
	{
		private PlayerIndex playerIndex;

		/// <summary>
		/// The player index of the player which controls this sprite.
		/// </summary>
		public PlayerIndex PlayerIndex { get { return playerIndex; } }

		/// <summary>
		/// Constructor for a player controlled animated sprite.
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		/// <param name="frameWidth"></param>
		/// <param name="frameHeight"></param>
		/// <param name="frame"></param>
		/// <param name="sheetX"></param>
		/// <param name="sheetY"></param>
		public ControlledSprite(PlayerIndex playerIndex, string resource, Vector2 position, Color color, int frameWidth, int frameHeight, int frame, int sheetX, int sheetY)
			: base(resource, position, color, frameWidth, frameHeight, frame, sheetX, sheetY)
		{
			this.playerIndex = playerIndex;
		}

		/// <summary>
		/// Constructor for a player controlled static sprite.
		/// </summary>
		/// <param name="playerIndex"></param>
		/// <param name="resource"></param>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public ControlledSprite(PlayerIndex playerIndex, string resource, Vector2 position, Color color)
			: base(resource, position, color)
		{
			this.playerIndex = playerIndex;
		}

		/// <summary>
		/// Updates the sprite's position based on the player's input.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			int width = 0;
			float x = Position.X;
			float y = Position.Y;

			if (IsAnimated)
				width = FrameWidth;
			else
				width = Texture.Width;

			// Check for player input.
			GamePlayer player = (Manager.Game as GameEngine).Players.Get(playerIndex);

			if (player.IsKeyDown(Keys.Left))
				x -= Speed.X;
			if (player.IsKeyDown(Keys.Right))
				x += Speed.X;
			if (player.IsKeyDown(Keys.Up))
				y -= Speed.Y;
			if (player.IsKeyDown(Keys.Down))
				y += Speed.Y;

			if (x < 0)
				x = 0;
			if (y < 0)
				y = 0;

			if (x > Manager.Game.Window.ClientBounds.Width - FrameWidth)
				x = Manager.Game.Window.ClientBounds.Width - FrameWidth;
			if (y > Manager.Game.Window.ClientBounds.Height - FrameHeight)
				y = Manager.Game.Window.ClientBounds.Height - FrameHeight;

			SetPosition(x, y);

			base.Update(gameTime);
		}
	}
}
