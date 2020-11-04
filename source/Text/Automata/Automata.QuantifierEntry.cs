// <copyright file="Automata.QuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class QuantifierEntry : Entry
		{
			#region Fields

			public readonly QuantifierKind Kind;
			public readonly int Maximum;
			public readonly int Minimum;
			public readonly QuantifierMode Mode;
			public readonly PrimitiveEntry PrimitiveEntry;

			#endregion

			#region Ctors

			public QuantifierEntry(PrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = kind;
				Mode = mode;

				var range = QuantifierHelper.GetRange(kind);

				Minimum = range.Minimum;
				Maximum = range.Maximum;
			}

			public QuantifierEntry(PrimitiveEntry primitiveEntry, Range<int> range, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = QuantifierHelper.GetKind(range);
				Mode = mode;

				Minimum = range.Minimum;
				Maximum = range.Maximum;
			}

			private QuantifierEntry(PrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode, int maximum, int minimum)
			{
				Kind = kind;
				Mode = mode;
				PrimitiveEntry = primitiveEntry;
				Maximum = maximum;
				Minimum = minimum;
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => "Quantifier";

			#endregion
		}

		#endregion
	}
}