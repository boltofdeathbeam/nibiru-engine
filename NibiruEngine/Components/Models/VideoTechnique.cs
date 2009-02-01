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
using Nibiru.Cameras;

namespace Nibiru.Models
{
	public abstract class VideoTechnique
	{
		public string Technique { get; private set; }
		public VideoEffect Effect { get; internal set; }

		public VideoTechnique(string technique)
		{
			Technique = technique;
		}

		public virtual void Setup(GameCamera camera, Matrix world)
		{
			if (Effect == null)
			{
				Log.Write(this, "You cannot setup a technique that has not been added to a video effect object.", LogLevels.Error);
				return;
			}
		}
	}
}
