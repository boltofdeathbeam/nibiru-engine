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
using Microsoft.Xna.Framework.Input;

using Nibiru.Interfaces;

namespace Nibiru
{
	/// <summary>
	/// The class used to represent a player entity in the game.
	/// </summary>
	public class GamePlayer
	{
		private PlayerIndex index;
		private KeyboardState previousKeyboardState;
		private KeyboardState keyboardState;
		private MouseState previousMouseState;
		private MouseState mouseState;
		private GamePadState previousGamePadState;
		private GamePadState gamePadState;

		/// <summary>
		/// The player index used to identify players one through four.
		/// </summary>
		public PlayerIndex Index { get { return index; } }

		/// <summary>
		/// Provides the current state of the mouse.
		/// </summary>
		public MouseState MouseState { get { return mouseState; } }

		/// <summary>
		/// Provides the previous state of the mouse.
		/// </summary>
		public MouseState PreviousMouseState { get { return previousMouseState; } }

		/// <summary>
		/// Provides the current state of the keyboard.
		/// </summary>
		public KeyboardState KeyboardState { get { return keyboardState; } }
		
		/// <summary>
		/// Provides the previous state of the keyboard.
		/// </summary>
		public KeyboardState PreviousKeyboardState { get { return previousKeyboardState; } }

		/// <summary>
		/// Provides the current state of the gamepad.
		/// </summary>
		public GamePadState GamePadState { get { return gamePadState; } }

		/// <summary>
		/// Provides the previous state of the gamepad.
		/// </summary>
		public GamePadState PreviousGamePadState { get { return previousGamePadState; } }

		/// <summary>
		/// Returns whether this player is the current player taking input from mouse and keyboard.
		/// </summary>
		public bool IsCurrentPlayer { get; set; }

		/// <summary>
		/// Constructor for a game player uses the PlayerIndex to identify players one through four.
		/// </summary>
		/// <param name="index"></param>
		public GamePlayer(PlayerIndex index)
		{
			this.index = index;
		}

		/// <summary>
		/// Checks whether the right mouse button is currently being pressed.
		/// </summary>
		/// <returns></returns>
		public bool IsRightButtonPressed()
		{
			return (mouseState.RightButton == ButtonState.Pressed);
		}

		/// <summary>
		/// Checks whether the middle mouse button is currently being pressed.
		/// </summary>
		/// <returns></returns>
		public bool IsMiddleButtonPressed()
		{
			return (mouseState.MiddleButton == ButtonState.Pressed);
		}

		/// <summary>
		/// Checks whether the left mouse button is currently beign pressed.
		/// </summary>
		/// <returns></returns>
		public bool IsLeftButtonPressed()
		{
			return (mouseState.LeftButton == ButtonState.Pressed);
		}

		/// <summary>
		/// Provides the difference between the previous and current mouse movement on the X axis.
		/// </summary>
		/// <returns></returns>
		public int MouseMovementX()
		{
			return mouseState.X - previousMouseState.X;
		}

		/// <summary>
		/// Provides the difference between the previous and current mouse movement on the Y axis.
		/// </summary>
		/// <returns></returns>
		public int MouseMovementY()
		{
			return mouseState.Y - previousMouseState.Y;
		}

		/// <summary>
		/// Checks whether the specific key is currently down.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyDown(Keys key)
		{
			return keyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// Checks whether the specific key is currently up.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool IsKeyUp(Keys key)
		{
			return keyboardState.IsKeyUp(key);
		}

		/// <summary>
		/// Checks whether the specific key was down last frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool WasKeyDown(Keys key)
		{
			return previousKeyboardState.IsKeyDown(key);
		}

		/// <summary>
		/// Checks whether the specific key was up last frame.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool WasKeyUp(Keys key)
		{
			return previousKeyboardState.IsKeyUp(key);
		}

		/// <summary>
		/// Updates the game player's input state and other variables.
		/// </summary>
		/// <param name="gameTime"></param>
		public virtual void Update(GameTime gameTime)
		{
			if (IsCurrentPlayer)
			{
				previousKeyboardState = keyboardState;
				keyboardState = Keyboard.GetState();

				previousMouseState = mouseState;
				mouseState = Mouse.GetState();
			}

			previousGamePadState = gamePadState;
			gamePadState = GamePad.GetState(index);
		}
	}
}
