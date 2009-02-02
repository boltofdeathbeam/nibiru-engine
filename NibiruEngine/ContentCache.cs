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
using Microsoft.Xna.Framework.Content;

using Nibiru.Interfaces;
using Nibiru.Scenes;

namespace Nibiru
{
	public class ContentCache
	{
		private Game game;
		private Dictionary<string, Texture2D> textures = null;
		private Dictionary<string, Model> models = null;
		private Dictionary<string, SkyContent> skies = null;
		private Dictionary<string, SpriteFont> fonts = null;
		private Dictionary<string, Effect> effects = null;
		private Dictionary<string, ParticleContent> particles = null;

		/// <summary>
		/// The game engine that this content cache is used by.
		/// </summary>
		public Game Game { get { return game; } }

		internal ContentCache(GameEngine game)
		{
			this.game = game;

			// Create all the dictionaries that we need.
			textures = new Dictionary<string, Texture2D>();
			models = new Dictionary<string, Model>();
			fonts = new Dictionary<string, SpriteFont>();
			effects = new Dictionary<string, Effect>();
			skies = new Dictionary<string, SkyContent>();
			particles = new Dictionary<string, ParticleContent>();
		}

		public void Load(string resource, out Texture2D obj)
		{
			if (textures.ContainsKey(resource))
			{
				if (textures[resource] != null)
				{
					obj = textures[resource];
					return;
				}
				else
				{
					obj = null;
					textures.Remove (resource);
					Log.Write(this, "Resource ["+resource+"] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<Texture2D>(resource);
				}
				catch (ContentLoadException)
				{
					Log.Write(this, "Resource [" + resource + "], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					textures.Add(resource, obj);
			}
		}

		public void Load(string resource, out Model obj)
		{
			if (models.ContainsKey(resource))
			{
				if (models[resource] != null)
				{
					obj = models[resource];
					return;
				}
				else
				{
					obj = null;
					models.Remove(resource);
					Log.Write(this, "Resource [" + resource + "] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<Model>(resource);
				}
				catch (ContentLoadException)
				{
					Log.Write(this, "Resource [" + resource + "], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					models.Add(resource, obj);
			}
		}

		public void Load(string resource, out SpriteFont obj)
		{
			if (fonts.ContainsKey(resource))
			{
				if (fonts[resource] != null)
				{
					obj = fonts[resource];
					return;
				}
				else
				{
					obj = null;
					fonts.Remove(resource);
					Log.Write(this, "Resource [" + resource + "] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<SpriteFont>(resource);
				}
				catch (ContentLoadException)
				{
					Log.Write(this, "Resource ["+resource+"], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					fonts.Add(resource, obj);
			}
		}

		public void Load(string resource, out Effect obj)
		{
			if (effects.ContainsKey(resource))
			{
				if (effects[resource] != null)
				{
					obj = effects[resource];
					return;
				}
				else
				{
					obj = null;
					effects.Remove(resource);
					Log.Write(this, "Resource [" + resource + "] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<Effect>(resource);
				}
				catch (ContentLoadException)
				{
					Log.Write(this, "Resource [" + resource + "], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					effects.Add(resource, obj);
			}
		}

		public void Load(string resource, out SkyContent obj)
		{
			if (skies.ContainsKey(resource))
			{
				if (skies[resource] != null)
				{
					obj = skies[resource];
					return;
				}
				else
				{
					obj = null;
					skies.Remove(resource);
					Log.Write(this, "Resource [" + resource + "] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<SkyContent>(resource);
				}
				catch (ContentLoadException e)
				{
					Log.Write(this, "Exception: " + e.Message);
					Log.Write(this, "Resource [" + resource + "], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					skies.Add(resource, obj);
			}
		}

		public void Load(string resource, out ParticleContent obj)
		{
			if (particles.ContainsKey(resource))
			{
				if (particles[resource] != null)
				{
					obj = particles[resource];
					return;
				}
				else
				{
					obj = null;
					particles.Remove(resource);
					Log.Write(this, "Resource [" + resource + "] was null in the cache. Pruning reference.", LogLevels.Warning);
				}
			}
			else
			{
				try
				{
					obj = game.Content.Load<ParticleContent>(resource);
				}
				catch (ContentLoadException e)
				{
					Log.Write(this, "Exception: " + e.Message);
					Log.Write(this, "Resource [" + resource + "], not found in content directory.", LogLevels.Error);
					obj = null;
					return;
				}

				if (obj == null)
				{
					Log.Write(this, "Unable to load resource [" + resource + "] into memory, reason unknown.", LogLevels.Error);
					return;
				}
				else
					particles.Add(resource, obj);
			}
		}

		public void Unload(string resource, out Texture2D obj)
		{
			if (textures.ContainsKey(resource))
			{
				obj = null;
				textures.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}

		public void Unload(string resource, out Model obj)
		{
			if (models.ContainsKey(resource))
			{
				obj = null;
				models.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}

		public void Unload(string resource, out SpriteFont obj)
		{
			if (fonts.ContainsKey(resource))
			{
				obj = null;
				fonts.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}

		public void Unload(string resource, out Effect obj)
		{
			if (effects.ContainsKey(resource))
			{
				obj = null;
				effects.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}

		public void Unload(string resource, out SkyContent obj)
		{
			if (skies.ContainsKey(resource))
			{
				obj = null;
				skies.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}

		public void Unload(string resource, out ParticleContent obj)
		{
			if (particles.ContainsKey(resource))
			{
				obj = null;
				particles.Remove(resource);
			}
			else
			{
				obj = null;
				Log.Write(this, "Unable to unload resource [" + resource + "] because it was not found in the cache.", LogLevels.Warning);
			}
		}
	}
}
