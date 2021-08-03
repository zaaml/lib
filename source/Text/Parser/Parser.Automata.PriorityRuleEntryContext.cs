// <copyright file="Parser.Automata.PriorityStateEntryContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private sealed class PriorityRuleEntryContext : RuleEntryContext
			{
				#region Ctors

				public PriorityRuleEntryContext(int priority)
				{
					Priority = priority;
				}

				#endregion

				#region Properties

				public int Priority { get; }

				#endregion
			}

			#endregion
		}

		#endregion
	}
}