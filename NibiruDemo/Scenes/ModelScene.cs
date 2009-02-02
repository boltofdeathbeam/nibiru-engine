using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
			Camera = new GameCamera(game, PlayerIndex.One, new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up, 1, 10000, 250.0f);
			Camera.AllowMove = true;
			Camera.AllowPitch = true;
			Camera.AllowYaw = false;
			Camera.AllowRoll = false;
			Camera.UseKeyboard = true;

			// Setup the sky dome that will render a sky/sun around the world.
			Sky = new GameSkyDome(game, Camera);
			
			// Setup a game terrain that will be loaded from the heightmap image.
			Terrain = new GameTerrain2(game, Camera, Sky, @"Models\terrain", @"Models\ground");

			// Create the spaceship avatar that the camera will center around.
			spaceship = new VideoModel(Camera, @"Models\spaceship");

			Attach(title);
			Attach(spaceship);
		}

		public override void Update(GameTime gameTime)
		{
			if (Players.Current.IsKeyDown(Keys.Escape))
				Game.Exit();

			// Move the sky's positional value in a continuous circle around the "planet".
			Sky.RealTime = false;
			Sky.Position -= 0.005f;

			base.Update(gameTime);
		}
	}
}
