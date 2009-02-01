using System;

using Microsoft.Xna.Framework;

namespace Nibiru.Interfaces
{
	public interface IEngineParticle : ILoadable, ICacheable, IManagable
	{
		void Draw(GameTime gameTime);
		void Update(GameTime gameTime);
	}
}
