// <copyright file="Automata.Syntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		[DebuggerDisplay("{" + nameof(Name) + "}")]
		protected class Syntax
		{
			protected Syntax(string name)
			{
				Name = name;
			}

			internal bool CollapseBacktracking { get; set; }

			internal bool Inline { get; set; }

			public string Name { get; }

			internal List<Production> Productions { get; } = new();
		}
	}
}