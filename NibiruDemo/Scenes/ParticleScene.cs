using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nibiru;
using Nibiru.Scenes;
using Nibiru.Particles;
using Nibiru.Cameras;
using Nibiru.Sprites;

namespace NibiruDemo
{
	class ParticleScene : GameScene
	{
		VideoFont title;
		VideoParticle effect;

		public ParticleScene(GameEngine game)
			: base(game)
		{
			// Setup a camera to view this scene.
			Camera = new GameCamera(game, PlayerIndex.One, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.Up, 1, 10000, 1.0f);
			//Camera.CurrentType = VideoCamera.Types.Third;
			//Camera.Near = 1;
			//Camera.Far = 10000;

			// Create a title sprite to display the name of this scene.
			title = new VideoFont("Nibiru Engine - Particle Demo Scene", @"Fonts\trebuchet", new Vector2(10, 10), Color.White);

			// Simply change the class of the effect to try out other effects.
			effect = new VideoParticle(Camera, @"Particles\smoke");
			
			Attach(title);
			Attach(effect);
		}
	}
}
