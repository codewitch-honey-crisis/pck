using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	// can't used named tuples since CodeDom doesn't really support them
	// (int Accept, (char[] Ranges, int Destination)[])
	using CharDfaEntry = KeyValuePair<int, KeyValuePair<string, int>[]>;
	/// <summary>
	/// Represents a simple FA based regular expression engine.
	/// </summary>
	/// <remarks>This is a specialialization of <see cref="FA.FA{TInput, TAccept}"/></remarks>
	public class CharFA<TAccept> : FA<char,TAccept>
	{
		public CharFA(bool isAccepting, TAccept accept = default(TAccept)) : base(isAccepting,accept) { }
		public CharFA() : base() { }

		
		public override string ToString()
		{
			var dfa = ToDfa() as CharFA<TAccept>;
			var sb = new StringBuilder();
			dfa._AppendTo(sb, new List<FA<char,TAccept>>());
			return sb.ToString();
		}
		void _AppendTo(StringBuilder sb, ICollection<FA<char,TAccept>> visited)
		{
			if (null != visited)
			{
				if (visited.Contains(this))
				{
					sb.Append("*");
					return;
				}
				visited.Add(this);
			}

			//var sb = new StringBuilder();
			var trgs = FillInputTransitionRangesGroupedByState();
			var delim = "";
			bool isAccepting = IsAccepting;
			if (1 < trgs.Count)
				sb.Append("(");
			foreach (var trg in trgs)
			{
				sb.Append(delim);
				//sb.Append("(");
				if (1 == trg.Value.Count && 1 == trg.Value[0].Length)
					_AppendRangeTo(sb, trg.Value[0]);
				else
				{
					sb.Append("[");
					foreach (var rng in trg.Value)
						_AppendRangeTo(sb, rng);
					sb.Append("]");
				}
				((CharFA<TAccept>)trg.Key)._AppendTo(sb, new List<FA<char,TAccept>>(visited));
				//sb.Append(")");
				delim = "|";
			}
			if (1 < trgs.Count)
				sb.Append(")");
			if (isAccepting && !IsFinal && !IsLoop)
				sb.Append("?");
		}
		public bool IsLiteral {
			get {
				var closure = FillClosure();
				int ic = closure.Count, i = 0;
				for (;i<ic;++i)
				{
					var fa = closure[i];
					if (!(fa.IsNeutral || fa.IsFinal || (0 == fa.EpsilonTransitions.Count && 1 == fa.Transitions.Count)))
						break;
				}
				return i == ic;
			}
		}
		/// <summary>
		/// Writes a Graphviz dot specification to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer</param>
		/// <param name="options">A <see cref="DotGraphOptions"/> instance with any options, or null to use the defaults</param>
		public override void WriteDotTo(TextWriter writer, DotGraphOptions options = null)
		{
			_WriteDotTo(FillClosure(), writer, options);
		}
		/// <summary>
		/// Writes a Graphviz dot specification of the specified closure to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="closure">The closure of all states</param>
		/// <param name="writer">The writer</param>
		/// <param name="options">A <see cref="DotGraphOptions"/> instance with any options, or null to use the defaults</param>
		static void _WriteDotTo(IList<FA<char,TAccept>> closure, TextWriter writer, DotGraphOptions options = null)
		{
			if (null == options) options = new DotGraphOptions();
			string spfx = null == options.StatePrefix ? "q" : options.StatePrefix;
			writer.WriteLine("digraph FA {");
			writer.WriteLine("rankdir=LR");
			writer.WriteLine("node [shape=circle]");
			var finals = new List<FA<char,TAccept>>();
			var neutrals = new List<FA<char,TAccept>>();
			var accepting = FillAcceptingStates(closure, null);
			foreach (var ffa in closure)
				if (ffa.IsFinal && !ffa.IsAccepting)
					finals.Add(ffa);

			IList<FA<char,TAccept>> fromStates = null;
			IList<FA<char,TAccept>> toStates = null;

			char tchar = default(char);
			if (null != options.DebugString)
			{
				toStates = closure[0].FillEpsilonClosure();
				if (null==fromStates)
					fromStates = toStates;
				foreach (char ch in options.DebugString)
				{
					tchar = ch;
					toStates = FillMove(fromStates, ch);
					if (0 == toStates.Count)
						break;
					fromStates = toStates;
				}
			}
			if (null != toStates)
			{
				toStates = FillEpsilonClosure(toStates, null);
			}
			int i = 0;
			foreach (var ffa in closure)
			{
				if (!finals.Contains(ffa))
				{
					if (ffa.IsAccepting)
						accepting.Add(ffa);
					else if (ffa.IsNeutral)
						neutrals.Add(ffa);
				}
				var rngGrps = ((CharFA<TAccept>)ffa).FillInputTransitionRangesGroupedByState(null);
				foreach (var rngGrp in rngGrps)
				{
					var di = closure.IndexOf(rngGrp.Key);
					writer.Write(spfx);
					writer.Write(i);
					writer.Write("->");
					writer.Write(spfx);
					writer.Write(di.ToString());
					writer.Write(" [label=\"");
					var sb = new StringBuilder();
					foreach (var range in rngGrp.Value)
						_AppendRangeTo(sb, range);
					if (sb.Length != 1 || " " == sb.ToString())
					{
						writer.Write('[');
						writer.Write(_EscapeLabel(sb.ToString()));
						writer.Write(']');
					}
					else
						writer.Write(_EscapeLabel(sb.ToString()));
					writer.WriteLine("\"]");
				}
				// do epsilons
				foreach (var fffa in ffa.EpsilonTransitions)
				{
					writer.Write(spfx);
					writer.Write(i);
					writer.Write("->");
					writer.Write(spfx);
					writer.Write(closure.IndexOf(fffa));
					writer.WriteLine(" [style=dashed,color=gray]");
				}
				++i;
			}
			string delim = "";
			i = 0;
			foreach (var ffa in closure)
			{
				writer.Write(spfx);
				writer.Write(i);
				writer.Write(" [");
				if (null != options.DebugString)
				{
					if (null != toStates && toStates.Contains(ffa))
						writer.Write("color=green,");
					else if (null != fromStates && fromStates.Contains(ffa) && (null == toStates || !toStates.Contains(ffa)))
						writer.Write("color=darkgreen,");
				}
				writer.Write("label=<");
				writer.Write("<TABLE BORDER=\"0\"><TR><TD>");
				writer.Write(spfx);
				writer.Write("<SUB>");
				writer.Write(i);
				writer.Write("</SUB></TD></TR>");

				if (null != options.DebugString && null != options.DebugSourceNfa && null != ffa.Tag)
				{
					var tags = ffa.Tag as IEnumerable;
					if (null != tags || ffa.Tag is FA<char,TAccept>)
					{
						writer.Write("<TR><TD>{");
						if (null == tags)
						{
							writer.Write(" q<SUB>");
							writer.Write(options.DebugSourceNfa.FillClosure().IndexOf((FA<char,TAccept>)ffa.Tag).ToString());
							writer.Write("</SUB>");
						}
						else
						{
							delim = "";
							foreach (var tag in tags)
							{
								writer.Write(delim);
								if (tag is FA<char,TAccept>)
								{
									writer.Write(delim);
									writer.Write(" q<SUB>");
									writer.Write(options.DebugSourceNfa.FillClosure().IndexOf((FA<char,TAccept>)tag).ToString());
									writer.Write("</SUB>");
									// putting a comma here is what we'd like
									// but it breaks dot no matter how its encoded
									delim = @" ";
								}
							}
						}
						writer.Write(" }</TD></TR>");
					}

				}
				if (ffa.IsAccepting)
				{
					writer.Write("<TR><TD>");
					writer.Write(Convert.ToString(ffa.AcceptSymbol).Replace("\"", "&quot;"));
					writer.Write("</TD></TR>");
					
				}
				writer.Write("</TABLE>");
				writer.Write(">");
				bool isfinal = false;
				if (accepting.Contains(ffa) || (isfinal = finals.Contains(ffa)))
					writer.Write(",shape=doublecircle");
				if (isfinal || neutrals.Contains(ffa))
				{
					if ((null == fromStates || !fromStates.Contains(ffa)) &&
						(null == toStates || !toStates.Contains(ffa)))
					{
						writer.Write(",color=gray");
					}
				}
				writer.WriteLine("]");
				++i;
			}
			delim = "";
			if (0 < accepting.Count)
			{
				foreach (var ntfa in accepting)
				{
					writer.Write(delim);
					writer.Write(spfx);
					writer.Write(closure.IndexOf(ntfa));
					delim = ",";
				}
				writer.WriteLine(" [shape=doublecircle]");
			}
			delim = "";
			if (0 < neutrals.Count)
			{

				foreach (var ntfa in neutrals)
				{
					if ((null == fromStates || !fromStates.Contains(ntfa)) &&
						(null == toStates || !toStates.Contains(ntfa))
						)
					{
						writer.Write(delim);
						writer.Write(spfx);
						writer.Write(closure.IndexOf(ntfa));
						delim = ",";
					}
				}
				writer.WriteLine(" [color=gray]");
				delim = "";
			}
			delim = "";
			if (0 < finals.Count)
			{
				foreach (var ntfa in finals)
				{
					writer.Write(delim);
					writer.Write(spfx);
					writer.Write(closure.IndexOf(ntfa));
					delim = ",";
				}
				writer.WriteLine(" [shape=doublecircle,color=gray]");
			}

			writer.WriteLine("}");

		}
		public static void _AppendRangeTo(StringBuilder builder, CharRange range)
		{
			_AppendRangeCharTo(builder, range.First);
			if (0 == range.Last.CompareTo(range.First)) return;
			if (range.Last == range.First + 1) // spit out 1 length ranges as two chars
			{
				_AppendRangeCharTo(builder, range.Last);
				return;
			}
			builder.Append('-');
			_AppendRangeCharTo(builder, range.Last);
		}
		static void _AppendRangeCharTo(StringBuilder builder, char rangeChar)
		{
			switch (rangeChar)
			{
				case '-':
				case '\\':
					builder.Append('\\');
					builder.Append(rangeChar);
					return;
				case '\t':
					builder.Append("\\t");
					return;
				case '\n':
					builder.Append("\\n");
					return;
				case '\r':
					builder.Append("\\r");
					return;
				case '\0':
					builder.Append("\\0");
					return;
				case '\f':
					builder.Append("\\f");
					return;
				case '\v':
					builder.Append("\\v");
					return;
				case '\b':
					builder.Append("\\b");
					return;
				default:
					if (!char.IsLetterOrDigit(rangeChar) && !char.IsSeparator(rangeChar) && !char.IsPunctuation(rangeChar) && !char.IsSymbol(rangeChar))
					{

						builder.Append("\\u");
						builder.Append(unchecked((ushort)rangeChar).ToString("x4"));

					}
					else
						builder.Append(rangeChar);
					break;
			}
		}
		static string _EscapeLabel(string label)
		{
			if (string.IsNullOrEmpty(label)) return label;

			string result = label.Replace("\\", @"\\");
			result = result.Replace("\"", "\\\"");
			result = result.Replace("\n", "\\n");
			result = result.Replace("\r", "\\r");
			result = result.Replace("\0", "\\0");
			result = result.Replace("\v", "\\v");
			result = result.Replace("\t", "\\t");
			result = result.Replace("\f", "\\f");
			return result;
		}

		/// <summary>
		/// Creates an FA that matches a literal string
		/// </summary>
		/// <param name="string">The string to match</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA machine that will match this literal</returns>
		public static CharFA<TAccept> Literal(IEnumerable<char> @string, TAccept accept = default(TAccept))
		{
			var result = new CharFA<TAccept>();
			var current = result;
			foreach (var ch in @string)
			{
				current.IsAccepting = false;
				var fa = new CharFA<TAccept>(true, accept);
				current.Transitions.Add(ch, fa);
				current = fa;
			}
			return result;
		}
		/// <summary>
		/// Creates an FA that will match any one of a set of a characters
		/// </summary>
		/// <param name="set">The set of characters that will be matched</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>An FA that will match the specified set</returns>
		public static CharFA<TAccept> Set(IEnumerable<char> set, TAccept accept = default(TAccept))
		{
			var result = new CharFA<TAccept>();
			var final = new CharFA<TAccept>(true, accept);
			foreach (var ch in set)
				result.Transitions.Add(ch, final);
			return result;
		}
		/// <summary>
		/// Creates a new FA that is a concatenation of two other FA expressions
		/// </summary>
		/// <param name="exprs">The FAs to concatenate</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that is the concatenation of the specified FAs</returns>
		public static CharFA<TAccept> Concat(IEnumerable<CharFA<TAccept>> exprs, TAccept accept = default(TAccept))
		{
			CharFA<TAccept> left = null;
			var right = left;
			foreach (var val in exprs)
			{
				if (null == val) continue;
				var nval = val.Clone();
				if (null == left)
				{
					left = nval;
					continue;
				}
				else if (null == right)
					right = nval;
				else
					_Concat(right, nval);

				_Concat(left, right);
			}
			right.FirstAcceptingState.AcceptSymbol = accept;
			return left;
		}
		static void _Concat(CharFA<TAccept> lhs, CharFA<TAccept> rhs)
		{
			var f = lhs.FirstAcceptingState;
			f.EpsilonTransitions.Add(rhs);
			f.IsAccepting = false;
		}
		/// <summary>
		/// Creates an FA that will match any one of a set of a characters
		/// </summary>
		/// <param name="ranges">The set ranges of characters that will be matched</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>An FA that will match the specified set</returns>
		public static CharFA<TAccept> Set(IEnumerable<CharRange> ranges, TAccept accept = default(TAccept))
		{
			var result = new CharFA<TAccept>();
			var final = new CharFA<TAccept>(true,accept);
			
			foreach (var ch in CharRange.ExpandRanges(ranges))
				result.Transitions.Add(ch, final);
			return result;
		}
		/// <summary>
		/// Creates a new FA that matches any one of the FA expressions passed
		/// </summary>
		/// <param name="exprs">The expressions to match</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that will match the union of the FA expressions passed</returns>
		public static CharFA<TAccept> Or(IEnumerable<CharFA<TAccept>> exprs, TAccept accept = default(TAccept))
		{
			var result = new CharFA<TAccept>();
			var final = new CharFA<TAccept>(true, accept);
			foreach (var fa in exprs)
			{
				var nfa = fa.Clone();
				result.EpsilonTransitions.Add(nfa);
				var nffa = nfa.FirstAcceptingState;
				nffa.IsAccepting = false;
				nffa.EpsilonTransitions.Add(final);

			}
			return result;
		}
		/// <summary>
		/// Creates a new FA that will match a repetition of one or more of the specified FA expression
		/// </summary>
		/// <param name="expr">The expression to repeat</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that matches the specified FA one or more times</returns>
		public static CharFA<TAccept> Repeat(CharFA<TAccept> expr, TAccept accept = default(TAccept))
		{
			var result = new CharFA<TAccept>();
			var final = new CharFA<TAccept>(true, accept);
			var e = expr.Clone();
			var afa = e.FirstAcceptingState;
			afa.IsAccepting = false;
			afa.EpsilonTransitions.Add(final);
			afa.EpsilonTransitions.Add(result);
			result.EpsilonTransitions.Add(e);
			return result;
		}
		/// <summary>
		/// Creates a new FA that matches the specified FA expression or empty
		/// </summary>
		/// <param name="expr">The expression to make optional</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that will match the specified expression or empty</returns>
		public static CharFA<TAccept> Optional(CharFA<TAccept> expr, TAccept accept = default(TAccept))
		{
			var result = expr.Clone();
			var f = result.FirstAcceptingState;
			f.AcceptSymbol = accept;
			result.EpsilonTransitions.Add(f);
			return result;
		}
		/// <summary>
		/// Creates a new FA that will match a repetition of zero or more of the specified FA expressions
		/// </summary>
		/// <param name="expr">The expression to repeat</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that matches the specified FA zero or more times</returns>
		public static CharFA<TAccept> Kleene(CharFA<TAccept> expr, TAccept accept = default(TAccept))
		{
			return Optional(Repeat(expr), accept);
		}

		protected override FA<char, TAccept> CreateFA(bool isAccepting = false, TAccept accept = default(TAccept))
		{
			return new CharFA<TAccept>(isAccepting, accept);
		}
		public new CharDfaEntry[] ToArray(IList<TAccept> symbolTable = null)
		{
			var dfa = ToDfa();
			var closure = dfa.FillClosure();
			var symbolLookup = new ListDictionary<TAccept, int>();
			if (null==symbolTable)
			{
				var i = 0;
				for (int jc = closure.Count, j = 0; j < jc; ++j)
				{
					var fa = closure[j];
					if (fa.IsAccepting && !symbolLookup.ContainsKey(fa.AcceptSymbol))
					{
						symbolLookup.Add(fa.AcceptSymbol, i);
						++i;
					}
				}
			} else
				for (int ic = symbolTable.Count, i = 0; i < ic; ++i)
					symbolLookup.Add(symbolTable[i], i);
			
			var result = new CharDfaEntry[closure.Count];
			for (var i = 0; i < result.Length; i++)
			{
				var fa = closure[i];
				var trgs = ((CharFA<TAccept>)fa).FillInputTransitionRangesGroupedByState();
				var trns = new KeyValuePair<string, int>[trgs.Count];
				var j = 0;

				foreach (var trg in trgs)
				{
					trns[j] = new KeyValuePair<string, int>(
						CharRange.ToPackedString(trg.Value),
						closure.IndexOf(trg.Key));

					++j;
				}
				result[i] = new CharDfaEntry(
					fa.IsAccepting ? symbolLookup[fa.AcceptSymbol] : -1,
					trns);

			}
			return result;
		}

		/// <summary>
		/// Returns a <see cref="IDictionary{FA,IList{KeyValuePair{Char,Char}}}"/>, keyed by state, that contains all of the outgoing local input transitions, expressed as a series of ranges
		/// </summary>
		/// <param name="result">The <see cref="IDictionary{FA,IList{CharRange}}"/> to fill, or null to create one.</param>
		/// <returns>A <see cref="IDictionary{FA,IList{CharRange}}"/> containing the result of the query</returns>
		public IDictionary<FA<char,TAccept>, IList<CharRange>> FillInputTransitionRangesGroupedByState(IDictionary<FA<char,TAccept>, IList<CharRange>> result = null)
		{
			if (null == result)
				result = new Dictionary<FA<char,TAccept>, IList<CharRange>>();
			// using the optimized dictionary we have little to do here.
			foreach (var trns in (IDictionary<FA<char,TAccept>, ICollection<char>>)Transitions)
				result.Add( trns.Key, new List<CharRange>(CharRange.GetRanges(trns.Value)));
			return result;
		}
		public new CharFA<TAccept> Clone()
		{
			return base.Clone() as CharFA<TAccept>;
		}
		static char _ReadRangeChar(IEnumerator<char> e)
		{
			char ch;
			if ('\\' != e.Current || !e.MoveNext())
			{
				return e.Current;
			}
			ch = e.Current;
			switch (ch)
			{
				case 't':
					ch = '\t';
					break;
				case 'n':
					ch = '\n';
					break;
				case 'r':
					ch = '\r';
					break;
				case '0':
					ch = '\0';
					break;
				case 'v':
					ch = '\v';
					break;
				case 'f':
					ch = '\f';
					break;
				case 'b':
					ch = '\b';
					break;
				case 'x':
					if (!e.MoveNext())
						throw new ExpectingException("Expecting input for escape \\x");
					ch = e.Current;
					byte x = _FromHexChar(ch);
					if (!e.MoveNext())
					{
						ch = unchecked((char)x);
						return ch;
					}
					x *= 0x10;
					x += _FromHexChar(e.Current);
					ch = unchecked((char)x);
					break;
				case 'u':
					if (!e.MoveNext())
						throw new ExpectingException("Expecting input for escape \\u");
					ch = e.Current;
					ushort u = _FromHexChar(ch);
					if (!e.MoveNext())
					{
						ch = unchecked((char)u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					if (!e.MoveNext())
					{
						ch = unchecked((char)u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					if (!e.MoveNext())
					{
						ch = unchecked((char)u);
						return ch;
					}
					u *= 0x10;
					u += _FromHexChar(e.Current);
					ch = unchecked((char)u);
					break;
				default: // return itself
					break;
			}
			return ch;
		}
		static byte _FromHexChar(char hex)
		{
			if (':' > hex && '/' < hex)
				return (byte)(hex - '0');
			if ('G' > hex && '@' < hex)
				return (byte)(hex - '7'); // 'A'-10
			if ('g' > hex && '`' < hex)
				return (byte)(hex - 'W'); // 'a'-10
			throw new ArgumentException("The value was not hex.", "hex");
		}
		static int _ParseEscape(ParseContext pc)
		{
			if ('\\' != pc.Current)
				return -1;
			if (-1 == pc.Advance())
				return -1;
			switch (pc.Current)
			{
				case 't':
					pc.Advance();
					return '\t';
				case 'n':
					pc.Advance();
					return '\n';
				case 'r':
					pc.Advance();
					return '\r';
				case 'x':
					if (-1 == pc.Advance())
						return 'x';
					byte b = _FromHexChar((char)pc.Current);
					b <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)b);
					b |= _FromHexChar((char)pc.Current);
					return unchecked((char)b);
				case 'u':
					if (-1 == pc.Advance())
						return 'u';
					ushort u = _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					u <<= 4;
					if (-1 == pc.Advance())
						return unchecked((char)u);
					u |= _FromHexChar((char)pc.Current);
					return unchecked((char)u);
				default:
					int i = pc.Current;
					pc.Advance();
					return (char)i;
			}
		}
		static IEnumerable<CharRange> _ParseRanges(IEnumerable<char> charRanges)
		{
			using (var e = charRanges.GetEnumerator())
			{
				var skipRead = false;

				while (skipRead || e.MoveNext())
				{
					skipRead = false;
					char first = _ReadRangeChar(e);
					if (e.MoveNext())
					{
						if ('-' == e.Current)
						{
							if (e.MoveNext())
								yield return new CharRange(first, _ReadRangeChar(e));
							else
								yield return new CharRange('-', '-');
						}
						else
						{
							yield return new CharRange(first, first);
							skipRead = true;
							continue;

						}
					}
					else
					{
						yield return new CharRange(first, first);
						yield break;
					}
				}
			}
			yield break;
		}
		static IEnumerable<CharRange> _ParseRanges(IEnumerable<char> charRanges, bool normalize)
		{
			if (!normalize)
				return _ParseRanges(charRanges);
			else
			{
				var result = new List<CharRange>(_ParseRanges(charRanges));
				CharRange.NormalizeRangeList(result);
				return result;
			}
		}
		/// <summary>
		/// Parses a regular expresion from the specified string
		/// </summary>
		/// <param name="string">The string</param>
		/// <param name="accepting">The symbol reported when accepting the specified expression</param>
		/// <returns>A new machine that matches the regular expression</returns>
		public static CharFA<TAccept> Parse(IEnumerable<char> @string, TAccept accept = default(TAccept)) => Parse(ParseContext.Create(@string), accept);
		/// <summary>
		/// Parses a regular expresion from the specified <see cref="TextReader"/>
		/// </summary>
		/// <param name="reader">The text reader</param>
		/// <param name="accepting">The symbol reported when accepting the specified expression</param>
		/// <returns>A new machine that matches the regular expression</returns>
		public static CharFA<TAccept> ReadFrom(TextReader reader, TAccept accept = default(TAccept)) => Parse(ParseContext.Create(reader), accept);
		internal static CharFA<TAccept> Parse(ParseContext pc, TAccept accept)
		{
			CharFA<TAccept> result = new CharFA<TAccept>(true,accept);
			
			CharFA<TAccept> f, next;
			int ch;
			pc.EnsureStarted();
			var current = result;
			while (true)
			{
				switch (pc.Current)
				{
					case -1:
						return result;
					case '.':
						pc.Advance();
						f = current.FirstAcceptingState as CharFA<TAccept>;

						current = Set(new CharRange[] { new CharRange(char.MinValue, char.MaxValue) }, accept);
						switch (pc.Current)
						{
							case '*':
								current = Kleene(current, accept);
								pc.Advance();
								break;
							case '+':
								current = Repeat(current, accept);
								pc.Advance();
								break;
							case '?':
								current = Optional(current, accept);
								pc.Advance();
								break;

						}
						f.IsAccepting= false;
						f.EpsilonTransitions.Add(current);
						break;
					case '\\':
						if (-1 != (ch = _ParseEscape(pc)))
						{
							next = null;
							switch (pc.Current)
							{
								case '*':
									next = new CharFA<TAccept>();
									next.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
									next = Kleene(next, accept);
									pc.Advance();
									break;
								case '+':
									next = new CharFA<TAccept>();
									next.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
									next = Repeat(next, accept);
									pc.Advance();
									break;
								case '?':
									next = new CharFA<TAccept>();
									next.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
									next = Optional(next, accept);
									pc.Advance();
									break;
								default:
									current = current.FirstAcceptingState as CharFA<TAccept>;
									current.IsAccepting = false;
									current.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
									break;
							}
							if (null != next)
							{
								current = current.FirstAcceptingState as CharFA<TAccept>;
								current.IsAccepting = false;
								current.EpsilonTransitions.Add(next);
								current = next;
							}
						}
						else
						{
							pc.Expecting(); // throw an error
							return null; // doesn't execute
						}
						break;
					case ')':
						return result;
					case '(':
						pc.Advance();
						pc.Expecting();
						f = current.FirstAcceptingState as CharFA<TAccept>;
						current = Parse(pc, accept);
						pc.Expecting(')');
						pc.Advance();
						switch (pc.Current)
						{
							case '*':
								current = Kleene(current, accept);
								pc.Advance();
								break;
							case '+':
								current = Repeat(current, accept);
								pc.Advance();
								break;
							case '?':
								current = Optional(current, accept);
								pc.Advance();
								break;
						}
						var ff = f.FirstAcceptingState;
						ff.EpsilonTransitions.Add(current);
						ff.IsAccepting = false;
						break;
					case '|':
						if (-1 != pc.Advance())
						{
							current = Parse(pc, accept);
							result = Or(new CharFA<TAccept>[] { result as CharFA<TAccept>, current as CharFA<TAccept>}, accept);
						}
						else
						{
							current = current.FirstAcceptingState as CharFA<TAccept>;
							result = Optional(result, accept);
						}
						break;
					case '[':
						pc.ClearCapture();
						pc.Advance();
						pc.Expecting();
						bool not = false;
						if ('^' == pc.Current)
						{
							not = true;
							pc.Advance();
							pc.Expecting();
						}
						pc.TryReadUntil(']', '\\', false);
						pc.Expecting(']');
						pc.Advance();

						var r = (!not && "." == pc.Capture) ?
							new CharRange[] { new CharRange(char.MinValue, char.MaxValue) } :
							_ParseRanges(pc.Capture, true);
						if (not)
							r = CharRange.NotRanges(r);
						f = current.FirstAcceptingState as CharFA<TAccept>;
						current = Set(r, accept);
						switch (pc.Current)
						{
							case '*':
								current = Kleene(current, accept);
								pc.Advance();
								break;
							case '+':
								current = Repeat(current, accept);
								pc.Advance();
								break;
							case '?':
								current = Optional(current, accept);
								pc.Advance();
								break;

						}
						f.IsAccepting = false;
						f.EpsilonTransitions.Add(current);
						break;
					default:
						ch = pc.Current;
						pc.Advance();
						next = null;
						switch (pc.Current)
						{
							case '*':
								next = new CharFA<TAccept>();
								next.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
								next = Kleene(next, accept);
								pc.Advance();
								break;
							case '+':
								next = new CharFA<TAccept>();
								next.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
								next = Repeat(next, accept);
								pc.Advance();
								break;
							case '?':
								next = new CharFA<TAccept>();

								next.Transitions.Add((char)ch, new CharFA<TAccept>(true, accept));
								next = Optional(next, accept);
								pc.Advance();
								break;
							default:
								current = current.FirstAcceptingState as CharFA<TAccept>;
								current.IsAccepting = false;
								current.Transitions.Add((char)ch, new CharFA<TAccept>(true,accept));
								break;
						}
						if (null != next)
						{
							current = current.FirstAcceptingState as CharFA<TAccept>;
							current.IsAccepting = false;
							current.EpsilonTransitions.Add(next);
							current = next;
						}
						break;
				}
			}
		}
	}
}
