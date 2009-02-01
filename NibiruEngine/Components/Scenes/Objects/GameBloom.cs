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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Interfaces;

namespace Nibiru.Scenes
{
	/// <summary>
	/// A special drawable component that is applied to the game engine's entire screen.
	/// </summary>
	internal class GameBloom : DrawableGameComponent, ILoadable
	{
		private SpriteBatch spriteBatch;

		private Effect bloomExtractEffect;
		private Effect bloomCombineEffect;
		private Effect gaussianBlurEffect;

		private ResolveTexture2D resolveTarget;
		private RenderTarget2D renderTarget1;
		private RenderTarget2D renderTarget2;

		private BloomConfig config = BloomConfig.PresetSettings[0];

		// Choose what display settings the bloom should use.
		public BloomConfig Config { get { return config; } set { config = value; } }

		public string Resource { get { return String.Empty; } }

		public bool Loaded { get; internal set; }

		public bool Persist { get; set; }

		public GameBloom(Game game) : base(game)
		{
			Throw.IfNull(this, "game", game);

			Loaded = false;
		}

		public virtual void Load(ContentCache cache)
		{
			Log.Write(this, "Loading game bloom.");

			spriteBatch = new SpriteBatch(Game.GraphicsDevice);

			bloomExtractEffect = Game.Content.Load<Effect>(@"Effects\bloomextract");
			bloomCombineEffect = Game.Content.Load<Effect>(@"Effects\bloomcombine");
			gaussianBlurEffect = Game.Content.Load<Effect>(@"Effects\gaussianblur");

			// Look up the resolution and format of our main backbuffer.
			PresentationParameters pp = Game.GraphicsDevice.PresentationParameters;

			int width = pp.BackBufferWidth;
			int height = pp.BackBufferHeight;

			SurfaceFormat format = pp.BackBufferFormat;

			// Create a texture for reading back the backbuffer contents.
			resolveTarget = new ResolveTexture2D(Game.GraphicsDevice, width, height, 1, format);

			// Create two rendertargets for the bloom processing. These are half the
			// size of the backbuffer, in order to minimize fillrate costs. Reducing
			// the resolution in this way doesn't hurt quality, because we are going
			// to be blurring the bloom images in any case.
			width /= 2;
			height /= 2;

			renderTarget1 = new RenderTarget2D(Game.GraphicsDevice, width, height, 1, format);
			renderTarget2 = new RenderTarget2D(Game.GraphicsDevice, width, height, 1, format);

			Loaded = true;
		}

		public virtual void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading game bloom.");

			resolveTarget.Dispose();
			renderTarget1.Dispose();
			renderTarget2.Dispose();

			Loaded = false;
		}

		/// <summary>
		/// This is where it all happens. Grabs a scene that has already been rendered,
		/// and uses postprocess magic to add a glowing bloom effect over the top of it.
		/// </summary>
		public override void Draw(GameTime gameTime)
		{
			// Resolve the scene into a texture, so we can
			// use it as input data for the bloom processing.
			Game.GraphicsDevice.ResolveBackBuffer(resolveTarget);

			// Pass 1: draw the scene into rendertarget 1, using a
			// shader that extracts only the brightest parts of the image.
			bloomExtractEffect.Parameters["BloomThreshold"].SetValue(Config.BloomThreshold);

			DrawFullscreenQuad(resolveTarget, renderTarget1, bloomExtractEffect);

			// Pass 2: draw from rendertarget 1 into rendertarget 2,
			// using a shader to apply a horizontal gaussian blur filter.
			SetBlurEffectParameters(1.0f / (float)renderTarget1.Width, 0);

			DrawFullscreenQuad(renderTarget1.GetTexture(), renderTarget2, gaussianBlurEffect);

			// Pass 3: draw from rendertarget 2 back into rendertarget 1,
			// using a shader to apply a vertical gaussian blur filter.
			SetBlurEffectParameters(0, 1.0f / (float)renderTarget1.Height);

			DrawFullscreenQuad(renderTarget2.GetTexture(), renderTarget1, gaussianBlurEffect);

			// Pass 4: draw both rendertarget 1 and the original scene
			// image back into the main backbuffer, using a shader that
			// combines them to produce the final bloomed result.
			Game.GraphicsDevice.SetRenderTarget(0, null);

			EffectParameterCollection parameters = bloomCombineEffect.Parameters;

			parameters["BloomIntensity"].SetValue(Config.BloomIntensity);
			parameters["BaseIntensity"].SetValue(Config.BaseIntensity);
			parameters["BloomSaturation"].SetValue(Config.BloomSaturation);
			parameters["BaseSaturation"].SetValue(Config.BaseSaturation);

			Game.GraphicsDevice.Textures[1] = resolveTarget;

			Viewport viewport = Game.GraphicsDevice.Viewport;

			DrawFullscreenQuad(renderTarget1.GetTexture(), viewport.Width, viewport.Height, bloomCombineEffect);
		}

		/// <summary>
		/// Helper for drawing a texture into a rendertarget, using
		/// a custom shader to apply postprocessing effects.
		/// </summary>
		void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget, Effect effect)
		{
			Game.GraphicsDevice.SetRenderTarget(0, renderTarget);

			DrawFullscreenQuad(texture, renderTarget.Width, renderTarget.Height, effect);

			Game.GraphicsDevice.SetRenderTarget(0, null);
		}

		/// <summary>
		/// Helper for drawing a texture into the current rendertarget,
		/// using a custom shader to apply postprocessing effects.
		/// </summary>
		void DrawFullscreenQuad(Texture2D texture, int width, int height, Effect effect)
		{
			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			// Begin the custom effect, if it is currently enabled.
			effect.Begin();
			effect.CurrentTechnique.Passes[0].Begin();

			// Draw the quad.
			spriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
			spriteBatch.End();

			// End the custom effect.
			effect.CurrentTechnique.Passes[0].End();
			effect.End();
		}

		/// <summary>
		/// Computes sample weightings and texture coordinate offsets
		/// for one pass of a separable gaussian blur filter.
		/// </summary>
		void SetBlurEffectParameters(float dx, float dy)
		{
			// Look up the sample weight and offset effect parameters.
			EffectParameter weightsParameter, offsetsParameter;

			weightsParameter = gaussianBlurEffect.Parameters["SampleWeights"];
			offsetsParameter = gaussianBlurEffect.Parameters["SampleOffsets"];

			// Look up how many samples our gaussian blur effect supports.
			int sampleCount = weightsParameter.Elements.Count;

			// Create temporary arrays for computing our filter settings.
			float[] sampleWeights = new float[sampleCount];
			Vector2[] sampleOffsets = new Vector2[sampleCount];

			// The first sample always has a zero offset.
			sampleWeights[0] = ComputeGaussian(0);
			sampleOffsets[0] = new Vector2(0);

			// Maintain a sum of all the weighting values.
			float totalWeights = sampleWeights[0];

			// Add pairs of additional sample taps, positioned
			// along a line in both directions from the center.
			for (int i = 0; i < sampleCount / 2; i++)
			{
				// Store weights for the positive and negative taps.
				float weight = ComputeGaussian(i + 1);

				sampleWeights[i * 2 + 1] = weight;
				sampleWeights[i * 2 + 2] = weight;

				totalWeights += weight * 2;

				// To get the maximum amount of blurring from a limited number of
				// pixel shader samples, we take advantage of the bilinear filtering
				// hardware inside the texture fetch unit. If we position our texture
				// coordinates exactly halfway between two texels, the filtering unit
				// will average them for us, giving two samples for the price of one.
				// This allows us to step in units of two texels per sample, rather
				// than just one at a time. The 1.5 offset kicks things off by
				// positioning us nicely in between two texels.
				float sampleOffset = i * 2 + 1.5f;

				Vector2 delta = new Vector2(dx, dy) * sampleOffset;

				// Store texture coordinate offsets for the positive and negative taps.
				sampleOffsets[i * 2 + 1] = delta;
				sampleOffsets[i * 2 + 2] = -delta;
			}

			// Normalize the list of sample weightings, so they will always sum to one.
			for (int i = 0; i < sampleWeights.Length; i++)
			{
				sampleWeights[i] /= totalWeights;
			}

			// Tell the effect about our new filter settings.
			weightsParameter.SetValue(sampleWeights);
			offsetsParameter.SetValue(sampleOffsets);
		}

		/// <summary>
		/// Evaluates a single point on the gaussian falloff curve.
		/// Used for setting up the blur filter weightings.
		/// </summary>
		float ComputeGaussian(float n)
		{
			float theta = Config.BlurAmount;

			return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) * Math.Exp(-(n * n) / (2 * theta * theta)));
		}
	}
}
