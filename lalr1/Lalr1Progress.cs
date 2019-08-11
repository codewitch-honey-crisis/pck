using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum Lalr1Status
	{
		Unknown = 0,
		ComputingStates,
		ComputingConfigurations,
		ComputingClosure,
		ComputingMove,
		CreatingLookaheadGrammar,
		ComputingReductions
	}
	public struct Lalr1Progress
	{
		public Lalr1Progress(Lalr1Status status,int count)
		{
			Status = status;
			Count = count;
		}
		public Lalr1Status Status { get; }
		public int Count { get; }
	}
}
