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
using Nibiru.Scenes;

namespace Nibiru
{
	public class SceneManager : AbstractManager
	{
		private List<GameScene> scenes = null;
		private GameScene currentScene = null;

		/// <summary>
		/// Allows the change of the current scene being displayed and updated.
		/// </summary>
		public GameScene Current { get { return currentScene; } set { currentScene = value; } }

		/// <summary>
		/// Constructor for a new scene manager, requires the game engine.
		/// </summary>
		/// <param name="engine"></param>
		public SceneManager(GameEngine engine)
			: base(engine)
		{
			scenes = new List<GameScene>();
			//UpdateOrder = 1;
		}

		public override void Load(ContentCache cache)
		{
			// Now that the game has decided to load content, lets do it.
			if (currentScene != null)
			{
				currentScene.Load(Game.Cache);

				Log.Write(this, "Adding the new scene's camera to the game engine.");
				Game.Components.Add(currentScene.Camera);
			}
		}

		public override void Unload(ContentCache cache)
		{
			Log.Write(this, "Removing the old scene's camera from the engine.");
			Game.Components.Remove(currentScene.Camera);

			currentScene.Unload(Game.Cache);
			currentScene = null;
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

		public void Attach(GameScene scene)
		{
			if (!scenes.Contains(scene))
			{
				scene.Manager = this;
				scenes.Add(scene);
			}
			else
				Log.Write(this, "Scene being added was already found in the list of scenes.", LogLevels.Warning);
		}

		public void Detatch(GameScene scene)
		{
			if (scenes.Contains(scene))
			{
				scene.Manager = null;
				scenes.Remove(scene);
			}
			else
				Log.Write(this, "Scene being removed was not found in the list of scenes.", LogLevels.Warning);
		}

		public void SwitchCurrent(GameScene newCurrent)
		{
			Throw.IfNull(this, "newCurrent", newCurrent);

			if (newCurrent == currentScene)
			{
				Log.Write(this, "Scene already loaded!", LogLevels.Warning);
				return;
			}

			Log.Write(this, "Switching current scene.");

			if (currentScene != null)
			{
				Log.Write(this, "Removing the old scene's camera from the engine.");
				Game.Components.Remove(currentScene.Camera);

				currentScene.Unload(Game.Cache);
			}

			currentScene = newCurrent;

			if (currentScene != null)
			{
				currentScene.Load(Game.Cache);

				Log.Write(this, "Adding the new scene's camera to the game engine.");
				Game.Components.Add(currentScene.Camera);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			if (currentScene != null)
				currentScene.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			if (currentScene != null)
				currentScene.Update(gameTime);
		}
	}
}
