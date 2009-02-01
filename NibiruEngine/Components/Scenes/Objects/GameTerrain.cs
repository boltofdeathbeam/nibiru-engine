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
using Nibiru.Models;
using Nibiru.Cameras;

namespace Nibiru.Scenes
{
	public class GameTerrain : DrawableGameComponent, ILoadable, ICacheable
	{
		private TerrainContent heightMap;
		private GameCamera camera;
		private Model model = null;
		private string resource = null;

		/// <summary>
		/// The model that is used to render this object.
		/// </summary>
		public Model Model { get { return model; } internal set { model = value; } }

		/// <summary>
		/// The resource path that will be used to load the model from the content pipeline.
		/// </summary>
		public string Resource { get { return resource; } }

		public bool Loaded { get; internal set; }

		public bool Persist { get; set; }

		public GameTerrain(GameEngine game, GameCamera camera, string resource)
			: base(game)
		{
			this.camera = camera;
			this.resource = resource;

			Loaded = false;
		}

		public virtual void Load(ContentCache cache)
		{
			Log.Write(this, "Loading game terrain.");

			cache.Load(resource, out model);

			if (model != null)
			{
				heightMap = model.Tag as TerrainContent;

				if (heightMap == null)
				{
					string message = "The terrain model did not have a TerrainMap " +
					"object attached. Are you sure you are using the " +
					"TerrainProcessor?";
					throw new InvalidOperationException(message);
				}
			}

			Loaded = true;
		}

		public virtual void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading game terrain.");

			cache.Unload(resource, out model);
			heightMap = null;

			Loaded = false;
		}

		public bool IsOnHeightMap(Vector3 position)
		{
			return heightMap.IsOnHeightmap(position);
		}

		public float GetHeight(Vector3 position)
		{
			return heightMap.GetHeight(position);
		}

		public override void Update(GameTime gameTime)
		{
			// We don't want the camera to go beneath the heightmap, so if the camera is
			// over the terrain, we'll move it up.
			/*if (heightMap.IsOnHeightmap(camera.Position))
			{
				// we don't want the camera to go beneath the terrain's height +
				// a small offset.
				float minimumHeight =
					heightMap.GetHeight(camera.Position) + 100;

				if (camera.Position.Y < minimumHeight)
				{
					camera.Position = new Vector3(camera.Position.X, minimumHeight, camera.Position.Z);
				}
			}*/

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (model != null)
			{
				Matrix[] boneTransforms = new Matrix[model.Bones.Count];
				model.CopyAbsoluteBoneTransformsTo(boneTransforms);

				foreach (ModelMesh mesh in model.Meshes)
				{
					foreach (BasicEffect effect in mesh.Effects)
					{
						effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.Identity;
						effect.View = camera.View;
						effect.Projection = camera.Projection;

						effect.EnableDefaultLighting();
						effect.PreferPerPixelLighting = true;

						// Set the specular lighting to match the sky color.
						effect.SpecularColor = new Vector3(0.6f, 0.4f, 0.2f);
						effect.SpecularPower = 8;

						// Set the fog to match the distant mountains
						// that are drawn into the sky texture.
						effect.FogEnabled = true;
						effect.FogColor = Vector3.Zero;
						effect.FogStart = 1000;
						effect.FogEnd = 3200;
					}

					mesh.Draw();
				}
			}

			base.Draw(gameTime);
		}
	}
}
