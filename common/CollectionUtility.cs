using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	static class CollectionUtility
	{
		
		#region CollectionAdapter

		internal sealed class CollectionAdapter<T> : ICollection<T>
		{
			internal CollectionAdapter(IEnumerable<T> inner)
			{
				_inner = inner;
			}
			readonly IEnumerable<T> _inner;
			public int Count { get { return Count(_inner); } }

			public bool IsReadOnly { get { return true; } }

			public void CopyTo(T[] array, int index)
			{
				CollectionUtility.CopyTo(_inner, array, index);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			void ICollection<T>.Add(T item)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			bool ICollection<T>.Remove(T item)
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			void ICollection<T>.Clear()
			{
				throw new NotSupportedException("The collection is read-only.");
			}

			public bool Contains(T item)
			{
				return Contains<T>(_inner, item);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
		}
		#endregion CollectionAdapter
		#region ListAdapter
		
		sealed class ListAdapter<T> : IList<T>
		{
			IEnumerable<T> _inner;
			internal ListAdapter(IEnumerable<T> inner)
			{
				_inner = inner;
			}
			static void _ThrowReadOnly()
			{
				throw new NotSupportedException("The list is read only.");
			}
			public T this[int index] { get { return GetAt(_inner, index); } set { _ThrowReadOnly(); } }

			public int Count { get { return Count(_inner); } }
			bool ICollection<T>.IsReadOnly { get { return true; } }

			void ICollection<T>.Add(T item)
			{
				_ThrowReadOnly();
			}

			void ICollection<T>.Clear()
			{
				_ThrowReadOnly();
			}

			public bool Contains(T item)
			{
				return _inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_inner.CopyTo(array, arrayIndex);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			public int IndexOf(T item)
			{
				return _inner.IndexOf(item);
			}

			void IList<T>.Insert(int index, T item)
			{
				_ThrowReadOnly();
			}

			bool ICollection<T>.Remove(T item)
			{
				_ThrowReadOnly();
				return false;
			}

			void IList<T>.RemoveAt(int index)
			{
				_ThrowReadOnly();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
		}
		#endregion ListAdapter
		#region ReadOnlyAdapter 
		
		sealed class ReadOnlyCollectionAdapter<T> : ICollection<T>
		{
			static void _ThrowReadOnly()
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			internal ReadOnlyCollectionAdapter(ICollection<T> inner)
			{
				_inner = inner;
			}
			readonly ICollection<T> _inner;

			public int Count { get { return _inner.Count; } }
			public bool IsReadOnly { get { return true; } }

			void ICollection<T>.Add(T item)
			{
				_ThrowReadOnly();
			}

			public void Clear()
			{
				_ThrowReadOnly();
			}

			public bool Contains(T item)
			{
				return _inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_inner.CopyTo(array, arrayIndex);
			}

			bool ICollection<T>.Remove(T item)
			{
				_ThrowReadOnly();
				return false;
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
		}
		
		sealed class ReadOnlyListAdapter<T> : IList<T>
		{
			static void _ThrowReadOnly()
			{
				throw new NotSupportedException("The collection is read-only.");
			}
			internal ReadOnlyListAdapter(IList<T> inner)
			{
				_inner = inner;
			}

			readonly IList<T> _inner;
			public T this[int index] { get { return _inner[index]; } set { _ThrowReadOnly(); } }

			public int Count { get { return _inner.Count; } }
			public bool IsReadOnly { get { return true; } }

			void ICollection<T>.Add(T item)
			{
				_ThrowReadOnly();
			}

			void ICollection<T>.Clear()
			{
				_ThrowReadOnly();
			}

			public bool Contains(T item)
			{
				return _inner.Contains(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				_inner.CopyTo(array, arrayIndex);
			}

			public IEnumerator<T> GetEnumerator()
			{
				return _inner.GetEnumerator();
			}

			public int IndexOf(T item)
			{
				return _inner.IndexOf(item);
			}

			void IList<T>.Insert(int index, T item)
			{
				_ThrowReadOnly();
			}

			bool ICollection<T>.Remove(T item)
			{
				_ThrowReadOnly();
				return false;
			}

			void IList<T>.RemoveAt(int index)
			{
				_ThrowReadOnly();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _inner.GetEnumerator();
			}
		}
		#endregion ReadOnlyAdapter
		public static ICollection<T> AsCollection<T>(this IEnumerable<T> collection)
		{
			var result = collection as ICollection<T>;
			if (null != result) return result;
			result = new CollectionAdapter<T>(collection);
			return result;
		}
		
		public static IList<T> AsList<T>(this IEnumerable<T> collection)
		{
			var result = collection as IList<T>;
			if (null != result) return result;
			result = new ListAdapter<T>(collection);
			return result;
		}
		
		public static IList<T> AsReadOnly<T>(IList<T> list) { return list.IsReadOnly ? list : new ReadOnlyListAdapter<T>(list); }
		public static ICollection<T> AsReadOnly<T>(ICollection<T> collection) { return collection.IsReadOnly ? collection : new ReadOnlyCollectionAdapter<T>(collection); }
		public static object GetAt(this IEnumerable collection, int index)
		{
			var e = collection.GetEnumerator();
			try
			{
				int i = -1;
				while (i < index && e.MoveNext())
					++i;
				if (i < index)
					throw new ArgumentOutOfRangeException("index");
				return e.Current;
			}
			finally
			{
				var d = e as IDisposable;
				if (null != d)
				{
					d.Dispose();
					d = null;
				}
				e = null;
			}
		}
		public static T GetAt<T>(this IEnumerable<T> collection, int index)
		{
			using (var e = collection.GetEnumerator())
			{
				int i = -1;
				while (i < index && e.MoveNext())
					++i;
				if (i < index)
					throw new ArgumentOutOfRangeException("index");
				return e.Current;
			}

		}
		public static int IndexOf<T>(this IEnumerable<T> collection, T item)
		{
			int result = 0;
			foreach (T cmp in collection)
			{
				if (Equals(item, cmp))
					return result;
				++result;
			}
			return -1;
		}
		public static int IndexOf<T>(this IEnumerable<T> collection, T item, IEqualityComparer<T> comparer)
		{
			if (null == comparer)
				return IndexOf(collection, item);
			int result = 0;
			foreach (T cmp in collection)
			{
				if (comparer.Equals(item, cmp))
					return result;
				++result;
			}

			return -1;
		}
		
		public static int Count(this IEnumerable collection)
		{
			IEnumerator e = collection.GetEnumerator();
			try
			{
				int result = 0;
				while (e.MoveNext())
					++result;
				return result;
			}
			finally
			{
				var d = e as IDisposable;
				if (null != d)
				{
					d.Dispose();
					d = null;
				}
				e = null;
			}
		}
		public static bool Equals<T>(this IList<T> lhs, IList<T> rhs)
		{
			if (object.ReferenceEquals(lhs, rhs))
				return true;
			else if (object.ReferenceEquals(null, lhs) || object.ReferenceEquals(null, rhs))
				return false;
			int c = lhs.Count;
			if (c != rhs.Count) return false;
			for (int i = 0; i < c; ++i)
				if (!object.Equals(lhs[i], rhs[i]))
					return false;
			return true;
		}
		public static bool Equals<T>(this ICollection<T> lhs, ICollection<T> rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
				return false;
			if (lhs.Count != rhs.Count)
				return false;
			using (var xe = lhs.GetEnumerator())
			using (var ye = rhs.GetEnumerator())
				while (xe.MoveNext() && ye.MoveNext())
					if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
						return false;
			return true;
		}
		public static bool Equals<TKey, TValue>(this ICollection<KeyValuePair<TKey, TValue>> lhs, ICollection<KeyValuePair<TKey, TValue>> rhs)
		{
			if (ReferenceEquals(lhs, rhs))
				return true;
			else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
				return false;
			if (lhs.Count != rhs.Count)
				return false;
			using (var xe = lhs.GetEnumerator())
			using (var ye = rhs.GetEnumerator())
				while (xe.MoveNext() && ye.MoveNext())
					if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
						return false;
			return true;
		}
		public static int GetHashCode<T>(this IEnumerable<T> collection)
		{
			int result = 0;
			if (!ReferenceEquals(null, collection))
			{
				foreach (T o in collection)
				{
					if (!ReferenceEquals(null, o))
					{
						result ^= o.GetHashCode();
					}
				}
			}
			return result;
		}
		public static int GetHashCode<T>(this IList<T> lhs)
		{
			if (object.ReferenceEquals(null, lhs))
				return int.MinValue;
			int result = 0;
			int c = lhs.Count;
			for (int i = 0; i < c; ++i)
				if (!object.ReferenceEquals(null, lhs[i]))
					result ^= lhs[i].GetHashCode();
			return result;
		}
		public static int CopyTo<T>(this IEnumerable<T> source, T[] destination, int destinationStartIndex)
		{
			if (null == source)
				throw new ArgumentNullException("value");
			if (null == destination)
				throw new ArgumentNullException("array");
			if (destinationStartIndex < destination.GetLowerBound(0) || destinationStartIndex > destination.GetUpperBound(0))
				throw new ArgumentOutOfRangeException("startIndex");
			int i = destinationStartIndex;
			foreach (T v in source)
			{
				destination[i] = v;
				++i;
			}
			return i;
		}
		public static IEnumerable<T> Range<T>(this IEnumerable<T> collection, int start = 0, int maxCount = 0)
		{
			var i = 0;
			var c = 0;
			foreach (var item in collection)
			{
				if (i >= start)
				{
					yield return item;
					++c;
				}
				if (0 < maxCount && c == maxCount)
					break;
				++i;
			}
			if (i < start)
				throw new ArgumentOutOfRangeException("start");
			//if (0 < count && c < count)
			//	throw new ArgumentOutOfRangeException("count");
		}
		public static IList<T> GetLongestCommonPrefix<T>(this IEnumerable<IList<T>> ss)
		{
			IList<T> result = null;
			foreach (var list in ss)
			{
				foreach (var list2 in ss)
				{
					if (!ReferenceEquals(list, list2))
					{
						var pfx = GetCommonPrefix<T>(new IList<T>[] { list, list2 });
						if (null == result || (null != pfx && pfx.Count > result.Count))
							result = pfx;
					}
				}
			}
			if (null == result) return new T[0];
			return result;
		}
		public static bool HasSingleItem<T>(this IEnumerable<T> collection)
		{
			var e = collection.GetEnumerator();
			try
			{
				return e.MoveNext() && !e.MoveNext();
			}
			finally
			{
				var d = e as IDisposable;
				if (null != d)
					d.Dispose();
			}
		}
		public static IList<T> GetCommonPrefix<T>(this IEnumerable<IList<T>> ss)
		{
			// adaptation of solution found here: https://stackoverflow.com/questions/33709165/get-common-prefix-of-two-string
			if (ss.IsNullOrEmpty())
				return new T[0];
			var first = ss.First();
			if (ss.HasSingleItem())
				return first;

			int prefixLength = 0;

			foreach (object item in ss.First())
			{
				foreach (IList<T> s in ss)
				{
					if (s.Count <= prefixLength || !Equals(s[prefixLength], item))
					{
						var result = new T[prefixLength];
						for (var i = 0; i < result.Length; i++)
							result[i] = first[i];

						return result;
					}
				}
				++prefixLength;
			}

			return first; // all strings identical up to length of ss[0]
		}
		public static T First<T>(this IEnumerable<T> collection)
		{
			using (var e = collection.GetEnumerator())
				if (e.MoveNext())
					return e.Current;
				else
					throw new ArgumentException("The collection was empty.", "collection");
		}
		/// <summary>
		/// Tests whether the enumeration is null or empty.
		/// </summary>
		/// <param name="collection">The enumeration to test</param>
		/// <returns>True if the enumeration is null, or if enumerating ends before the first element. Otherwise, this method returns true.</returns>
		/// <remarks>For actual collections, testing the "Count" property should be slightly faster.</remarks>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
		{
			if (null == collection) return true;
			var e = collection.GetEnumerator();
			try
			{
				return !e.MoveNext();
			}
			finally
			{
				var d = e as IDisposable;
				if (null != d)
				{
					d.Dispose();
					d = null;
				}
				e = null;
			}
		}
		public static IEnumerable<T> Cast<T>(this IEnumerable collection)
		{
			foreach (object o in collection)
			{
				yield return (T)o;
			}
		}
		public static IEnumerable<T> Convert<T>(this IEnumerable collection)
		{
			Type t = typeof(T);
			foreach (object o in collection)
			{
				yield return (T)System.Convert.ChangeType(o, t);
			}
		}
		public static T[] ToArray<T>(this IEnumerable<T> source)
		{
			var arr = source as T[];
			if (null != arr) return arr;
			var result = new List<T>(source);
			return result.ToArray();
		}

		public static Array ToArray(this ICollection source, Type elementType)
		{
			var al = new ArrayList(source);
			return al.ToArray(elementType);
		}
		
		public static Array ToArray(this IEnumerable source, Type elementType)
		{
			var al = new ArrayList();
			foreach (object o in source)
				al.Add(o);
			return al.ToArray(elementType);
		}
		public static object[] ToArray(this IEnumerable source)
		{
			var al = new ArrayList();
			foreach (object o in source)
				al.Add(o);
			return al.ToArray();
		}
		public static bool Contains<T>(this IEnumerable<T> collection, T item)
		{
			foreach (T cmp in collection)
				if (Equals(item, cmp))
					return true;
			return false;
		}
		public static IEnumerable<T> Select<T>(this IEnumerable<T> axis, Func<T, bool> predicate)
		{
			foreach(var item in axis)
			{
				if (predicate(item))
					yield return item;
			}
		}
		public static bool Contains<T>(this IEnumerable<T> collection, T item, IEqualityComparer<T> comparer)
		{
			if (null == comparer)
				return Contains(collection, item);
			foreach (T cmp in collection)
				if (comparer.Equals(item, cmp))
					return true;
			return false;
		}
		public static bool StartsWith<T>(this IEnumerable<T> collection, IEnumerable<T> values, IEqualityComparer<T> equalityComparer = null)
		{
			if (null == equalityComparer)
				equalityComparer = EqualityComparer<T>.Default;
			using (var x = collection.GetEnumerator())
			{
				using (var y = values.GetEnumerator())
				{
					while (y.MoveNext())
					{
						if (!x.MoveNext())
							return false;
						if (!equalityComparer.Equals(x.Current, y.Current))
							return false;
					}
				}
			}
			return true;
		}
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
		{
			foreach (var item in values)
				collection.Add(item);
		}
		public static void AddRangeUnique<T>(this ICollection<T> collection, IEnumerable<T> values)
		{
			foreach (var item in values)
				if(!collection.Contains(item))
					collection.Add(item);
		}
		public static IEnumerable<T> NonNulls<T>(this IEnumerable<T> collection)
		{
			foreach (var item in collection)
				if (null != item)
					yield return item;
		}
		public static string ToString(this IEnumerable e, string itemFormat = null)
		{
			if (null != itemFormat)
				itemFormat=string.Concat("{0:", itemFormat, "}");
			var sb = new StringBuilder();
			sb.Append("{ ");
			var d = "";
			bool appended = false;
			foreach (var i in e)
			{
				sb.Append(d);
				if (null == itemFormat)
				{
					sb.Append(i);
				}
				else
					sb.AppendFormat(itemFormat, i);
				d = ", ";
				appended = true;
			}
			if (appended)
				sb.Append(" }");
			else
				sb.Append("}");
			return sb.ToString();
		}
		public static IEnumerable<IList<T>> Split<T>(this IEnumerable<T> collection, T delim, IEqualityComparer<T> equalityComparer = null)
		{
			if (null == equalityComparer)
				equalityComparer = EqualityComparer<T>.Default;
			if (collection.IsNullOrEmpty())
				yield break;
			var l = new List<T>();
			foreach (var item in collection)
			{
				if (!equalityComparer.Equals(item, delim))
				{
					l.Add(item);
				}
				else
				{
					yield return l;
					l = new List<T>();

				}
			}
			yield return l;
		}
		public static IEnumerable<T> Join<T>(this IEnumerable<IList<T>> segments, IEnumerable<T> delim)
		{
			if (IsNullOrEmpty(delim))
			{
				foreach (var l in segments)
				{
					var ic = l.Count;
					for (var i = 0; i < ic; ++i)
						yield return l[i];
				}
				yield break;
			}
			var first = true;
			foreach (var l in segments)
			{
				if (first)
					first = false;
				else
					foreach (var i in delim)
						yield return i;
				var ic = l.Count;
				for (var i = 0; i < ic; ++i)
					yield return l[i];
			}
		}
		public static IEnumerable<T> Replace<T>(this IEnumerable<T> collection, T oldValue, IEnumerable<T> newValues, IEqualityComparer<T> equalityComparer = null)
		{
			return Join(Split(collection, oldValue, equalityComparer), newValues);
		}

	}
}
