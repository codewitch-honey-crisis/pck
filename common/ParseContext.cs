// a class that helps with writing hand-rolled parsers, which we use for regex and ebnf parsing
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace Pck
{

	#region ExpectingException
	/// <summary>
	/// An exception encountered during parsing where the stream contains one thing, but another is expected
	/// </summary>
	[Serializable]
	sealed class ExpectingException : Exception
	{
		/// <summary>
		/// Initialize the exception with the specified message.
		/// </summary>
		/// <param name="message">The message</param>
		public ExpectingException(string message) : base(message) { }
		/// <summary>
		/// The list of expected strings.
		/// </summary>
		public string[] Expecting { get; internal set; }
		/// <summary>
		/// The position when the error was realized.
		/// </summary>
		public long Position { get; internal set; }
		/// <summary>
		/// The line of the error
		/// </summary>
		public int Line { get; internal set; }
		/// <summary>
		/// The column of the error
		/// </summary>
		public int Column { get; internal set; }

	}
	#endregion ExpectingException
	/// <summary>
	/// My old parsecontext class, used by the CharFA to parse a regex. Use this or
	/// it's replacement here: https://www.codeproject.com/Articles/5162847/ParseContext-2-0-Easier-Hand-Rolled-Parsers
	/// </summary>
	abstract partial class ParseContext : IEnumerator<char>, IDisposable
	{
		public bool TryReadWhiteSpace()
		{
			EnsureStarted();
			if (-1 == Current || !char.IsWhiteSpace((char)Current))
				return false;
			CaptureCurrent();
			while (-1 != Advance() && char.IsWhiteSpace((char)Current))
				CaptureCurrent();
			return true;
		}
		public bool TrySkipWhiteSpace()
		{
			EnsureStarted();
			if (-1 == Current || !char.IsWhiteSpace((char)Current))
				return false;
			while (-1 != Advance() && char.IsWhiteSpace((char)Current)) ;
			return true;
		}
		public bool TryReadUntil(int character, bool readCharacter = true)
		{
			EnsureStarted();
			if (0 > character) character = -1;
			CaptureCurrent();
			if (Current == character)
			{
				return true;
			}
			while (-1 != Advance() && Current != character)
				CaptureCurrent();
			CaptureCurrent();
			if (Current == character)
			{
				if (readCharacter)
					Advance();
				return true;
			}
			return false;
		}
		public bool TrySkipUntil(int character, bool skipCharacter = true)
		{
			EnsureStarted();
			if (0 > character) character = -1;
			if (Current == character)
				return true;
			while (-1 != Advance() && Current != character) ;
			if (Current == character)
			{
				if (skipCharacter)
					Advance();
				return true;
			}
			return false;
		}
		public bool TryReadUntil(int character, int escapeChar, bool readCharacter = true)
		{
			EnsureStarted();
			if (0 > character) character = -1;
			if (-1 == Current) return false;
			if (Current == character)
			{
				if (readCharacter)
				{
					CaptureCurrent();
					Advance();
				}
				return true;
			}

			do
			{
				if (escapeChar == Current)
				{
					CaptureCurrent();
					if (-1 == Advance())
						return false;
					CaptureCurrent();
				}
				else
				{
					if (character == Current)
					{
						if (readCharacter)
						{
							CaptureCurrent();
							Advance();
						}
						return true;
					}
					else
						CaptureCurrent();
				}
			}
			while (-1 != Advance());

			return false;
		}
		public bool TrySkipUntil(int character, int escapeChar, bool skipCharacter = true)
		{
			EnsureStarted();
			if (0 > character) character = -1;
			if (Current == character)
				return true;
			while (-1 != Advance() && Current != character)
			{
				if (character == escapeChar)
					if (-1 == Advance())
						break;
			}
			if (Current == character)
			{
				if (skipCharacter)
					Advance();
				return true;
			}
			return false;
		}
		private static bool _ContainsChar(char[] chars, char ch)
		{
			foreach (char cmp in chars)
				if (cmp == ch)
					return true;
			return false;
		}

		public bool TryReadUntil(bool readCharacter = true, params char[] anyOf)
		{
			EnsureStarted();
			if (null == anyOf)
				anyOf = Array.Empty<char>();
			CaptureCurrent();
			if (-1 != Current && _ContainsChar(anyOf, (char)Current))
			{
				if (readCharacter)
				{
					CaptureCurrent();
					Advance();
				}
				return true;
			}
			while (-1 != Advance() && !_ContainsChar(anyOf, (char)Current))
				CaptureCurrent();
			if (-1 != Current && _ContainsChar(anyOf, (char)Current))
			{
				if (readCharacter)
				{
					CaptureCurrent();
					Advance();
				}
				return true;
			}
			return false;
		}
		public bool TrySkipUntil(bool skipCharacter = true, params char[] anyOf)
		{
			EnsureStarted();
			if (null == anyOf)
				anyOf = Array.Empty<char>();
			if (-1 != Current && _ContainsChar(anyOf, (char)Current))
			{
				if (skipCharacter)
					Advance();
				return true;
			}
			while (-1 != Advance() && !_ContainsChar(anyOf, (char)Current)) ;
			if (-1 != Current && _ContainsChar(anyOf, (char)Current))
			{
				if (skipCharacter)
					Advance();
				return true;
			}
			return false;
		}
		public bool TryReadUntil(string text)
		{
			EnsureStarted();
			if (string.IsNullOrEmpty(text))
				return false;
			while (-1 != Current && TryReadUntil(text[0], false))
			{
				bool found = true;
				for (int i = 1; i < text.Length; ++i)
				{
					if (Advance() != text[i])
					{
						found = false;
						break;
					}
					CaptureCurrent();
				}
				if (found)
				{
					Advance();
					return true;
				}
			}

			return false;
		}
		public bool TrySkipUntil(string text)
		{
			EnsureStarted();
			if (string.IsNullOrEmpty(text))
				return false;
			while (-1 != Current && TrySkipUntil(text[0], false))
			{
				bool found = true;
				for (int i = 1; i < text.Length; ++i)
				{
					if (Advance() != text[i])
					{
						found = false;
						break;
					}
				}
				if (found)
				{
					Advance();
					return true;
				}
			}
			return false;
		}


		public bool TryReadDigits()
		{
			EnsureStarted();
			if (-1 == Current || !char.IsDigit((char)Current))
				return false;
			CaptureCurrent();
			while (-1 != Advance() && char.IsDigit((char)Current))
				CaptureCurrent();
			return true;
		}
		public bool TrySkipDigits()
		{
			EnsureStarted();
			if (-1 == Current || !char.IsDigit((char)Current))
				return false;
			while (-1 != Advance() && char.IsDigit((char)Current)) ;
			return true;
		}

		public bool TryReadJsonString()
		{
			EnsureStarted();
			if ('\"' != Current)
				return false;
			CaptureCurrent();
			while (-1 != Advance() && '\r' != Current && '\n' != Current && Current != '\"')
			{
				CaptureCurrent();
				if ('\\' == Current)
				{
					if (-1 == Advance() || '\r' == Current || '\n' == Current)
						return false;
					CaptureCurrent();

				}
			}
			if (Current == '\"')
			{
				CaptureCurrent();
				Advance(); // move past the string
				return true;
			}
			return false;
		}
		static bool _IsHexChar(char hex)
		{
			return (
				(':' > hex && '/' < hex) ||
				('G' > hex && '@' < hex) ||
				('g' > hex && '`' < hex)
			);
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
		public bool TrySkipJsonString()
		{
			EnsureStarted();
			if ('\"' != Current)
				return false;
			while (-1 != Advance() && '\r' != Current && '\n' != Current && Current != '\"')
				if ('\\' == Current)
					if (-1 == Advance() || '\r' == Current || '\n' == Current)
						return false;

			if (Current == '\"')
			{
				Advance(); // move past the string
				return true;
			}
			return false;
		}
		public bool TryParseJsonString(out string result)
		{
			result = null;
			var sb = new StringBuilder();
			EnsureStarted();
			if ('\"' != Current)
				return false;
			CaptureCurrent();
			while (-1 != Advance() && '\r' != Current && '\n' != Current && Current != '\"')
			{
				CaptureCurrent();
				if ('\\' == Current)
				{
					if (-1 == Advance() || '\r' == Current || '\n' == Current)
						return false;
					CaptureCurrent();
					switch (Current)
					{
						case 'f':
							sb.Append('\f');
							break;
						case 'r':
							sb.Append('\r');
							break;
						case 'n':
							sb.Append('\n');
							break;
						case 't':
							sb.Append('\t');
							break;
						case '\\':
							sb.Append('\\');
							break;
						case '/':
							sb.Append('/');
							break;
						case '\"':
							sb.Append('\"');
							break;
						case 'b':
							sb.Append('\b');
							break;
						case 'u':
							CaptureCurrent();
							if (-1 == Advance())
								return false;
							int ch = 0;
							if (!_IsHexChar((char)Current))
								return false;
							ch <<= 4;
							ch |= _FromHexChar((char)Current);

							CaptureCurrent();
							if (-1 == Advance())
								return false;
							if (!_IsHexChar((char)Current))
								return false;
							ch <<= 4;
							ch |= _FromHexChar((char)Current);

							CaptureCurrent();
							if (-1 == Advance())
								return false;
							if (!_IsHexChar((char)Current))
								return false;
							ch <<= 4;
							ch |= _FromHexChar((char)Current);

							CaptureCurrent();
							if (-1 == Advance())
								return false;
							if (!_IsHexChar((char)Current))
								return false;
							ch <<= 4;
							ch |= _FromHexChar((char)Current);

							sb.Append((char)ch);
							break;
						default:
							return false;
					}
				}
				else
					sb.Append((char)Current);

			}
			if (Current == '\"')
			{
				CaptureCurrent();
				Advance(); // move past the string
				result = sb.ToString();
				return true;
			}
			return false;
		}
		public string ParseJsonString()
		{
			var sb = new StringBuilder();
			EnsureStarted();
			Expecting('\"');
			while (-1 != Advance() && '\r' != Current && '\n' != Current && Current != '\"')
			{
				if ('\\' == Current)
				{
					Advance();
					switch (Current)
					{
						case 'b':
							sb.Append('\b');
							break;
						case 'f':
							sb.Append('\f');
							break;
						case 'v':
							sb.Append('\v');
							break;
						case 'n':
							sb.Append('\n');
							break;
						case 'r':
							sb.Append('\r');
							break;
						case 't':
							sb.Append('\t');
							break;
						case '\\':
							sb.Append('\\');
							break;
						case '\"':
							sb.Append('\"');
							break;
						case 'u':
							int ch = 0;
							Advance();
							Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f');
							ch <<= 4;
							ch |= _FromHexChar((char)Current);
							Advance();
							Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f');
							ch <<= 4;
							ch |= _FromHexChar((char)Current);
							Advance();
							Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f');
							ch <<= 4;
							ch |= _FromHexChar((char)Current);
							Advance();
							Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'a', 'B', 'b', 'C', 'c', 'D', 'd', 'E', 'e', 'F', 'f');
							ch <<= 4;
							ch |= _FromHexChar((char)Current);
							sb.Append((char)ch);
							break;
						default:
							Expecting('b', 'n', 'r', 't', '\\', '/', '\"', 'u');
							break;
					}
				}
				else
					sb.Append((char)Current);
			}
			Expecting('\"');
			Advance();
			return sb.ToString();
		}
		public bool TryReadJsonValue()
		{
			TryReadWhiteSpace();
			if ('t' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'r')
					return false;
				CaptureCurrent();
				if (Advance() != 'u')
					return false;
				CaptureCurrent();
				if (Advance() != 'e')
					return false;
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('f' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'a')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (Advance() != 's')
					return false;
				CaptureCurrent();
				if (Advance() != 'e')
					return false;
				CaptureCurrent();
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('n' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'u')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('-' == Current || '.' == Current || char.IsDigit((char)Current))
				return TryReadReal();
			if ('\"' == Current)
				return TryReadJsonString();

			if ('[' == Current)
			{
				CaptureCurrent();
				Advance();
				if (TryReadJsonValue())
				{
					TryReadWhiteSpace();
					while (',' == Current)
					{
						CaptureCurrent();
						Advance();
						if (!TryReadJsonValue()) return false;
						TryReadWhiteSpace();
					}
				}
				TryReadWhiteSpace();
				if (']' != Current)
					return false;
				CaptureCurrent();
				Advance();
				return true;
			}
			if ('{' == Current)
			{
				CaptureCurrent();
				Advance();
				TryReadWhiteSpace();
				if (TryReadJsonString())
				{
					TryReadWhiteSpace();
					if (':' != Current) return false;
					CaptureCurrent();
					Advance();
					if (!TryReadJsonValue())
						return false;

					TryReadWhiteSpace();
					while (',' == Current)
					{
						CaptureCurrent();
						Advance();
						TryReadWhiteSpace();
						if (!TryReadJsonString())
							return false;
						TryReadWhiteSpace();
						if (':' != Current) return false;
						CaptureCurrent();
						Advance();
						if (!TryReadJsonValue())
							return false;
						TryReadWhiteSpace();
					}
				}
				TryReadWhiteSpace();
				if ('}' != Current)
					return false;
				CaptureCurrent();
				Advance();
				return true;
			}
			return false;
		}
		public bool TrySkipJsonValue()
		{
			TrySkipWhiteSpace();
			if ('t' == Current)
			{
				if (Advance() != 'r')
					return false;
				if (Advance() != 'u')
					return false;
				if (Advance() != 'e')
					return false;
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('f' == Current)
			{
				if (Advance() != 'a')
					return false;
				if (Advance() != 'l')
					return false;
				if (Advance() != 's')
					return false;
				if (Advance() != 'e')
					return false;
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('n' == Current)
			{
				if (Advance() != 'u')
					return false;
				if (Advance() != 'l')
					return false;
				if (Advance() != 'l')
					return false;
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('-' == Current || '.' == Current || char.IsDigit((char)Current))
				return TrySkipReal();
			if ('\"' == Current)
				return TrySkipJsonString();

			if ('[' == Current)
			{
				Advance();
				if (TrySkipJsonValue())
				{
					TrySkipWhiteSpace();
					while (',' == Current)
					{
						Advance();
						if (!TrySkipJsonValue()) return false;
						TrySkipWhiteSpace();
					}
				}
				TrySkipWhiteSpace();
				if (']' != Current)
					return false;
				Advance();
				return true;
			}
			if ('{' == Current)
			{
				Advance();
				TrySkipWhiteSpace();
				if (TrySkipJsonString())
				{
					TrySkipWhiteSpace();
					if (':' != Current) return false;
					Advance();
					if (!TrySkipJsonValue())
						return false;
					TrySkipWhiteSpace();
					while (',' == Current)
					{
						Advance();
						TrySkipWhiteSpace();
						if (!TrySkipJsonString())
							return false;
						TrySkipWhiteSpace();
						if (':' != Current) return false;
						Advance();
						if (!TrySkipJsonValue())
							return false;
						TrySkipWhiteSpace();
					}
				}
				TrySkipWhiteSpace();
				if ('}' != Current)
					return false;
				Advance();
				return true;
			}
			return false;
		}
		public bool TryParseJsonValue(out object result)
		{
			result = null;
			TryReadWhiteSpace();
			if ('t' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'r')
					return false;
				CaptureCurrent();
				if (Advance() != 'u')
					return false;
				CaptureCurrent();
				if (Advance() != 'e')
					return false;
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				result = true;
				return true;
			}
			if ('f' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'a')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (Advance() != 's')
					return false;
				CaptureCurrent();
				if (Advance() != 'e')
					return false;
				CaptureCurrent();
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				result = false;
				return true;
			}
			if ('n' == Current)
			{
				CaptureCurrent();
				if (Advance() != 'u')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (Advance() != 'l')
					return false;
				CaptureCurrent();
				if (-1 != Advance() && char.IsLetterOrDigit((char)Current))
					return false;
				return true;
			}
			if ('-' == Current || '.' == Current || char.IsDigit((char)Current))
			{
				double r;
				if (TryParseReal(out r))
				{
					result = r;
					return true;
				}
				return false;
			}
			if ('\"' == Current)
			{
				string s;
				if (TryParseJsonString(out s))
				{
					result = s;
					return true;
				}
				return false;
			}
			if ('[' == Current)
			{
				CaptureCurrent();
				Advance();
				var l = new List<object>();
				object v;
				if (TryParseJsonValue(out v))
				{
					l.Add(v);
					TryReadWhiteSpace();
					while (',' == Current)
					{
						CaptureCurrent();
						Advance();
						if (!TryParseJsonValue(out v)) return false;
						l.Add(v);
						TryReadWhiteSpace();
					}
				}
				TryReadWhiteSpace();
				if (']' != Current)
					return false;
				CaptureCurrent();
				Advance();
				result = l;
				return true;
			}
			if ('{' == Current)
			{
				CaptureCurrent();
				Advance();
				TryReadWhiteSpace();
				string n;
				object v;
				var d = new Dictionary<string, object>();
				if (TryParseJsonString(out n))
				{
					TryReadWhiteSpace();
					if (':' != Current) return false;
					CaptureCurrent();
					Advance();
					if (!TryParseJsonValue(out v))
						return false;
					d.Add(n, v);
					TryReadWhiteSpace();
					while (',' == Current)
					{
						CaptureCurrent();
						Advance();
						TryReadWhiteSpace();
						if (!TryParseJsonString(out n))
							return false;
						TryReadWhiteSpace();
						if (':' != Current) return false;
						CaptureCurrent();
						Advance();
						if (!TryParseJsonValue(out v))
							return false;
						d.Add(n, v);
						TryReadWhiteSpace();
					}
				}
				TryReadWhiteSpace();
				if ('}' != Current)
					return false;
				CaptureCurrent();
				Advance();
				result = d;
				return true;
			}
			return false;
		}

		public object ParseJsonValue()
		{
			TrySkipWhiteSpace();
			if ('t' == Current)
			{
				Advance(); Expecting('r');
				Advance(); Expecting('u');
				Advance(); Expecting('e');
				Advance();
				return true;
			}
			if ('f' == Current)
			{
				Advance(); Expecting('a');
				Advance(); Expecting('l');
				Advance(); Expecting('s');
				Advance(); Expecting('e');
				Advance();
				return true;
			}
			if ('n' == Current)
			{
				Advance(); Expecting('u');
				Advance(); Expecting('l');
				Advance(); Expecting('l');
				Advance();
				return null;
			}
			if ('-' == Current || '.' == Current || char.IsDigit((char)Current))
				return ParseReal();
			if ('\"' == Current)
				return ParseJsonString();
			if ('[' == Current)
			{
				Advance();
				TrySkipWhiteSpace();
				var l = new List<object>();
				if (']' != Current)
				{
					l.Add(ParseJsonValue());
					TrySkipWhiteSpace();
					while (',' == Current)
					{
						Advance();
						l.Add(ParseJsonValue());
						TrySkipWhiteSpace();
					}
				}
				TrySkipWhiteSpace();
				Expecting(']');
				Advance();
				return l;
			}
			if ('{' == Current)
			{
				Advance();
				TrySkipWhiteSpace();
				var d = new Dictionary<string, object>();
				if ('}' != Current)
				{

					string n = ParseJsonString();
					TrySkipWhiteSpace();
					Expecting(':');
					Advance();
					object v = ParseJsonValue();
					d.Add(n, v);
					TrySkipWhiteSpace();
					while (',' == Current)
					{
						Advance();
						TrySkipWhiteSpace();
						n = ParseJsonString();
						TrySkipWhiteSpace();
						Expecting(':');
						Advance();
						v = ParseJsonValue();
						d.Add(n, v);
						TrySkipWhiteSpace();
					}
				}
				TrySkipWhiteSpace();
				if ('}' != Current)
					return false;
				Advance();
				return d;
			}
			return false;
		}


		public bool TryReadInteger()
		{
			EnsureStarted();
			bool neg = false;
			if ('-' == Current)
			{
				neg = true;
				CaptureCurrent();
				Advance();
			}
			else if ('0' == Current)
			{
				CaptureCurrent();
				Advance();
				if (-1 == Current) return true;
				return !char.IsDigit((char)Current);
			}
			if (-1 == Current || (neg && '0' == Current) || !char.IsDigit((char)Current))
				return false;
			if (!TryReadDigits())
				return false;
			return true;
		}

		public bool TrySkipInteger()
		{
			EnsureStarted();
			bool neg = false;
			if ('-' == Current)
			{
				neg = true;
				Advance();
			}
			else if ('0' == Current)
			{
				Advance();
				if (-1 == Current) return true;
				return !char.IsDigit((char)Current);
			}
			if (-1 == Current || (neg && '0' == Current) || !char.IsDigit((char)Current))
				return false;
			if (!TrySkipDigits())
				return false;
			return true;
		}
		// must be object because we don't know the int type. To be lexically valid we must use BigInteger when necessary
		public bool TryParseInteger(out object result)
		{
			result = null;
			EnsureStarted();
			if (-1 == Current) return false;
			bool neg = false;
			if ('-' == Current)
			{
				CaptureCurrent();
				Advance();
				neg = true;
			}
			int l = CaptureBuffer.Length;
			if (TryReadDigits())
			{
				string num = CaptureBuffer.ToString(l, CaptureBuffer.Length - l);
				if (neg)
					num = '-' + num;
				int r;
				if (int.TryParse(num, out r))
				{
					result = r;
					return true;
				}
				long ll;
				if (long.TryParse(num, out ll))
				{
					result = ll;
					return true;
				}

			}
			return false;
		}
		public object ParseInteger()
		{
			EnsureStarted();
			Expecting('-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
			bool neg = ('-' == Current);
			if (neg)
			{
				Advance();
				Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
			}
			long i = 0;
			if (!neg)
			{
				i += ((char)Current) - '0';
				while (-1 != Advance() && char.IsDigit((char)Current))
				{
					i *= 10;
					i += ((char)Current) - '0';
				}

			}
			else
			{
				i -= ((char)Current) - '0';
				while (-1 != Advance() && char.IsDigit((char)Current))
				{
					i *= 10;
					i -= ((char)Current) - '0';
				}
			}
			if (i <= int.MaxValue && i >= int.MinValue)
				return (int)i;
			else if (i <= long.MaxValue && i >= long.MinValue)
				return i;
			return i;
		}
		public bool TryReadReal()
		{
			EnsureStarted();
			bool readAny = false;
			if ('-' == Current)
			{
				CaptureCurrent();
				Advance();
			}
			if (char.IsDigit((char)Current))
			{
				if (!TryReadDigits())
					return false;
				readAny = true;
			}
			if ('.' == Current)
			{
				CaptureCurrent();
				Advance();
				if (!TryReadDigits())
					return false;
				readAny = true;
			}
			if ('E' == Current || 'e' == Current)
			{
				CaptureCurrent();
				Advance();
				if ('-' == Current || '+' == Current)
				{
					CaptureCurrent();
					Advance();
				}
				return TryReadDigits();
			}

			return readAny;
		}
		public bool TrySkipReal()
		{
			bool readAny = false;
			EnsureStarted();
			if ('-' == Current)
				Advance();
			if (char.IsDigit((char)Current))
			{
				if (!TrySkipDigits())
					return false;
				readAny = true;
			}
			if ('.' == Current)
			{
				Advance();
				if (!TrySkipDigits())
					return false;
				readAny = true;
			}
			if ('E' == Current || 'e' == Current)
			{
				Advance();
				if ('-' == Current || '+' == Current)
					Advance();
				return TrySkipDigits();
			}

			return readAny;
		}
		public bool TryParseReal(out double result)
		{
			result = default(double);
			int l = CaptureBuffer.Length;
			if (!TryReadReal())
				return false;
			return double.TryParse(CaptureBuffer.ToString(l, CaptureBuffer.Length - l), out result);
		}

		public double ParseReal()
		{
			EnsureStarted();
			var sb = new StringBuilder();
			Expecting('-', '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
			bool neg = ('-' == Current);
			if (neg)
			{
				sb.Append((char)Current);
				Advance();
				Expecting('.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
			}
			while (-1 != Current && char.IsDigit((char)Current))
			{
				sb.Append((char)Current);
				Advance();
			}
			if ('.' == Current)
			{
				sb.Append((char)Current);
				Advance();
				Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
				sb.Append((char)Current);
				while (-1 != Advance() && char.IsDigit((char)Current))
				{
					sb.Append((char)Current);
				}
			}
			if ('E' == Current || 'e' == Current)
			{
				sb.Append((char)Current);
				Advance();
				Expecting('+', '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
				switch (Current)
				{
					case '+':
					case '-':
						sb.Append((char)Current);
						Advance();
						break;
				}
				Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
				sb.Append((char)Current);
				while (-1 != Advance())
				{
					Expecting('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
					sb.Append((char)Current);
				}
			}
			return double.Parse(sb.ToString());

		}
		public bool TryReadCLineComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			CaptureCurrent();
			if ('/' != Advance())
				return false;
			CaptureCurrent();
			while (-1 != Advance() && '\r' != Current && '\n' != Current)
				CaptureCurrent();
			return true;
		}
		public bool TrySkipCLineComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			if ('/' != Advance())
				return false;
			while (-1 != Advance() && '\r' != Current && '\n' != Current) ;
			return true;
		}
		public bool TryReadCBlockComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			CaptureCurrent();
			if ('*' != Advance())
				return false;
			CaptureCurrent();
			if (-1 == Advance())
				return false;
			return TryReadUntil("*/");
		}
		public bool TrySkipCBlockComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			if ('*' != Advance())
				return false;
			if (-1 == Advance())
				return false;
			return TrySkipUntil("*/");
		}
		public bool TryReadCComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			CaptureCurrent();
			if ('*' == Advance())
			{
				CaptureCurrent();
				if (-1 == Advance())
					return false;
				return TryReadUntil("*/");
			}
			if ('/' == Current)
			{
				CaptureCurrent();
				while (-1 != Advance() && '\r' != Current && '\n' != Current)
					CaptureCurrent();
				return true;
			}
			return false;
		}
		public bool TrySkipCComment()
		{
			EnsureStarted();
			if ('/' != Current)
				return false;
			if ('*' == Advance())
			{
				if (-1 == Advance())
					return false;
				return TrySkipUntil("*/");
			}
			if ('/' == Current)
			{
				while (-1 != Advance() && '\r' != Current && '\n' != Current) ;
				return true;
			}
			return false;
		}
		public bool TryReadCCommentsAndWhitespace()
		{
			bool result = false;
			while (-1 != Current)
			{
				if (!TryReadWhiteSpace() && !TryReadCComment())
					break;
				result = true;
			}
			if (TryReadWhiteSpace())
				result = true;
			return result;
		}
		public bool TrySkipCCommentsAndWhiteSpace()
		{
			bool result = false;
			while (-1 != Current)
			{
				if (!TrySkipWhiteSpace() && !TrySkipCComment())
					break;
				result = true;
			}
			if (TrySkipWhiteSpace())
				result = true;
			return result;
		}
		string _GetExpectingMessageRanges(int[] expecting)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			for (var i = 0; i < expecting.Length; i++)
			{
				var first = expecting[i];
				++i;
				var last = expecting[i];
				if (first == last)
				{
					if (-1 == first)
						sb.Append("(end of stream)");
					else
						sb.Append((char)first);
				}
				else
				{
					sb.Append((char)first);
					sb.Append('-');
					sb.Append((char)last);
				}
			}
			sb.Append(']');
			string at = string.Concat(" at line ", Line, ", column ", Column, ", position ", Position);
			if (-1 == Current)
			{
				if (0 == expecting.Length)
					return string.Concat("Unexpected end of input", at, ".");
				return string.Concat("Unexpected end of input. Expecting ", sb.ToString(), at, ".");
			}
			if (0 == expecting.Length)
				return string.Concat("Unexpected character \"", (char)Current, "\" in input", at, ".");
			return string.Concat("Unexpected character \"", (char)Current, "\" in input. Expecting ", sb.ToString(), at, ".");

		}
		string _GetExpectingMessage(int[] expecting)
		{
			StringBuilder sb = null;
			switch (expecting.Length)
			{
				case 0:
					break;
				case 1:
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					break;
				case 2:
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					sb.Append(" or ");
					if (-1 == expecting[1])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[1]);
						sb.Append("\"");
					}
					break;
				default: // length > 2
					sb = new StringBuilder();
					if (-1 == expecting[0])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[0]);
						sb.Append("\"");
					}
					int l = expecting.Length - 1;
					int i = 1;
					for (; i < l; ++i)
					{
						sb.Append(", ");
						if (-1 == expecting[i])
							sb.Append("end of input");
						else
						{
							sb.Append("\"");
							sb.Append((char)expecting[i]);
							sb.Append("\"");
						}
					}
					sb.Append(", or ");
					if (-1 == expecting[i])
						sb.Append("end of input");
					else
					{
						sb.Append("\"");
						sb.Append((char)expecting[i]);
						sb.Append("\"");
					}
					break;
			}
			string at = string.Concat(" at line ", Line, ", column ", Column, ", position ", Position);
			if (-1 == Current)
			{
				if (0 == expecting.Length)
					return string.Concat("Unexpected end of input", at, ".");
				return string.Concat("Unexpected end of input. Expecting ", sb.ToString(), at, ".");
			}
			if (0 == expecting.Length)
				return string.Concat("Unexpected character \"", (char)Current, "\" in input", at, ".");
			return string.Concat("Unexpected character \"", (char)Current, "\" in input. Expecting ", sb.ToString(), at, ".");

		}
		[DebuggerHidden()]
		public void Expecting(params int[] expecting)
		{
			ExpectingException ex = null;
			switch (expecting.Length)
			{
				case 0:
					if (-1 == Current)
						ex = new ExpectingException(_GetExpectingMessage(expecting));
					break;
				case 1:
					if (expecting[0] != Current)
						ex = new ExpectingException(_GetExpectingMessage(expecting));
					break;
				default:
					if (0 > Array.IndexOf<int>(expecting, Current))
						ex = new ExpectingException(_GetExpectingMessage(expecting));
					break;
			}
			if (null != ex)
			{
				ex.Position = Position;
				ex.Line = Line;
				ex.Column = Column;
				ex.Expecting = new string[expecting.Length];
				for (int i = 0; i < ex.Expecting.Length; i++)
					ex.Expecting[i] = Convert.ToString(expecting[i]);
				throw ex;
			}
		}
		[DebuggerHidden()]
		public void ThrowExpectingRanges(int[] expecting)
		{
			ExpectingException ex = null;
			ex = new ExpectingException(_GetExpectingMessageRanges(expecting));
			ex.Position = Position;
			ex.Line = Line;
			ex.Column = Column;
			ex.Expecting = null;
			throw ex;

		}
		
		StringBuilder _captureBuffer;
		/// <summary>
		/// Reports the line the parser is on
		/// </summary>
		/// <remarks>The line starts at one.</remarks>
		public int Line { get; set; }
		/// <summary>
		/// Reports the column the parser is on
		/// </summary>
		/// <remarks>The column starts at one.</remarks>
		public int Column { get; set; }
		/// <summary>
		/// Reports the position the parser is on
		/// </summary>
		/// <remarks>The position starts at zero.</remarks>
		public long Position { get; set; }
		/// <summary>
		/// Reports the current character, or -1 if past end of stream
		/// </summary>
		public int Current { get; protected set; }
		protected ParseContext()
		{
			_captureBuffer = new StringBuilder();
			Position = 0;
			Line = 1;
			Column = 1;
			Current = -2;
		}
		public void EnsureStarted()
		{
			if (-2 == Current)
				Advance();
		}
		public abstract int Advance();
		public abstract void Close();
		void IDisposable.Dispose()
		{
			Close();
		}
		public StringBuilder CaptureBuffer {
			get { return _captureBuffer; }
		}
		public string Capture {
			get { return _captureBuffer.ToString(); }
		}

		char IEnumerator<char>.Current { get { if (0 > Current) throw new InvalidOperationException(); return (char)Current; } }
		object IEnumerator.Current { get { return ((IEnumerator<char>)this).Current; } }

		public string GetCapture(int startIndex, int count)
		{
			return _captureBuffer.ToString(startIndex, count);
		}
		public string GetCapture(int startIndex=0)
		{
			return _captureBuffer.ToString(startIndex, _captureBuffer.Length - startIndex);
		}
		public void CaptureCurrent()
		{
			if (-1 < Current)
			{
				CaptureBuffer.Append((char)Current);
			}
		}
		public void ClearCapture()
		{
			CaptureBuffer.Clear();
		}
		public static ParseContext Create(IEnumerable<char> input)
		{
			return new CharEnumeratorParseContext(input.GetEnumerator());
		}
		public static ParseContext Create(TextReader input)
		{
			return new TextReaderParseContext(input);
		}
		public static ParseContext CreateFromUrl(string url)
		{
			var wreq = WebRequest.Create(url);
			var wresp = wreq.GetResponse();
			return Create(new StreamReader(wresp.GetResponseStream()));
		}
		public static ParseContext CreateFromFile(string filepath)
		{
			return Create(new StreamReader(filepath));
		}
		bool IEnumerator.MoveNext()
		{
			return -1 < Advance();
		}

		void IEnumerator.Reset()
		{
			throw new NotImplementedException();
		}
		#region CharEnumeratorParseContext
		internal partial class CharEnumeratorParseContext : ParseContext
		{
			IEnumerator<char> _enumerator;
			internal CharEnumeratorParseContext(IEnumerator<char> enumerator)
			{
				_enumerator = enumerator;
			}
			public override int Advance()
			{
				if (-2 == Current)
				{
					if (_enumerator.MoveNext())
					{
						Current = _enumerator.Current;
					}
					else
						Current = -1;
					return Current;

				}
				if (_enumerator.MoveNext())
				{
					Current = _enumerator.Current;
					++Position;
					++Column;
					switch (Current)
					{
						case '\r':
							Column = 1;
							break;
						case '\n':
							Column = 1; ++Line;
							break;
					}
				}
				else
				{
					if (-1 != Current)
					{ // last read moves us past the end. subsequent reads don't move anything
						++Position;
						++Column;
					}
					Current = -1;
				}
				return Current;
			}
			public override void Close()
			{
				if (null != _enumerator)
					_enumerator.Dispose();
				_enumerator = null;
			}
		}
		#endregion CharEnumeratorParseContext
		#region TextReaderParseContext
		internal partial class TextReaderParseContext : ParseContext
		{
			TextReader _reader;
			internal TextReaderParseContext(TextReader reader)
			{
				_reader = reader;
			}
			public override int Advance()
			{
				int och = Current;
				if (-2 == och)
				{
					return Current = _reader.Read();
				}
				if (-1 != (Current = _reader.Read()))
				{
					++Position;
					++Column;
					switch (Current)
					{
						case '\r':
							Column = 1;
							break;
						case '\n':
							Column = 1; ++Line;
							break;
					}
				}
				else
				{
					if (-1 != och) // last read moves us past the end. subsequent reads don't move anything
					{
						++Column;
						++Position;
					}
				}
				return Current;
			}
			public override void Close()
			{
				if (null != _reader)
					_reader.Dispose();
				_reader = null;
			}
		}
		#endregion TextReaderParseContext
	}
	
}
