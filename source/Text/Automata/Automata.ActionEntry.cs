// <copyright file="Automata.ActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected class ActionEntry : Entry
		{
			public ActionEntry(Action<AutomataContext> action)
			{
				Action = action;
			}

			public Action<AutomataContext> Action { get; }

			protected override string DebuggerDisplay => "Predicate";
		}
	}
}