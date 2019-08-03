using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class RegexRepeatExpression : RegexUnaryExpression, IEquatable<RegexRepeatExpression>
	{
		public override bool IsSingleElement => true;
		public RegexRepeatExpression(RegexExpression expression,int minOccurs=-1,int maxOccurs=-1)
		{
			Expression = expression;
			MinOccurs = minOccurs;
			MaxOccurs = maxOccurs;
		}
		public RegexRepeatExpression() { }

		public int MinOccurs { get; set; } = -1;
		public int MaxOccurs { get; set; } = -1; // kleene by default

		public override CharFA<TAccept> ToFA<TAccept>(TAccept accept)
			=> null != Expression ? CharFA<TAccept>.Repeat(Expression.ToFA(accept),MinOccurs,MaxOccurs, accept) : null;

		protected internal override void AppendTo(StringBuilder builder)
		{
			var ise = null!=Expression && Expression.IsSingleElement;
			if (!ise)
				builder.Append('(');
			if(null!=Expression)
				Expression.AppendTo(builder);
			if (!ise)
				builder.Append(')');

			switch (MinOccurs)
			{
				case -1:
				case 0:
					switch(MaxOccurs)
					{
						case -1:
						case 0:
							builder.Append('*');
							break;
						default:
							builder.Append('{');
							if (-1 != MinOccurs)
								builder.Append(MinOccurs);
							builder.Append(',');
							builder.Append(MaxOccurs);
							builder.Append('}');
							break;
					}
					break;
				case 1:
					switch (MaxOccurs)
					{
						case -1:
						case 0:
							builder.Append('+');
							break;
						default:
							builder.Append("{1,");
							builder.Append(MaxOccurs);
							builder.Append('}');
							break;
					}
					break;
				default:
					builder.Append('{');
					if (-1 != MinOccurs)
						builder.Append(MinOccurs);
					builder.Append(',');
					if (-1 != MaxOccurs)
						builder.Append(MaxOccurs);
					builder.Append('}');
					break;
			}
		}
		protected override RegexExpression CloneImpl()
			=> Clone();
		public RegexRepeatExpression Clone()
		{
			return new RegexRepeatExpression(Expression,MinOccurs,MaxOccurs);
		}
		#region Value semantics
		public bool Equals(RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if(Equals(Expression, rhs.Expression))
			{
				var lmio = Math.Max(0, MinOccurs);
				var lmao = Math.Max(0, MaxOccurs);
				var rmio = Math.Max(0, rhs.MinOccurs);
				var rmao = Math.Max(0, rhs.MaxOccurs);
				return lmio == rmio && lmao == rmao;
			}
			return false;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as RegexRepeatExpression);

		public override int GetHashCode()
		{
			var result = MinOccurs ^ MaxOccurs;
			if (null != Expression)
				return result ^ Expression.GetHashCode();
			return result;
		}

		public static bool operator ==(RegexRepeatExpression lhs, RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(RegexRepeatExpression lhs, RegexRepeatExpression rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
