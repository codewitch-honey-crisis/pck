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
		
		static IList<CfgMessage> _CreateParserClass(CfgDocument cfg,string name,IProgress<CfgLL1Progress> progress,out CodeTypeDeclaration parserClass)
		{
			var result = new List<CfgMessage>();
			var sm = new Dictionary<string, int>();
			var ii = 0;
			var syms = new List<string>();

			cfg.FillSymbols(syms);
		

			foreach (var sym in syms)
			{
				sm.Add(sym, ii);
				++ii;
			}
			CfgLL1ParseTable pt;
			result.AddRange(cfg.TryToLL1ParseTable(progress,out pt));
			var hasErrors = false;
			foreach (var msg in result)
			{
				if (ErrorLevel.Error == msg.ErrorLevel)
				{
					hasErrors = true;
					break;
				}
			}
			if (!hasErrors)
			{
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
				var substitutions = new int[syms.Count];
				for (var i = 0; i < substitutions.Length; i++)
				{
					var s = cfg.GetAttribute(syms[i], "substitute", null) as string;
					if (!string.IsNullOrEmpty(s) && cfg.IsSymbol(s) && s != syms[i])
						substitutions[i] = cfg.GetIdOfSymbol(s);
					else
						substitutions[i] = -1;
				}
				var attrSets = new ParseAttribute[syms.Count][];
				for (ii = 0; ii < attrSets.Length; ii++)
				{
					CfgAttributeList attrs;
					if (cfg.AttributeSets.TryGetValue(syms[ii], out attrs))
					{
						attrSets[ii] = new ParseAttribute[attrs.Count];
						var j = 0;
						foreach (var attr in attrs)
						{
							attrSets[ii][j] = new ParseAttribute(attr.Name, attr.Value);
							++j;
						}
					}
					else
						attrSets[ii] = null;
				}


				parserClass = new CodeTypeDeclaration();
				parserClass.Name = name;
				parserClass.Attributes = MemberAttributes.FamilyOrAssembly;
				parserClass.BaseTypes.Add(typeof(LL1TableParser));
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
						parserClass.Members.Add(f);
					}
				}
				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_Symbols";
				f.Type = new CodeTypeReference(typeof(string[]));
				f.InitExpression = CodeDomUtility.Serialize(syms.ToArray());
				parserClass.Members.Add(f);

				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_ParseTable";
				f.Type = new CodeTypeReference(typeof(int[][][]));
				f.InitExpression = CodeDomUtility.Serialize(ipt);
				parserClass.Members.Add(f);

				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_InitCfg";
				f.Type = new CodeTypeReference(typeof(int[]));
				f.InitExpression = CodeDomUtility.Serialize(new int[] { cfg.GetIdOfSymbol(cfg.StartSymbol), cfg.FillNonTerminals().Count });
				parserClass.Members.Add(f);

				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_NodeFlags";
				f.Type = new CodeTypeReference(typeof(int[]));
				f.InitExpression = CodeDomUtility.Serialize(nodeFlags);
				parserClass.Members.Add(f);

				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_Substitutions";
				f.Type = new CodeTypeReference(typeof(int[]));
				f.InitExpression = CodeDomUtility.Serialize(substitutions);
				parserClass.Members.Add(f);

				f = new CodeMemberField();
				f.Attributes = MemberAttributes.Static;
				f.Name = "_AttributeSets";
				f.Type = new CodeTypeReference(attrSets.GetType());
				f.InitExpression = CodeDomUtility.Serialize(attrSets);
				parserClass.Members.Add(f);

				var ctor = new CodeConstructor();
				ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ITokenizer), "tokenizer"));
				ctor.BaseConstructorArgs.AddRange(new CodeExpression[] {
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_ParseTable"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_InitCfg"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_Symbols"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_NodeFlags"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_Substitutions"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(parserClass.Name), "_AttributeSets"),
				new CodeArgumentReferenceExpression("tokenizer")
			});
				ctor.Attributes = MemberAttributes.Public;
				parserClass.Members.Add(ctor);
				ctor = new CodeConstructor();
				ctor.ChainedConstructorArgs.AddRange(new CodeExpression[] {new CodePrimitiveExpression(null)
			});
				ctor.Attributes = MemberAttributes.Public;
				parserClass.Members.Add(ctor);
			}
			else
				parserClass = null;
			return result;
		}
		public static void WriteClassTo(CfgDocument cfg, string name, string @namespace, string language, TextWriter writer)
			=> WriteClassTo(cfg, name, @namespace, language, null, writer);
		public static void WriteClassTo(CfgDocument cfg, string name, string @namespace, string language, IProgress<CfgLL1Progress> progress, TextWriter writer)
		{
			var msgs = TryWriteClassTo(cfg, name, @namespace, language, progress, writer);
			CfgException.ThrowIfErrors(msgs);
		}
		public static IList<CfgMessage> TryWriteClassTo(CfgDocument cfg, string name, string @namespace, string language, TextWriter writer)
			=> TryWriteClassTo(cfg, name, @namespace, language, null, writer);
		public static IList<CfgMessage> TryWriteClassTo(CfgDocument cfg, string name, string @namespace,string language, IProgress<CfgLL1Progress> progress,TextWriter writer)
		{
			var result = new List<CfgMessage>();
			if (string.IsNullOrEmpty(language))
				language = "cs";
			var cdp = CodeDomProvider.CreateProvider(language);
			CodeTypeDeclaration parser;
			result.AddRange(_CreateParserClass(cfg, name, progress, out parser));
			if (null != parser)
			{
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
			}
			return result;
		}
	}
}
