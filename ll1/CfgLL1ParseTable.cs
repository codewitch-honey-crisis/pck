using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public struct CfgLL1ParseTableEntry
	{
		public CfgRule Rule;
		public CfgLL1ParseTableEntry(CfgRule rule)
		{
			Rule = rule;
		}
	}
	public class CfgLL1ParseTable : Dictionary<string, IDictionary<string, CfgLL1ParseTableEntry>>
	{
		static string _MakeSafeCsv(string field)
		{
			if (-1 < field.IndexOfAny(new char[] { ',', '\"', '\n', '\r' }))
				return string.Concat("\"", field.Replace("\"", "\"\""), "\"");
			return field;
		}
		public override string ToString()
		{
			var sb = new StringBuilder();
			var ntl = new List<string>();
			var tl = new List<string>();
			foreach (var entry in this)
			{
				ntl.Add(entry.Key);
				foreach (var entry2 in entry.Value)
					if (!tl.Contains(entry2.Key))
						tl.Add(entry2.Key);
			}

			sb.Append("LL(1) Parse Table");
			foreach (var t in tl)
			{
				sb.Append(",");
				sb.Append(_MakeSafeCsv(t));
			}
			sb.AppendLine();
			foreach (var nt in ntl)
			{
				sb.Append(_MakeSafeCsv(nt));
				foreach (var t in tl)
				{
					if (!Equals("#ERROR", t))
					{
						sb.Append(",");
						CfgLL1ParseTableEntry lr;
						IDictionary<string, CfgLL1ParseTableEntry> d;
						if (TryGetValue(nt, out d) && d.TryGetValue(t, out lr))
						{
							var r = lr.Rule;
							sb.Append(_MakeSafeCsv(r.Left));
							sb.Append(" ->");
							foreach (var sym in r.Right)
							{
								sb.Append(" ");
								sb.Append(_MakeSafeCsv(sym));
							}
						}
					}
				}
				sb.AppendLine();
			}
			return sb.ToString();
		}
		public int[][][] ToArray(IList<string> symbolTable = null)
		{
			if (null == symbolTable)
			{
				symbolTable = new List<string>();
				var ntl = new List<string>();
				var tl = new List<string>();
				foreach (var entry in this)
				{
					ntl.Add(entry.Key);
					foreach (var entry2 in entry.Value)
						if (!tl.Contains(entry2.Key))
							tl.Add(entry2.Key);
				}
				symbolTable.AddRange(ntl);
				symbolTable.AddRange(tl);
			}
			var result = new int[Count][][];
			foreach (var r in this)
			{
				var ntid = symbolTable.IndexOf(r.Key);
				if (0 > ntid) throw new ArgumentException(string.Concat("Non-terminal \"", r.Key, "\" not present in the symbol table"), "symbolTable");
				result[ntid] = new int[symbolTable.Count - Count][];
				foreach (var rr in r.Value)
				{
					var tid = symbolTable.IndexOf(rr.Key);
					if (0 > tid) throw new ArgumentException(string.Concat("Terminal \"", rr.Key, "\" not present in the symbol table"), "symbolTable");
					tid -= Count;
					result[ntid][tid] = new int[rr.Value.Rule.Right.Count];
					var i = 0;
					foreach (var sym in rr.Value.Rule.Right)
					{
						var sid = symbolTable.IndexOf(sym);
						if (0 > sid) throw new ArgumentException(string.Concat("Symbol \"", sym, "\" not present in the symbol table"), "symbolTable");
						result[ntid][tid][i] = sid;
						++i;
					}
				}
			}
			return result;
		}
	}
}
