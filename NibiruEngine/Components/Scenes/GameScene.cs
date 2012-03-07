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

namespace Nibiru.Scenes
{
	public enum BloomOptions { Default, Soft, Desaturated, Saturated, Blurry, Subtle };

	public class GameScene : IManagable, ILoadable
	{
		private GameEngine game = null;
		private SceneManager manager = null;
		internal List<IEngineSprite> spriteList = null;
		internal List<IEngineModel> modelList = null;
		internal List<IEngineParticle> particleList = null;
		private GameTerrain2 terrain = null;
		private GameSkyDome sky = null;
		private GameCamera camera = null;
		private GameBloom bloom;
		private BloomOptions bloomOption = BloomOptions.Default;
		private bool enableBloom = false;

		/// <summary>
		/// Allows the bloom effect to be turned on and off.
		/// </summary>
		public bool EnableBloom { get { return enableBloom; } set { enableBloom = value; Bloom.Visible = value; } }

		/// <summary>
		/// Sets or Gets the option to use when bloom is enabled.
		/// </summary>
		public BloomOptions BloomOption
		{
			get { return bloomOption; }
			set
			{
				bloomOption = value;

				BloomConfig config = BloomConfig.PresetSettings[0];

				switch (value)
				{
					case BloomOptions.Soft:
						config = BloomConfig.PresetSettings[1];
						break;
					case BloomOptions.Desaturated:
						config = BloomConfig.PresetSettings[2];
						break;
					case BloomOptions.Saturated:
						config = BloomConfig.PresetSettings[3];
						break;
					case BloomOptions.Blurry:
						config = BloomConfig.PresetSettings[4];
						break;
					case BloomOptions.Subtle:
						config = BloomConfig.PresetSettings[5];
						break;
				}

				Bloom.Config = config;
			}
		}

		/// <summary>
		/// The sprite manager that will be updating and drawing this sprite.
		/// </summary>
		public IEngineManager Manager { get { return manager; } set { manager = (SceneManager)value; } }

		public GameEngine Game { get { return game; } }

		/// <summary>
		/// Gives the ability to mark an object to be destroyed on the next update call.
		/// </summary>
		public bool Destroy { get; set; }

		/// <summary>
		/// The base background color that the engine will use to clear the screen during frames.
		/// </summary>
		public Color BackgroundColor { get; set; }

		/// <summary>
		/// The sprite manager used to display and manage 2D images on the screen.
		/// </summary>
		public SpriteManager Sprites { get { return game.Sprites; } }

		/// <summary>
		/// The model manager used to display and manage 3D objects on the screen.
		/// </summary>
		public ModelManager Models { get { return game.Models; } }

		/// <summary>
		/// The particle effect manager used to control and display them on the screen.
		/// </summary>
		public ParticleManager Particles { get { return game.Particles; } }

		/// <summary>
		/// The player manager used to manage and record input for the various players in the game.
		/// </summary>
		public PlayerManager Players { get { return game.Players; } }

		/// <summary>
		/// The sound manager used to control audio files.
		/// </summary>
		public SoundManager Sounds { get { return game.Sounds; } }

		/// <summary>
		/// The generated geometry model used to render terrain into the scene.
		/// </summary>
		public GameTerrain2 Terrain { get { return terrain; } set { terrain = value; } }

		/// <summary>
		/// The image used to render a game sky into the scene.
		/// </summary>
		public GameSkyDome Sky { get { return sky; } set { sky = value; } }

		/// <summary>
		/// Provides bloom post-processing capabilities to the engine.
		/// </summary>
		internal GameBloom Bloom { get { return bloom; } set { bloom = value; } }

		/// <summary>
		/// The primary camera that will be used to view the scene.
		/// </summary>
		public GameCamera Camera { get { return camera; } set { camera = value; } }

		public bool Persist { get; set; }

		public bool Loaded { get; internal set; }

		//public string Resource { get { return String.Empty; } }

		/// <summary>
		/// Constructor to create a new game scene, requires the game engine.
		/// </summary>
		/// <param name="game">The game engine the scene belongs to.</param>
		public GameScene(GameEngine game)
		{
			this.game = game;
			spriteList = new List<IEngineSprite>();
			modelList = new List<IEngineModel>();
			particleList = new List<IEngineParticle>();

			BackgroundColor = Color.Black; // default color

			// Create and add the bloom post-processing component.
			bloom = new GameBloom(game);

			// Use the bloom post-processing effects.
			EnableBloom = true;
			BloomOption = BloomOptions.Subtle; // default bloom

			Loaded = false;
		}

		public void Attach(IEngineSprite entity)
		{
			if (!spriteList.Contains(entity))
				spriteList.Add(entity);
			else
				Log.Write(this, "Unable to attach 2D entity, was already found in the list of existing ones.", LogLevels.Warning);
		}

		public void Attach(IEngineModel entity)
		{
			if (!modelList.Contains(entity))
				modelList.Add(entity);
			else
				Log.Write(this, "Unable to attach 3D entity, was already found in the list of existing ones.", LogLevels.Warning);
		}

		public void Attach(IEngineParticle entity)
		{
			if (!particleList.Contains(entity))
				particleList.Add(entity);
			else
				Log.Write(this, "Unable to attach particle entity, was already found in the list of existing ones.", LogLevels.Warning);
		}

		/// <summary>
		/// Loads the scene's entities into memory.
		/// </summary>
		public void Load(ContentCache cache)
		{
			Log.Write(this, "Loading...");

			foreach (IEngineSprite entity in spriteList)
			{
				game.Sprites.Attach(entity);
				entity.Load(cache);
			}

			foreach (IEngineModel entity in modelList)
			{
				game.Models.Attach(entity);
				entity.Load(cache);
			}

			foreach (IEngineParticle entity in particleList)
			{
				game.Particles.Attach(entity);
				entity.Load(cache);
			}

			// Add all the necessary components to the game engine for drawining the scene.

			if (Sky != null)
			{
				Sky.Load(cache);
				game.Components.Add(Sky);
			}

			if (Terrain != null)
			{
				Terrain.Load(cache);
				game.Components.Add(Terrain);
			}

			game.Components.Add(game.Models);
			game.Components.Add(game.Particles);
			game.Components.Add(game.Sprites);

			if (Bloom != null)
			{
				Bloom.Load(cache);
				game.Components.Add(Bloom);
			}
			
			Loaded = true;
		}

		/// <summary>
		/// Unloads all resources used by this scene's entities.
		/// </summary>
		public void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading...");

			foreach (IEngineSprite entity in spriteList)
			{
				entity.Unload(cache);
				game.Sprites.Detatch(entity);
			}

			foreach (IEngineModel entity in modelList)
			{
				entity.Unload(cache);
				game.Models.Detatch(entity);
			}

			foreach (IEngineParticle entity in particleList)
			{
				entity.Unload(cache);
				game.Particles.Detatch(entity);
			}

			game.Components.Remove(game.Models);
			game.Components.Remove(game.Particles);
			game.Components.Remove(game.Sprites);

			if (Terrain != null)
			{
				Terrain.Unload(cache);
				game.Components.Remove(Terrain);
			}

			if (Sky != null)
			{
				Sky.Unload(cache);
				game.Components.Remove(Sky);
			}
			
			if (Bloom != null)
			{
				Bloom.Unload(cache);
				game.Components.Remove(Bloom);
			}

			Loaded = false;
		}

		public virtual void Draw(GameTime gameTime)
		{
			Manager.Game.GraphicsDevice.Clear(Color.Black);

			// Make sure we are using the depth buffer, otherwise things draw weird.
			Manager.Game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
		}

		public virtual void Update(GameTime gameTime)
		{

		}
	}

	/// <summary>
	/// Class holds all the settings used to tweak the bloom effect.
	/// </summary>
	class BloomConfig
	{
		// Controls how bright a pixel needs to be before it will bloom.
		// Zero makes everything bloom equally, while higher values select
		// only brighter colors. Somewhere between 0.25 and 0.5 is good.
		public readonly float BloomThreshold;

		// Controls how much blurring is applied to the bloom image.
		// The typical range is from 1 up to 10 or so.
		public readonly float BlurAmount;

		// Controls the amount of the bloom and base images that
		// will be mixed into the final scene. Range 0 to 1.
		public readonly float BloomIntensity;
		public readonly float BaseIntensity;

		// Independently control the color saturation of the bloom and
		// base images. Zero is totally desaturated, 1.0 leaves saturation
		// unchanged, while higher values increase the saturation level.
		public readonly float BloomSaturation;
		public readonly float BaseSaturation;

		/// <summary>
		/// Constructs a new bloom settings descriptor.
		/// </summary>
		public BloomConfig(float bloomThreshold, float blurAmount,
							 float bloomIntensity, float baseIntensity,
							 float bloomSaturation, float baseSaturation)
		{
			BloomThreshold = bloomThreshold;
			BlurAmount = blurAmount;
			BloomIntensity = bloomIntensity;
			BaseIntensity = baseIntensity;
			BloomSaturation = bloomSaturation;
			BaseSaturation = baseSaturation;
		}

		/// <summary>
		/// Table of preset bloom settings used by BloomOptions
		/// </summary>
		public static BloomConfig[] PresetSettings =
        {
            //              Thresh  Blur Bloom  Base  BloomSat BaseSat
            new BloomConfig(0.25f,  4,   1.25f, 1,    1,       1),
            new BloomConfig(0,      3,   1,     1,    1,       1),
            new BloomConfig(0.5f,   8,   2,     1,    0,       1),
            new BloomConfig(0.25f,  4,   2,     1,    2,       0),
            new BloomConfig(0,      2,   1,     0.1f, 1,       1),
            new BloomConfig(0.5f,   2,   1,     1,    1,       1),
        };
	}
}
