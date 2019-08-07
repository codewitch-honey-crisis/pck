using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{

	public static class LL1ParserCodeGenerator
	{
		
		static CodeTypeDeclaration _CreateParserClass(CfgDocument cfg,string name)
		{
			var sm = new Dictionary<string, int>();
			var ii = 0;
			var syms = new List<string>();

			cfg.FillSymbols(syms);
		

			foreach (var sym in syms)
			{
				sm.Add(sym, ii);
				++ii;
			}
			var pt = cfg.ToLL1ParseTable();
			var ipt = pt.ToArray(syms);
			var nodeFlags = new int[syms.Count];
			for (var i = 0; i < nodeFlags.Length; ++i)
			{
				var o = cfg.GetAttribute(syms[i], "hidden", false);
				if (o is bool && (bool)o)
					nodeFlags[i] |= 2;
				o = cfg.GetAttribute(syms[i], "collapsed", false);
				if (o is bool && (bool)o)
					nodeFlags[i] |= 1;
			}
			var attrSets = new KeyValuePair<string, object>[syms.Count][];
			for (ii = 0; ii < attrSets.Length; ii++)
			{
				CfgAttributeList attrs;
				if (cfg.AttributeSets.TryGetValue(syms[ii], out attrs))
				{
					attrSets[ii] = new KeyValuePair<string, object>[attrs.Count];
					var j = 0;
					foreach (var attr in attrs)
					{
						attrSets[ii][j] = new KeyValuePair<string, object>(attr.Name, attr.Value);
						++j;
					}
				}
				else
					attrSets[ii] = null;
			}
			

			var result = new CodeTypeDeclaration();
			result.Name = name;
			result.Attributes = MemberAttributes.FamilyOrAssembly;
			result.BaseTypes.Add(typeof(LL1TableParser));
			CodeMemberField f;
			foreach (var s in syms)
			{
				if (null != s)
				{
					f = new CodeMemberField();
					f.Attributes = MemberAttributes.Const | MemberAttributes.Public;
					f.Name = s.Replace("#", "_").Replace("'", "_").Replace("<", "_").Replace(">", "_");
					f.Type = new CodeTypeReference(typeof(int));
					f.InitExpression = CodeDomUtility.Serialize(cfg.GetIdOfSymbol(s));
					result.Members.Add(f);
				}
			}
			f = new CodeMemberField();
			f.Attributes = MemberAttributes.Static;
			f.Name = "_Symbols";
			f.Type = new CodeTypeReference(typeof(string[]));
			f.InitExpression = CodeDomUtility.Serialize(syms.ToArray());
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Attributes = MemberAttributes.Static;
			f.Name = "_ParseTable";
			f.Type = new CodeTypeReference(typeof(int[][][]));
			f.InitExpression = CodeDomUtility.Serialize(ipt);
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Attributes = MemberAttributes.Static;
			f.Name = "_InitCfg";
			f.Type = new CodeTypeReference(typeof(int[]));
			f.InitExpression = CodeDomUtility.Serialize(new int[] { cfg.GetIdOfSymbol(cfg.StartSymbol), cfg.FillNonTerminals().Count });
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Attributes = MemberAttributes.Static;
			f.Name = "_NodeFlags";
			f.Type = new CodeTypeReference(typeof(int[]));
			f.InitExpression = CodeDomUtility.Serialize(nodeFlags);
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Attributes = MemberAttributes.Static;
			f.Name = "_AttributeSets";
			f.Type = new CodeTypeReference(attrSets.GetType());
			f.InitExpression = CodeDomUtility.Serialize(attrSets);
			result.Members.Add(f);

			var ctor = new CodeConstructor();
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IEnumerable<Token>), "tokenizer"));
			ctor.BaseConstructorArgs.AddRange(new CodeExpression[] {
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_ParseTable"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_InitCfg"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_Symbols"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_NodeFlags"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_AttributeSets"),
				new CodeArgumentReferenceExpression("tokenizer")
			});
			ctor.Attributes = MemberAttributes.Public;
			result.Members.Add(ctor);
			ctor = new CodeConstructor();
			ctor.ChainedConstructorArgs.AddRange(new CodeExpression[] {new CodePrimitiveExpression(null)
			});
			ctor.Attributes = MemberAttributes.Public;
			result.Members.Add(ctor);
			return result;
		}
		
		public static void WriteClassTo(CfgDocument cfg, string name, string @namespace,string language, TextWriter writer)
		{
			if (string.IsNullOrEmpty(language))
				language = "cs";
			var cdp = CodeDomProvider.CreateProvider(language);
			var parser = _CreateParserClass(cfg, name);
			var opts = new CodeGeneratorOptions();
			opts.BlankLinesBetweenMembers = false;
			if (string.IsNullOrEmpty(@namespace))
				cdp.GenerateCodeFromType(parser, writer, opts);
			else
			{
				var cns = new CodeNamespace(@namespace);
				cns.Types.Add(parser);
				cdp.GenerateCodeFromNamespace(cns, writer, opts);
			}
			//cdp.GenerateCodeFromType(parser, writer, opts);
		}
		
	}
}
