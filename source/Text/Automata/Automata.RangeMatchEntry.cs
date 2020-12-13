// <copyright file="Automata.RangeMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected sealed class RangeMatchEntry : PrimitiveMatchEntry
		{
			#region Ctors

			public RangeMatchEntry(TOperand minOperand, TOperand maxOperand)
			{
				MinOperand = minOperand;
				MaxOperand = maxOperand;

				IntRange = new Interval<int>(ConvertOperand(minOperand), ConvertOperand(maxOperand));
			}

			internal RangeMatchEntry(Interval<int> range)
			{
				IntRange = range;

				MinOperand = (TOperand) Convert.ChangeType(range.Minimum, typeof(TOperand));
				MaxOperand = (TOperand) Convert.ChangeType(range.Maximum, typeof(TOperand));
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => $"[{MinOperand};{MaxOperand}]";

			internal Interval<int> IntRange { get; }

			public TOperand MaxOperand { get; }

			public TOperand MinOperand { get; }

			#endregion

			#region Methods

			private bool Equals(RangeMatchEntry other)
			{
				return EqualityComparer<TOperand>.Default.Equals(MaxOperand, other.MaxOperand) && EqualityComparer<TOperand>.Default.Equals(MinOperand, other.MinOperand);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;

				if (ReferenceEquals(this, obj))
					return true;

				var match = obj as RangeMatchEntry;

				return match != null && Equals(match);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (EqualityComparer<TOperand>.Default.GetHashCode(MaxOperand) * 397) ^ EqualityComparer<TOperand>.Default.GetHashCode(MinOperand);
				}
			}

			public override bool Match(TOperand operand)
			{
				return IntRange.Contains(ConvertOperand(operand));
			}

			public override bool Match(int operand)
			{
				return IntRange.Contains(operand);
			}

			#endregion
		}

		#endregion
	}
}