using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	public class LexDocument : IEquatable<LexDocument>, ICloneable
	{
		public IDictionary<string, LexAttributeList> AttributeSets { get; } = new ListDictionary<string, LexAttributeList>();
		public LexRuleList Rules { get; } = new LexRuleList();
		public static LexDocument Parse(IEnumerable<char> @string)
			=> Parse(ParseContext.Create(@string));
		public static LexDocument ReadFrom(TextReader reader)
			=> Parse(ParseContext.Create(reader));
		public static LexDocument ReadFrom(string filename)
		{
			using (var sr = File.OpenText(filename))
				return ReadFrom(sr);
		}
		internal static LexDocument Parse(ParseContext pc)
		{
			var result = new LexDocument();
			while (-1 != pc.Current)
			{
				var line = pc.Line;
				var column = pc.Column;
				var position = pc.Position;
				LexNode.SkipCommentsAndWhitespace(pc);
				while ('\n' == pc.Current)
				{
					pc.Advance();
					LexNode.SkipCommentsAndWhitespace(pc);
				}
				var id = LexNode.ParseIdentifier(pc);
				LexNode.SkipCommentsAndWhitespace(pc);

				pc.Expecting(':', '-', '=');
				if (':' == pc.Current) // attribute set
				{
					pc.Advance();
					var d = new LexAttributeList();
					while (-1 != pc.Current && '\n' != pc.Current)
					{
						var attr = LexAttribute.Parse(pc);
						d.Add(attr);

						LexNode.SkipCommentsAndWhitespace(pc);
						pc.Expecting('\n', ',', -1);
						if (',' == pc.Current)
							pc.Advance();
					}
					result.AttributeSets.Add(id, d);
					LexNode.SkipCommentsAndWhitespace(pc);
				}
				else if ('=' == pc.Current)
				{
					pc.Advance();
					LexNode.SkipCommentsAndWhitespace(pc);
					pc.Expecting('\'');
					pc.Advance();
					var l = pc.CaptureBuffer.Length;
					pc.TryReadUntil('\'', '\\', false);
					pc.Expecting('\'');
					pc.Advance();
					var rx = pc.GetCapture(l);
					// make sure to capture the line numbers properly:
					var rpc = ParseContext.Create(rx);
					rpc.Line = pc.Line;
					rpc.Column = pc.Column;
					rpc.Position = pc.Position;
					var rule = new LexRule(id, RegexExpression.Parse(rpc));
					rule.SetLocationInfo(line, column, position);
					result.Rules.Add(rule);
				}
				else if ('-' == pc.Current)
				{
					pc.TrySkipUntil('\n', true);
				}
				LexNode.SkipCommentsAndWhitespace(pc);
				if ('\n' == pc.Current)
					pc.Advance();
			}
			return result;
		}
		public object GetAttribute(string symbol, string name, object @default = null)
		{
			LexAttributeList l;
			if (AttributeSets.TryGetValue(symbol, out l))
			{
				var i = l.IndexOf(name);
				if (-1 < i)
					return l[i].Value;
			}
			return @default;
		}
		public override string ToString()
		{
			var sb = new StringBuilder();
			foreach (var attrSet in AttributeSets)
			{
				if (0 < attrSet.Value.Count)
				{
					sb.Append(string.Concat(attrSet.Key, ":"));
					var delim = "";
					for (int jc = attrSet.Value.Count, j = 0; j < jc; ++j)
					{
						sb.Append(string.Concat(delim, attrSet.Value[j].ToString()));
						delim = ", ";
					}
					sb.AppendLine();
				}
			}
			for(int ic=Rules.Count,i=0;i<ic;++i)
				sb.AppendLine(Rules[i].ToString());
			return sb.ToString();
		}
		public CharFA<string> ToLexer()
		{
			var result = new CharFA<string>();
			for(int ic=Rules.Count,i=0;i<ic;++i)
			{
				var rule = Rules[i];
				result.EpsilonTransitions.Add(rule.Right.ToFA(rule.Left));
			}
			return result.ToDfa() as CharFA<string>;
		}

		public LexDocument Clone()
		{
			var result = new LexDocument();
			foreach (var attrs in AttributeSets)
			{
				var d = new LexAttributeList();
				result.AttributeSets.Add(attrs.Key, d);
				foreach (var attr in attrs.Value)
					d.Add(attr.Clone());
			}
			var ic = Rules.Count;
			for (var i = 0; i < ic; ++i)
				result.Rules.Add(Rules[i].Clone());
			return result;
		}
		object ICloneable.Clone()
			=> Clone();

		#region Value semantics
		public bool Equals(LexDocument rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			else if (ReferenceEquals(rhs, null)) return false;
			if (AttributeSets.Count != rhs.AttributeSets.Count)
				return false;
			foreach (var attrs in AttributeSets)
			{
				LexAttributeList d;
				if (!rhs.AttributeSets.TryGetValue(attrs.Key, out d))
				{
					if (d.Count != attrs.Value.Count)
						return false;
					foreach (var attr in attrs.Value)
					{
						var i = d.IndexOf(attr.Name);
						if (0 > i || !Equals(d[i].Value, attr.Value))
							return false;
					}
				}
			}
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			var lc = Rules.Count;
			var rc = rhs.Rules.Count;
			if (lc != rc) return false;
			for (var i = 0; i < lc; ++i)
				if (Rules[i] != rhs.Rules[i])
					return false;
			return true;
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as LexDocument);

		public override int GetHashCode()
		{
			var result = 0;
			foreach (var attrs in AttributeSets)
			{
				if (null != attrs.Key)
					result ^= attrs.Key.GetHashCode();
				foreach (var attr in attrs.Value)
				{
					if (null != attr.Name)
						result ^= attr.Name.GetHashCode();
					if (null != attr.Value)
						result ^= attr.Value.GetHashCode();
				}
			}
			for (int ic = Rules.Count, i = 0; i < ic; ++i)
				if (null != Rules[i])
					result ^= Rules[i].GetHashCode();
			return result;
		}
		public static bool operator ==(LexDocument lhs, LexDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(LexDocument lhs, LexDocument rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion
	}
}
