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

using Nibiru.Interfaces;

namespace Nibiru
{
	/// <summary>
	/// Collision event delegate used to provide event arguments.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void CollisionEvent(Object sender, CollisionEventArgs args);

	internal class CollisionEventData
	{
		/// <summary>
		/// The object being collided against.
		/// </summary>
		public ICollidable Collidee { get; set; }

		/// <summary>
		/// The object colliding against the collidee.
		/// </summary>
		public ICollidable Collider { get; set; }

		/// <summary>
		/// The collision event to be called when it occurs.
		/// </summary>
		public CollisionEvent Event { get; set; }

		/// <summary>
		/// Whether the collision event repeats itself.
		/// </summary>
		public bool Repeat { get; set; }

		public CollisionEventData(ICollidable collidee, ICollidable collider, bool repeat, CollisionEvent evnt)
		{
			Collidee = collidee;
			Collider = collider;
			Event = evnt;
			Repeat = repeat;
		}
	}

	/// <summary>
	/// Collision event arguments which provide collidee and collider objects.
	/// </summary>
	public class CollisionEventArgs
	{
		/// <summary>
		/// The object being collided against.
		/// </summary>
		public ICollidable Collidee { get; private set; }

		/// <summary>
		/// The object colliding against the collidee.
		/// </summary>
		public ICollidable Collider { get; private set; }

		/// <summary>
		/// Whether the collision event repeats itself.
		/// </summary>
		public bool Repeat { get; private set; }

		/// <summary>
		/// Constructor for collision event argument object.
		/// </summary>
		/// <param name="collidee">The object being collided against.</param>
		/// <param name="collider">The object colliding against the collidee.</param>
		/// <param name="repeat">Whether the collision event repeats itself.</param>
		public CollisionEventArgs(ICollidable collidee, ICollidable collider, bool repeat)
		{
			Collidee = collidee;
			Collider = collider;
			Repeat = repeat;
		}
	}
}
