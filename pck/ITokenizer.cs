using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public interface ITokenizer : IEnumerable<Token>
	{
		void Restart(IEnumerable<char> input);
		
	}
}
