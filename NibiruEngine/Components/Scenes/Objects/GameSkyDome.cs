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
	public class GameSkyDome : DrawableGameComponent, ILoadable, ICacheable
	{
		private string resource;

		private Model dome;
		private Effect effect;
		private Texture2D day, sunset, night;
		private GameCamera camera;

		private float position = 0.0f;
		private float fPhi = 0.0f;

		private bool realTime;

		private Vector4 lightDirection = new Vector4(100.0f, 100.0f, 100.0f, 1.0f);
		private Vector4 lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		private Vector4 lightColorAmbient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
		private Vector4 fogColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		private float fDensity = 0.000028f;
		private float sunLightness = 0.3f;
		private float sunRadiusAttenuation = 256.0f;
		private float largeSunLightness = 0.05f;
		private float largeSunRadiusAttenuation = 2.0f;
		private float dayToSunsetSharpness = 1.5f;
		private float hazeTopAltitude = 100.0f;

		/// <summary>
		/// The resource that will be used to load the model from the content pipeline.
		/// </summary>
		//public string Resource { get { return resource; } }

		public bool Persist { get; set; }

		public bool Loaded { get; internal set; }

		public Texture2D NightTexture { get { return night; } }
		public Texture2D DayTexture { get { return day; } }
		public Texture2D SunsetTexture { get { return sunset; } }

		public Vector4 LightDirection { get { return lightDirection; } set { lightDirection = value; } }

		public Vector4 LightColor { get { return lightColor; } set { lightColor = value; } }

		public Vector4 LightColorAmbient { get { return lightColorAmbient; } set { lightColorAmbient = value; } }

		public Vector4 FogColor { get { return fogColor; } set { fogColor = value; } }

		public float FogDensity { get { return fDensity; } set { fDensity = value; } }

		public float SunLightness { get { return sunLightness; } set { sunLightness = value; } }

		public float SunRadiusAttenuation { get { return sunRadiusAttenuation; } set { sunRadiusAttenuation = value; } }

		public float LargeSunLightness { get { return largeSunLightness; } set { largeSunLightness = value; } }

		public float LargeSunRadiusAttenuation { get { return largeSunRadiusAttenuation; } set { largeSunRadiusAttenuation = value; } }

		public float DayToSunsetSharpness { get { return dayToSunsetSharpness; } set { dayToSunsetSharpness = value; } }

		public float HazeTopAltitude { get { return hazeTopAltitude; } set { hazeTopAltitude = value; } }

		/// <summary>
		/// Gets/Sets Theta value
		/// </summary>
		public float Position { get { return position; } set { position = value; } }

		/// <summary>
		/// Gets/Sets Phi value
		/// </summary>
		public float Phi { get { return fPhi; } set { fPhi = value; } }

		/// <summary>
		/// Gets/Sets actual time computation
		/// </summary>
		public bool RealTime { get { return realTime; } set { realTime = value; } }

		public GameSkyDome(GameEngine game, GameCamera camera)
			: base(game)
		{
			this.camera = camera;
		}

		public void Load(ContentCache cache)
		{
			cache.Load (@"Models\skydome", out dome);
			cache.Load (@"Effects\atmosphere", out effect);

			cache.Load (@"Textures\daysky", out day);
			cache.Load (@"Textures\sunset", out sunset);
			cache.Load (@"Textures\nightsky", out night);

			effect.CurrentTechnique = effect.Techniques["SkyDome"];

			foreach (ModelMesh mesh in dome.Meshes)
			{
				foreach (ModelMeshPart part in mesh.MeshParts)
				{
					part.Effect = effect;
				}
			}

			realTime = true;
		}

		public void Unload(ContentCache cache)
		{
			cache.Unload(@"Models\skydome", out dome);
			cache.Unload(@"Effects\atmosphere", out effect);

			cache.Unload(@"Textures\daysky", out day);
			cache.Unload(@"Textures\sunset", out sunset);
			cache.Unload(@"Textures\nightsky", out night);
		}

		public override void Update(GameTime gameTime)
		{
			if (realTime)
			{
				this.position = ((float)DateTime.Now.Hour * 60 + DateTime.Now.Minute) * (float)(Math.PI) / 12.0f / 60.0f;
			}

			LightDirection = this.GetDirection();
			LightDirection.Normalize();

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			Matrix[] boneTransforms = new Matrix[dome.Bones.Count];
			dome.CopyAbsoluteBoneTransformsTo(boneTransforms);

			Matrix View = camera.View;
			Matrix Projection = camera.Projection;

			GraphicsDevice.DepthStencilState = DepthStencilState.None;

			foreach (ModelMesh mesh in dome.Meshes)
			{
				Matrix World = boneTransforms[mesh.ParentBone.Index] *
					Matrix.CreateTranslation(camera.Position.X, camera.Position.Y - 50.0f, camera.Position.Z);

				Matrix WorldIT = Matrix.Invert(World);
				WorldIT = Matrix.Transpose(WorldIT);

				foreach (Effect effect in mesh.Effects)
				{
					effect.Parameters["WorldIT"].SetValue(WorldIT);
					effect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
					effect.Parameters["ViewInv"].SetValue(Matrix.Invert(View));
					effect.Parameters["World"].SetValue(World);

					effect.Parameters["SkyTextureNight"].SetValue(night);
					effect.Parameters["SkyTextureSunset"].SetValue(sunset);
					effect.Parameters["SkyTextureDay"].SetValue(day);

					effect.Parameters["isSkydome"].SetValue(true);

					effect.Parameters["LightDirection"].SetValue(LightDirection);
					effect.Parameters["LightColor"].SetValue(LightColor);
					effect.Parameters["LightColorAmbient"].SetValue(LightColorAmbient);
					effect.Parameters["FogColor"].SetValue(FogColor);
					effect.Parameters["fDensity"].SetValue(FogDensity);
					effect.Parameters["SunLightness"].SetValue(SunLightness);
					effect.Parameters["sunRadiusAttenuation"].SetValue(SunRadiusAttenuation);
					effect.Parameters["largeSunLightness"].SetValue(LargeSunLightness);
					effect.Parameters["largeSunRadiusAttenuation"].SetValue(LargeSunRadiusAttenuation);
					effect.Parameters["dayToSunsetSharpness"].SetValue(DayToSunsetSharpness);
					effect.Parameters["hazeTopAltitude"].SetValue(HazeTopAltitude);

					mesh.Draw();
				}
			}

			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		private Vector4 GetDirection()
		{
			float y = (float)Math.Cos((double)this.position);
			float x = (float)(Math.Sin((double)this.position) * Math.Cos(this.fPhi));
			float z = (float)(Math.Sin((double)this.position) * Math.Sin(this.fPhi));
			float w = 1.0f;

			return new Vector4(x, y, z, w);
		}
	}
}
