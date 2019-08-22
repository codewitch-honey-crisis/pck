using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	public class XbnfDocument : IEquatable<XbnfDocument>, ICloneable
	{
		public XbnfProduction StartProduction {
			get {
				var ic = Productions.Count;
				var firstNT = -1;
				for (var i = 0;i<ic;++i)
				{
					var prod = Productions[i];
					var hi = prod.Attributes.IndexOf("start");
					if(-1<hi)
					{
						var o = prod.Attributes[i].Value;
						if (o is bool && (bool)o)
							return prod;
					}
					if (-1 == firstNT && !prod.IsTerminal)
						firstNT = i;
				}
				if (-1!=firstNT)
					return Productions[firstNT];
				return null;
			}
			set {
				if (null!=value && !Productions.Contains(value))
					throw new InvalidOperationException(string.Concat("The production \"",value.Name,"\" is not the grammar."));
				for (int ic = Productions.Count, i = 0; i < ic; ++i)
				{
					if (null != value && Productions[i] == value)
					{
						var prod = Productions[i];
						prod.Attributes.Remove("start");
						prod.Attributes.Add(new XbnfAttribute("start", true));
					}
					else
					{
						var prod = Productions[i];
						var hi = prod.Attributes.IndexOf("start");
						if (-1 < hi)
							prod.Attributes.RemoveAt(hi);
					}
				}
			}
		}
		public XbnfProductionList Productions { get; } = new XbnfProductionList();
		public IList<XbnfMessage> TryValidate(IList<XbnfMessage> result = null)
		{
			if (null == result)
				result = new List<XbnfMessage>();
			var refCounts = new Dictionary<string, int>(EqualityComparer<string>.Default);

			foreach (var prod in Productions)
				refCounts.Add(prod.Name, 0);
			foreach (var prod in Productions)
			{
				_ValidateExpression(prod.Expression, refCounts, result);
			}
			foreach (var rc in refCounts)
			{
				if (0 == rc.Value)
				{
					var prod = Productions[rc.Key];
					object o;
					var i = prod.Attributes.IndexOf("hidden");
					var isHidden = false;
					if (-1<i)
					{
						o = prod.Attributes[i].Value;
						isHidden = (o is bool && (bool)o);
					}
					if (!isHidden && !Equals(rc.Key, StartProduction.Name))
						result.Add(new XbnfMessage(XbnfErrorLevel.Warning, -1, string.Concat("Unreferenced production \"", rc.Key, "\""),
							prod.Line, prod.Column, prod.Position));
				}
			}
			return result;
		}
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
		void _ValidateExpression(XbnfExpression expr, IDictionary<string, int> refCounts, IList<XbnfMessage> messages)
		{
			var l = expr as XbnfLiteralExpression;
			if (null != l)
			{

				string id = null;
				for(int ic = Productions.Count,i=0;i<ic;++i)
				{
					var ll = Productions[i].Expression as XbnfLiteralExpression;
					if(ll==l)
					{
						id = Productions[i].Name;
						break;
					}
				}
				// don't count itself. only things just like itself
				if (!string.IsNullOrEmpty(id) && !ReferenceEquals(Productions[id].Expression, l))
					refCounts[id] += 1;
			}
			
			var r = expr as XbnfRefExpression;
			if (null != r)
			{
				int rc;
				if (null == r.Symbol)
				{
					messages.Add(
						new XbnfMessage(
							XbnfErrorLevel.Error, -1,
							"Null reference expression",
							expr.Line, expr.Column, expr.Position));
					return;
				}
				if (!refCounts.TryGetValue(r.Symbol, out rc))
				{
					messages.Add(
						new XbnfMessage(
							XbnfErrorLevel.Error, -1,
							string.Concat(
								"Reference to undefined symbol \"",
								r.Symbol,
								"\""),
							expr.Line, expr.Column, expr.Position));
					return;
				}
				refCounts[r.Symbol] = rc + 1;
				return;
			}
			var b = expr as XbnfBinaryExpression;
			if (null != b)
			{
				if (null == b.Left && null == b.Right)
				{
					messages.Add(
						new XbnfMessage(
							XbnfErrorLevel.Warning, -1,
								"Nil expression",
							expr.Line, expr.Column, expr.Position));
					return;
				}
				_ValidateExpression(b.Left, refCounts, messages);
				_ValidateExpression(b.Right, refCounts, messages);
				return;
			}
			var u = expr as XbnfUnaryExpression;
			if (null != u)
			{
				if (null == u.Expression)
				{
					messages.Add(
						new XbnfMessage(
							XbnfErrorLevel.Warning, -1,
								"Nil expression",
							expr.Line, expr.Column, expr.Position));
					return;
				}
				_ValidateExpression(u.Expression, refCounts, messages);
			}
		}
	}
}
