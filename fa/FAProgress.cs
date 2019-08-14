using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public enum FAStatus
	{
		Unknown,
		DfaTransform,
		TrimDuplicates
	}
	public struct FAProgress
	{
		public FAProgress(FAStatus status,int count)
		{
			Status = status;
			Count = count;
		}
		public FAStatus Status { get; }
		public int Count { get; }
	}
}
