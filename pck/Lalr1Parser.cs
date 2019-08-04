using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum LRNodeType
	{
		Initial = -1,
		Shift = 0,
		Reduce = 1,
		Accept = 2,
		EndDocument = -2
	}

}
