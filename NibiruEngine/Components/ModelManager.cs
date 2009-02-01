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

using Nibiru.Interfaces;
using Nibiru.Models;

namespace Nibiru
{
	public class ModelManager : AbstractManager
	{
		private List<IEngineModel> modelList;

		public ModelManager(GameEngine engine)
			: base(engine)
		{
			modelList = new List<IEngineModel>();
		}

		public override void Load(ContentCache cache)
		{

		}

		public override void Unload(ContentCache cache)
		{

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
		/// Attaches a video model to the list controlled by this manager. This will load the model from the content pipeline and link the sprite with the manager for drawing and updating.
		/// </summary>
		/// <param name="sprite"></param>
		public void Attach(IEngineModel model)
		{
			if (!modelList.Contains(model))
			{
				model.Manager = this;
				modelList.Add(model);
			}
			else
				Log.Write(this, "Model being added was already in list.", LogLevels.Warning);
		}

		/// <summary>
		/// Marks a sprite to be removed on next update. This breaks the link between manager and sprite, it will no longer be drawn or updated.
		/// </summary>
		/// <param name="sprite"></param>
		public void Remove(IEngineModel model)
		{
			model.Destroy = true;
		}

		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			List<IEngineModel> toRemove = new List<IEngineModel>();

			// Update all the video sprites
			foreach (IEngineModel model in modelList)
			{
				if (model.Destroy)
					toRemove.Add(model);
				else
					model.Update(gameTime);
			}

			foreach (IEngineModel model in toRemove)
			{
				modelList.Remove(model);
				model.Manager = null;

				if (model is VideoModel)
				{
					// Clear the mesh
				}
				else if (model is VideoPrimitive)
				{
					if (model.Resource != null)
						(model as VideoPrimitive).Texture = null;
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// Allows the game component to draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{
			// Draw all the video models
			foreach (IEngineModel model in modelList)
				model.Draw(gameTime);

			base.Draw(gameTime);
		}
	}
}
