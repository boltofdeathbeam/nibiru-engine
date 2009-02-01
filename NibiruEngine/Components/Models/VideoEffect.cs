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

namespace Nibiru.Models
{
	public abstract class VideoEffect
	{
		private string resource;
		public string Resource { get { return resource; } }

		private Effect effect;

		private List<VideoTechnique> techniques;
		public List<VideoTechnique> Techniques { get { return techniques; } }

		public EffectParameterCollection Parameters
		{
			get { return effect.Parameters; }
		}

		public VideoEffect(string resource)
		{
			this.resource = resource;
			this.techniques = new List<VideoTechnique>();
		}

		public void Load(Effect effect)
		{
			this.effect = effect;
		}

		public void Add(VideoTechnique technique)
		{
			if (technique != null)
			{
				technique.Effect = this;
				techniques.Add(technique);
			}
		}

		public void SetCurrentTechnique(VideoTechnique technique)
		{
			effect.CurrentTechnique = effect.Techniques[technique.Technique];
		}

		public EffectTechnique GetCurrentTechnique()
		{
			return effect.CurrentTechnique;
		}

		public void Begin()
		{
			effect.Begin();
		}

		public void End()
		{
			effect.End();
		}
	}
}
