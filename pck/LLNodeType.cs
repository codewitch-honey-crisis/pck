namespace Pck
{
	/// <summary>
	/// An enumeration indicating the node types of the parser
	/// </summary>
	public enum LLNodeType
	{
		/// <summary>
		/// Indicates the initial state.
		/// </summary>
		Initial = 0,
		/// <summary>
		/// Parser is on a non-terminal
		/// </summary>
		NonTerminal = 1,
		/// <summary>
		/// Parser is ending a non-terminal node
		/// </summary>
		EndNonTerminal = 2,
		/// <summary>
		/// Parser is on a terminal node
		/// </summary>
		Terminal = 3,
		/// <summary>
		/// Parser is on an error node
		/// </summary>
		Error = 4,
		/// <summary>
		/// The parser is at the end of the document
		/// </summary>
		EndDocument = 5
	}
}
