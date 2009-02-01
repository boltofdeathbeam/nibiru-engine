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
	/// Maintains and updates all game players available to the game engine. 
	/// Also allows the current player controlling the keyboard & mouse to be set.
	/// </summary>
	public class PlayerManager : AbstractManager
	{
		private GamePlayer[] players;
		private GamePlayer currentPlayer;

		/// <summary>
		/// The current game player that is in control of the keyboard/mouse.
		/// </summary>
		public GamePlayer Current { get { return currentPlayer; } }

		/// <summary>
		/// Constructor for the player manager. Creates all four engine players.
		/// </summary>
		/// <param name="engine"></param>
		public PlayerManager(GameEngine engine)
			: base(engine)
		{
			players = new GamePlayer[4];
			players[0] = new GamePlayer(PlayerIndex.One);
			players[1] = new GamePlayer(PlayerIndex.Two);
			players[2] = new GamePlayer(PlayerIndex.Three);
			players[3] = new GamePlayer(PlayerIndex.Four);
		}

		public override void Load(ContentCache cache)
		{

		}

		public override void Unload(ContentCache cache)
		{

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

		/// <summary>
		/// Obtains the engine player represented by the index provided.
		/// </summary>
		/// <param name="index">The player index to be retreived.</param>
		/// <returns>The player represented by the index.</returns>
		public GamePlayer Get(PlayerIndex index)
		{
			switch (index)
			{
				case PlayerIndex.One:
					return players[0];
				case PlayerIndex.Two:
					return players[1];
				case PlayerIndex.Three:
					return players[2];
				case PlayerIndex.Four:
					return players[3];
			}

			return null;
		}

		/// <summary>
		/// Sets the current player to the engine player described by the index provided.
		/// </summary>
		/// <param name="index">The player index to become the current player.</param>
		public void SetCurrent(PlayerIndex index)
		{
			currentPlayer = Get(index);
		}

		/// <summary>
		/// Updates all of the players being controlled by the manager.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			foreach (GamePlayer player in players)
			{
				if (player == currentPlayer)
					player.IsCurrentPlayer = true;

				player.Update(gameTime);
			}

			base.Update(gameTime);
		}
	}
}
