using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
		static IDictionary<string, IList<CharRange>> _charClasses = _GetCharacterClasses();
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
				//Debug.Assert(null != val.FirstAcceptingState);
				var nval = val.Clone();
				//Debug.Assert(null != nval.FirstAcceptingState);
				if (null == left)
				{
					left = nval;
					//Debug.Assert(null != left.FirstAcceptingState);
					continue;
				}
				else if (null == right)
				{
					right = nval;
					//Debug.Assert(null != right.FirstAcceptingState);
				}
				else
				{
					//Debug.Assert(null != right.FirstAcceptingState);
					_Concat(right, nval);
					//Debug.Assert(null != right.FirstAcceptingState);

				}
				//Debug.Assert(null != left.FirstAcceptingState);
				_Concat(left, right.Clone());
				//Debug.Assert(null != left.FirstAcceptingState);

			}
			if (null != right)
			{
				right.FirstAcceptingState.AcceptSymbol = accept;
			}
			else
			{
				left.FirstAcceptingState.AcceptSymbol = accept;
			}
			return left;
		}
		static void _Concat(CharFA<TAccept> lhs, CharFA<TAccept> rhs)
		{
			//Debug.Assert(lhs != rhs);
			var f = lhs.FirstAcceptingState;
			//Debug.Assert(null != rhs.FirstAcceptingState);
			f.IsAccepting = false;
			f.EpsilonTransitions.Add(rhs);
			//Debug.Assert(null!= lhs.FirstAcceptingState);

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
				if (null != fa)
				{
					var nfa = fa.Clone();
					result.EpsilonTransitions.Add(nfa);
					var nffa = nfa.FirstAcceptingState;
					nffa.IsAccepting = false;
					nffa.EpsilonTransitions.Add(final);
				}
				else if(!result.EpsilonTransitions.Contains(final))
					result.EpsilonTransitions.Add(final);
			}
			return result;
		}
		/// <summary>
		/// Creates a new FA that will match a repetition of the specified FA expression
		/// </summary>
		/// <param name="expr">The expression to repeat</param>
		/// <param name="minOccurs">The minimum number of times to repeat or -1 for unspecified (0)</param>
		/// <param name="maxOccurs">The maximum number of times to repeat or -1 for unspecified (unbounded)</param>
		/// <param name="accept">The symbol to accept</param>
		/// <returns>A new FA that matches the specified FA one or more times</returns>
		public static CharFA<TAccept> Repeat(CharFA<TAccept> expr, int minOccurs = -1, int maxOccurs = -1, TAccept accept = default(TAccept))
		{
			expr = expr.Clone();
			if (minOccurs >0 && maxOccurs > 0 && minOccurs > maxOccurs)
				throw new ArgumentOutOfRangeException(nameof(maxOccurs));
			CharFA<TAccept> result;
			switch (minOccurs)
			{
				case -1:
				case 0:
					switch (maxOccurs)
					{
						case -1:
						case 0:
							result = new CharFA<TAccept>();
							var final = new CharFA<TAccept>(true,accept);
							final.EpsilonTransitions.Add(result);
							foreach (var afa in expr.FillAccepting())
							{
								afa.IsAccepting = false;
								afa.EpsilonTransitions.Add(final);
							}
							result.EpsilonTransitions.Add(expr);
							result.EpsilonTransitions.Add(final);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
						case 1:
							result = Optional(expr, accept);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
						default:
							var l = new List<CharFA<TAccept>>();
							expr = Optional(expr);
							l.Add(expr);
							for (int i = 1; i < maxOccurs; ++i)
							{
								l.Add(expr.Clone());
							}
							result = Concat(l, accept);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
					}
				case 1:
					switch (maxOccurs)
					{
						case -1:
						case 0:
							result = new CharFA<TAccept>();
							var final = new CharFA<TAccept>(true,accept);
							final.EpsilonTransitions.Add(result);
							foreach (var afa in expr.FillAccepting())
							{
								afa.IsAccepting = false;
								afa.EpsilonTransitions.Add(final);
							}
							result.EpsilonTransitions.Add(expr);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
						case 1:
							//Debug.Assert(null != expr.FirstAcceptingState);
							return expr;
						default:
							result = Concat(new CharFA<TAccept>[] { expr, Repeat(expr.Clone(), 0, maxOccurs - 1) },accept);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
					}
				default:
					switch (maxOccurs)
					{
						case -1:
						case 0:
							result = Concat(new CharFA<TAccept>[] { Repeat(expr, minOccurs, minOccurs, accept), Repeat(expr, 0, 0, accept) },accept);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;
						case 1:
							throw new ArgumentOutOfRangeException(nameof(maxOccurs));
						default:
							if (minOccurs == maxOccurs)
							{
								var l = new List<CharFA<TAccept>>();
								l.Add(expr);
								//Debug.Assert(null != expr.FirstAcceptingState);
								for (int i = 1; i < minOccurs; ++i)
								{
									var e = expr.Clone();
									//Debug.Assert(null != e.FirstAcceptingState);
									l.Add(e);
								}
								result = Concat( l, accept);
								//Debug.Assert(null != result.FirstAcceptingState);
								return result;
							}
							result = Concat(new CharFA<TAccept>[] { Repeat(expr.Clone(), minOccurs, minOccurs, accept), Repeat(Optional(expr.Clone()), maxOccurs - minOccurs, maxOccurs - minOccurs, accept) },accept);
							//Debug.Assert(null != result.FirstAcceptingState);
							return result;


					}
			}
			// should never get here
			throw new NotImplementedException();
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

		protected override FA<char, TAccept> CreateFA(bool isAccepting = false, TAccept accept = default(TAccept))
		{
			return new CharFA<TAccept>(isAccepting, accept);
		}
		public new CharFA<TAccept> Reduce()
		{
			return base.Reduce() as CharFA<TAccept>;
		}
		public new CharFA<TAccept> ToDfa(IProgress<FAProgress> progress=null)
		{
			return base.ToDfa(progress) as CharFA<TAccept>;
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
					if(null!=symbolTable[i])
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
		
		public static IDictionary<string, IList<CharRange>> CharacterClasses
			=> _charClasses;
		static IDictionary<string,IList<CharRange>> _GetCharacterClasses()
		{
			var result = new Dictionary<string, IList<CharRange>>();
			result.Add("alnum", 
				new List<CharRange>(
					new CharRange[] {
						new CharRange('A','Z'),
						new CharRange('a', 'z'),
						new CharRange('0', '9')
					}));
			result.Add("alpha",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('A','Z'),
						new CharRange('a', 'z')
					}));
			result.Add("ascii",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('\0','\x7F')
					}));
			result.Add("blank",
				new List<CharRange>(
					new CharRange[] {
						new CharRange(' ',' '),
						new CharRange('\t','\t')
					}));
			result.Add("cntrl",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('\0','\x1F'),
						new CharRange('\x7F','\x7F')
					}));
			result.Add("digit",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('0', '9')
					}));
			result.Add("^digit", new List<CharRange>(CharRange.NotRanges(result["digit"])));
			result.Add("graph", 
				new List<CharRange>(
					new CharRange[] {
						new CharRange('\x21', '\x7E')
					}));
			result.Add("lower",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('a', 'z')
					}));
			result.Add("print",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('\x20', '\x7E')
					}));
			// [!"\#$%&'()*+,\-./:;<=>?@\[\\\]^_`{|}~]	
			result.Add("punct",
				new List<CharRange>(
					CharRange.GetRanges("!\"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~")
					));
			//[ \t\r\n\v\f]
			result.Add("space",
					new List<CharRange>(
						CharRange.GetRanges(" \t\r\n\v\f")
						));
			result.Add("^space", new List<CharRange>(CharRange.NotRanges(result["space"])));
			result.Add("upper",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('A', 'Z')
					}));
			result.Add("word",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('0', '9'),
						new CharRange('A', 'Z'),
						new CharRange('_', '_'),
						new CharRange('a', 'z')
					}));
			result.Add("^word",new List<CharRange>(CharRange.NotRanges(result["word"])));
			result.Add("xdigit",
				new List<CharRange>(
					new CharRange[] {
						new CharRange('0', '9'),
						new CharRange('A', 'F'),
						new CharRange('a', 'f')
					}));
			return result;
		}
	
	}
}
