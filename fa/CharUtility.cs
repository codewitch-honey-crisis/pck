using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Pck
{
	static class CharUtility
	{
		static readonly ICollection<char> _whiteSpace;
		static readonly ICollection<char> _control;
		static readonly ICollection<char> _digit;
		static readonly ICollection<char> _letter;
		static readonly ICollection<char> _lower;
		static readonly ICollection<char> _upper;
		static readonly ICollection<char> _number;
		static readonly ICollection<char> _punctuation;
		static readonly ICollection<char> _separator;
		static readonly ICollection<char> _symbol;
		public static ICollection<char> WhiteSpace => _whiteSpace;
		public static ICollection<char> Control => _control;
		public static ICollection<char> Digit=> _digit;
		public static ICollection<char> Letter => _letter;
		public static ICollection<char> Lower => _lower;
		public static ICollection<char> Upper => _upper;
		public static ICollection<char> Number => _number;
		public static ICollection<char> Punctuation => _punctuation;
		public static ICollection<char> Separator => _separator;
		public static ICollection<char> Symbol => _symbol;
		class _ROCharCollection : ICollection<char>
		{
			
			ICollection<char> _inner;
			public _ROCharCollection(ICollection<char> inner)
			{
				_inner = inner;
			}
			static void _ThrowReadOnly() { throw new NotSupportedException("The collection is read only."); }
			public int Count => _inner.Count;

			public bool IsReadOnly => true;

			public void Add(char item)
			{
				_ThrowReadOnly();
			}

			public void Clear()
			{
				_ThrowReadOnly();
			}

			public bool Contains(char item)
			{
				return _inner.Contains(item);
			}

			public void CopyTo(char[] array, int arrayIndex)
			{
				_inner.CopyTo(array, arrayIndex);
			}

			public IEnumerator<char> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			public bool Remove(char item)
			{
				_ThrowReadOnly();
				return false;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
		}
		static CharUtility()
		{
			var whiteSpace = new HashSet<char>();
			var control = new HashSet<char>();
			var digit = new HashSet<char>();
			var letter = new HashSet<char>();
			var lower = new HashSet<char>();
			var upper = new HashSet<char>();
			var number = new HashSet<char>();
			var punctuation = new HashSet<char>();
			var separator= new HashSet<char>();
			var symbol= new HashSet<char>();
			for (int i = char.MinValue; i <= char.MaxValue; ++i)
			{
				var ch = unchecked((char)i);
				if (char.IsWhiteSpace(ch))
					whiteSpace.Add(ch);
				if (char.IsControl(ch))
					control.Add(ch);
				if (char.IsDigit(ch))
					digit.Add(ch);
				if (char.IsLetter(ch))
					letter.Add(ch);
				if (char.IsLower(ch))
					lower.Add(ch);
				if (char.IsUpper(ch))
					upper.Add(ch);
				if (char.IsNumber(ch))
					number.Add(ch);
				if (char.IsPunctuation(ch))
					punctuation.Add(ch);
				if (char.IsSeparator(ch))
					separator.Add(ch);
				if (char.IsSymbol(ch))
					symbol.Add(ch);
			}
			_whiteSpace = new _ROCharCollection(whiteSpace);
			_control = new _ROCharCollection(control);
			_digit = new _ROCharCollection(digit);
			_letter = new _ROCharCollection(letter);
			_lower = new _ROCharCollection(lower);
			_upper = new _ROCharCollection(upper);
			_number = new _ROCharCollection(number);
			_punctuation = new _ROCharCollection(punctuation);
			_separator = new _ROCharCollection(separator);
			_symbol = new _ROCharCollection(symbol);

		}
		
	}
}
