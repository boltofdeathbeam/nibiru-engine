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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Nibiru
{
	/// <summary>
	/// Class that holds game configuration settings that can be modified.
	/// </summary>
	public class GameConfig
	{
		private GameEngine game = null;

		/// <summary>
		/// Constructor for game configuration object. Needs the game engine.
		/// </summary>
		/// <param name="game">The game engine this configuration belongs to.</param>
		public GameConfig(GameEngine game)
		{
			this.game = game;
		}
	}
}