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

namespace Nibiru
{
	/// <summary>
	/// The primary game engine abstract class that allows full use and control of the Nibiru library.
	/// </summary>
	public abstract class GameEngine : Game, IDisposable
	{
		private GraphicsDeviceManager graphics;
		private SceneManager scenes;
		private PlayerManager players;
		private ParticleManager particles;
		private SpriteManager sprites;
		private ModelManager models;
		private SoundManager sounds;
		private ContentCache cache;

		/// <summary>
		/// The manager which allows control of the graphics device.
		/// </summary>
		internal GraphicsDeviceManager Graphics { get { return graphics; } }

		/// <summary>
		/// The content cache that provides all of the game resources.
		/// </summary>
		internal ContentCache Cache { get { return cache; } }

		/// <summary>
		/// Allows the level of logging output to be modified.
		/// </summary>
		public LogLevels LogLevel { get { return Log.Level; } set { Log.Level = value; } }

		/// <summary>
		/// Gives access to game configuration settings that define how the game engine works.
		/// </summary>
		public GameConfig Configuration { get; private set; }

		/// <summary>
		/// The directory name to use when loading the content pipeline files. If you decide to use
		/// a different folder than "Content", the Nibiru content will not be available.
		/// </summary>
		public string ContentDirectory { get; set; }

		/// <summary>
		/// The scene manager controls what content is being used by the engine.
		/// </summary>
		public SceneManager Scenes { get { return scenes; } }

		/// <summary>
		/// The sprite manager used to display and manage 2D images on the screen.
		/// </summary>
		internal SpriteManager Sprites { get { return sprites; } }
		
		/// <summary>
		/// The model manager used to display and manage 3D objects on the screen.
		/// </summary>
		internal ModelManager Models { get { return models; } }

		/// <summary>
		/// The particle effect manager used to control and display them on the screen.
		/// </summary>
		internal ParticleManager Particles { get { return particles; } }

		/// <summary>
		/// The player manager used to manage and record input for the various players in the game.
		/// </summary>
		public PlayerManager Players { get { return players; } }

		/// <summary>
		/// The sound manager used to control audio files.
		/// </summary>
		internal SoundManager Sounds { get { return sounds; } }

		/// <summary>
		/// Constructor used to create a new game engine object.
		/// </summary>
		public GameEngine() : base()
		{
			// Normally we will setup the engine to log only warnings and errors
			Log.Level = LogLevels.Warning;

			// Create a new configuration object
			Configuration = new GameConfig(this);

			// Setup the content directory to the default nibiru folder.
			ContentDirectory = "Content";

			// Create the graphics device that will be used by the engine.
			graphics = new GraphicsDeviceManager(this);

			// We require Vertex/Pixel Shaders version 2.0 or higher.
			graphics.MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
			graphics.MinimumVertexShaderProfile = ShaderProfile.VS_2_0;

			// Create the content cache used to load any resource.
			cache = new ContentCache(this);

			// Create all the manager components needed by the engine.
			scenes = new SceneManager(this);
			sounds = new SoundManager(this);
			players = new PlayerManager(this);
			models = new ModelManager(this);
			sprites = new SpriteManager(this);
			particles = new ParticleManager(this);

			// Add all of the manager components to the engine's loop.
			
			Components.Add(players);
			Components.Add(sounds);
			Components.Add(Scenes);
		}

		protected override void Initialize()
		{
			Log.Write(this, "Initializing game engine...", LogLevels.Info);
			
			Content.RootDirectory = ContentDirectory;
			Log.Write(this, "Using content directory: " + ContentDirectory, LogLevels.Info);

			base.Initialize();
		}

		protected override void LoadContent()
		{
			models.Load(cache);
			sprites.Load(cache);
			particles.Load(cache);
			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			models.Unload(cache);
			sprites.Unload(cache);
			particles.Unload(cache);
			base.UnloadContent();
		}
	}
}
