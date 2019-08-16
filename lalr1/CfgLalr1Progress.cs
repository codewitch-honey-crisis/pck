using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum CfgLalr1Status
	{
		Unknown = 0,
		ComputingStates,
		ComputingConfigurations,
		ComputingClosure,
		ComputingMove,
		CreatingLookaheadGrammar,
		ComputingReductions
	}
	public struct CfgLalr1Progress
	{
		public CfgLalr1Progress(CfgLalr1Status status,int count)
		{
			Status = status;
			Count = count;
		}
		public CfgLalr1Status Status { get; }
		public int Count { get; }
	}
}
