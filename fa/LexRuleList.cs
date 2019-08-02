using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class LexRuleList : List<LexRule>
	{
		public LexRule this[string symbol] {
			get {
				for (int ic = Count, i = 0; i < ic; ++i)
				{
					var item = this[i];
					if (symbol == item.Left)
						return item;
				}
				throw new KeyNotFoundException();
			}
		}
		public int IndexOf(string symbol)
		{
			for (int ic = Count, i = 0; i < ic; ++i)
			{
				var item = this[i];
				if (symbol == item.Left)
					return i;
			}
			return -1;
		}
		public bool Contains(string symbol)
			=> -1 < IndexOf(symbol);
		public bool Remove(string symbol)
		{
			var i = IndexOf(symbol);
			if (-1 < i)
			{
				RemoveAt(i);
				return true;
			}
			return false;
		}
	}
}
