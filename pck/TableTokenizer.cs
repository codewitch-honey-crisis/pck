using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	using CharDfaEntry = KeyValuePair<int, KeyValuePair<char[], int>[]>;
	public class TableTokenizer : ITokenizer
	{
		CharDfaEntry[] _dfaTable;
		int _eosSymbol;
		int _errorSymbol;
		IEnumerable<char> _input;
		string[] _symbols;
		string[] _blockEnds;
		
		public TableTokenizer(CharDfaEntry[] dfaTable,string[] symbols, string[] blockEnds,IEnumerable<char> input)
		{
			_dfaTable = dfaTable;
			_symbols = symbols;
			_blockEnds = blockEnds;
			_input = input;
			_eosSymbol = -1;
			_errorSymbol = -1;
			for(var i=symbols.Length-1;i>=0;i--)
			{
				if ("#ERROR" == symbols[i])
					_errorSymbol = i;
				else if ("#EOS" == symbols[i])
					_eosSymbol = i;
				if (-1 != _errorSymbol && -1 != _eosSymbol)
					break;
			}
			if (-1 == _errorSymbol || -1 == _eosSymbol)
				throw new ArgumentException("Error in symbol table", "symbols");


		}
		public IEnumerator<Token> GetEnumerator()
		{
			return new _TokenEnumerator(_dfaTable,_errorSymbol,_eosSymbol, _symbols,_blockEnds, _input);
		}

		public void Restart(IEnumerable<char> input)
		{
			_input = input;
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

		sealed class _TokenEnumerator : IEnumerator<Token>
		{
			string[] _blockEnds;
			string[] _symbols;
			// our underlying input enumerator - works on strings or char arrays
			IEnumerator<char> _input;
			// location information
			long _position;
			int _line;
			int _column;
			int _errorSymbol;
			int _eosSymbol;
			// an integer we use so we can tell if the enumeration is started or running, or past the end.
			int _state;
			// this holds the current token we're on.
			Token _token;
			
			// the DFA Table is a composite "regular expression" with tagged symbols for each one.
			CharDfaEntry[] _dfaTable;
			// this holds our current value
			StringBuilder _buffer;
			internal _TokenEnumerator(CharDfaEntry[] dfaTable, int errorSymbol,int eosSymbol, string[] symbols, string[] blockEnds, IEnumerable<char> @string)
			{
				_dfaTable = dfaTable;
				_errorSymbol = errorSymbol;
				_eosSymbol = eosSymbol;
				_symbols = symbols;
				_blockEnds = blockEnds;
				if(null!=@string)
					_input = @string.GetEnumerator();
				_buffer = new StringBuilder();
				_state = -1;
				_line = 1;
				_column = 1;
				_position = 0;
			}
			public Token Current { get { return _token; } }
			object IEnumerator.Current => Current;

			public void Dispose()
			{
				_state = -3;
				if (null != _input)
				{
					_input.Dispose();
					_input = null;
				}
			}
			public bool MoveNext()
			{
				switch (_state)
				{
					case -3:
						throw new ObjectDisposedException(GetType().FullName);
					case -2:
						if (_token.SymbolId != _eosSymbol)
						{
							_state = -2;
							goto case 0;
						}
						return false;
					case -1:
					case 0:
						_token = new Token();
						// store our current location before we advance
						_token.Column = _column;
						_token.Line = _line;
						_token.Position = _position;
						// this is where the real work happens:
						_token.SymbolId = _Lex();
						_token.Symbol = _symbols[_token.SymbolId];
						// store our value and length from the lex
						_token.Value = _buffer.ToString();
						_token.Length = _buffer.Length;
						return true;
					default:
						return false;
				}

			}
			/// <summary>
			/// This is where the work happens
			/// </summary>
			/// <returns>The symbol that was matched. members _state _line,_column,_position,_buffer and _input are also modified.</returns>
			int _Lex()
			{
				int acc;
				var states = 0;
				_buffer.Clear();
				switch (_state)
				{
					case -1: // initial
						if (!_MoveNextInput())
						{
							_state = -2;
							acc = _dfaTable[states].Key;
							if (-1 != acc)
								return acc;
							else
								return _errorSymbol;
						}
						_state = 0; // running
						break;
					case -2: // end of stream
						return _eosSymbol;
				}
				// Here's where we run most of the match. we run one interation of the DFA state machine.
				// We match until we can't match anymore (greedy matching) and then report the symbol of the last 
				// match we found, or an error ("#ERROR") if we couldn't find one.
				while (true)
				{
					var next = -1;
					// go through all the transitions
					for(var i =0;i<_dfaTable[states].Value.Length;i++)
					{
						var entry = _dfaTable[states].Value[i];
						var found = false;
						// go through all the ranges to see if we matched anything.
						for (var j=0;j<entry.Key.Length;j++)
						{
							var ch = _input.Current;
							var first = entry.Key[j];
							++j;
							var last = entry.Key[j];
							if (ch > last) continue;
							if (first > ch) break;
							found = true;
							break;
							
						}
						if(found)
						{
							// set the transition destination
							next = entry.Value;
							break;
						}
					}
					
					if (-1 == next) // couldn't find any states
						break;
					_buffer.Append(_input.Current);

					states = next;
					if (!_MoveNextInput())
					{
						// end of stream
						_state = -2;
						acc = _dfaTable[states].Key;
						if (-1 != acc) // do we accept?
							return acc;
						else
							return _errorSymbol;
					}
				}
				acc = _dfaTable[states].Key;
				if (-1 != acc) // do we accept?
				{
					var be=_blockEnds[acc];
					if (!string.IsNullOrEmpty(be))
					{
						// we have to resolve our blockends. This is tricky. We break out of the FA 
						// processing and instead we loop until we match the block end. We have to 
						// be very careful when we match only partial block ends and we have to 
						// handle the case where there's no terminating block end.
						var more = true;
						while (more)
						{
							while (more)
							{
								if (_input.Current != be[0])
								{
									_buffer.Append(_input.Current);
									more = _MoveNextInput();
									if (!more)
										return _errorSymbol;
									break;
								}
								else
								{
									var i = 0;
									var found = true;
									while (i < be.Length && _input.Current == be[i])
									{
										if (!(more = _MoveNextInput()))
										{
											++i;
											found = false;
											if (i < be.Length)
												acc = _errorSymbol;
											break;
										}
										++i;

									}
									if (be.Length != i)
										found = false;
									if (!found)
									{
										_buffer.Append(be.Substring(0, i));
									}
									else
									{
										more = false;
										_buffer.Append(be);
										break;
									}
									if (found)
									{
										more = _MoveNextInput();
										if (!more)
											break;
									}
								}

							}
						}
					}
					return acc;
				}
				else
				{
					// handle the error condition
					_buffer.Append(_input.Current);
					if (!_MoveNextInput())
						_state = -2;
					return _errorSymbol;
				}
			}
			/// <summary>
			/// Advances the input, and tracks location information
			/// </summary>
			/// <returns>True if the underlying MoveNext returned true, otherwise false.</returns>
			bool _MoveNextInput()
			{
				if (_input.MoveNext())
				{
					if (-1 != _state)
					{
						++_position;
						if ('\n' == _input.Current)
						{
							_column = 1;
							++_line;
						}
						else
							++_column;
					}
					return true;
				}
				else if (0 == _state)
				{
					++_position;
					++_column;
				}
				_state = -2;
				return false;
			}
			
			public void Reset()
			{
				_input.Reset();
				_state = -1;
				_line = 1;
				_column = 1;
				_position = 0;
				_token = default(Token);
			}
		}
	}
}
