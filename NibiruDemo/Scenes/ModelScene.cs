using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nibiru;
using Nibiru.Scenes;
using Nibiru.Models;
using Nibiru.Cameras;
using Nibiru.Sprites;

namespace NibiruDemo
{
	class ModelScene : GameScene
	{
		VideoFont title;
		VideoModel spaceship;

		public ModelScene(GameEngine game)
			: base(game)
		{
			// Create a title sprite to display the name of this scene.
			title = new VideoFont("Nibiru Engine - Model Demo Scene", @"Fonts\trebuchet", new Vector2(10, 10), Color.White);

			// Setup a camera to view this scene.
			Camera = new GameCamera(game);
			Camera.CurrentType = VideoCamera.Types.Third;
			Camera.Near = 1;
			Camera.Far = 10000;

			Sky = new GameSky(game, Camera, @"Images\sky", @"Effects\sky");
			Terrain = new GameTerrain(game, Camera, @"Images\terrain");

			spaceship = new VideoModel(Camera, @"Models\spaceship");

			Attach(title);
			Attach(spaceship);
		}
	}
}
