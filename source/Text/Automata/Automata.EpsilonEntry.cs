// <copyright file="Automata.EpsilonEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected sealed class EpsilonEntry : Entry
		{
			#region Static Fields and Constants

			public static readonly EpsilonEntry Instance = new EpsilonEntry();

			#endregion

			#region Ctors

			private EpsilonEntry()
			{
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => "Epsilon";

			#endregion
		}

		#endregion
	}
}