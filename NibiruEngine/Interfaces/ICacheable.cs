using System;
using System.Collections.Generic;

namespace Nibiru.Interfaces
{
	public interface ICacheable
	{
		string Resource { get; }
		bool Persist { get; set; }
	}
}
