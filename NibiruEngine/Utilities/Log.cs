using System;
using System.Collections.Generic;
using System.Text;

namespace Nibiru
{
	public enum LogLevels { None = 0, Error, Warning, Info, Debug };

	internal static class Log
	{
		public static LogLevels Level { get; set; }

		internal static void Write(string text)
		{
			Write(null, text);
		}

		internal static void Write(object it, string text)
		{
			Write(it, text, LogLevels.Debug);
		}

		/// <summary>
		/// Static function used to log textual information to the console output.
		/// </summary>
		/// <param name="it"></param>
		/// <param name="text"></param>
		/// <param name="level"></param>
		internal static void Write(object it, string text, LogLevels level)
		{
			string label = "";

			if (level > Level)
				return;

			switch (level)
			{
				case LogLevels.Debug:
					label = "[DEBUG] ";
					break;

				case LogLevels.Error:
					label = "[ERROR] ";
					break;

				case LogLevels.Info:
					break;

				case LogLevels.Warning:
					label = "[WARNING] ";
					break;
			}

			if (it == null)
				Console.WriteLine(label + text);
			else
				Console.WriteLine(it.GetType().Name + ": " + label + text);
		}
	}
}
