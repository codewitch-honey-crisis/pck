﻿using Pck;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	using CharDfaEntry = KeyValuePair<int, KeyValuePair<string, int>[]>;

	/// <summary>
	/// Generates a tokenizer class from a LexDocument and symbol table
	/// </summary>
	static class TokenizerCodeGenerator
	{
		
		static CodeTypeDeclaration _CreateTokenizerClass(LexDocument lex, IList<string> symbolTable, string name,IProgress<FAProgress> progress=null)
		{
			var lexer = lex.ToLexer(progress);
			var ii = 0;
			var syms = new List<string>(symbolTable);
			var bes = new string[syms.Count];
			for (ii = 0; ii < bes.Length; ii++)
				bes[ii] = lex.GetAttribute(syms[ii], "blockEnd", null) as string;
			var dfaTable = lexer.ToArray(syms);
			var result = new CodeTypeDeclaration();
			var tt = new List<string>();
			for (int ic = lex.Rules.Count, i = 0; i < ic; ++i)
			{
				var t = lex.Rules[i].Left;
				if (!tt.Contains(t))
					tt.Add(t);
			}
			tt.Add("#EOS");
			tt.Add("#ERROR");
			for (int ic = syms.Count, i = 0; i < ic; ++i)
			{
				if (!tt.Contains(syms[i]))
					syms[i] = null;
			}
			result.Name = name;
			result.BaseTypes.Add(typeof(TableTokenizer));
			result.Attributes = MemberAttributes.FamilyOrAssembly;
			CodeMemberField f;
			foreach (var t in tt)
			{
				if (null != t)
				{
					f = new CodeMemberField();
					f.Attributes = MemberAttributes.Const | MemberAttributes.Public;
					f.Name = t.Replace("#", "_").Replace("'", "_").Replace("<", "_").Replace(">", "_");
					f.Type = new CodeTypeReference(typeof(int));
					f.InitExpression = CodeDomUtility.Serialize(syms.IndexOf(t));
					result.Members.Add(f);
				}
			}

			f = new CodeMemberField();
			f.Name = "_Symbols";
			f.Type = new CodeTypeReference(typeof(string[]));
			f.Attributes = MemberAttributes.Static;
			var arr = new string[syms.Count];
			syms.CopyTo(arr, 0);
			f.InitExpression = CodeDomUtility.Serialize(arr);
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Name = "_BlockEnds";
			f.Type = new CodeTypeReference(typeof(string[]));
			f.Attributes = MemberAttributes.Static;
			f.InitExpression = CodeDomUtility.Serialize(bes);
			result.Members.Add(f);

			f = new CodeMemberField();
			f.Name = "_DfaTable";
			f.Type = new CodeTypeReference(typeof(CharDfaEntry[]));
			f.Attributes = MemberAttributes.Static;
			f.InitExpression = CodeDomUtility.Serialize(dfaTable);
			result.Members.Add(f);

			var ctor = new CodeConstructor();
			ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IEnumerable<char>), "input"));
			ctor.BaseConstructorArgs.AddRange(new CodeExpression[] {
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_DfaTable"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_Symbols"),
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(result.Name), "_BlockEnds"),
				new CodeArgumentReferenceExpression("input")
			});
			ctor.Attributes = MemberAttributes.Public;
			result.Members.Add(ctor);
			return result;
		}
		
		public static void WriteClassTo(LexDocument lex, IList<string> symbolTable, string name,string @namespace, string language, TextWriter writer,IProgress<FAProgress> progress=null)
		{
			if (string.IsNullOrEmpty(language))
				language = "cs";
			var cdp = CodeDomProvider.CreateProvider(language);
			var tokenizer = _CreateTokenizerClass(lex, symbolTable, name,progress);
			var opts = new CodeGeneratorOptions();
			opts.BlankLinesBetweenMembers = false;
			if (string.IsNullOrEmpty(@namespace))
				cdp.GenerateCodeFromType(tokenizer, writer, opts);
			else
			{
				var cns = new CodeNamespace(@namespace);
				cns.Types.Add(tokenizer);
				cdp.GenerateCodeFromNamespace(cns, writer, opts);
			}
		}
	}
}
