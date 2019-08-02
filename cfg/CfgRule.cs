using System;
using System.Collections.Generic;
using System.Text;

namespace Pck
{
	public class CfgRule : CfgNode, IEquatable<CfgRule>, ICloneable
	{
		public CfgRule(string left,params string[] right)
		{
			Left = left;
			Right.AddRange(right);
		}
		public CfgRule(string left, IEnumerable<string> right)
		{
			Left = left;
			Right.AddRange(right);
		}
		public string Left { get; set; }
		public IList<string> Right { get; } = new List<string>();
		public bool IsNil { get { return 0 == Right.Count; } }
		public bool IsDirectlyRecursive {
			get {
				for (int ic = Right.Count, i = 0; i < ic; ++i)
				{
					if (Right[i] == Left)
						return true;
				}
				return false;
			}
		}
		public bool IsDirectlyLeftRecursive { get { return !IsNil && Right[0] == Left; } }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(Left);
			sb.Append(" ->");
			for(int ic=Right.Count,i=0;i<ic;++i)
			{
				sb.Append(" ");
				sb.Append(Right[i]);
			}
			return sb.ToString();
		}

		public CfgRule Clone()
		{
			return new CfgRule(Left, Right);
		}
		object ICloneable.Clone()
			=> Clone();

		#region Value semantics
		public bool Equals(CfgRule rhs)
		{
			if (ReferenceEquals(rhs, this)) return true;
			if (ReferenceEquals(rhs, null)) return false;
			return Left == rhs.Left && CollectionUtility.Equals(Right,rhs.Right);
		}
		public override bool Equals(object rhs)
			=> Equals(rhs as CfgRule);

		public override int GetHashCode()
		{
			var result = 0;
			if (null != Left)
				result ^= Left.GetHashCode();
			result ^= CollectionUtility.GetHashCode(Right);
			return result;
		}
		public static bool operator ==(CfgRule lhs, CfgRule rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return true;
			if (ReferenceEquals(lhs, null)) return false;
			return lhs.Equals(rhs);
		}
		public static bool operator !=(CfgRule lhs, CfgRule rhs)
		{
			if (ReferenceEquals(lhs, rhs)) return false;
			if (ReferenceEquals(lhs, null)) return true;
			return !lhs.Equals(rhs);
		}
		#endregion

	}
}
