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

namespace Nibiru.Particles
{
	/// <summary>
	/// Custom vertex structure for drawing point sprite particles.
	/// </summary>
	struct Particle
	{
		// Stores the starting position of the particle.
		public Vector3 Position;

		// Stores the starting velocity of the particle.
		public Vector3 Velocity;

		// Four random values, used to make each particle look slightly different.
		public Color Random;

		// The time (in seconds) at which this particle was created.
		public float Time;

		// Describe the layout of this vertex structure.
		public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
            new VertexElement(0, 24, VertexElementFormat.Color, VertexElementMethod.Default, VertexElementUsage.Color, 0),
            new VertexElement(0, 28, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
        };

		// Describe the size of this vertex structure.
		public const int SizeInBytes = 32;
	}
}
