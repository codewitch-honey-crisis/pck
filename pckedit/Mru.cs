namespace Pck
{
	using Microsoft.Win32;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;

	class Mru : IEnumerable<string>
	{
		static readonly string _ExeName = Path.GetFileName(Assembly.GetExecutingAssembly().GetModules()[0].Name);
		const int _MaxMRU = 10;
		List<string> _paths;
		[DllImport("shell32.dll", CharSet = CharSet.Auto)]
		private static extern void
			SHAddToRecentDocs(int flag, string path);
		public Mru()
		{
			_paths = new List<string>();	
		}
		public int Count { get { return _paths.Count; } }
		public bool Add(string filepath)
		{
			if (!_paths.Contains(filepath))
			{
				SHAddToRecentDocs(2, filepath);
				_paths.Insert(0,filepath);
				while(_MaxMRU < _paths.Count)
					_paths.RemoveAt(_paths.Count - 1);
				return true;
			}
			return false;
		}
		public void Save()
		{
			var s = string.Concat(@"Software\", _ExeName, @"\MRU");
			RegistryKey key = null;
			try
			{
				key = Registry.CurrentUser.OpenSubKey(s, true);
				if (null == key)
					key = Registry.CurrentUser.CreateSubKey(s);
				if (null == key)
					throw new Exception("Could not access the registry.");
				var names = key.GetValueNames();
				for (var i = 0; i < names.Length; i++)
					key.DeleteValue(names[i], false);
				for (int ic = _paths.Count, i = 0; i < ic; ++i)
					key.SetValue(i.ToString(), _paths[i]);
			}
			finally
			{
				if (null != key)
					key.Close();
				key = null;
			}
		}
		public void Load()
		{
			_paths.Clear();
			using (var key = Registry.CurrentUser.OpenSubKey(string.Concat(@"Software\", _ExeName, @"\MRU"), false))
			{
				if (null == key)
					return;
				var names = key.GetValueNames();
				//Array.Sort(names); // uncomment to make values come back asciibetically
				for (var i = 0; i < names.Length; i++)
				{
					var s = key.GetValue(names[i]) as string;
					if (!string.IsNullOrEmpty(s))
						_paths.Add(s);
				}
			}
		}

		public IEnumerator<string> GetEnumerator()
		{
			return ((IEnumerable<string>)_paths).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<string>)_paths).GetEnumerator();
		}
	}
}
