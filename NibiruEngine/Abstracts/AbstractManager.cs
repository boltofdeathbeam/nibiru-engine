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

namespace Nibiru
{
	/// <summary>
	/// An abstract game engine manager used to draw and update in the game loop.
	/// </summary>
	public abstract class AbstractManager : DrawableGameComponent, IEngineManager
	{
		/// <summary>
		/// The game engine object the manager is linked with.
		/// </summary>
		public new GameEngine Game { get { return base.Game as GameEngine; } }

		public bool Loaded { get; internal set; }

		/// <summary>
		/// Abstract constructor for a manager, requires a game engine.
		/// </summary>
		/// <param name="engine"></param>
		public AbstractManager(GameEngine game)
			: base(game)
		{
			
		}

		public abstract void Load(ContentCache cache);
		public abstract void Unload(ContentCache cache);
	}
}
