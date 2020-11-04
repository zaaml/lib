// <copyright file="Automata.SingleMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Globalization;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected class SingleMatchEntry : PrimitiveMatchEntry
		{
			#region Ctors

			public SingleMatchEntry(TOperand operand)
			{
				Operand = operand;
				IntOperand = ConvertOperand(operand);
			}

			#endregion

			#region Properties

			protected override string DebuggerDisplay => $"{Operand.ToString(CultureInfo.InvariantCulture)}";

			public int IntOperand { get; }

			public TOperand Operand { get; }

			#endregion

			#region Methods

			private bool Equals(SingleMatchEntry other)
			{
				return EqualityComparer<TOperand>.Default.Equals(Operand, other.Operand);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;

				if (ReferenceEquals(this, obj))
					return true;

				return obj is SingleMatchEntry match && Equals(match);
			}

			public override int GetHashCode()
			{
				return EqualityComparer<TOperand>.Default.GetHashCode(Operand);
			}

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

			#endregion
		}

		#endregion
	}
}