// <copyright file="Parser.Automata.LeftRecursionPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class LeftRecursionPredicate
			{
				#region Fields

				private readonly int _priority;
				private readonly ParserState _state;
				public readonly PredicateEntry Entry;

				#endregion

				#region Ctors

				public LeftRecursionPredicate(ParserState state, int priority)
				{
					_state = state;
					_priority = priority;

					Entry = new PredicateEntry((Func<AutomataContext, bool>) Predicate);
				}

				#endregion

				#region Methods

				private bool Predicate(AutomataContext context)
				{
					var stateEntryContext = (PriorityStateEntryContext) context.GetTopStateEntryContext(_state);
					var pr = stateEntryContext?.Priority ?? 0;

					return _priority >= pr;
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}