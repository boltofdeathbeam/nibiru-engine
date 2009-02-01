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
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Cameras;

namespace Nibiru.Particles
{
	public class VideoParticleTrail
	{
		private VideoParticle effect;
		private float particleInterval;
		private Vector3 lastPosition;
		private float timeLeft;

		/// <summary>
		/// Constructor for a new particle trail.
		/// </summary>
		public VideoParticleTrail(VideoParticle effect, float particleInternal, Vector3 initial)
		{
			this.effect = effect;
			this.particleInterval = 1.0f / particleInternal;
			this.lastPosition = initial;
		}

		/// <summary>
		/// Update the trail's particles and positions.
		/// </summary>
		public void Update(GameTime gameTime, Vector3 newPosition)
		{
			Throw.IfNull(this, "gameTime", gameTime);

			// Work out how much time has passed since the previous update.
			float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (elapsedTime > 0)
			{
				// Work out how fast we are moving.
				Vector3 velocity = (newPosition - lastPosition) / elapsedTime;

				// If we had any time left over that we didn't use during the
				// previous update, add that to the current elapsed time.
				float timeToSpend = timeLeft + elapsedTime;

				// Counter for looping over the time interval.
				float currentTime = -timeLeft;

				// Create particles as long as we have a big enough time interval.
				while (timeToSpend > particleInterval)
				{
					currentTime += particleInterval;
					timeToSpend -= particleInterval;

					// Work out the optimal position for this particle. This will produce
					// evenly spaced particles regardless of the object speed, particle
					// creation frequency, or game update rate.
					float mu = currentTime / elapsedTime;

					Vector3 position = Vector3.Lerp(lastPosition, newPosition, mu);

					// Create the particle.
					effect.AddParticle(position, velocity);
				}

				// Store any time we didn't use, so it can be part of the next update.
				timeLeft = timeToSpend;
			}

			lastPosition = newPosition;
		}
	}
}
