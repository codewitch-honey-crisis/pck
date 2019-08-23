using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{

	public enum ErrorLevel
	{
		Message=0,
		Warning = 1,
		Error=2
	}
	public interface IMessage
	{
		ErrorLevel ErrorLevel { get;}
		string Message { get; }
		int Line { get; }
		int Column { get; }
		long Position { get; }
	}
}
