/* Nibiru Demo - Demonstration program showing off the Nibiru Engine
 * 
 * Copyright (C) 2009 Philippe Durand <draekz@gmail.com>
 * This file is part of Nibiru Engine.
 * 
 * License: GNU General Public License (GPL) v3
 * 
 * Nibiru Demo is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * Nibiru Demo is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with Nibiru Demo.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Nibiru;
using Nibiru.Scenes;
using Nibiru.Sprites;

namespace NibiruDemo
{
	class GameDemo : GameEngine
	{
		SpriteScene spriteScene = null;
		ModelScene modelScene = null;
		ParticleScene particleScene = null;

		public GameDemo() : base (1280, 800, false)
		{
			// We will output all message types.
			LogLevel = LogLevels.Debug;
		}

		protected override void LoadContent()
		{
			// Set the player controlling the mouse and keyboard
			Players.SetCurrent(PlayerIndex.One);

			// Create a scene to demonstrate the sprite capabilities of the engine.
			spriteScene = new SpriteScene(this);
			Scenes.Attach(spriteScene);
			
			// Create a scene to demonstrate the model capabilities of the engine.
			modelScene = new ModelScene(this);
			Scenes.Attach(modelScene);

			// Create a scene to demonstrate the particle effect capabilities of the engine.
			particleScene = new ParticleScene(this);
			Scenes.Attach(particleScene);

			// Set the current scene, which loads and unloads scene content.
			Scenes.SwitchCurrent(spriteScene);

			base.LoadContent();
		}

		protected override void Update(GameTime gameTime)
		{
			if (Players.Current.IsKeyDown(Keys.D1) && !Players.Current.WasKeyDown(Keys.D1))
				Scenes.SwitchCurrent(spriteScene);
			else if (Players.Current.IsKeyDown(Keys.D2) && !Players.Current.WasKeyDown(Keys.D2))
				Scenes.SwitchCurrent(modelScene);
			else if (Players.Current.IsKeyDown(Keys.D3) && !Players.Current.WasKeyDown(Keys.D3))
				Scenes.SwitchCurrent(particleScene);

			base.Update(gameTime);
		}
	}
}
