using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	public class XbnfDocument : IEquatable<XbnfDocument>, ICloneable
	{
		public XbnfProductionList Productions { get; } = new XbnfProductionList();
		public XbnfDocument Clone()
		{
			var result = new XbnfDocument();
			for(int ic=Productions.Count,i=0;i<ic;++i)
				result.Productions.Add(Productions[i].Clone());
			return result;
		}
		object ICloneable.Clone()
			=> Clone();
		public override string ToString()
		{
			var sb = new StringBuilder();
			for(int ic = Productions.Count,i=0;i<ic;++i)
				sb.AppendLine(Productions[i].ToString());
			return sb.ToString();
		}
		internal static XbnfDocument Parse(ParseContext pc)
		{
			var result = new XbnfDocument();
			while (-1 != pc.Current)
			{
				result.Productions.Add(XbnfProduction.Parse(pc));
				// have to do this so trailing whitespace
				// doesn't get read as a production
				pc.TryReadCCommentsAndWhitespace();
			}
			return result;
		}
		public static XbnfDocument Parse(IEnumerable<char> @string)
			=> Parse(ParseContext.Create(@string));
		public static XbnfDocument ReadFrom(TextReader reader)
			=> Parse(ParseContext.CreateFrom(reader));
		public static XbnfDocument ReadFrom(string filename)
		{
			using (var sr = File.OpenText(filename))
				return ReadFrom(sr);
		}
		#region Value semantics
		public bool Equals(XbnfDocument rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			var lc = Productions.Count;
			var rc = rhs.Productions.Count;
			if (lc != rc) return false;
			for (var i=0;i<lc;++i)
				if (Productions[i] != rhs.Productions[i])
					return false;
			return true;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as XbnfDocument);

		public override int GetHashCode()
		{
			var result = 0;
			for(int ic=Productions.Count,i=0;i<ic;++i)
				if (null != Productions[i])
					result ^=Productions[i].GetHashCode();
			
			return result;
		}
		public static bool operator==(XbnfDocument lhs, XbnfDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(XbnfDocument lhs, XbnfDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
