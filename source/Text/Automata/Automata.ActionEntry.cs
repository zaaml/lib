// <copyright file="Automata.ActionEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class ActionEntry : Entry
		{
			#region Ctors

			public ActionEntry(Action<AutomataContext> action)
			{
				Action = action;
			}

			#endregion

			#region Properties

			public Action<AutomataContext> Action { get; }

			protected override string DebuggerDisplay => "Predicate";

			#endregion
		}

		#endregion
	}
}