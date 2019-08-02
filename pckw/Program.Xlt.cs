using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Pck
{
	partial class Program
	{
		static int _Xlt(string[] args)
		{
			string inpFile = null;
			string outFile = null;
			string transform = null;
			string assembly = null;
			var optIndex = -1;
			for (var i = 0; i < args.Length; ++i)
			{
				if ("--help" == args[i] || "/?" == args[i] || "/help" == args[i])
				{
					Console.Error.Write("Usage: ");
					_PrintUsageXlt();
					return 0;
				}
				if (args[i].StartsWith("/"))
				{
					optIndex = i;
					if (i == args.Length - 1)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageXlt();
						return 1;
					}
					switch (args[i])
					{
						case "/transform":
							++i;
							transform = args[i];
							break;
						case "/assembly":
							++i;
							assembly = args[i];
							break;
						default:
							Console.Error.Write("Usage: ");
							_PrintUsageXlt();
							return 1;
					}
				}
				else
				{
					if (-1 != optIndex)
					{
						Console.Error.Write("Usage: ");
						_PrintUsageXlt();
						return 1;
					}
					if (0 == i)
						inpFile = args[i];
					else if (1 == i)
						outFile = args[i];
					else
					{
						Console.Error.Write("Usage: ");
						_PrintUsageXlt();
						return 1;
					}
				}
			}
			TextReader inp = null;
			TextWriter outp = null;
			try
			{
				// first, if assembly is specified, load it
				if (!string.IsNullOrEmpty(assembly))
				{
					var asm = _LoadAssembly(assembly);
					if (null == asm)
						Console.Error.WriteLine("Warning: Could not load assembly: {0}", assembly);
				}

				// just loading it was enough
				_PopulateTransforms();
				if (null == inpFile)
					inp = Console.In;
				else
					inp = new StreamReader(inpFile);
				if (null == outFile)
					outp = Console.Out;
				else
					outp = new StreamWriter(outFile);
				MethodInfo meth = null;
				if (!string.IsNullOrEmpty(transform))
				{
					KeyValuePair<TransformAttribute, MethodInfo> ti;
					if (!_byName.TryGetValue(transform, out ti))
					{
						Console.Error.WriteLine("Error: Transform not found: {0} (did you forget an assembly reference?)", transform);
						return 1;
					}
					meth = ti.Value;
				}
				if (null == meth)
				{
					if (!string.IsNullOrEmpty(inpFile))
					{
						if (!string.IsNullOrEmpty(outFile))
						{
							var fx = Path.GetExtension(inpFile);
							var tx = Path.GetExtension(outFile);

							KeyValuePair<TransformAttribute, MethodInfo> ti;
							if (_byExts.TryGetValue(new KeyValuePair<string, string>(fx, tx), out ti))
								meth = ti.Value;
						}

					}
				}
				if (null == meth)
					Console.Error.WriteLine("Error: Transform not found (did you forget an assembly reference?)");
				else
					meth.Invoke(null, new object[] { inp, outp });
				Console.Error.WriteLine("Translation complete.");
				return 0;
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("Error: {0}", ex.Message);
				return 1;
			}
			finally
			{
				if (null != inp)
					inp.Close();
				if (null != outp)
					outp.Close();
			}
		}
	}
}
