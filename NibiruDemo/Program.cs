using System;

namespace NibiruDemo
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (GameDemo game = new GameDemo())
			{
				game.Run();
			}
		}
	}
}

