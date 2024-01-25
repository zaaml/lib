// <copyright file="Automata.RangeMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected sealed class RangeMatchEntry : PrimitiveMatchEntry
		{
			public RangeMatchEntry(TOperand minOperand, TOperand maxOperand)
			{
				MinOperand = minOperand;
				MaxOperand = maxOperand;

				Range = new Range<int>(ConvertFromOperand(minOperand), ConvertFromOperand(maxOperand));
			}

			internal RangeMatchEntry(Range<int> range)
			{
				Range = range;

				MinOperand = ConvertToOperand(range.Minimum);
				MaxOperand = ConvertToOperand(range.Maximum);
			}

			protected override string DebuggerDisplay => $"[{MinOperand};{MaxOperand}]";

			public static IEqualityComparer<RangeMatchEntry> EqualityComparer => RangeMatchEntryEqualityComparer.Instance;

			internal Range<int> Range { get; }

			public TOperand MaxOperand { get; }

			public TOperand MinOperand { get; }

			public override bool Match(TOperand operand)
			{
				return Range.Contains(ConvertFromOperand(operand));
			}

			public override bool Match(int operand)
			{
				return Range.Contains(operand);
			}

			private sealed class RangeMatchEntryEqualityComparer : IEqualityComparer<RangeMatchEntry>
			{
				public static readonly RangeMatchEntryEqualityComparer Instance = new();

				private RangeMatchEntryEqualityComparer()
				{
				}

				public bool Equals(RangeMatchEntry x, RangeMatchEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					var equalityComparer = EqualityComparer<TOperand>.Default;

					return equalityComparer.Equals(x.MaxOperand, y.MaxOperand) && equalityComparer.Equals(x.MinOperand, y.MinOperand);
				}

				public int GetHashCode(RangeMatchEntry obj)
				{
					var equalityComparer = EqualityComparer<TOperand>.Default;

					unchecked
					{
						return (equalityComparer.GetHashCode(obj.MaxOperand) * 397) ^ equalityComparer.GetHashCode(obj.MinOperand);
					}
				}
			}
		}
	}
}