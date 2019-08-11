using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	[Transform("cgtToPck",".cgt",".pck","Translates a Gold Parser cgt file into a pck spec. (requires manual intervention)")]
	class CgtToPckTransform
	{
		public static void Transform(Stream input,Stream output)
		{
			var str = _ReadString(input);
			if ("GOLD Parser Tables/v1.0" != str)
				throw new InvalidOperationException("The stream is not in Gold Parser CGT format.");
			bool caseSensitive = true;
			int startId = -3;
			int eosId = -1;
			int wsId = -3;
			int blockStartId = -3;
			int blockEndId = -3;
			string[] symbols = null;
			string[] characterSets = null;
			IDictionary<int, int> symbolMap = null;
			(bool Accepting, int AcceptSymbolId, (int CharacterSetIndex, int TargetStateIndex)[])[] dfaStates = null;
			(int Left, int[] Right)[] rules = null;
			(int SymbolId, int Action, int Target)[][] lalrStates = null;
			while (true)
			{
				var rec = _ReadRecord(input);
				if (null == rec)
					break;
				switch ((byte)rec[0])
				{
					case 84:
						symbols = new string[(short)rec[1]];
						characterSets = new string[(short)rec[2]];
						rules = new (int Left, int[] Right)[(short)rec[3]];
						dfaStates = new (bool Accepting, int AcceptSymbolId, (int CharacterSetIndex, int TargetStateIndex)[])[(short)rec[4]];
						lalrStates = new (int SymbolId, int Action, int Target)[(short)rec[5]][];
						symbolMap = new Dictionary<int, int>(symbols.Length);
						break;
					case 83:
						switch ((short)rec[3])
						{
							case 5:
								blockEndId = (short)rec[1];
								break;
							case 4:
								blockStartId = (short)rec[1];
								break;
							case 3:
								eosId = (short)rec[1];
								break;
							case 2:
								wsId = (short)rec[1];
								break;

						}
						symbols[(short)rec[1]] = (string)rec[2];
						
						break;
					case 82:
						var left = (int)(short)rec[2];
						var right = new int[rec.Length - 4];
						for (var i = 4; i < rec.Length; ++i)
							right[i - 4] = (short)rec[i];
						rules[(short)rec[1]] = (left, right);
						break;
					case 80:
						caseSensitive = (bool)rec[5];
						startId = (short)rec[6];
						break;
					case 76:
						var c = (rec.Length - 3) / 4;
						var arr = new (int SymbolId, int Action, int Target)[c];
						var j = 0;
						for (var i = 3; i < rec.Length; i += 4)
						{
							arr[j] = ((short)rec[i], (short)rec[i + 1], (short)rec[i + 2]);
							++j;
						}
						lalrStates[(short)rec[1]] = arr;
						break;
					case 68:
						var dc = (rec.Length - 5) / 3;
						var darr = new (int CharacterSetIndex, int TargetStateIndex)[dc];
						j = 0;
						for (var i = 5; i < rec.Length; i += 3)
						{
							darr[j] = ((short)rec[i], (short)rec[i + 1]);
							++j;
						}
						dfaStates[(short)rec[1]] = ((bool)rec[2], (short)rec[3], darr);
						break;
					case 67:
						characterSets[(short)rec[1]] = (string)rec[2];
						break;
				}
			}
			// fixup the symbols.
			symbols[eosId] = "#EOS";
			symbols[1] = "#ERROR";
			var impl = 2;
			var nts = new HashSet<int>();
			for (var i = 0; i < rules.Length; i++)
				nts.Add(rules[i].Left);
			for(var i = 0;i<symbols.Length;i++)
			{
				if(nts.Contains(i))
					symbols[i] = _MakeSafeIdentifier(symbols[i]);
				else if("#ERROR"!=symbols[i] && "#EOS"!=symbols[i])
				{
					var isTotallySafe = true;
					for(int j=0;j<symbols[i].Length;j++)
					{
						if(_NotIdentifierChars.Contains(symbols[i][j]))
						{
							isTotallySafe = false;
							break;
						}
					}
					if (!isTotallySafe)
					{
						symbols[i] = string.Concat("implicit", impl.ToString());
						++impl;
					}
				}
			}
			
			var mappedRules=new (string Left, string[] Right)[rules.Length];
			var cfg = new CfgDocument();
			for (var i = 0; i < rules.Length; i++)
			{
				var r = rules[i];
				var rule = new CfgRule(symbols[r.Left]);
				for (var j = 0; j < r.Right.Length; j++)
					rule.Right.Add(symbols[r.Right[j]]);
				cfg.Rules.Add(rule);
			}

			// now build the lexer info
			var closure = new CharFA<string>[dfaStates.Length];
			for (var i = 0; i < dfaStates.Length; i++)
				closure[i] = new CharFA<string>();
			for (var i = 0; i < dfaStates.Length; i++)
			{
				var fa = closure[i];
				var dfa = dfaStates[i];
				if (dfa.Accepting)
				{
					fa.AcceptSymbol = symbols[dfa.AcceptSymbolId];
					fa.IsAccepting = true;
				}
				for (var j = 0; j < dfa.Item3.Length; ++j)
				{
					var trn = dfa.Item3[j];
					for (int k = 0; k < characterSets[trn.CharacterSetIndex].Length; ++k)
						fa.Transitions.Add(characterSets[trn.CharacterSetIndex][k], closure[trn.TargetStateIndex]);
				}
			}
			var lexer = closure[0];
			// now crack apart the individual terminals from the FSM
			var acc = new List<string>();
			var accs = lexer.FillAccepting();
			foreach (var afa in accs)
				if (!acc.Contains(afa.AcceptSymbol))
					acc.Add(afa.AcceptSymbol);
			var grps = new Dictionary<string, ICollection<CharFA<string>>>();
			foreach (var ac in acc)
			{
				foreach(var acs in accs)
				{
					if(Equals(acs.AcceptSymbol,ac))
					{
						ICollection<CharFA<string>> col;
						if(!grps.TryGetValue(ac, out col))
						{
							col = new List<CharFA<string>>();
							grps.Add(ac, col);
						}
						if (!col.Contains(acs))
							col.Add(acs as CharFA<string>);
					}
				}
			}
			var lex = new LexDocument();
			foreach(var grp in grps)
			{
				var fa = lexer.ClonePathToAny(grp.Value);
				foreach(var cfa in fa.FillClosure())
					if(cfa.IsAccepting && cfa.AcceptSymbol!=grp.Key)
							cfa.IsAccepting = false;
						
				try
				{
					lex.Rules.Add(new LexRule(grp.Key, fa.ToString()));
				}
				catch
				{
					lex.Rules.Add(new LexRule(grp.Key, ""));
				}
			}
			var sw = new StreamWriter(output);
			sw.WriteLine(cfg.ToString());
			sw.WriteLine("// TODO: Finish porting the lex output");
			sw.Write(lex.ToString());
			sw.Flush();
		}
		static string _MakeSafeIdentifier(string id)
		{
			if (string.IsNullOrEmpty(id))
				return id;
			var sb = new StringBuilder();
			for (var i = 0; i < id.Length; i++)
				if (_NotIdentifierChars.Contains(id[i]))
					sb.Append('_');
				else
					sb.Append(id[i]);

			return sb.ToString();
		}
		const string _NotIdentifierChars = "()[]{}<>,:;-=|/\'\" \t\r\n\f\v";
		static string _ReadString(Stream stream)
		{
			var bytes = new List<byte>(64);
			while (true)
			{
				var a = (byte)stream.ReadByte();
				var b = (byte)stream.ReadByte();
				if (a == 0 && b == 0)
					break;
				bytes.Add(a);
				bytes.Add(b);
			}
			return Encoding.Unicode.GetString(bytes.ToArray());
		}
		static object _ReadEntry(Stream stream)
		{
			var i = stream.ReadByte();
			switch (i)
			{
				case 83:
					return _ReadString(stream);
				case 69:
					return null;
				case 66:
					i = stream.ReadByte();
					return !(0 == i);
				case 98:
					return (byte)stream.ReadByte();
				case 73:
					i = stream.ReadByte();
					i += 256 * stream.ReadByte();
					return unchecked((short)i);
			}
			throw new IOException("Invalid data found in stream.");
		}
		static object[] _ReadRecord(Stream stream)
		{
			int val = stream.ReadByte();
			if (-1 == val) return null;
			if (77 != val) throw new IOException("Invalid data found in stream.");
			val = stream.ReadByte();
			val += 256 * stream.ReadByte();
			var ic = val;
			var result = new object[ic];
			for (var i = 0; i < ic; ++i)
				result[i] = _ReadEntry(stream);

			return result;
		}
	}
}
