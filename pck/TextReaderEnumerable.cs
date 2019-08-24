using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Pck
{
	
	public sealed class FileReaderEnumerable : TextReaderEnumerable
	{
		protected override bool CanCreateReader => true;
		readonly string _filename;
		public FileReaderEnumerable(string filename)
		{
			if (null == filename) throw new ArgumentNullException("filename");
			if (0 == filename.Length) throw new ArgumentException("The filename must not be empty.", "filename");
			_filename = filename;
		}
		protected override TextReader CreateTextReader()
		{
			return File.OpenText(_filename);
		}
	}
	public sealed class ConsoleReaderEnumerable : TextReaderEnumerable
	{
		protected override bool CanCreateReader => false;
		public ConsoleReaderEnumerable()
		{
		}
		protected override TextReader CreateTextReader()
		{
			return Console.In;
		}
	}
	public sealed class UrlReaderEnumerable : TextReaderEnumerable
	{
		protected override bool CanCreateReader => true;
		readonly string _url;
		public UrlReaderEnumerable(string url)
		{
			if (null == url) throw new ArgumentNullException("url");
			if (0 == url.Length) throw new ArgumentException("The url must not be empty.", "url");
			_url = url;
		}
		protected override TextReader CreateTextReader()
		{
			var wq = WebRequest.Create(_url);
			var wr = wq.GetResponse();
			return new StreamReader(wr.GetResponseStream());
		}
	}
	public abstract class TextReaderEnumerable : IEnumerable<char>
	{
		#region _OnceReaderEnumerable
		sealed class _OnceTextReaderEnumerable : TextReaderEnumerable
		{
			TextReader _reader;
			internal _OnceTextReaderEnumerable(TextReader reader)
			{
				_reader = reader;
			}
			protected override TextReader CreateTextReader()
			{
				if (null == _reader)
					throw new NotSupportedException("This method can only be called once.");
				var r = _reader;
				_reader = null;
				return r;
			}
			protected override bool CanCreateReader => false;
		}
		#endregion
		public static TextReaderEnumerable FromReader(TextReader reader)
		{
			if (null == reader)
				throw new ArgumentNullException("reader");
			return new _OnceTextReaderEnumerable(reader);
		}
		public IEnumerator<char> GetEnumerator()
		{
			return new TextReaderEnumerator(this);
		}
		protected abstract bool CanCreateReader { get; }
		protected abstract TextReader CreateTextReader();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();
		sealed class TextReaderEnumerator : IEnumerator<char>
		{
			TextReaderEnumerable _outer;
			TextReader _reader;
			int _state;
			char _current;
			internal TextReaderEnumerator(TextReaderEnumerable outer)
			{
				_outer = outer;
				_reader = null;
				if (_outer.CanCreateReader)
					Reset();
				else
				{
					_state = -1;
					_reader = _outer.CreateTextReader(); // doesn't really recreate it
				}
			}

			public char Current 
			{
				get {
					switch(_state)
					{
						case -3:
							throw new ObjectDisposedException(GetType().Name);
						case -2:
							throw new InvalidOperationException("The cursor is past the end of input.");
						case -1:
							throw new InvalidOperationException("The cursor is before the start of input.");
					}
					return _current;
				}
			}
			object IEnumerator.Current => Current;

			public void Dispose()
			{
				// Dispose of unmanaged resources.
				_Dispose(true);
				// Suppress finalization.
				GC.SuppressFinalize(this);
			}
			~TextReaderEnumerator()
			{
				_Dispose(false);
			}
			// Protected implementation of Dispose pattern.
			void _Dispose(bool disposing)
			{
				if (null==_reader)
					return;

				if (disposing)
				{
					_reader.Close();
					_reader = null;
					_state = -3;
				}

			}

			public bool MoveNext()
			{
				switch(_state)
				{
					case -3:
						throw new ObjectDisposedException(GetType().Name);
					case -2:
						return false;
				}
				int i = _reader.Read();
				if (-1 == _state && 
					((BitConverter.IsLittleEndian && '\uFEFF' == i) || 
						(!BitConverter.IsLittleEndian && '\uFFFE'==i))) // skip the byte order mark
					i = _reader.Read();
				_state = 0;
				if (-1 == i)
				{
					_state = -2;
					return false;
				}
				_current = unchecked((char)i);
				return true;
			}

			public void Reset()
			{
				// don't bother if we haven't moved.
				if (-1 == _state) return;
				try
				{
					
					// optimization for streamreader.
					var sr = _reader as StreamReader;
					if (null != sr && null != sr.BaseStream && sr.BaseStream.CanSeek && 0L == sr.BaseStream.Seek(0, SeekOrigin.Begin))
					{
						_state = -1;
						return;
					}
				}
				catch (IOException) { }
				if (!_outer.CanCreateReader)
					throw new NotSupportedException();
				_Dispose(true);
				_reader = _outer.CreateTextReader();
				_state = -1;
			}
		}
	}
}
