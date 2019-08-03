using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class RegexUnaryExpression : RegexExpression
	{
		public RegexExpression Expression { get; set; }

	}
}
