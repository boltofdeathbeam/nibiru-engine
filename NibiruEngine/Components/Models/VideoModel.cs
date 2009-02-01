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
using Nibiru.Cameras;

namespace Nibiru.Models
{
	public class VideoModel : VideoPrimitive
	{
		private Model model = null;

		/// <summary>
		/// The model that is used to render this object.
		/// </summary>
		public Model Model { get { return model; } internal set { model = value; } }

		public VideoModel(GameCamera camera, string resource)
			: base(camera, resource, 0)
		{
			Destroy = false;
			UseBasicEffect = true;
		}

		/// <summary>
		/// Registers a collision between two video models, resulting in the delegate specified being called.
		/// </summary>
		/// <param name="one"></param>
		/// <param name="two"></param>
		/// <param name="collision"></param>
		public void RegisterCollision(VideoModel collider, bool repeat, CollisionEvent collision)
		{
			collisions.Add(new CollisionEventData(this, collider, repeat, collision));
		}

		public override void Load(ContentCache cache)
		{
			Log.Write(this, "Loading model resource for video model.");

			cache.Load(Resource, out model);
		}

		public override void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading model resource for video model.");

			cache.Unload(Resource, out model);
		}

		public override bool CollidesWith(ICollidable collider)
		{
			// Go through all the models meshes and check for collisions
			foreach (ModelMesh meshes in (collider as VideoModel).Model.Meshes)
			{
				foreach (ModelMesh colliderMeshes in (collider as VideoModel).Model.Meshes)
				{
					if (meshes.BoundingSphere.Transform(GenerateWorldMatrix()).Intersects(
						colliderMeshes.BoundingSphere.Transform((collider as VideoModel).GenerateWorldMatrix())))
						return true;
				}
			}

			return false;
		}

		public override void Update(GameTime gameTime)
		{
			List<CollisionEventData> toRemove = new List<CollisionEventData>();

			// Now check for collisions
			foreach (CollisionEventData collision in collisions)
			{
				if ((collision.Collidee as VideoModel).CollidesWith((collision.Collider as VideoModel)))
				{
					collision.Event(this, new CollisionEventArgs(collision.Collidee, collision.Collider, collision.Repeat));

					if (!collision.Repeat)
						toRemove.Add(collision);
				}
			}

			foreach (CollisionEventData collision in toRemove)
				collisions.Remove(collision);
		}

		public override void Draw(GameTime gameTime)
		{
			Matrix[] transforms = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(transforms);

			foreach (ModelMesh mesh in model.Meshes)
			{
				if (UseBasicEffect)
				{
					foreach (BasicEffect be in mesh.Effects)
					{
						be.EnableDefaultLighting();
						be.Projection = camera.Projection;
						be.View = camera.View;
						be.World = GenerateWorldMatrix() * mesh.ParentBone.Transform;
					}
				}
				else
				{
					foreach (Effect effect in mesh.Effects)
					{
						effect.Parameters["World"].SetValue(GenerateWorldMatrix() * mesh.ParentBone.Transform);
						effect.Parameters["View"].SetValue(camera.View);
						effect.Parameters["Projection"].SetValue(camera.Projection);
					}
				}

				mesh.Draw();
			}
		}
	}
}
