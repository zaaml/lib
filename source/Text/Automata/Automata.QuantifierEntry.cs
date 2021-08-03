// <copyright file="Automata.QuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class QuantifierEntry : Entry
		{
			public readonly QuantifierKind Kind;
			public readonly int Maximum;
			public readonly int Minimum;
			public readonly QuantifierMode Mode;
			public readonly PrimitiveEntry PrimitiveEntry;

			public QuantifierEntry(PrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = kind;
				Mode = mode;

				var range = QuantifierHelper.GetRange(kind);

				Minimum = range.Minimum;
				Maximum = range.Maximum;
			}

			public QuantifierEntry(PrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode)
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

			protected override string DebuggerDisplay => $"{PrimitiveEntry}{GetQuantifierKindString(Kind, Minimum, Maximum)}";

			private string GetQuantifierKindString(QuantifierKind kind, int minimum, int maximum)
			{
				return kind switch
				{
					QuantifierKind.Generic => $"{{{minimum},{maximum}}}",
					QuantifierKind.ZeroOrOne => "?",
					QuantifierKind.ZeroOrMore => "*",
					QuantifierKind.OneOrMore => "+",
					_ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
				};
			}
		}
	}
}