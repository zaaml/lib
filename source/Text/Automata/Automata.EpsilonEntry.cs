// <copyright file="Automata.EpsilonEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected sealed class EpsilonEntry : Entry
		{
			public static readonly EpsilonEntry Instance = new EpsilonEntry();

			private EpsilonEntry()
			{
			}

			protected override string DebuggerDisplay => "Epsilon";
		}
	}
}