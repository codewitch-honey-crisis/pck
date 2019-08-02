using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class XbnfUnaryExpression : XbnfExpression
	{
		public XbnfExpression Expression { get; set; } = null;
	}
}
