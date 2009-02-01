using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace NibiruPipeline
{
	/// <summary>
	/// Design time class for holding a skydome. This is created by
	/// the SkyProcessor, then written out to a compiled XNB file.
	/// At runtime, the data is loaded into the runtime Sky class.
	/// </summary>
	public class SkyContent
	{
		public ModelContent Model;
		public TextureContent Texture;
	}

	/// <summary>
	/// Content pipeline support class for saving out SkyContent objects.
	/// </summary>
	[ContentTypeWriter]
	public class SkyContentWriter : ContentTypeWriter<SkyContent>
	{
		/// <summary>
		/// Saves sky data into an XNB file.
		/// </summary>
		protected override void Write(ContentWriter output, SkyContent value)
		{
			output.WriteObject(value.Model);
			output.WriteObject(value.Texture);
		}

		/// <summary>
		/// Tells the content pipeline what CLR type the sky
		/// data will be loaded into at runtime.
		/// </summary>
		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return "Nibiru.SkyContent, NibiruEngine";
		}

		/// <summary>
		/// Tells the content pipeline what worker type
		/// will be used to load the sky data.
		/// </summary>
		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return "Nibiru.SkyContentReader, NibiruEngine";
		}
	}
}
