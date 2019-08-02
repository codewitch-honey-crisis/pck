using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	[Transform("pckToYacc",".pck",".y","Translates a pck spec to a yacc spec")]
	static class PckToYaccTransform
	{
		public static void Transform(TextReader input, TextWriter output)
			=> Transform(CfgDocument.ReadFrom(input), output);
		public static void Transform(CfgDocument cfg,TextWriter output)
		{
			var terms = cfg.FillTerminals();
			for (int ic = terms.Count, i = 0; i < ic; ++i)
			{
				var t = terms[i];
				if("#ERROR"!=t&&"#EOS"!=t)
					output.WriteLine(string.Concat("%token ", t));
			}
			output.WriteLine("%%");
			foreach (var nt in cfg.EnumNonTerminals())
			{
				var nt2 = _EscId(nt);
				var rules = cfg.FillNonTerminalRules(nt);
				if (0 < rules.Count) // sanity check
				{
					output.Write(nt2);
					var delim = ":";
					for (int ic = rules.Count, i = 0; i < ic; ++i)
					{
						var rule = rules[i];
						output.Write(delim);
						for (int jc = rule.Right.Count, j = 0; j < jc; ++j)
						{
							output.Write(" ");
							output.Write(_EscId(rule.Right[j]));
						}
						delim = Environment.NewLine + "\t|";
					}
					output.WriteLine(";");
				}
			}
		}
		static string _EscId(string id)
		{
			return id;
		}
	}
}
