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

namespace Nibiru.Interfaces
{
	public interface IEngineCamera : ILoadable, ICollidable
	{
		Quaternion Orientation { get; set; }
		Vector3 Position { get; set; }
		Matrix Projection { get; }
		Vector3 Direction { get; }
		Matrix View { get; }
		Matrix ViewProjection { get; }
		Vector3 X { get; }
		Vector3 Y { get; }
		Vector3 Z { get; }

		void LookAt(Vector3 target);
		void LookAt(Vector3 eye, Vector3 target, Vector3 up);

		void Move(float dx, float dy, float dz);
		void Move(Vector3 direction, Vector3 distance);

		void Perspective(float fovx, float aspect, float znear, float zfar);
		void Rotate(float headingDegrees, float pitchDegrees, float rollDegrees);

		void Zoom(float zoom, float minZoom, float maxZoom);

		void Update(GameTime gameTime);
	}
}
