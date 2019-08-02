using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public abstract class XbnfBinaryExpression : XbnfExpression
	{
		public XbnfExpression Left { get; set; }
		public XbnfExpression Right { get; set; }
	}
}
