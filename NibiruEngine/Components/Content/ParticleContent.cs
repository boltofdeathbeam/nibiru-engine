using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Nibiru
{
	/// <summary>
	/// Settings class describes all the tweakable options used
	/// to control the appearance of a particle system.
	/// </summary>
	public class ParticleContent
	{
		// Effect used to render this particle system.
		public Effect ParticleEffect;

		// Specifies which effect technique to use. If these particles will never
		// rotate, we can use a simpler pixel shader that requires less GPU power.
		public string TechniqueName;

		// Name of the texture used by this particle system.
		public string TextureName = null;

		// Maximum number of particles that can be displayed at one time.
		public int MaxParticles = 100;

		// How long these particles will last.
		public TimeSpan Duration = TimeSpan.FromSeconds(1);

		// Controls how much particles are influenced by the velocity of the object
		// which created them. You can see this in action with the explosion effect,
		// where the flames continue to move in the same direction as the source
		// projectile. The projectile trail particles, on the other hand, set this
		// value very low so they are less affected by the velocity of the projectile.
		public float EmitterVelocitySensitivity = 1;

		// Range of values controlling how much X and Z axis velocity to give each
		// particle. Values for individual particles are randomly chosen from somewhere
		// between these limits.
		public float MinHorizontalVelocity = 0;
		public float MaxHorizontalVelocity = 0;

		// Range of values controlling how much Y axis velocity to give each particle.
		// Values for individual particles are randomly chosen from somewhere between
		// these limits.
		public float MinVerticalVelocity = 0;
		public float MaxVerticalVelocity = 0;

		// Alpha blending settings.
		public Blend SourceBlend = Blend.SourceAlpha;
		public Blend DestinationBlend = Blend.InverseSourceAlpha;
	}

	/// <summary>
	/// Content Pipeline class for loading ParticleContent data from XNB format.
	/// </summary>
	class ParticleContentReader : ContentTypeReader<ParticleContent>
	{
		protected override ParticleContent Read(ContentReader input,
												 ParticleContent existingInstance)
		{
			ParticleContent settings = new ParticleContent();

			settings.ParticleEffect = input.ReadObject<Effect>();
			settings.TechniqueName = input.ReadString();
			settings.MaxParticles = input.ReadInt32();
			settings.Duration = input.ReadObject<TimeSpan>();
			settings.EmitterVelocitySensitivity = input.ReadSingle();
			settings.MinHorizontalVelocity = input.ReadSingle();
			settings.MaxHorizontalVelocity = input.ReadSingle();
			settings.MinVerticalVelocity = input.ReadSingle();
			settings.MaxVerticalVelocity = input.ReadSingle();
			settings.SourceBlend = input.ReadObject<Blend>();
			settings.DestinationBlend = input.ReadObject<Blend>();

			return settings;
		}
	}
}
