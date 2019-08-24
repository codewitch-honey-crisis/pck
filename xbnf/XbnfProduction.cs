using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfProduction : XbnfNode, IEquatable<XbnfProduction>, ICloneable
	{
		public XbnfProduction(string name,XbnfExpression expression=null)
		{
			Name = name;
			Expression = expression;
		}
		public bool IsTerminal {
			get {
				if (null != Expression && Expression.IsTerminal)
					return true;
				var i = Attributes.IndexOf("terminal");
				if(-1<i && Attributes[i].Value is bool && (bool)Attributes[i].Value)
					return true;
				return false;
			}
		}
		public XbnfProduction() { }
		public XbnfAttributeList Attributes { get; } = new XbnfAttributeList();
		public string Name { get; set; } = null;
		public XbnfExpression Expression { get; set; } = null;
		public XbnfProduction Clone()
		{
			var result = new XbnfProduction();
			result.Name = Name;
			for(int ic=Attributes.Count,i=0;i<ic;++i)
				result.Attributes.Add(Attributes[i].Clone());
			if(null!=Expression)
				result.Expression = Expression.Clone();
			return result;
		}
		object ICloneable.Clone()
			=> Clone();
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(Name);
			var ic = Attributes.Count;
			if(0<ic)
			{
				sb.Append("<");
				var delim = "";
				for (var i = 0;i<ic;++i)
				{
					sb.Append(delim);
					sb.Append(Attributes[i]);
					delim = ", ";
				}
				sb.Append(">");
			}
			if (null != Expression) {
				sb.Append("= ");
				sb.Append(Expression);
			}
			sb.Append(";");
			return sb.ToString();
		}
		/*
		internal static XbnfProduction Parse(ParseContext pc)
		{
			var result = new XbnfProduction();
			pc.TrySkipCCommentsAndWhiteSpace();
			var l = pc.Line;
			var c = pc.Column;
			var p = pc.Position;
			// read identifier
			result.Name=ParseIdentifier(pc);
			// read attributes
			if ('<'==pc.Current)
			{
				pc.Advance();
				while (-1 != pc.Current && '>' != pc.Current)
				{
					result.Attributes.Add(XbnfAttribute.Parse(pc));
					pc.TrySkipCCommentsAndWhiteSpace();
					pc.Expecting('>', ',');
					if (',' == pc.Current)
						pc.Advance();
				}
				pc.Expecting('>');
				pc.Advance();
			}
			pc.TrySkipCCommentsAndWhiteSpace();
			pc.Expecting(';', '=');
			if ('='==pc.Current)
			{
				pc.Advance();
				result.Expression = XbnfExpression.Parse(pc);
			}
			pc.Expecting(';');
			pc.Advance();
			result.SetLocation(l, c, p);
			return result;
		}
		*/
		#region Value semantics
		public bool Equals(XbnfProduction rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			if(Name == rhs.Name)
			{
				if(Expression==rhs.Expression)
				{
					for(int ic=Attributes.Count,i=0;i<ic;++i)
						if (!rhs.Attributes.Contains(Attributes[i]))
							return false;
					return true;
				}
			}
			return false;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfProduction);

		public override int GetHashCode()
		{
			var result = 0;
			for(int ic=Attributes.Count,i=0;i<ic;++i)
				result ^= Attributes[i].GetHashCode();
			
			if (null != Name)
				result ^=Name.GetHashCode();
			if (null != Expression)
				result ^= Expression.GetHashCode();

			return result;
		}
		public static bool operator ==(XbnfProduction lhs, XbnfProduction rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfProduction lhs, XbnfProduction rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
