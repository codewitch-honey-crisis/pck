using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Pck
{
	/// <summary>
	/// Represents a state in a finite automata machine.
	/// </summary>
	/// <typeparam name="TInput">The input type.</typeparam>
	/// <typeparam name="TAccept">The accept symbol type</typeparam>
	public class FA<TInput,TAccept> : ICloneable 
		where TInput:IEquatable<TInput>
	{
		/// <summary>
		/// If you create a derived specialization, overload this to make the base class use your class for any new object creations.
		/// </summary>
		/// <param name="isAccepting">True if the new state is accepting</param>
		/// <param name="accept">The value of the accepting symbol. Unused if not accepting.</param>
		/// <returns></returns>
		protected virtual FA<TInput, TAccept> CreateFA(bool isAccepting = false,TAccept accept = default(TAccept))
		{
			return new FA<TInput, TAccept>(isAccepting, AcceptSymbol);
		}
		/// <summary>
		/// Indicates the input transitions for this state
		/// </summary>
		public IDictionary<TInput, FA<TInput, TAccept>> Transitions { get; } = new _TrnsDic();
		/// <summary>
		/// Indicates the epsilon transitions for this state
		/// </summary>
		public IList<FA<TInput, TAccept>> EpsilonTransitions { get; } = new List<FA<TInput, TAccept>>();
		/// <summary>
		/// True if this state accepts, otherwise false
		/// </summary>
		public bool IsAccepting { get; set; } = false;
		/// <summary>
		/// Indicates the accepting symbol returned by this state.
		/// </summary>
		public TAccept AcceptSymbol { get; set; } = default(TAccept);

		/// <summary>
		/// A user-defined value to attach to the state.
		/// </summary>
		/// <remarks>Does not get serialized during code genration.</remarks>
		public object Tag { get; set; }
		/// <summary>
		/// Constructs a nonaccepting instance of an FA state
		/// </summary>
		public FA() {}

		/// <summary>
		/// Indicates whether or not the state has any outgoing transitions
		/// </summary>
		public bool IsFinal {
			get { return 0 == Transitions.Count && 0 == EpsilonTransitions.Count; }
		}
		/// <summary>
		/// Indicates whether or not the state loops back to itself eventually.
		/// </summary>
		public bool IsLoop {
			get { return FillDescendants().Contains(this); }
		}
		/// <summary>
		/// Indicates whether or not the state does nothing - that is, it can be eliminated without changing the semantics of the machine.
		/// </summary>
		public bool IsNeutral {
			get { return !IsAccepting && 0 == Transitions.Count && 1 == EpsilonTransitions.Count; }
		}

		/// <summary>
		/// Constructs an FA state with the specified parameters
		/// </summary>
		/// <param name="isAccepting">True if this state is accepting</param>
		/// <param name="acceptSymbol">The accept symbol to report, if accepting</param>
		public FA(bool isAccepting, TAccept acceptSymbol = default(TAccept))
		{
			IsAccepting = isAccepting;
			AcceptSymbol = acceptSymbol;
		}
		/// <summary>
		/// Lexes a token from the input source.
		/// </summary>
		/// <param name="input">The input enumerator to lex on.</param>
		/// <param name="errorSymbol">The symbol to use to indicate an error.</param>
		/// <returns>A key value pair with the symbol and value found during the lex. The input enumerator is advanced as necessary.</returns>
		public KeyValuePair<TAccept,TInput[]> Lex(IEnumerator<TInput> input, TAccept errorSymbol)
		{
			var states =FillEpsilonClosure();
			var values = new List<TInput>();
			if (!input.MoveNext())
			{
				var asc = FillAcceptingStates(states,null);
				if (0 < asc.Count)
					return new KeyValuePair<TAccept, TInput[]>(asc[0].AcceptSymbol, new TInput[0]);
				else
					return new KeyValuePair<TAccept, TInput[]>(errorSymbol, new TInput[0]);
			}
			// Here's where we run most of the match. FillMove runs one interation of the NFA state machine.
			// We match until we can't match anymore (greedy matching) and then report the symbol of the last 
			// match we found, or an error ("#ERROR") if we couldn't find one.
			while (true)
			{
				var next = FillMove(states, input.Current);
				if (0 == next.Count) // couldn't find any states
					break;
				values.Add(input.Current);
				states = next;
				if (!input.MoveNext())
				{
					// end of stream
					var asc = FillAcceptingStates( states,null);
					if (0 < asc.Count)
						return new KeyValuePair<TAccept, TInput[]>(asc[0].AcceptSymbol, values.ToArray());
					else
						return new KeyValuePair<TAccept, TInput[]>(errorSymbol, values.ToArray());
				}
			}
			var ascol = FillAcceptingStates(states, null);
			if (0<ascol.Count) // do we accept?
				return new KeyValuePair<TAccept, TInput[]>(ascol[0].AcceptSymbol, values.ToArray());
			else
			{
				// handle the error condition
				values.Add(input.Current);
				input.MoveNext();
				return new KeyValuePair<TAccept, TInput[]>(errorSymbol, values.ToArray());
			}	
		}
		public bool HasSingleAcceptingState {
			get {
				return 1 == FillAccepting().Count;
			}
		}
		/// <summary>
		/// Computes the set of all states reachable from this state, including itself. Puts the result in the <paramref name="result"/> field amd returns the same collection."/>
		/// </summary>
		/// <param name="result">The collection to fill, or null for one to be created</param>
		/// <returns>Either <paramref name="result"/> or a new collection filled with the result of the closure computation.</returns>
		public IList<FA<TInput,TAccept>> FillClosure(IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result)
				result = new List<FA<TInput,TAccept>>();
			if (!result.Contains(this))
			{
				result.Add(this);
				var tl = Transitions as IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>;
				for (int ic = tl.Count, i = 0; i < ic; ++i)
					tl[i].Key.FillClosure(result);
				for (int ic = EpsilonTransitions.Count, i = 0; i < ic; ++i)
					EpsilonTransitions[i].FillClosure(result);
			}
			return result;
		}
		/// <summary>
		/// Computes the set of all states reachable from this state. Puts the result in the <paramref name="result"/> field amd returns the same collection."/>
		/// </summary>
		/// <param name="result">The collection to fill, or null for one to be created</param>
		/// <returns>Either <paramref name="result"/> or a new collection filled with the result of the closure computation.</returns>
		public IList<FA<TInput,TAccept>> FillDescendants(IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result)
				result = new List<FA<TInput,TAccept>>();
			var tl = Transitions as IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>;
			for (int ic = tl.Count, i = 0; i < ic; ++i)
				tl[i].Key.FillClosure(result);
			for (int ic = EpsilonTransitions.Count, i = 0; i < ic; ++i)
				EpsilonTransitions[i].FillClosure(result);
			return result;
		}
		/// <summary>
		/// Computes the set of all states reachable from this state on no input, including itself. Puts the result in the <paramref name="result"/> field amd returns the same collection."/>
		/// </summary>
		/// <param name="result">The collection to fill, or null for one to be created</param>
		/// <returns>Either <paramref name="result"/> or a new collection filled with the result of the epsilon closure computation.</returns>
		public IList<FA<TInput,TAccept>> FillEpsilonClosure(IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result)
				result = new List<FA<TInput,TAccept>>();
			if (!result.Contains(this))
			{
				result.Add(this);
				for (int ic = EpsilonTransitions.Count, i = 0; i < ic; ++i)
					EpsilonTransitions[i].FillEpsilonClosure(result);
			}
			return result;
		}
		/// <summary>
		/// Takes a set of states and computes the total epsilon closure as a set of states
		/// </summary>
		/// <param name="states">The states to examine</param>
		/// <param name="result">The result to be filled</param>
		/// <returns>The epsilon closure of <paramref name="states"/></returns>
		public static IList<FA<TInput,TAccept>> FillEpsilonClosure(IEnumerable<FA<TInput,TAccept>> states, IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result)
				result = new List<FA<TInput,TAccept>>();
			foreach (var fa in states)
				fa.FillEpsilonClosure(result);
			return result;
		}
		/// <summary>
		/// Creates a clone of this FA state
		/// </summary>
		/// <returns>A new FA that is equal to this FA</returns>
		public FA<TInput, TAccept> Clone()
		{
			var closure = FillClosure();
			var nclosure = new FA<TInput, TAccept>[closure.Count];
			for (var i = 0; i < nclosure.Length; i++)
			{
				nclosure[i] = CreateFA();
				nclosure[i].AcceptSymbol = closure[i].AcceptSymbol;
				nclosure[i].IsAccepting = closure[i].IsAccepting;
				nclosure[i].Tag = closure[i].Tag;
			}
			for (var i = 0; i < nclosure.Length; i++)
			{
				var t = nclosure[i].Transitions;
				var e = nclosure[i].EpsilonTransitions;
				foreach (var trns in closure[i].Transitions)
				{
					var id = closure.IndexOf(trns.Value);
					t.Add(trns.Key, nclosure[id]);
				}
				foreach (var trns in closure[i].EpsilonTransitions)
				{
					var id = closure.IndexOf(trns);
					e.Add(nclosure[id]);
				}
			}
			return nclosure[0];
		}
		object ICloneable.Clone() => Clone();
		public void Finalize(TAccept accept=default(TAccept))
		{
			var asc = FillAccepting();
			var ascc = asc.Count;
			if (1 == ascc) return; // don't need to do anything
			var final = CreateFA(true, accept);
			for(var i=0;i<ascc;++i)
			{
				var fa = asc[i];
				fa.IsAccepting = false;
				fa.EpsilonTransitions.Add(final);
			}
		}
		/// <summary>
		/// Retrieves all the states reachable from this state that are accepting.
		/// </summary>
		/// <param name="result">The list of accepting states. Will be filled after the call.</param>
		/// <returns>The resulting list of accepting states. This is the same value as the result parameter, if specified.</returns>
		public IList<FA<TInput, TAccept>> FillAccepting(IList<FA<TInput, TAccept>> result = null)
			=> FillAcceptingStates(FillClosure(),result);
		/// <summary>
		/// Retrieves all the states in this closure that are accepting
		/// </summary>
		/// <param name="closure">The closure to examine</param>
		/// <param name="result">The list of accepting states. Will be filled after the call.</param>
		/// <returns>The resulting list of accepting states. This is the same value as the result parameter, if specified.</returns>
		public static IList<FA<TInput,TAccept>> FillAcceptingStates(IList<FA<TInput,TAccept>> closure,IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result)
				result = new List<FA<TInput,TAccept>>();
			for (int ic = closure.Count, i = 0; i < ic; ++i)
			{
				var fa = closure[i];
				if (fa.IsAccepting)
					if (!result.Contains(fa))
						result.Add(fa);
			}
			return result;
		}
		/// <summary>
		/// Reduces the complexity of the graph, and returns the result as a new graph
		/// </summary>
		/// <returns>A new graph with a complexity of 1</returns>
		public FA<TInput,TAccept> Reduce()
		{
			var fa = Clone();
			while (true)
			{
				var cc = fa.FillClosure().Count;
				fa.Finalize();
				fa = fa.ToDfa();
				fa.TrimDuplicates();
				if(fa.FillClosure().Count==cc)
					return fa;
			}
		}
		/// <summary>
		/// Indicates whether this state is a duplicate of another state.
		/// </summary>
		/// <param name="rhs">The state to compare with</param>
		/// <returns>True if the states are duplicates (one can be removed without changing the language of the machine)</returns>
		public bool IsDuplicate(FA<TInput,TAccept> rhs)
		{
			return null != rhs && IsAccepting==rhs.IsAccepting &&
				(!IsAccepting || Equals(AcceptSymbol,rhs.AcceptSymbol)) &&
				_SetComparer.Default.Equals(EpsilonTransitions, rhs.EpsilonTransitions) &&
				_SetComparer.Default.Equals((IDictionary<FA<TInput,TAccept>, ICollection<TInput>>)Transitions, (IDictionary<FA<TInput,TAccept>, ICollection<TInput>>)rhs.Transitions);
		}
		/// <summary>
		/// Fills a dictionary of duplicates by state for any duplicates found in the state graph
		/// </summary>
		/// <param name="result">The resulting dictionary to be filled.</param>
		/// <returns>The resulting dictionary of duplicates</returns>
		public IDictionary<FA<TInput, TAccept>, ICollection<FA<TInput, TAccept>>> FillDuplicatesGroupedByState(IDictionary<FA<TInput, TAccept>, ICollection<FA<TInput, TAccept>>> result = null)
			=>FillDuplicatesGroupedByState(FillClosure());

		/// <summary>
		/// Fills a dictionary of duplicates by state for any duplicates found in the state graph
		/// </summary>
		/// <param name="closure">The closure to examine</param>
		/// <param name="result">The resulting dictionary to be filled.</param>
		/// <returns>The resulting dictionary of duplicates</returns>
		public static IDictionary<FA<TInput,TAccept>, ICollection<FA<TInput,TAccept>>> FillDuplicatesGroupedByState(IList<FA<TInput,TAccept>> closure, IDictionary<FA<TInput,TAccept>, ICollection<FA<TInput,TAccept>>> result = null)
		{
			if (null == result)
				result = new Dictionary<FA<TInput,TAccept>, ICollection<FA<TInput,TAccept>>>();
			var cl = closure;
			int c = cl.Count;
			for (int i = 0; i < c; i++)
			{
				var s = cl[i];
				for (int j = i + 1; j < c; j++)
				{
					var cmp = cl[j];
					if (s.IsDuplicate(cmp))
					{
						ICollection<FA<TInput,TAccept>> col = new List<FA<TInput,TAccept>>();
						if (!result.ContainsKey(s))
							result.Add(s, col);
						else
							col = result[s];
						if (!col.Contains(cmp))
							col.Add(cmp);
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Trims duplicate states from the graph.
		/// </summary>
		public void TrimDuplicates() =>TrimDuplicates(FillClosure());
		/// <summary>
		/// Trims duplicate states from the graph
		/// </summary>
		/// <param name="closure">The closure to alter.</param>
		public static void TrimDuplicates(IList<FA<TInput,TAccept>> closure)
		{
			var lclosure = closure;
			var dups = new Dictionary<FA<TInput,TAccept>, ICollection<FA<TInput,TAccept>>>();
			int oc = 0;
			int c = -1;
			while (c < oc)
			{
				c = lclosure.Count;
				FillDuplicatesGroupedByState(lclosure, dups);
				if (0 < dups.Count)
				{
					foreach (KeyValuePair<FA<TInput,TAccept>, ICollection<FA<TInput,TAccept>>> de in dups)
					{
						var replacement = de.Key;
						var targets = de.Value;
						for (int i = 0; i < c; ++i)
						{
							var s = lclosure[i];

							var repls = new List<KeyValuePair<FA<TInput,TAccept>, FA<TInput,TAccept>>>();
							var td = (IDictionary<FA<TInput,TAccept>, ICollection<TInput>>)s.Transitions;
							foreach (var trns in td)
								if (targets.Contains(trns.Key))
									repls.Add(new KeyValuePair<FA<TInput,TAccept>, FA<TInput,TAccept>>(trns.Key, replacement));
							foreach (var repl in repls)
							{
								var inps = td[repl.Key];
								td.Remove(repl.Key);
								td.Add(repl.Key, inps);
							}

							int lc = s.EpsilonTransitions.Count;
							for (int j = 0; j < lc; ++j)
								if (targets.Contains(s.EpsilonTransitions[j]))
									s.EpsilonTransitions[j] = de.Key;
						}
					}
					dups.Clear();
				}
				else
					break;
				oc = c;
				var f = lclosure[0];
				lclosure = f.FillClosure();
				c = lclosure.Count;
			}
		}
		/// <summary>
		/// Returns a duplicate state machine, except one that only goes from this state to the state specified in <paramref name="to"/>. Any state that does not lead to that state is eliminated from the resulting graph.
		/// </summary>
		/// <param name="to">The state to track the path to</param>
		/// <returns>A new state machine that only goes from this state to the state indicated by <paramref name="to"/></returns>
		public FA<TInput, TAccept> ClonePathTo(FA<TInput, TAccept> to)
		{
			var closure = FillClosure();
			var nclosure = new FA<TInput, TAccept>[closure.Count];
			for (var i = 0; i < nclosure.Length; i++)
			{
				nclosure[i] = CreateFA();
				nclosure[i].AcceptSymbol = closure[i].AcceptSymbol;
				nclosure[i].IsAccepting = closure[i].IsAccepting;
				nclosure[i].Tag = closure[i].Tag;
			}
			for (var i = 0; i < nclosure.Length; i++)
			{
				var t = nclosure[i].Transitions;
				var e = nclosure[i].EpsilonTransitions;
				foreach (var trns in closure[i].Transitions)
				{
					if (trns.Value.FillClosure().Contains(to))
					{
						var id = closure.IndexOf(trns.Value);

						t.Add(trns.Key, nclosure[id]);
					}
				}
				foreach (var trns in closure[i].EpsilonTransitions)
				{
					if (trns.FillClosure().Contains(to))
					{
						var id = closure.IndexOf(trns);
						e.Add(nclosure[id]);
					}
				}
			}
			return nclosure[0];
		}
		/// <summary>
		/// Returns a duplicate state machine, except one that only goes from this state to any state specified in <paramref name="to"/>. Any state that does not lead to one of those states is eliminated from the resulting graph.
		/// </summary>
		/// <param name="to">The collection of destination states</param>
		/// <returns>A new state machine that only goes from this state to the states indicated by <paramref name="to"/></returns>
		public FA<TInput, TAccept> ClonePathToAny(IEnumerable<FA<TInput, TAccept>> to)
		{
			var closure = FillClosure();
			var nclosure = new FA<TInput, TAccept>[closure.Count];
			for (var i = 0; i < nclosure.Length; i++)
			{
				nclosure[i] = CreateFA();
				nclosure[i].AcceptSymbol = closure[i].AcceptSymbol;
				nclosure[i].IsAccepting = closure[i].IsAccepting;
				nclosure[i].Tag = closure[i].Tag;
			}
			for (var i = 0; i < nclosure.Length; i++)
			{
				var t = nclosure[i].Transitions;
				var e = nclosure[i].EpsilonTransitions;
				foreach (var trns in closure[i].Transitions)
				{
					if (_ContainsAny(trns.Value.FillClosure(), to))
					{
						var id = closure.IndexOf(trns.Value);

						t.Add(trns.Key, nclosure[id]);
					}
				}
				foreach (var trns in closure[i].EpsilonTransitions)
				{
					if (_ContainsAny(trns.FillClosure(), to))
					{
						var id = closure.IndexOf(trns);
						e.Add(nclosure[id]);
					}
				}
			}
			return nclosure[0];
		}
		
		static bool _ContainsAny(ICollection<FA<TInput,TAccept>> col, IEnumerable<FA<TInput,TAccept>> any)
		{
			foreach (var fa in any)
				if (col.Contains(fa))
					return true;
			return false;
		}
		/// <summary>
		/// Fills a collection with the result of moving each of the specified <paramref name="states"/> by the specified input.
		/// </summary>
		/// <param name="states">The states to examine</param>
		/// <param name="input">The input to use</param>
		/// <param name="result">The states that are now entered as a result of the move</param>
		/// <returns><paramref name="result"/> or a new collection if it wasn't specified.</returns>
		public static IList<FA<TInput,TAccept>> FillMove(IEnumerable<FA<TInput,TAccept>> states, TInput input, IList<FA<TInput,TAccept>> result = null)
		{
			if (null == result) result = new List<FA<TInput,TAccept>>();
			foreach (var fa in FillEpsilonClosure(states))
			{
				// examine each of the states reachable from this state on no input

				FA<TInput,TAccept> ofa;
				// see if this state has this input in its transitions
				if (fa.Transitions.TryGetValue(input, out ofa))
					foreach (var efa in ofa.FillEpsilonClosure())
						if (!result.Contains(efa)) // if it does, add it if it's not already there
							result.Add(efa);
			}
			return result;
		}
		public FA<TInput,TAccept> ToDfa()
		{
			// The DFA states are keyed by the set of NFA states they represent.
			var dfaMap = new Dictionary<List<FA<TInput,TAccept>>, FA<TInput,TAccept>>(_SetComparer.Default);

			var unmarked = new HashSet<FA<TInput,TAccept>>();

			// compute the epsilon closure of the initial state in the NFA
			var states = new List<FA<TInput,TAccept>>();

			FillEpsilonClosure(states);

			// create a new state to represent the current set of states. If one 
			// of those states is accepting, set this whole state to be accepting.
			FA<TInput, TAccept> dfa = CreateFA();
			var al = new List<TAccept>();
			foreach (var fa in states)
				if (fa.IsAccepting)
					if (!al.Contains(fa.AcceptSymbol))
						al.Add(fa.AcceptSymbol);
			int ac = al.Count;
			if (1 == ac)
				dfa.AcceptSymbol = al[0];
			else if (1 < ac)
				dfa.AcceptSymbol = al[0]; // could throw, just choose the first one
			dfa.IsAccepting = 0 < ac;

			FA<TInput,TAccept> result = dfa; // store the initial state for later, so we can return it.

			// add it to the dfa map
			dfaMap.Add(states, dfa);
			dfa.Tag = new List<FA<TInput, TAccept>>(states);
			// add it to the unmarked states, signalling that we still have work to do.
			unmarked.Add(dfa);
			bool done = false;
			while (!done)
			{
				done = true;
				var mapKeys = new HashSet<List<FA<TInput,TAccept>>>(dfaMap.Keys, _SetComparer.Default);
				foreach (var mapKey in mapKeys)
				{
					dfa = dfaMap[mapKey];
					if (unmarked.Contains(dfa))
					{
						// when we get here, mapKey represents the epsilon closure of our 
						// current dfa state, which is indicated by kvp.Value

						// build the transition list for the new state by combining the transitions
						// from each of the old states

						// retrieve every possible input for these states
						var inputs = new HashSet<TInput>();
						foreach (var state in mapKey)
						{
							var dtrns = state.Transitions as IDictionary<FA<TInput, TAccept>, ICollection<TInput>>;
							foreach (var trns in dtrns)
								foreach (var inp in trns.Value)
									inputs.Add(inp);
						}

						foreach (var input in inputs)
						{
							var acc = new List<TAccept>();
							var ns = new List<FA<TInput,TAccept>>();
							foreach (var state in mapKey)
							{
								FA<TInput,TAccept> dst = null;
								if (state.Transitions.TryGetValue(input, out dst))
								{
									foreach (var d in dst.FillEpsilonClosure())
									{
										if (d.IsAccepting)
											if (!acc.Contains(d.AcceptSymbol))
												acc.Add(d.AcceptSymbol);
										if (!ns.Contains(d))
											ns.Add(d);
									}
								}
							}

							FA<TInput,TAccept> ndfa;
							if (!dfaMap.TryGetValue(ns, out ndfa))
							{
								ac = acc.Count;
								ndfa = CreateFA(0<ac);

								if (1 == ac) 
									ndfa.AcceptSymbol = acc[0];
								else if (1 < ac)
									ndfa.AcceptSymbol = acc[0]; // could throw, instead just set it to the first state's accept
								dfaMap.Add(ns, ndfa);
								unmarked.Add(ndfa);
								ndfa.Tag = new List<FA<TInput, TAccept>>(ns);
								done = false;
							}
							dfa.Transitions.Add(input, ndfa);
						}
						unmarked.Remove(dfa);
					}
				}
			}
			return result;
		}
		/// <summary>
		/// Returns a DFA table that can be used to lex or match
		/// </summary>
		/// <param name="symbolLookup">The symbol table to use, or null to just implicitly tag symbols with integer IDs.</param>
		/// <returns>A DFA table that can be used to efficiently match or lex input</returns>
		public KeyValuePair<int, KeyValuePair<TInput[], int>[]>[] ToArray(IList<TAccept> symbolTable = null)
		{
			var dfa = ToDfa();
			var closure = dfa.FillClosure();
			var symbolLookup = new Dictionary<TAccept, int>();
			if (null==symbolTable)
			{
				// have to use this here because symbols can be null and the base dictionary class doesn't accept those
				var i = 0;
				for (int jc = closure.Count, j = 0; j < jc; ++j)
				{
					var fa = closure[j];
					if (fa.IsAccepting && !symbolLookup.ContainsKey(fa.AcceptSymbol))
					{
						symbolLookup.Add(fa.AcceptSymbol, i);
						++i;
					}
				}
			}  else for(int ic = symbolTable.Count,i=0;i<ic;++i)
					symbolLookup.Add(symbolTable[i], i);
			
			var result = new KeyValuePair<int, KeyValuePair<TInput[], int>[]>[closure.Count];
			for (var i = 0; i < result.Length; i++)
			{
				var fa = closure[i];
				var trgs = fa.Transitions as IList<KeyValuePair<FA<TInput,TAccept>,ICollection<TInput>>>;
				var trns = new KeyValuePair<TInput[], int>[trgs.Count];
				var j = 0;
				foreach (var trg in trgs)
				{
					var arr = new TInput[trg.Value.Count];
					trg.Value.CopyTo(arr,0);
					trns[j] = new KeyValuePair<TInput[], int>(
						arr,
						closure.IndexOf(trg.Key));

					++j;
				}
				result[i] = new KeyValuePair<int, KeyValuePair<TInput[], int>[]>(
					fa.IsAccepting ? symbolLookup[fa.AcceptSymbol] : -1,
					trns);

			}
			return result;
		}

		/// <summary>
		/// Returns the first state that accepts from a given FA, or null if none do.
		/// </summary>
		public FA<TInput,TAccept> FirstAcceptingState {
			get {
				foreach (var fa in FillClosure())
					if (fa.IsAccepting)
						return fa;
				return null;
			}
		}
		
		

		#region DotGraphOptions
		/// <summary>
		/// Represents optional rendering parameters for a dot graph.
		/// </summary>
		public sealed class DotGraphOptions
		{
			/// <summary>
			/// The resolution, in dots-per-inch to render at
			/// </summary>
			public int Dpi { get; set; } = 300;
			/// <summary>
			/// The prefix used for state labels
			/// </summary>
			public string StatePrefix { get; set; } = "q";

			/// <summary>
			/// If non-null, specifies a debug render using the specified input string.
			/// </summary>
			/// <remarks>The debug render is useful for tracking the transitions in a state machine</remarks>
			public IEnumerable<TInput> DebugString { get; set; } = null;
			/// <summary>
			/// If non-null, specifies the source NFA from which this DFA was derived - used for debug view
			/// </summary>
			public FA<TInput,TAccept> DebugSourceNfa { get; set; } = null;
		}

		/// <summary>
		/// Writes a Graphviz dot specification to the specified <see cref="TextWriter"/>
		/// </summary>
		/// <param name="writer">The writer</param>
		/// <param name="options">A <see cref="DotGraphOptions"/> instance with any options, or null to use the defaults</param>
		public virtual void WriteDotTo(TextWriter writer, DotGraphOptions options = null)
		{
			_WriteDotTo(FillClosure(), writer, options);
		}
		static void _WriteDotTo(IList<FA<TInput, TAccept>> closure, TextWriter writer, DotGraphOptions options = null)
		{
			if (null == options) options = new DotGraphOptions();
			string spfx = null == options.StatePrefix ? "q" : options.StatePrefix;
			writer.WriteLine("digraph FA {");
			writer.WriteLine("rankdir=LR");
			writer.WriteLine("node [shape=circle]");
			var finals = new List<FA<TInput, TAccept>>();
			var neutrals = new List<FA<TInput, TAccept>>();
			var accepting = new List<FA<TInput, TAccept>>();
			foreach (var ffa in closure)
			{
				if (ffa.IsAccepting)
					accepting.Add(ffa);
				if (ffa.IsFinal && !ffa.IsAccepting)
					finals.Add(ffa);
			}
			IList<FA<TInput, TAccept>> fromStates = null;
			IList<FA<TInput, TAccept>> toStates = null;
			var tchar = default(TInput);
			if(null!=options.DebugString)
				toStates = closure[0].FillEpsilonClosure();
			if (null != options.DebugString)
			{
				foreach (var ch in options.DebugString)
				{
					fromStates = FillEpsilonClosure(toStates, null);
					tchar = ch;
					toStates = FillMove(fromStates, ch);
					if (0 == toStates.Count)
						break;

				}
			}
			if (null != toStates)
			{
				toStates = FillEpsilonClosure(toStates, null);
			}
			int i = 0;
			foreach (var ffa in closure)
			{
				if (!finals.Contains(ffa))
				{
					if (ffa.IsAccepting)
						accepting.Add(ffa);
					else if (ffa.IsNeutral)
						neutrals.Add(ffa);
				}
				foreach (var trns in ffa.Transitions)
				{
					var di = closure.IndexOf(trns.Value);
					writer.Write(spfx);
					writer.Write(i);
					writer.Write("->");
					writer.Write(spfx);
					writer.Write(di.ToString());
					writer.Write(" [label=\"");
					writer.Write(_EscapeLabel(Convert.ToString(trns.Key)));
					writer.WriteLine("\"]");
				}
				// do epsilons
				foreach (var fffa in ffa.EpsilonTransitions)
				{
					writer.Write(spfx);
					writer.Write(i);
					writer.Write("->");
					writer.Write(spfx);
					writer.Write(closure.IndexOf(fffa));
					writer.WriteLine(" [style=dashed,color=gray]");
				}


				++i;
			}
			string delim = "";
			i = 0;
			foreach (var ffa in closure)
			{
				writer.Write(spfx);
				writer.Write(i);
				writer.Write(" [");
				if (null != options.DebugString)
				{
					if (null != toStates && toStates.Contains(ffa))
					{
						writer.Write("color=green,");
					}
					if (null != fromStates && fromStates.Contains(ffa) && (null == toStates || !toStates.Contains(ffa)))
					{
						writer.Write("color=darkgreen,");
					}
				}
				writer.Write("label=<");
				writer.Write("<TABLE BORDER=\"0\"><TR><TD>");
				writer.Write(spfx);
				writer.Write("<SUB>");
				writer.Write(i);
				writer.Write("</SUB></TD></TR>");

				if (ffa.IsAccepting)
				{
					writer.Write("<TR><TD>");
					writer.Write(Convert.ToString(ffa.AcceptSymbol).Replace("\"", "&quot;"));
					writer.Write("</TD></TR>");

				}
				writer.Write("</TABLE>");
				writer.Write(">");
				bool isfinal = false;
				if (accepting.Contains(ffa) || (isfinal = finals.Contains(ffa)))
					writer.Write(",shape=doublecircle");
				if (isfinal || neutrals.Contains(ffa))
				{
					if ((null == fromStates || !fromStates.Contains(ffa)) &&
						(null == toStates || !toStates.Contains(ffa)))
					{
						writer.Write(",color=gray");
					}
				}
				writer.WriteLine("]");
				++i;
			}
			delim = "";
			if (0 < accepting.Count)
			{
				foreach (var ntfa in accepting)
				{
					writer.Write(delim);
					writer.Write(spfx);
					writer.Write(closure.IndexOf(ntfa));
					delim = ",";
				}
				writer.WriteLine(" [shape=doublecircle]");
			}
			delim = "";
			if (0 < neutrals.Count)
			{
				delim = "";
				foreach (var ntfa in neutrals)
				{
					if ((null == fromStates || !fromStates.Contains(ntfa)) &&
						(null == toStates || !toStates.Contains(ntfa))
						)
					{
						writer.Write(delim);
						writer.Write(spfx);
						writer.Write(closure.IndexOf(ntfa));
						delim = ",";
					}
				}
				writer.WriteLine(" [color=gray]");
				delim = "";
				if (null != fromStates)
				{
					foreach (var ntfa in neutrals)
					{
						if (fromStates.Contains(ntfa) && (null == toStates || !toStates.Contains(ntfa)))
						{
							writer.Write(delim);
							writer.Write(spfx);
							writer.Write(closure.IndexOf(ntfa));
							delim = ",";
						}
					}

					writer.WriteLine(" [color=darkgreen]");
				}
				if (null != toStates)
				{
					delim = "";
					foreach (var ntfa in neutrals)
					{
						if (toStates.Contains(ntfa))
						{
							writer.Write(delim);
							writer.Write(spfx);
							writer.Write(closure.IndexOf(ntfa));
							delim = ",";
						}
					}
					writer.WriteLine(" [color=green]");
				}


			}
			delim = "";
			if (0 < finals.Count)
			{
				foreach (var ntfa in finals)
				{
					writer.Write(delim);
					writer.Write(spfx);
					writer.Write(closure.IndexOf(ntfa));
					delim = ",";
				}
				writer.WriteLine(" [shape=doublecircle,color=gray]");
			}

			writer.WriteLine("}");

		}
		#endregion
		static string _EscapeLabel(string label)
		{
			if (string.IsNullOrEmpty(label)) return label;

			var result = label.Replace("\\", @"\\");
			result = result.Replace("\"", "\\\"");
			result = result.Replace("\n", "\\n");
			result = result.Replace("\r", "\\r");
			result = result.Replace("\0", "\\0");
			result = result.Replace("\v", "\\v");
			result = result.Replace("\t", "\\t");
			result = result.Replace("\f", "\\f");
			return result;
		}
		/// <summary>
		/// Renders Graphviz output for this machine to the specified file
		/// </summary>
		/// <param name="filename">The output filename. The format to render is indicated by the file extension.</param>
		/// <param name="options">A <see cref="DotGraphOptions"/> instance with any options, or null to use the defaults</param>
		public void RenderToFile(string filename, DotGraphOptions options = null)
		{
			if (null == options)
				options = new DotGraphOptions();
			string args = "-T";
			string ext = Path.GetExtension(filename);
			if (0 == string.Compare(".png", ext, StringComparison.InvariantCultureIgnoreCase))
				args += "png";
			else if (0 == string.Compare(".jpg", ext, StringComparison.InvariantCultureIgnoreCase))
				args += "jpg";
			else if (0 == string.Compare(".bmp", ext, StringComparison.InvariantCultureIgnoreCase))
				args += "bmp";
			else if (0 == string.Compare(".svg", ext, StringComparison.InvariantCultureIgnoreCase))
				args += "svg";
			if (0 < options.Dpi)
				args += " -Gdpi=" + options.Dpi.ToString();

			args += " -o\"" + filename + "\"";

			var psi = new ProcessStartInfo("dot", args)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardInput = true
			};
			using (var proc = Process.Start(psi))
			{
				WriteDotTo(proc.StandardInput, options);
				proc.StandardInput.Close();
				proc.WaitForExit();
			}

		}
		/// <summary>
		/// Renders Graphviz output for this machine to a stream
		/// </summary>
		/// <param name="format">The output format. The format to render can be any supported dot output format. See dot command line documation for details.</param>
		/// <param name="options">A <see cref="DotGraphOptions"/> instance with any options, or null to use the defaults</param>
		/// <returns>A stream containing the output. The caller is expected to close the stream when finished.</returns>
		public Stream RenderToStream(string format, bool copy = false, DotGraphOptions options = null)
		{
			if (null == options)
				options = new DotGraphOptions();
			string args = "-T";
			args += string.Concat(" ", format);
			if (0 < options.Dpi)
				args += " -Gdpi=" + options.Dpi.ToString();

			var psi = new ProcessStartInfo("dot", args)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true
			};
			using (var proc = Process.Start(psi))
			{
				WriteDotTo(proc.StandardInput, options);
				proc.StandardInput.Close();
				if (!copy)
					return proc.StandardOutput.BaseStream;
				else
				{
					MemoryStream stm = new MemoryStream();
					proc.StandardOutput.BaseStream.CopyTo(stm);
					proc.StandardOutput.BaseStream.Close();
					proc.WaitForExit();
					return stm;
				}
			}
		}
		#region _SetComparer
		sealed class _SetComparer : IEqualityComparer<IList<FA<TInput,TAccept>>>, IEqualityComparer<ICollection<FA<TInput,TAccept>>>, IEqualityComparer<IDictionary<TInput, FA<TInput,TAccept>>>
		{
			// unordered comparison
			public bool Equals(IList<FA<TInput,TAccept>> lhs, IList<FA<TInput,TAccept>> rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return true;
				else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
					return false;
				if (lhs.Count != rhs.Count)
					return false;
				using (var xe = lhs.GetEnumerator())
				using (var ye = rhs.GetEnumerator())
					while (xe.MoveNext() && ye.MoveNext())
						if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
							return false;
				return true;
			}
			// unordered comparison
			public bool Equals(ICollection<FA<TInput,TAccept>> lhs, ICollection<FA<TInput,TAccept>> rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return true;
				else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
					return false;
				if (lhs.Count != rhs.Count)
					return false;
				using (var xe = lhs.GetEnumerator())
					using (var ye = rhs.GetEnumerator())
						while (xe.MoveNext() && ye.MoveNext())
							if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
								return false;
				return true;
			}
			public bool Equals(IDictionary<TInput, FA<TInput,TAccept>> lhs, IDictionary<TInput, FA<TInput,TAccept>> rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return true;
				else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
					return false;
				if (lhs.Count != rhs.Count)
					return false;
				using (var xe = lhs.GetEnumerator())
				using (var ye = rhs.GetEnumerator())
					while (xe.MoveNext() && ye.MoveNext())
						if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
							return false;
				return true;
			}
			public bool Equals(IDictionary<FA<TInput,TAccept>, ICollection<TInput>> lhs, IDictionary<FA<TInput,TAccept>, ICollection<TInput>> rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return true;
				else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
					return false;
				if (lhs.Count != rhs.Count)
					return false;
				using (var xe = lhs.GetEnumerator())
				using (var ye = rhs.GetEnumerator())
					while (xe.MoveNext() && ye.MoveNext())
						if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
							return false;
				return true;
			}
			public static bool _EqualsInput(ICollection<TInput> lhs, ICollection<TInput> rhs)
			{
				if (ReferenceEquals(lhs, rhs))
					return true;
				else if (ReferenceEquals(null, lhs) || ReferenceEquals(null, rhs))
					return false;
				if (lhs.Count != rhs.Count)
					return false;
				using (var xe = lhs.GetEnumerator())
				using (var ye = rhs.GetEnumerator())
					while (xe.MoveNext() && ye.MoveNext())
						if (!rhs.Contains(xe.Current) || !lhs.Contains(ye.Current))
							return false;
				return true;
			}
			public int GetHashCode(IList<FA<TInput,TAccept>> lhs)
			{
				var result = 0;
				for(int ic=lhs.Count,i=0;i<ic;++i)
				{
					var fa = lhs[i];
					if (null != fa)
						result ^= fa.GetHashCode();
				}
				return result;
			}
			public int GetHashCode(ICollection<FA<TInput,TAccept>> lhs)
			{
				var result = 0;
				foreach (var fa in lhs)
					if (null != fa)
						result ^= fa.GetHashCode();
				return result;
			}
			public int GetHashCode(IDictionary<TInput, FA<TInput,TAccept>> lhs)
			{
				var result = 0;
				foreach (var kvp in lhs)
					result ^= kvp.GetHashCode();
				return result;
			}
			public static readonly _SetComparer Default = new _SetComparer();
		}
		#endregion
		#region _TrnsDic
		/// <summary>
		/// _TrnsDic is a specialized transition container that can return its transitions in 3 different ways:
		/// 1. a dictionary where each transition state is keyed by an individual input character (default)
		/// 2. a dictionary where each collection of inputs is keyed by the transition state (used mostly by optimizations)
		/// 3. an indexable list of pairs where the key is the transition state and the value is the collection of inputs
		/// use casts to get at the appropriate interface for your operation.
		/// </summary>
		class _TrnsDic : 
			IDictionary<TInput, FA<TInput,TAccept>>, // #1
			IDictionary<FA<TInput,TAccept>, ICollection<TInput>>, // #2
			IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>> // #3
		{
			IDictionary<FA<TInput,TAccept>, ICollection<TInput>> _inner = 
				new ListDictionary<FA<TInput,TAccept>, ICollection<TInput>>();

			public FA<TInput,TAccept> this[TInput key] {
				get {
					foreach (var trns in _inner)
					{
						if (trns.Value.Contains(key))
							return trns.Key;
					}
					throw new KeyNotFoundException();
				}
				set {
					Remove(key);
					ICollection<TInput> hs;
					if (_inner.TryGetValue(value, out hs))
					{
						hs.Add(key);
					}
					else
					{
						hs = new HashSet<TInput>();
						hs.Add(key);
						_inner.Add(value, hs);
					}
				}
			}

			public ICollection<TInput> Keys {
				get {
					return new _KeysCollection(_inner);
				}

			}

			sealed class _KeysCollection : ICollection<TInput>
			{
				IDictionary<FA<TInput,TAccept>, ICollection<TInput>> _inner;
				public _KeysCollection(IDictionary<FA<TInput,TAccept>, ICollection<TInput>> inner)
				{
					_inner = inner;
				}
				public int Count {
					get {
						var result = 0;
						foreach (var val in _inner.Values)
							result += val.Count;
						return result;
					}
				}
				void _ThrowReadOnly() { throw new NotSupportedException("The collection is read-only."); }
				public bool IsReadOnly => true;

				public void Add(TInput item)
				{
					_ThrowReadOnly();
				}

				public void Clear()
				{
					_ThrowReadOnly();
				}

				public bool Contains(TInput item)
				{
					foreach (var val in _inner.Values)
						if (val.Contains(item))
							return true;
					return false;
				}

				public void CopyTo(TInput[] array, int arrayIndex)
				{
					var si = arrayIndex;
					foreach (var val in _inner.Values)
					{
						val.CopyTo(array, si);
						si += val.Count;
					}
				}

				public IEnumerator<TInput> GetEnumerator()
				{
					foreach (var val in _inner.Values)
						foreach (var ch in val)
							yield return ch;
				}

				public bool Remove(TInput item)
				{
					_ThrowReadOnly();
					return false;
				}

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			}
			sealed class _ValuesCollection : ICollection<FA<TInput,TAccept>>
			{
				IDictionary<FA<TInput,TAccept>, ICollection<TInput>> _inner;
				public _ValuesCollection(IDictionary<FA<TInput,TAccept>, ICollection<TInput>> inner)
				{
					_inner = inner;
				}
				public int Count {
					get {
						var result = 0;
						foreach (var val in _inner.Values)
							result += val.Count;
						return result;
					}
				}
				void _ThrowReadOnly() { throw new NotSupportedException("The collection is read-only."); }
				public bool IsReadOnly => true;

				public void Add(FA<TInput,TAccept> item)
				{
					_ThrowReadOnly();
				}

				public void Clear()
				{
					_ThrowReadOnly();
				}

				public bool Contains(FA<TInput,TAccept> item)
				{
					return _inner.Keys.Contains(item);
				}

				public void CopyTo(FA<TInput,TAccept>[] array, int arrayIndex)
				{
					var si = arrayIndex;
					foreach (var trns in _inner)
					{
						foreach (var ch in trns.Value)
						{
							array[si] = trns.Key;
							++si;
						}
					}
				}

				public IEnumerator<FA<TInput,TAccept>> GetEnumerator()
				{
					foreach (var trns in _inner)
						foreach (var ch in trns.Value)
							yield return trns.Key;
				}

				public bool Remove(FA<TInput,TAccept> item)
				{
					_ThrowReadOnly();
					return false;
				}

				IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			}
			public ICollection<FA<TInput,TAccept>> Values =>new _ValuesCollection(_inner); 

			public int Count {
				get {
					var result = 0;
					foreach (var trns in _inner)
						result += trns.Value.Count;
					return result;
				}
			}
			IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>> _InnerList =>_inner as IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>; 
			ICollection<FA<TInput,TAccept>> IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.Keys =>_inner.Keys; 
			ICollection<ICollection<TInput>> IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.Values =>_inner.Values; 
			int ICollection<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.Count =>_inner.Count; 
			public bool IsReadOnly =>_inner.IsReadOnly; 

			KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.this[int index] { get => _InnerList[index]; set { _InnerList[index] = value; } }

			ICollection<TInput> IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.this[FA<TInput,TAccept> key] { get { return _inner[key]; } set { _inner[key] = value; } }

			public void Add(TInput key, FA<TInput,TAccept> value)
			{
				if (ContainsKey(key))
					throw new InvalidOperationException("The key is already present in the dictionary.");
				ICollection<TInput> hs;
				if (_inner.TryGetValue(value, out hs))
				{
					hs.Add(key);
				}
				else
				{
					hs = new HashSet<TInput>();
					hs.Add(key);
					_inner.Add(value, hs);
				}
			}

			public void Add(KeyValuePair<TInput, FA<TInput,TAccept>> item)
				=>Add(item.Key, item.Value);
			

			public void Clear()
				=>_inner.Clear();
			

			public bool Contains(KeyValuePair<TInput, FA<TInput,TAccept>> item)
			{
				ICollection<TInput> hs;
				return _inner.TryGetValue(item.Value, out hs) && hs.Contains(item.Key);
			}

			public bool ContainsKey(TInput key)
			{
				foreach (var trns in _inner)
				{
					if (trns.Value.Contains(key))
						return true;
				}
				return false;
			}

			public void CopyTo(KeyValuePair<TInput, FA<TInput,TAccept>>[] array, int arrayIndex)
			{
				using (var e = ((IEnumerable<KeyValuePair<TInput, FA<TInput, TAccept>>>)this).GetEnumerator())
				{
					var i = arrayIndex;
					while(e.MoveNext())
					{
						array[i] = e.Current;
						++i;
					}
				}
			}

			public IEnumerator<KeyValuePair<TInput, FA<TInput,TAccept>>> GetEnumerator()
			{
				foreach (var trns in _inner)
					foreach (var ch in trns.Value)
						yield return new KeyValuePair<TInput, FA<TInput,TAccept>>(ch, trns.Key);
			}

			public bool Remove(TInput key)
			{
				FA<TInput,TAccept> rem = null;
				foreach (var trns in _inner)
				{
					if (trns.Value.Contains(key))
					{
						trns.Value.Remove(key);
						if (0 == trns.Value.Count)
						{
							rem = trns.Key;
							break;
						}
						return true;
					}
				}
				if (null != rem)
				{
					_inner.Remove(rem);
					return true;
				}
				return false;
			}

			public bool Remove(KeyValuePair<TInput, FA<TInput,TAccept>> item)
			{
				ICollection<TInput> hs;
				if (_inner.TryGetValue(item.Value, out hs))
				{
					if (hs.Contains(item.Key))
					{
						if (1 == hs.Count)
							_inner.Remove(item.Value);
						else
							hs.Remove(item.Key);
						return true;
					}
				}
				return false;
			}

			public bool TryGetValue(TInput key, out FA<TInput,TAccept> value)
			{
				foreach (var trns in _inner)
				{
					if (trns.Value.Contains(key))
					{
						value = trns.Key;
						return true;
					}
				}
				value = null;
				return false;
			}

			IEnumerator IEnumerable.GetEnumerator()
				=>GetEnumerator();
			
			void IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.Add(FA<TInput,TAccept> key, ICollection<TInput> value)
				=>_inner.Add(key, value);

			bool IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.ContainsKey(FA<TInput,TAccept> key)
				=>_inner.ContainsKey(key);
			
			bool IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.Remove(FA<TInput,TAccept> key)
				=>_inner.Remove(key);
			

			bool IDictionary<FA<TInput,TAccept>, ICollection<TInput>>.TryGetValue(FA<TInput,TAccept> key, out ICollection<TInput> value)
				=>_inner.TryGetValue(key, out value);
			

			void ICollection<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.Add(KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> item)
				=>_inner.Add(item);
			
			bool ICollection<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.Contains(KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> item)
				=>_inner.Contains(item);
			

			void ICollection<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.CopyTo(KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>[] array, int arrayIndex)
				=>_inner.CopyTo(array, arrayIndex);
			

			bool ICollection<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.Remove(KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> item)
				=>_inner.Remove(item);
			

			IEnumerator<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>> IEnumerable<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.GetEnumerator()
			=> _inner.GetEnumerator();
			

			int IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.IndexOf(KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> item)
				=> _InnerList.IndexOf(item);

			void IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.Insert(int index, KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>> item)
				=> _InnerList.Insert(index, item);

			void IList<KeyValuePair<FA<TInput,TAccept>, ICollection<TInput>>>.RemoveAt(int index)
				=> _InnerList.RemoveAt(index);
		}
		#endregion
	}
}
