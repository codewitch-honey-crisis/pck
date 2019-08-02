using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class XbnfProductionList : List<XbnfProduction>
	{
		public XbnfProduction this[string name] {
			get {
				for (int ic = Count, i = 0; i < ic; ++i)
				{
					var item = this[i];
					if (name == item.Name)
						return item;
				}
				throw new KeyNotFoundException();
			}
		}
		public int IndexOf(string name)
		{
			for (int ic = Count, i = 0; i < ic; ++i)
			{
				var item = this[i];
				if (name == item.Name)
					return i;
			}
			return -1;
		}
		public bool Contains(string name)
			=> -1 < IndexOf(name);
		public bool Remove(string name)
		{
			var i = IndexOf(name);
			if (-1 < i)
			{
				RemoveAt(i);
				return true;
			}
			return false;
		}
	}
}
