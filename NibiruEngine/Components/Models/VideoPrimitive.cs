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
using Nibiru.Cameras;

namespace Nibiru.Models
{
	public class VideoPrimitive : IEngineModel
	{
		internal ModelManager manager;
		internal GameCamera camera;
		internal string resource;
		internal List<VideoEffect> effects = new List<VideoEffect>();
		internal List<CollisionEventData> collisions = new List<CollisionEventData>();

		private bool useTexture = false;
		private Texture2D texture = null;
		private VertexPositionTexture[] vertexTextures;
		private VertexPositionColor[] vertexColors;
		
		/// <summary>
		/// The sprite manager that will be updating and drawing this sprite.
		/// </summary>
		public IEngineManager Manager { get { return manager; } set { manager = value as ModelManager; } }

		/// <summary>
		/// Always returns null. The primitives do not use resources.
		/// </summary>
		public string Resource { get { return resource; } }

		public bool Loaded { get; internal set; }

		/// <summary>
		/// The texture that will be used for the single sprite or animation sprite sheet.
		/// </summary>
		public Texture2D Texture { get { return texture; } set { texture = value; } }

		/// <summary>
		/// Sets whether the model is to be rendered using only the basic effect.
		/// </summary>
		public bool UseBasicEffect { get; set; }

		/// <summary>
		/// Gives the ability to mark an object to be destroyed on the next update call.
		/// </summary>
		public bool Destroy { get; set; }

		/// <summary>
		/// Mark this video primitive's resources to stay persistent after destruction.
		/// </summary>
		public bool Persist { get; set; }

		public VideoPrimitive(GameCamera camera, int size)
		{
			this.camera = camera;
			this.resource = String.Empty;
			this.useTexture = false;
			this.vertexColors = new VertexPositionColor[size];

			Loaded = false;
		}

		public VideoPrimitive(GameCamera camera, string resource, int size)
		{
			this.camera = camera;
			this.resource = resource;
			this.useTexture = true;
			this.vertexTextures = new VertexPositionTexture[size];

			Loaded = false;
		}

		public virtual void Load(ContentCache cache)
		{
			cache.Load(Resource, out texture);

			Loaded = true;
		}

		public virtual void Unload(ContentCache cache)
		{
			cache.Unload(Resource, out texture);

			Loaded = false;
		}

		public void Attach(VideoEffect effect)
		{
			if (effect != null)
			{
				effect.Load(Manager.Game.Content.Load<Effect>(effect.Resource));

				effects.Add(effect);
			}
			else
				Log.Write(this, "Unable to add NULL effect!", LogLevels.Error);
		}

		public void Detatch(VideoEffect effect)
		{
			if (effect != null && effects.Contains(effect))
				effects.Remove(effect);
			else
				Log.Write(this, "Unable to remove effect!", LogLevels.Error);
		}

		public void SetVertex(int index, Vector3 position, Color color)
		{
			if (useTexture)
			{
				Log.Write(this, "Attempting to add color vertex to a texture primitive type!", LogLevels.Error);
				return;
			}

			vertexColors[index] = new VertexPositionColor(position, color);
		}

		public virtual Matrix GenerateWorldMatrix()
		{
			return Matrix.Identity;
		}

		public virtual bool CollidesWith(ICollidable collider)
		{
			// TODO: Code this
			return false;
		}

		public virtual void Update(GameTime gameTime)
		{

		}

		public virtual void Draw(GameTime gameTime)
		{
			// Setup the graphics device to receive the vertex
			if (useTexture)
				Manager.Game.GraphicsDevice.VertexDeclaration =
					new VertexDeclaration(Manager.Game.GraphicsDevice, VertexPositionTexture.VertexElements);
			else
				Manager.Game.GraphicsDevice.VertexDeclaration =
					new VertexDeclaration(Manager.Game.GraphicsDevice, VertexPositionColor.VertexElements);

			if (effects == null || effects.Count < 1)
			{
				// We have not added any special effects, use the basic
				BasicEffect effect = new BasicEffect(Manager.Game.GraphicsDevice, null);
				effect.World = GenerateWorldMatrix();
				effect.View = camera.View;
				effect.Projection = camera.Projection;

				if (useTexture)
				{
					effect.Texture = texture;
					effect.TextureEnabled = true;
				}
				else
					effect.VertexColorEnabled = true;

				// Begin effect and draw for each pass
				effect.Begin();
				foreach (EffectPass pass in effect.CurrentTechnique.Passes)
				{
					pass.Begin();
					if (useTexture)
						Manager.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, vertexTextures, 0, vertexTextures.Length / 2);
					else
						Manager.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertexColors, 0, vertexColors.Length / 2);
					pass.End();
				}
				effect.End();
			}
			else
			{
				// Go through each effect
				foreach (VideoEffect effect in effects)
				{
					// Setup the technique to be performed
					foreach (VideoTechnique technique in effect.Techniques)
					{
						effect.SetCurrentTechnique(technique);

						// Call the technique's setup
						technique.Setup(camera, GenerateWorldMatrix());

						effect.Begin();
						foreach (EffectPass pass in effect.GetCurrentTechnique().Passes)
						{
							pass.Begin();
							if (useTexture)
								Manager.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, vertexTextures, 0, vertexTextures.Length / 2);
							else
								Manager.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertexColors, 0, vertexColors.Length / 2);
							pass.End();
						}
						effect.End();
					}
				}
			}
		}
	}
}
