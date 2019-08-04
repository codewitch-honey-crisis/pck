using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
namespace Pck
{
	static partial class Program
	{
		#region Usage
		static void _PrintUsageFagen()
		{
			Console.Error.Write(string.Concat(_name, " "));
			Console.Error.WriteLine("fagen [<specfile> [<outputfile>]] [/class <classname>] [/namespace <namespace>] [/language <language>]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  <specfile>\tThe pck specification file to use (or stdin)");
			Console.Error.WriteLine("  <outputfile>\tThe file to write (or stdout)");
			Console.Error.WriteLine("  <classname>\tThe name of the class to generate (or taken from the filename or from the start symbol of the grammar)");
			Console.Error.WriteLine("  <namespace>\tThe namespace to generate the code under (or none)");
			Console.Error.WriteLine("  <language>\tThe .NET language to generate the code for (or draw from filename or C#)");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  Generates an FA tokenizer/lexer in the specified .NET language.");
			Console.Error.WriteLine();
			Console.Error.WriteLine();
		}
		static void _PrintUsageLl1gen()
		{
			Console.Error.Write(string.Concat(_name, " "));
			Console.Error.WriteLine("ll1gen [<specfile> [<outputfile>]] [/class <classname>] [/namespace <namespace>] [/language <language>]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  <specfile>\tThe pck specification file to use (or stdin)");
			Console.Error.WriteLine("  <outputfile>\tThe file to write (or stdout)");
			Console.Error.WriteLine("  <classname>\tThe name of the class to generate (or taken from the filename or from the start symbol of the grammar)");
			Console.Error.WriteLine("  <namespace>\tThe namespace to generate the code under (or none)");
			Console.Error.WriteLine("  <language>\tThe .NET language to generate the code for (or draw from filename or C#)");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  Generates an LL(1) parser in the specified .NET language.");
			Console.Error.WriteLine();
			Console.Error.WriteLine();
		}
		static void _PrintUsageLl1()
		{
			Console.Error.Write(string.Concat(_name, " "));
			Console.Error.WriteLine("ll1 [<specfile> [<outputfile>]]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  <specfile>\tThe pck specification file to use (or stdin)");
			Console.Error.WriteLine("  <outputfile>\tThe file to write (or stdout)");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  Factors a pck grammar spec so that it can be used with an LL(1) parser.");
			Console.Error.WriteLine();
			Console.Error.WriteLine();
		}
		static void _PrintUsageTree()
		{
			Console.Error.Write(string.Concat(_name, " "));
			Console.Error.WriteLine("ll1tree <grammarfile> <inputfile>");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  <grammarfile>\tThe grammar file to use");
			Console.Error.WriteLine("  <inputfile>\tThe file to parse");
			Console.Error.WriteLine();
		}
		static void _PrintUsageXlt()
		{
			Console.Error.Write(string.Concat(_name, " "));
			Console.Error.WriteLine("xlt [<inputfile> [<outputfile>]] [/transform <transform>] [/assembly <assembly>]");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  <inputfile>\tThe input file to use (or stdin)");
			Console.Error.WriteLine("  <outputfile>\tThe file to write (or stdout)");
			Console.Error.WriteLine("  <transform>\tThe name of the transform to use (or taken from the input and/or output filenames)");
			Console.Error.WriteLine("  <assembly>\tThe assembly to reference");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  Translates an input format to an output format.");
			Console.Error.WriteLine();
			Console.Error.WriteLine("  Available transforms include: ");
			Console.Error.WriteLine();
			foreach(var xfrm in _byName)
				Console.Error.WriteLine(
					string.Concat(
						"   ", 
						xfrm.Key, 
						string.Concat(
							"\t",
							xfrm.Value.Key.Description)));
			
			Console.Error.WriteLine();
		}
		
		static void _PrintUsage()
		{
			Console.Error.WriteLine(string.Concat("Usage: ",_name," <command> [<arguments>]"));
			Console.Error.WriteLine();
			Console.Error.WriteLine("Commands:");
			Console.Error.WriteLine();
			_PrintUsageFagen();
			_PrintUsageLl1gen();
			_PrintUsageLl1();
			_PrintUsageTree();
			_PrintUsageXlt();
			
			Console.Error.WriteLine();
		}
		#endregion

		static int Main(string[] args)
		{
			_PopulateTransforms();
			if(0==args.Length)
			{
				_PrintUsage();
				return 1;
			} else if(1==args.Length && "/?"==args[0]||"--help"==args[0]||"-?"==args[0])
			{
				_PrintUsage();
				return 0;
			}
			var sargs = new string[args.Length - 1];
			for (var i = 0; i < sargs.Length; i++)
				sargs[i] = args[i + 1];
			switch (args[0])
			{
				case "fagen":
					return _Fagen(sargs);
				case "ll1gen":
					return _Ll1gen(sargs);
				case "ll1":
					return _Ll1(sargs);
				case "xlt":
					return _Xlt(sargs);
				case "ll1tree":
					return _LL1Tree(sargs);
				default:
					_PrintUsage();
					return 1;

			}
		}
		static Assembly _LoadAssembly(string asm)
		{
			Assembly result = null;
			try
			{
				result = Assembly.Load(asm);
			}
			catch { };
			if (null == result)
			{
				try
				{
					result = Assembly.LoadFile(asm);
				}
				catch { }
			}
			return result;
		}
		static void _PopulateTransforms()
		{
			var asms = AppDomain.CurrentDomain.GetAssemblies();
			for (var i = 0; i < asms.Length; i++)
			{
				var types = asms[i].GetTypes();
				for (var j = 0; j < types.Length; j++)
				{
					var type = types[j];
					foreach (var cattr in type.GetCustomAttributes<TransformAttribute>())
					{
						var meth = type.GetMethod("Transform", new Type[] { typeof(TextReader), typeof(TextWriter) });
						if (null != meth && meth.IsStatic)
						{
							var entry = new KeyValuePair<TransformAttribute, MethodInfo>(cattr, meth);
							_byName[entry.Key.Name] = entry;
							if (!string.IsNullOrEmpty(entry.Key.FromExtension) && !string.IsNullOrEmpty(entry.Key.ToExtension))
								_byExts[new KeyValuePair<string, string>(entry.Key.FromExtension.ToLowerInvariant(), entry.Key.ToExtension.ToLowerInvariant())] = entry;
							break;
						}
					}

				}
			}
		}
		// rips the lex info out of the document so it can
		// paste it back later.
		// technically, we no longer need this since we have access
		// to the CfgDocument from here, but we'll keep it so that
		// we can pull it out without the dependency
		static IList<string> _RipSymbols(LexDocument l, string inp)
		{
			var result = new List<string>();
			var terms = new List<string>();
			var terms2 = new List<string>();
			var sr = new StringReader(inp);
			string line;
			while (null != (line = sr.ReadLine()))
			{
				line = line.Trim();
				var i = line.IndexOf("//");
				var j = line.IndexOf('\'');
				if (-1 < i && (0 > j || j > i)) // remove the comment
					line = line.Substring(0, i);
				var sbid = new StringBuilder();
				for (i = 0; i < line.Length; i++)
				{
					var ch = line[i];
					if (_NotIdentifierChars.Contains(ch))
						break;
					sbid.Append(ch);
				}
				while (i < line.Length && char.IsWhiteSpace(line[i]))
					++i;
				if (i < line.Length && '-' == line[i])
				{
					if (!result.Contains(sbid.ToString()))
						result.Add(sbid.ToString());
				}
				else if (i < line.Length && '=' == line[i])
				{
					var o = l.GetAttribute(sbid.ToString(), "hidden", null);
					if (o is bool && (bool)o)
					{
						if (!terms.Contains(sbid.ToString()) && !terms2.Contains(sbid.ToString()))
							terms2.Add(sbid.ToString());
					}
					else if (!terms.Contains(sbid.ToString()) && !terms2.Contains(sbid.ToString()))
						terms.Add(sbid.ToString());
				}
			}
			result.AddRange(terms);
			result.AddRange(terms2);
			result.Add("#EOS");
			result.Add("#ERROR");
			return result;
		}
		// rips the lex info out of the document so it can
		// paste it back later.
		// technically we don't need this but we're keeping it
		// see the other rip routine
		static string _RipLex(string inp)
		{
			var sr = new StringReader(inp);
			string line;
			var sb = new StringBuilder();
			while (null != (line = sr.ReadLine()))
			{
				line = line.Trim();
				var i = line.IndexOf("//");
				var j = line.IndexOf('\'');
				if (-1 < i && (0 > j || j > i)) // remove the comment
					line = line.Substring(0, i);
				var sbid = new StringBuilder();
				for (i = 0; i < line.Length; i++)
				{
					var ch = line[i];
					if (_NotIdentifierChars.Contains(ch))
						break;
					sbid.Append(ch);
				}
				while (i < line.Length && char.IsWhiteSpace(line[i]))
					++i;
				if (i == line.Length || '=' == line[i])
				{
					sb.Append(sbid.ToString());
					sb.AppendLine(line.Substring(i));
				}
			}
			return sb.ToString();
		}
		const string _NotIdentifierChars = "()[]{}<>,:;-=|/\'\" \t\r\n\f\v";
		static readonly IDictionary<string, KeyValuePair<TransformAttribute, MethodInfo>> _byName = new Dictionary<string, KeyValuePair<TransformAttribute, MethodInfo>>(StringComparer.InvariantCultureIgnoreCase);
		static readonly IDictionary<KeyValuePair<string, string>, KeyValuePair<TransformAttribute, MethodInfo>> _byExts = new Dictionary<KeyValuePair<string, string>, KeyValuePair<TransformAttribute, MethodInfo>>();
		static readonly string _name= Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().GetModules()[0].Name);
	}
}
