using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nibiru;
using Nibiru.Scenes;
using Nibiru.Sprites;

namespace NibiruDemo
{
	class SpriteScene : GameScene
	{
		VideoFont title;
		AutomatedSprite rings;
		ControlledSprite skull;

		public SpriteScene(GameEngine game)
			: base(game)
		{
			// Create a title sprite to display the name of this scene.
			title = new VideoFont("Nibiru Engine - Sprite Demo Scene", @"Fonts\trebuchet", new Vector2(10, 10), Color.White);

			// Create some bouncing rings.
			rings = new AutomatedSprite(@"Images\threerings", new Vector2(250, 250), Color.White, 75, 75, 0, 6, 8);
			rings.Speed = new Vector2 (1, 1);

			// Create a player controlled sprite.
			skull = new ControlledSprite(PlayerIndex.One, @"Images\skullball", new Vector2(550, 550), Color.White, 75, 75, 0, 6, 8);
			skull.Speed = new Vector2(2, 2);
			
			
			Attach(title);
			Attach(rings);
			Attach(skull);
		}
	}
}
