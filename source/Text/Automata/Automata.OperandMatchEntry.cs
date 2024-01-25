// <copyright file="Automata.SingleMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Globalization;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class OperandMatchEntry : PrimitiveMatchEntry
		{
			public OperandMatchEntry(TOperand operand)
			{
				Operand = operand;
				IntOperand = ConvertFromOperand(operand);
			}

			protected override string DebuggerDisplay => $"{Operand.ToString(CultureInfo.InvariantCulture)}";

			public int IntOperand { get; }

			public TOperand Operand { get; }

			public override bool Match(TOperand operand)
			{
				return Equals(operand, Operand);
			}

			public override bool Match(int operand)
			{
				return IntOperand == operand;
			}

			public override string ToString()
			{
				return DebuggerDisplay;
			}

			public static IEqualityComparer<OperandMatchEntry> EqualityComparer => SingleMatchEntryEqualityComparer.Instance;

			private sealed class SingleMatchEntryEqualityComparer : IEqualityComparer<OperandMatchEntry>
			{
				public static readonly SingleMatchEntryEqualityComparer Instance = new();

				private SingleMatchEntryEqualityComparer()
				{
				}

				public bool Equals(OperandMatchEntry x, OperandMatchEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					return EqualityComparer<TOperand>.Default.Equals(x.Operand, y.Operand);
				}

				public int GetHashCode(OperandMatchEntry obj)
				{
					return EqualityComparer<TOperand>.Default.GetHashCode(obj.Operand);
				}
			}
		}
	}
}