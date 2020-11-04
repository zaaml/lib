// <copyright file="Automata.Builder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Fields

		private int _inlineStateCounter;

		#endregion

		#region Properties

		private HashSet<FiniteState> States { get; } = new HashSet<FiniteState>();

		#endregion

		#region Methods

		protected void AddState(FiniteState state, Production production)
		{
			state.Productions.Add(production);

			States.Add(state);
		}

		protected void AddState(FiniteState state, params Production[] productions)
		{
			AddState(state, productions.AsEnumerable());
		}

		protected void AddState(FiniteState state, IEnumerable<Production> productions)
		{
			foreach (var production in productions)
				AddState(state, production);
		}

		protected Production CreateProduction(params Entry[] entries)
		{
			return new Production(entries);
		}

		protected StateEntry Inline(IEnumerable<Entry> entries)
		{
			var internalState = new InternalState("Internal_" + _inlineStateCounter++);

			AddState(internalState, new Production(entries));

			return new StateEntry(internalState);
		}

		protected StateEntry Inline(params Entry[] entries)
		{
			return Inline(entries.AsEnumerable());
		}

		protected static RangeMatchEntry Range(TOperand from, TOperand to)
		{
			return new RangeMatchEntry(from, to);
		}

		protected static SetMatchEntry Set(params PrimitiveMatchEntry[] primitiveMatches)
		{
			return new SetMatchEntry(primitiveMatches);
		}

		protected static SingleMatchEntry Single(TOperand operand)
		{
			return new SingleMatchEntry(operand);
		}

		#endregion
	}
}