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
	public class GameTerrain2 : DrawableGameComponent, ILoadable, ICacheable
	{
		private TerrainContent heightMap = null;
		private GameCamera camera = null;
		private GameSkyDome dome = null;
		private Model model = null;
		private Texture2D texture = null;
		private Effect effect;
		private string modelResource = null;
		private string textureResource = null;
		private string effectResource = null;

		/// <summary>
		/// The model that is used to render this object.
		/// </summary>
		public Model Model { get { return model; } internal set { model = value; } }

		public bool Loaded { get; internal set; }
		public bool Persist { get; set; }

		public GameTerrain2(GameEngine game, GameCamera camera, GameSkyDome dome, string modelResource, string textureResource)
			: base(game)
		{
			Throw.IfNull(this, "dome", dome);
			Throw.IfNull(this, "camera", camera);

			this.dome = dome;
			this.camera = camera;

			this.modelResource = modelResource;
			this.textureResource = textureResource;
			this.effectResource = @"Effects\atmosphere";

			Loaded = false;
		}

		public virtual void Load(ContentCache cache)
		{
			Log.Write(this, "Loading game terrain.");

			cache.Load(modelResource, out model);
			cache.Load(textureResource, out texture);
			cache.Load(effectResource, out effect);

			foreach (ModelMesh mesh in model.Meshes)
			{
				foreach (ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = effect;
				}
			}

			Loaded = true;
		}

		public virtual void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading game terrain.");

			cache.Unload(modelResource, out model);
			cache.Unload(textureResource, out texture);
			cache.Unload(effectResource, out effect);
			
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
				/*Matrix[] boneTransforms = new Matrix[model.Bones.Count];
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
				}*/

				Matrix View = camera.View;
				Matrix Projection = camera.Projection;

				foreach (ModelMesh mesh in model.Meshes)
				{
					Matrix World = Matrix.CreateScale(10.0f) * Matrix.CreateRotationX((float)Math.PI / 2.0f);
					Matrix WorldIT = Matrix.Invert(World);
					WorldIT = Matrix.Transpose(WorldIT);

					foreach (Effect effect in mesh.Effects)
					{
						effect.Parameters["WorldIT"].SetValue(WorldIT);
						effect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
						effect.Parameters["ViewInv"].SetValue(Matrix.Invert(View));
						effect.Parameters["World"].SetValue(World);

						effect.Parameters["DiffuseTexture"].SetValue(texture);
						effect.Parameters["SkyTextureNight"].SetValue(dome.NightTexture);
						effect.Parameters["SkyTextureSunset"].SetValue(dome.SunsetTexture);
						effect.Parameters["SkyTextureDay"].SetValue(dome.DayTexture);

						effect.Parameters["isSkydome"].SetValue(false);

						effect.Parameters["LightDirection"].SetValue(dome.LightDirection);
						effect.Parameters["LightColor"].SetValue(dome.LightColor);
						effect.Parameters["LightColorAmbient"].SetValue(dome.LightColorAmbient);
						effect.Parameters["FogColor"].SetValue(dome.FogColor);
						effect.Parameters["fDensity"].SetValue(0.0003f);
						effect.Parameters["SunLightness"].SetValue(dome.SunLightness);
						effect.Parameters["sunRadiusAttenuation"].SetValue(dome.SunRadiusAttenuation);
						effect.Parameters["largeSunLightness"].SetValue(dome.LargeSunLightness);
						effect.Parameters["largeSunRadiusAttenuation"].SetValue(dome.LargeSunRadiusAttenuation);
						effect.Parameters["dayToSunsetSharpness"].SetValue(dome.DayToSunsetSharpness);
						effect.Parameters["hazeTopAltitude"].SetValue(dome.HazeTopAltitude);

						mesh.Draw();
					}
				}
			}
			else
				Log.Write(this, "No model was found!", LogLevels.Error);


			base.Draw(gameTime);
		}
	}
}
