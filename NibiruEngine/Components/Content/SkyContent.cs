using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nibiru.Interfaces;

namespace Nibiru
{
	public class SkyContent
	{
		Model skyModel;
		Texture skyTexture;

		/// <summary>
		/// Constructor is internal: this should only ever
		/// be called by the SkyReader class.
		/// </summary>
		internal SkyContent(ContentReader input)
		{
			skyModel = input.ReadObject<Model>();

			skyTexture = input.ReadObject<Texture>();
		}


		/// <summary>
		/// Helper for drawing the skydome mesh.
		/// </summary>
		public void Draw(Matrix view, Matrix projection)
		{
			foreach (ModelMesh mesh in skyModel.Meshes)
			{
				foreach (Effect effect in mesh.Effects)
				{
					effect.Parameters["View"].SetValue(view);
					effect.Parameters["Projection"].SetValue(projection);
					effect.Parameters["Texture"].SetValue(skyTexture);
				}

				mesh.Draw(SaveStateMode.SaveState);
			}
		}
	}


	/// <summary>
	/// Helper for reading a Sky object from the compiled XNB format.
	/// </summary>
	public class SkyContentReader : ContentTypeReader<SkyContent>
	{
		protected override SkyContent Read(ContentReader input, SkyContent existingInstance)
		{
			return new SkyContent(input);
		}
	}
}
