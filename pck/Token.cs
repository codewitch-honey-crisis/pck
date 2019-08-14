using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	/// <summary>
	/// Represents a single token produced from a lexer/tokenizer, and consumed by a parser
	/// A token contains the symbol, the value, and the location information for each lexeme returned from a lexer/tokenizer
	/// </summary>
	public struct Token
	{
		public string Symbol { get; set; }
		public int SymbolId { get; set; }
		public int Line { get; set; }
		public int Column { get; set; }
		public long Position { get; set; }
		public int Length { get; set; }
		public string Value { get; set; }

		public override string ToString()
		{
			return string.Concat(Symbol, "(", string.Concat(SymbolId.ToString(), ") : ", Value));
			return string.Concat(Symbol, ": ", Value);
		}
	}
}
