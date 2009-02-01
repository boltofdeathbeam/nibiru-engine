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
using Nibiru.Sprites;

namespace Nibiru
{
	/// <summary>
	/// The manager component that updates and draws engine sprites.
	/// </summary>
	public class SpriteManager : AbstractManager
	{
		private SpriteBatch spriteBatch;
		private List<IEngineSprite> spriteList;

		/// <summary>
		/// The sprite batch that will be used to draw the sprites this manager controls.
		/// </summary>
		public SpriteBatch SpriteBatch { get { return spriteBatch; } }

		/// <summary>
		/// Constructor for a sprite manager, requires a game engine.
		/// </summary>
		/// <param name="game"></param>
		public SpriteManager(GameEngine game)
			: base(game)
		{
			spriteList = new List<IEngineSprite>();
		}

		public override void Load(ContentCache cache)
		{
			spriteBatch = new SpriteBatch(Game.GraphicsDevice);
		}

		public override void Unload(ContentCache cache)
		{
			spriteBatch = null;
		}

		protected override void LoadContent()
		{
			Load(Game.Cache);

			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			Unload(Game.Cache);

			base.UnloadContent();
		}

		/// <summary>
		/// Attaches a video sprite to the list controlled by this manager. This will load the texture from the content pipeline and link the sprite with the manager for drawing and updating.
		/// </summary>
		/// <param name="sprite"></param>
		public void Attach(IEngineSprite sprite)
		{
			if (!spriteList.Contains(sprite))
			{
				sprite.Destroy = false;
				sprite.Manager = this;
				spriteList.Add(sprite);
			}
			else
				Log.Write(this, "Sprite being added was already in list.", LogLevels.Warning);
		}

		/// <summary>
		/// Marks a sprite to be removed on next update. This breaks the link between manager and sprite, it will no longer be drawn or updated.
		/// </summary>
		/// <param name="sprite"></param>
		public void Remove(IEngineSprite sprite)
		{
			sprite.Destroy = true;
		}

		/// <summary>
		/// Allows the sprite manager to update itself, all its sprites, and look for collision events.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			List<IEngineSprite> toRemove = new List<IEngineSprite>();

			// Update all the video sprites
			foreach (IEngineSprite sprite in spriteList)
			{
				if (sprite.Destroy)
					toRemove.Add(sprite);
				else
					sprite.Update(gameTime);
			}

			foreach (IEngineSprite sprite in toRemove)
			{
				spriteList.Remove(sprite);
				sprite.Manager = null;

				if (sprite is VideoSprite)
				{
					(sprite as VideoSprite).Texture = null;
				}
				else if (sprite is VideoFont)
				{
					(sprite as VideoFont).Font = null;
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// Allows the sprite manager to draw itself and all its sprites.
		/// </summary>
		/// <param name="gameTime"></param>
		public override void Draw(GameTime gameTime)
		{
			spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);

			// Draw all video sprites
			foreach (IEngineSprite sprite in spriteList)
			{
				sprite.Draw(gameTime);
			}

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
