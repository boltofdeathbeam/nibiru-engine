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
using Nibiru.Particles;

namespace Nibiru
{
	public class ParticleManager : AbstractManager
	{
		private List<IEngineParticle> particles;

		public ParticleManager(GameEngine game)
			: base(game)
		{
			particles = new List<IEngineParticle>();
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

		public void Attach(IEngineParticle effect)
		{
			if (!particles.Contains(effect))
			{
				effect.Manager = this;
				particles.Add(effect);
			}
		}

		public override void Update(GameTime gameTime)
		{
			List<VideoParticle> toRemove = new List<VideoParticle>();

			foreach (VideoParticle effect in particles)
			{
				if (effect.Destroy)
					toRemove.Add(effect);

				effect.AddParticle(Vector3.Zero, Vector3.Zero);

				effect.Update(gameTime);
			}

			foreach (VideoParticle effect in toRemove)
				particles.Remove(effect);

			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			foreach (VideoParticle effect in particles)
			{
				effect.Draw(gameTime);
			}

			base.Draw(gameTime);
		}
	}
}
