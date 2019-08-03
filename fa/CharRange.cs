using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pck
{


	/// <summary>
	/// Represents an ascending range of characters.
	/// </summary>
	public struct CharRange : IEquatable<CharRange>,IList<char>
	{
		public CharRange(char first, char last) { First = (first<=last)?first:last; Last = (first<=last)?last:first; } 
		char IList<char>.this[int index] { get => this[index]; set { _ThrowReadOnly(); } }
		public char this[int index] { get { if (0 > index || Length <= index) throw new IndexOutOfRangeException(); return (char)(First + index); } }
		public int Length { get { return Last - First + 1; } }
		public char First { get; }
		public char Last { get; }

		public static IEnumerable<CharRange> GetRanges(IEnumerable<char> sortedString)
		{
			char first = '\0';
			char last = '\0';
			using (IEnumerator<char> e = sortedString.GetEnumerator())
			{
				bool moved = e.MoveNext();
				while (moved)
				{
					first = last = e.Current;
					while ((moved = e.MoveNext()) && (e.Current == last || e.Current == last + 1))
					{
						last = e.Current;
					}
					yield return new CharRange(first, last);

				}
			}
		}
		public static char[] ToPackedChars(IEnumerable<CharRange> ranges) 
		{
			var rl = new List<CharRange>(ranges);
			NormalizeRangeList(rl);
			var result = new char[rl.Count * 2];
			int j = 0;
			for(int i = 0;i<result.Length;i++)
			{
				result[i] = rl[j].First;
				++i;
				result[i] = rl[j].Last;
				++j;
			}
			return result;
		}
		public static string ToPackedString(IEnumerable<CharRange> ranges)
		{
			var rl = new List<CharRange>(ranges);
			NormalizeRangeList(rl);
			int j = 0;
			var result = new StringBuilder();
			for (int ic= rl.Count * 2,i = 0; i < ic; ++i)
			{
				result.Append(rl[j].First);
				++i;
				result.Append(rl[j].Last);
				++j;
			}
			return result.ToString();
		}
		public static IEnumerable<char> ExpandRanges(IEnumerable<CharRange> ranges)
		{
			var seen = new HashSet<char>();
			foreach (var range in ranges)
				foreach (char ch in range)
					if(seen.Add(ch))
						yield return ch;
		}
		public static IEnumerable<CharRange> NotRanges(IEnumerable<CharRange> ranges)
		{
			// expects ranges to be normalized
			var last = char.MaxValue;
			using (var e = ranges.GetEnumerator())
			{
				if (!e.MoveNext())
				{
					yield return new CharRange(char.MinValue, char.MaxValue);
					yield break;
				}
				if (e.Current.First> char.MinValue)
				{
					yield return new CharRange(char.MinValue, unchecked((char)(e.Current.First - 1)));
					last = e.Current.Last;
					if (char.MaxValue == last)
						yield break;
				}
				while (e.MoveNext())
				{
					if (char.MaxValue == last)
						yield break;
					if (unchecked((char)(last + 1)) < e.Current.First)
						yield return new CharRange(unchecked((char)(last + 1)), unchecked((char)(e.Current.First - 1)));
					last = e.Current.Last;
				}
				if (char.MaxValue > last)
					yield return new CharRange(unchecked((char)(last + 1)), char.MaxValue);
				
			}

		}
		public static void NormalizeRangeList(List<CharRange> ranges)
		{
			ranges.Sort(delegate (CharRange left, CharRange	right)
			{
				return left.First.CompareTo(right.First);
			});
			var or = default(CharRange);
			for (int i = 1; i < ranges.Count; ++i)
			{
				if (ranges[i - 1].Last >= ranges[i].First)
				{
					var nr = new CharRange(ranges[i - 1].First, ranges[i].Last);
					ranges[i - 1] = or = nr;
					ranges.RemoveAt(i);
					--i; // compensated for by ++i in for loop
				}
			}
		}
		int ICollection<char>.Count => Length;
		bool ICollection<char>.IsReadOnly => true;

		public bool Equals(CharRange rhs)
			=> First == rhs.First && Last == rhs.Last;
		public override bool Equals(object obj)
			=>obj is CharRange && Equals((CharRange)obj);
		public override int GetHashCode() 
			=> First ^ Last;
		public override string ToString()
		{
			if (First == Last)
				return _Escape(First);
			if (First + 1 == Last)
				return string.Concat(_Escape(First), _Escape(Last));
			return string.Concat(_Escape(First),"-", _Escape(Last));
		}
		void _ThrowReadOnly()
		{
			throw new NotSupportedException("The collection is read-only.");
		}
		void ICollection<char>.Add(char item)
		{
			_ThrowReadOnly();
		}

		void ICollection<char>.Clear()
		{
			_ThrowReadOnly();
		}

		bool ICollection<char>.Contains(char item)
			=> item >= First && item <= Last;
		

		void ICollection<char>.CopyTo(char[] array, int arrayIndex)
		{
			char ch = First;
			for(int ic = Length,i = arrayIndex;i<ic;++i)
			{
				array[i] = ch;
				++ch;
			}
		}

		IEnumerator<char> IEnumerable<char>.GetEnumerator()
		{
			if (First != Last)
			{
				for (char ch = First; ch < Last; ++ch)
					yield return ch;
				yield return Last;
			}
			else
				yield return First;
		}

		IEnumerator IEnumerable.GetEnumerator()
			=>((IEnumerable<char>)this).GetEnumerator();

		int IList<char>.IndexOf(char item)
		{
			if (First <= item && Last >= item)
				return item - First;
			return -1;
		}

		void IList<char>.Insert(int index, char item)
		{
			_ThrowReadOnly();
		}

		bool ICollection<char>.Remove(char item)
		{
			_ThrowReadOnly();
			return false;
		}

		void IList<char>.RemoveAt(int index)
		{
			_ThrowReadOnly();
		}

		#region _Escape
		string _Escape(char ch)
		{
			switch(ch)
			{
				case '\n':
					return @"\n";
				case '\r':
					return @"\r";
				case '\t':
					return @"\t";
				case '\f':
					return @"\f";
				case '\b':
					return @"\b";
				case '-':
					return @"\-";
				case '[':
					return @"\[";
				case ']':
					return @"\]";
				case '(':
					return @"\(";
				case ')':
					return @"\)";
				case '?':
					return @"\?";
				case '+':
					return @"\+";
				case '*':
					return @"\*";
				case '.':
					return @"\.";
				case '^':
					return @"\^";
				case ' ':
					return " ";
				default:
					if(char.IsControl(ch) || char.IsWhiteSpace(ch))
						return @"\u" + ((int)ch).ToString("x4");
					break;
			}
			return ch.ToString();
		}
		#endregion
		public static bool operator ==(CharRange lhs, CharRange rhs) => lhs.Equals(rhs);
		public static bool operator !=(CharRange lhs, CharRange rhs) => !lhs.Equals(rhs);

	}
}
