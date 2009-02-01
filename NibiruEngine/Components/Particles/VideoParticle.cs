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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Nibiru.Interfaces;
using Nibiru.Cameras;

namespace Nibiru.Particles
{
	/// <summary>
	/// The main component in charge of displaying particles.
	/// </summary>
	public class VideoParticle : IEngineParticle
	{
		private static Random random = new Random();

		private ParticleManager manager = null;
		private GameCamera camera = null;

		private Particle[] particles;

		private string resource;

		private DynamicVertexBuffer vertexBuffer;
		private VertexDeclaration vertexDeclaration;

		private BlendFunction alphaBlendOperation = BlendFunction.Add;
		private Blend sourceBlend = Blend.SourceAlpha;
		private Blend destinationBlend = Blend.InverseSourceAlpha;
		private CompareFunction alphaFunction = CompareFunction.Never;

		private bool pointSpriteEnable = true;
		private bool alphaBlendEnable = true;
		private bool alphaTestEnable = true;
		private bool depthBufferEnable = false;
		private bool depthBufferWriteEnable = false;

		private ParticleContent config;
		private Effect particleEffect;

		private EffectParameter effectViewParameter;
		private EffectParameter effectProjectionParameter;
		private EffectParameter effectViewportHeightParameter;
		private EffectParameter effectTimeParameter;

		private int firstActiveParticle;
		private int firstNewParticle;
		private int firstFreeParticle;
		private int firstRetiredParticle;
		private int drawCounter;
		private int referenceAlpha = 0;

		private float pointSizeMax = 256;
		private float currentTime;

		/// <summary>
		/// The sprite manager that will be updating and drawing this sprite.
		/// </summary>
		public IEngineManager Manager { get { return manager; } set { manager = value as ParticleManager; } }

		public bool Destroy { get; set; }

		public bool Persist { get; set; }

		public string Resource { get { return resource; } }

		public bool Loaded { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public VideoParticle(GameCamera camera, string resource)
		{
			this.camera = camera;
			this.resource = resource;

			Loaded = false;
		}

		/// <summary>
		/// Loads graphics for the particle system.
		/// </summary>
		public void Load(ContentCache cache)
		{
			Log.Write(this, "Loading effect resource for the video particle effect.");

			cache.Load(resource, out config);

			particles = new Particle[config.MaxParticles];

			InitializeParticleEffect();

			vertexDeclaration = new VertexDeclaration(Manager.Game.GraphicsDevice,
													  Particle.VertexElements);

			// Create a dynamic vertex buffer.
			int size = Particle.SizeInBytes * particles.Length;

			vertexBuffer = new DynamicVertexBuffer(Manager.Game.GraphicsDevice, size,
												   BufferUsage.WriteOnly |
												   BufferUsage.Points);

			Loaded = true;
		}

		public void Unload(ContentCache cache)
		{
			Log.Write(this, "Unloading effect resource for the video particle effect.");

			cache.Unload(resource, out config);
			config = null;
			particles = null;
			vertexDeclaration = null;
			vertexBuffer = null;

			Loaded = false;
		}

		/// <summary>
		/// Helper for initializing the particle effect.
		/// </summary>
		void InitializeParticleEffect()
		{
			particleEffect = config.ParticleEffect;

			EffectParameterCollection parameters = particleEffect.Parameters;

			// Look up shortcuts for parameters that change every frame.
			effectViewParameter = parameters["View"];
			effectProjectionParameter = parameters["Projection"];
			effectViewportHeightParameter = parameters["ViewportHeight"];
			effectTimeParameter = parameters["CurrentTime"];

			// Choose the appropriate effect technique.
			particleEffect.CurrentTechnique = particleEffect.Techniques[config.TechniqueName];
		}

		/// <summary>
		/// Updates the particle system.
		/// </summary>
		public virtual void Update(GameTime gameTime)
		{
			if (gameTime == null)
				throw new ArgumentNullException("gameTime");

			currentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

			RetireActiveParticles();
			FreeRetiredParticles();

			// If we let our timer go on increasing for ever, it would eventually
			// run out of floating point precision, at which point the particles
			// would render incorrectly. An easy way to prevent this is to notice
			// that the time value doesn't matter when no particles are being drawn,
			// so we can reset it back to zero any time the active queue is empty.

			if (firstActiveParticle == firstFreeParticle)
				currentTime = 0;

			if (firstRetiredParticle == firstActiveParticle)
				drawCounter = 0;
		}


		/// <summary>
		/// Helper for checking when active particles have reached the end of
		/// their life. It moves old particles from the active area of the queue
		/// to the retired section.
		/// </summary>
		void RetireActiveParticles()
		{
			float particleDuration = (float)config.Duration.TotalSeconds;

			while (firstActiveParticle != firstNewParticle)
			{
				// Is this particle old enough to retire?
				float particleAge = currentTime - particles[firstActiveParticle].Time;

				if (particleAge < particleDuration)
					break;

				// Remember the time at which we retired this particle.
				particles[firstActiveParticle].Time = drawCounter;

				// Move the particle from the active to the retired queue.
				firstActiveParticle++;

				if (firstActiveParticle >= particles.Length)
					firstActiveParticle = 0;
			}
		}


		/// <summary>
		/// Helper for checking when retired particles have been kept around long
		/// enough that we can be sure the GPU is no longer using them. It moves
		/// old particles from the retired area of the queue to the free section.
		/// </summary>
		void FreeRetiredParticles()
		{
			while (firstRetiredParticle != firstActiveParticle)
			{
				// Has this particle been unused long enough that
				// the GPU is sure to be finished with it?
				int age = drawCounter - (int)particles[firstRetiredParticle].Time;

				// The GPU is never supposed to get more than 2 frames behind the CPU.
				// We add 1 to that, just to be safe in case of buggy drivers that
				// might bend the rules and let the GPU get further behind.
				if (age < 3)
					break;

				// Move the particle from the retired to the free queue.
				firstRetiredParticle++;

				if (firstRetiredParticle >= particles.Length)
					firstRetiredParticle = 0;
			}
		}


		/// <summary>
		/// Draws the particle system.
		/// </summary>
		public virtual void Draw(GameTime gameTime)
		{
			SetCamera(camera.View, camera.Projection);

			GraphicsDevice device = Manager.Game.GraphicsDevice;

			SaveRenderState(device.RenderState);

			// Restore the vertex buffer contents if the graphics device was lost.
			if (vertexBuffer.IsContentLost)
			{
				vertexBuffer.SetData(particles);
			}

			// If there are any particles waiting in the newly added queue,
			// we'd better upload them to the GPU ready for drawing.
			if (firstNewParticle != firstFreeParticle)
			{
				AddNewParticlesToVertexBuffer();
			}

			// If there are any active particles, draw them now!
			if (firstActiveParticle != firstFreeParticle)
			{
				SetParticleRenderStates(device.RenderState);

				// Set an effect parameter describing the viewport size. This is needed
				// to convert particle sizes into screen space point sprite sizes.
				effectViewportHeightParameter.SetValue(device.Viewport.Height);

				// Set an effect parameter describing the current time. All the vertex
				// shader particle animation is keyed off this value.
				effectTimeParameter.SetValue(currentTime);

				// Set the particle vertex buffer and vertex declaration.
				device.Vertices[0].SetSource(vertexBuffer, 0,
											 Particle.SizeInBytes);

				device.VertexDeclaration = vertexDeclaration;

				// Activate the particle effect.
				particleEffect.Begin();

				foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
				{
					pass.Begin();

					if (firstActiveParticle < firstFreeParticle)
					{
						// If the active particles are all in one consecutive range,
						// we can draw them all in a single call.
						device.DrawPrimitives(PrimitiveType.PointList,
											  firstActiveParticle,
											  firstFreeParticle - firstActiveParticle);
					}
					else
					{
						// If the active particle range wraps past the end of the queue
						// back to the start, we must split them over two draw calls.
						device.DrawPrimitives(PrimitiveType.PointList,
											  firstActiveParticle,
											  particles.Length - firstActiveParticle);

						if (firstFreeParticle > 0)
						{
							device.DrawPrimitives(PrimitiveType.PointList,
												  0,
												  firstFreeParticle);
						}
					}

					pass.End();
				}

				particleEffect.End();

				RevertRenderStates(device.RenderState);
				// Reset a couple of the more unusual renderstates that we changed,
				// so as not to mess up any other subsequent drawing.
				//device.RenderState.PointSpriteEnable = false;
				//device.RenderState.DepthBufferWriteEnable = true;
			}

			drawCounter++;
		}


		/// <summary>
		/// Helper for uploading new particles from our managed
		/// array to the GPU vertex buffer.
		/// </summary>
		void AddNewParticlesToVertexBuffer()
		{
			int stride = Particle.SizeInBytes;

			if (firstNewParticle < firstFreeParticle)
			{
				// If the new particles are all in one consecutive range,
				// we can upload them all in a single call.
				vertexBuffer.SetData(firstNewParticle * stride, particles,
									 firstNewParticle,
									 firstFreeParticle - firstNewParticle,
									 stride, SetDataOptions.NoOverwrite);
			}
			else
			{
				// If the new particle range wraps past the end of the queue
				// back to the start, we must split them over two upload calls.
				vertexBuffer.SetData(firstNewParticle * stride, particles,
									 firstNewParticle,
									 particles.Length - firstNewParticle,
									 stride, SetDataOptions.NoOverwrite);

				if (firstFreeParticle > 0)
				{
					vertexBuffer.SetData(0, particles,
										 0, firstFreeParticle,
										 stride, SetDataOptions.NoOverwrite);
				}
			}

			// Move the particles we just uploaded from the new to the active queue.
			firstNewParticle = firstFreeParticle;
		}

		void SaveRenderState(RenderState renderState)
		{
			pointSpriteEnable = renderState.PointSpriteEnable;
			pointSizeMax = renderState.PointSizeMax;

			alphaBlendEnable = renderState.AlphaBlendEnable;
			alphaBlendOperation = renderState.AlphaBlendOperation;
			sourceBlend = renderState.SourceBlend;
			destinationBlend = renderState.DestinationBlend;

			alphaTestEnable = renderState.AlphaTestEnable;
			alphaFunction = renderState.AlphaFunction;
			referenceAlpha = renderState.ReferenceAlpha;

			depthBufferEnable = renderState.DepthBufferEnable;
			depthBufferWriteEnable = renderState.DepthBufferWriteEnable;
		}

		void RevertRenderStates(RenderState renderState)
		{
			// Enable point sprites.
			renderState.PointSpriteEnable = pointSpriteEnable;
			renderState.PointSizeMax = pointSizeMax;

			// Set the alpha blend mode.
			renderState.AlphaBlendEnable = alphaBlendEnable;
			renderState.AlphaBlendOperation = alphaBlendOperation;
			renderState.SourceBlend = sourceBlend;
			renderState.DestinationBlend = destinationBlend;

			// Set the alpha test mode.
			renderState.AlphaTestEnable = alphaTestEnable;
			renderState.AlphaFunction = alphaFunction;
			renderState.ReferenceAlpha = referenceAlpha;

			// Enable the depth buffer (so particles will not be visible through
			// solid objects like the ground plane), but disable depth writes
			// (so particles will not obscure other particles).
			renderState.DepthBufferEnable = depthBufferEnable;
			renderState.DepthBufferWriteEnable = depthBufferWriteEnable;
		}

		/// <summary>
		/// Helper for setting the renderstates used to draw particles.
		/// </summary>
		void SetParticleRenderStates(RenderState renderState)
		{
			// Enable point sprites.
			renderState.PointSpriteEnable = true;
			renderState.PointSizeMax = 256;

			// Set the alpha blend mode.
			renderState.AlphaBlendEnable = true;
			renderState.AlphaBlendOperation = BlendFunction.Add;
			renderState.SourceBlend = config.SourceBlend;
			renderState.DestinationBlend = config.DestinationBlend;

			// Set the alpha test mode.
			renderState.AlphaTestEnable = true;
			renderState.AlphaFunction = CompareFunction.Greater;
			renderState.ReferenceAlpha = 0;

			// Enable the depth buffer (so particles will not be visible through
			// solid objects like the ground plane), but disable depth writes
			// (so particles will not obscure other particles).
			renderState.DepthBufferEnable = true;
			renderState.DepthBufferWriteEnable = false;
		}

		/// <summary>
		/// Sets the camera view and projection matrices
		/// that will be used to draw this particle system.
		/// </summary>
		public void SetCamera(Matrix view, Matrix projection)
		{
			effectViewParameter.SetValue(view);
			effectProjectionParameter.SetValue(projection);
		}


		/// <summary>
		/// Adds a new particle to the system.
		/// </summary>
		public void AddParticle(Vector3 position, Vector3 velocity)
		{
			// Figure out where in the circular queue to allocate the new particle.
			int nextFreeParticle = firstFreeParticle + 1;

			if (nextFreeParticle >= particles.Length)
				nextFreeParticle = 0;

			// If there are no free particles, we just have to give up.
			if (nextFreeParticle == firstRetiredParticle)
				return;

			// Adjust the input velocity based on how much
			// this particle system wants to be affected by it.
			velocity *= config.EmitterVelocitySensitivity;

			// Add in some random amount of horizontal velocity.
			float horizontalVelocity = MathHelper.Lerp(config.MinHorizontalVelocity,
													   config.MaxHorizontalVelocity,
													   (float)random.NextDouble());

			double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

			velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
			velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

			// Add in some random amount of vertical velocity.
			velocity.Y += MathHelper.Lerp(config.MinVerticalVelocity,
										  config.MaxVerticalVelocity,
										  (float)random.NextDouble());

			// Choose four random control values. These will be used by the vertex
			// shader to give each particle a different size, rotation, and color.
			Color randomValues = new Color((byte)random.Next(255),
										   (byte)random.Next(255),
										   (byte)random.Next(255),
										   (byte)random.Next(255));

			// Fill in the particle vertex structure.
			particles[firstFreeParticle].Position = position;
			particles[firstFreeParticle].Velocity = velocity;
			particles[firstFreeParticle].Random = randomValues;
			particles[firstFreeParticle].Time = currentTime;

			firstFreeParticle = nextFreeParticle;
		}
	}
}
