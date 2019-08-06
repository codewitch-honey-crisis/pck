using System.Collections.Generic;

namespace Pck
{
	public class Lalr1ParseTable : List<IDictionary<string, (int RuleOrStateId, string Left, string[] Right)>>
	{

	}
}
