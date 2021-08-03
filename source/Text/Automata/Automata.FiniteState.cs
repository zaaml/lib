// <copyright file="Automata.FiniteState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		[DebuggerDisplay("{" + nameof(Name) + "}")]
		protected abstract class Rule : EntryPoint
		{
			#region Ctors

			protected Rule(string name)
			{
				Name = name;
			}

			#endregion

			#region Properties

			internal bool Inline { get; set; }

			public string Name { get; }

			internal List<Production> Productions { get; } = new List<Production>();

			#endregion
		}

		protected class Rule<TContext> : Rule where TContext : AutomataContext
		{
			#region Ctors

			protected Rule(string name) : base(name)
			{
			}

			#endregion
		}

		#endregion
	}
}