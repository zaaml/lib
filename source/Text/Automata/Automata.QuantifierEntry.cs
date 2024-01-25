// <copyright file="Automata.QuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
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

			public static IEqualityComparer<QuantifierEntry> EqualityComparer => QuantifierEntryEqualityComparer.Instance;

			protected Interval<int> Interval => new(Minimum, IntervalEndPoint.Closed, Maximum, Maximum == int.MaxValue ? IntervalEndPoint.Unbounded : IntervalEndPoint.Closed);

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

			private sealed class QuantifierEntryEqualityComparer : IEqualityComparer<QuantifierEntry>
			{
				public static readonly QuantifierEntryEqualityComparer Instance = new();

				private QuantifierEntryEqualityComparer()
				{
				}

				public bool Equals(QuantifierEntry x, QuantifierEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					return x.Kind == y.Kind && x.Maximum == y.Maximum && x.Minimum == y.Minimum && x.Mode == y.Mode && EntryEqualityComparer.Instance.Equals(x.PrimitiveEntry, y.PrimitiveEntry);
				}

				public int GetHashCode(QuantifierEntry obj)
				{
					unchecked
					{
						var hashCode = (int)obj.Kind;

						hashCode = (hashCode * 397) ^ obj.Maximum;
						hashCode = (hashCode * 397) ^ obj.Minimum;
						hashCode = (hashCode * 397) ^ (int)obj.Mode;
						hashCode = (hashCode * 397) ^ EntryEqualityComparer.Instance.GetHashCode(obj.PrimitiveEntry);

						return hashCode;
					}
				}
			}
		}
	}
}