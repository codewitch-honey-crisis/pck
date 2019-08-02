using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public sealed class CfgException : Exception
	{
		public IList<CfgMessage> Messages { get; }
		public CfgException(string message, int errorCode = -1, int line = 0, int column = 0, long position = -1) :
			this(new CfgMessage[] { new CfgMessage(CfgErrorLevel.Error, errorCode, message, line, column, position) })
		{ }
		static string _FindMessage(IEnumerable<CfgMessage> messages)
		{
			var l = new List<CfgMessage>(messages);
			if (null == messages) return "";
			int c = 0;
			foreach (var m in l)
			{
				if (CfgErrorLevel.Error == m.ErrorLevel)
				{
					if (1 == l.Count)
						return m.ToString();
					return string.Concat(m, " (multiple messages)");
				}
				++c;
			}
			foreach (var m in messages)
				return m.ToString();
			return "";
		}
		public CfgException(IEnumerable<CfgMessage> messages) : base(_FindMessage(messages))
		{
			Messages = new List<CfgMessage>(messages);
		}
		public static void ThrowIfErrors(IEnumerable<CfgMessage> messages)
		{
			if (null == messages) return;
			foreach (var m in messages)
				if (CfgErrorLevel.Error == m.ErrorLevel)
					throw new CfgException(messages);
		}
	}
}
