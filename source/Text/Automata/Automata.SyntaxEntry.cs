// <copyright file="Automata.RuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class SyntaxEntry : PrimitiveEntry
		{
			public SyntaxEntry(Syntax syntax)
			{
				Syntax = syntax;
			}

			protected override string DebuggerDisplay => Syntax.Name;

			public static IEqualityComparer<SyntaxEntry> EqualityComparer => SyntaxEntryEqualityComparer.Instance;

			public Syntax Syntax { get; }

			private sealed class SyntaxEntryEqualityComparer : IEqualityComparer<SyntaxEntry>
			{
				public static readonly SyntaxEntryEqualityComparer Instance = new();

				private SyntaxEntryEqualityComparer()
				{
				}

				public bool Equals(SyntaxEntry x, SyntaxEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					return ReferenceEquals(x.Syntax, y.Syntax);
				}

				public int GetHashCode(SyntaxEntry obj)
				{
					return obj.Syntax.GetHashCode();
				}
			}
		}

		protected abstract class RuleEntryContext
		{
		}
	}
}