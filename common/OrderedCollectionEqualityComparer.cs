using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{

	class OrderedCollectionEqualityComparer<T> : IEqualityComparer<IList<T>>
	{
		public static readonly OrderedCollectionEqualityComparer<T> Default = new OrderedCollectionEqualityComparer<T>();
		public OrderedCollectionEqualityComparer(IEqualityComparer<T> itemComparer)
		{
			_itemComparer = itemComparer ?? EqualityComparer<T>.Default;
		}
		public OrderedCollectionEqualityComparer() : this(EqualityComparer<T>.Default) { }
		IEqualityComparer<T> _itemComparer;
		public bool Equals(IList<T> x, IList<T> y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(null, x)) return false;
			if (ReferenceEquals(null, y)) return false;
			var c = x.Count;
			if (y.Count != c) return false;
			for (int i = 0; i < c; ++i)
				if (!_itemComparer.Equals(x[i], y[i]))
					return false;
			return true;
		}

		public int GetHashCode(IList<T> obj)
		{
			var c = obj.Count;
			var result = 0;
			for (int i = 0; i < c; ++i)
				if (null != obj[i])
					result ^= obj.GetHashCode();
			return result;
		}
		public bool Equals(ICollection<T> x, ICollection<T> y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(null, x)) return false;
			if (ReferenceEquals(null, y)) return false;
			var c = x.Count;
			if (y.Count != c) return false;
			using (var ex = x.GetEnumerator())
			{
				using (var ey = y.GetEnumerator())
				{
					while (true)
					{
						var moved = false;
						if ((moved = ex.MoveNext()) != ey.MoveNext())
							return false;
						if (!moved)
							break;
						if (!_itemComparer.Equals(ex.Current, ey.Current))
							return false;
					}
				}
			}
			return true;
		}
		public int GetHashCode(ICollection<T> obj)
		{
			var result = 0;
			foreach(var item in obj)
				if (null != item)
					result ^=item.GetHashCode();
			return result;
		}

		public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(null, x)) return false;
			if (ReferenceEquals(null, y)) return false;
			using (var ex = x.GetEnumerator())
			{
				using (var ey = y.GetEnumerator())
				{
					while (true)
					{
						var moved = false;
						if ((moved = ex.MoveNext()) != ey.MoveNext())
							return false;
						if (!moved)
							break;
						if (!_itemComparer.Equals(ex.Current, ey.Current))
							return false;
					}
				}
			}
			return true;
		}
		public int GetHashCode(IEnumerable<T> obj)
		{
			var result = 0;
			foreach (var item in obj)
				if (null != item)
					result ^= item.GetHashCode();

			return result;
		}
	}
}
