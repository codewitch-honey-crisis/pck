using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum CfgLL1Status
	{
		Unknown=0,
		ComputingPredicts,
		ComputingFollows,
		Factoring,
		CreatingParseTable
	}
	public struct CfgLL1Progress
	{
		public CfgLL1Progress(CfgLL1Status status,int count)
		{
			Status = status;
			Count = count;
		}
		public CfgLL1Status Status { get; }
		public int Count { get; }
	}
}
