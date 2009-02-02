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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Interfaces;
using Nibiru.Cameras;

namespace Nibiru.Scenes
{
	public class GameSky : DrawableGameComponent, ILoadable, ICacheable
	{
		private GameCamera camera;
		private SkyContent model = null;
		private string resource = null;

		/// <summary>
		/// The model that is used to render this object.
		/// </summary>
		public SkyContent Model { get { return model; } internal set { model = value; } }

		/// <summary>
		/// The resource that will be used to load the model from the content pipeline.
		/// </summary>
		//public string Resource { get { return resource; } }

		public bool Persist { get; set; }

		public bool Loaded { get; internal set; }

		public GameSky(GameEngine game, GameCamera camera, string resource, string effect)
			: base(game)
		{
			this.camera = camera;
			this.resource = resource;

			Loaded = false;
		}

		public virtual void Load(ContentCache cache)
		{
			Log.Write(this, "Loading game sky.");

			cache.Load(resource, out model);

			Loaded = true;
		}

		public virtual void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading game sky.");

			cache.Unload(resource, out model);

			Loaded = false;
		}

		public override void Draw(GameTime gameTime)
		{
			if (model != null)
				model.Draw(camera.View, camera.Projection);

			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}
	}
}
