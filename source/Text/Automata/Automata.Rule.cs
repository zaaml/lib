// <copyright file="Automata.FiniteState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		[DebuggerDisplay("{" + nameof(Name) + "}")]
		protected class Rule
		{
			protected Rule(string name)
			{
				Name = name;
			}

			internal bool Inline { get; set; }

			public string Name { get; }

			internal List<Production> Productions { get; } = new();
		}
	}
}