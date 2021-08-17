// <copyright file="Automata.RuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class RuleEntry : PrimitiveEntry
		{
			public RuleEntry(Rule rule)
			{
				Rule = rule;
			}

			protected override string DebuggerDisplay => Rule.Name;

			public static IEqualityComparer<RuleEntry> EqualityComparer => RuleEntryEqualityComparer.Instance;

			public Rule Rule { get; }

			private sealed class RuleEntryEqualityComparer : IEqualityComparer<RuleEntry>
			{
				public static readonly RuleEntryEqualityComparer Instance = new();

				private RuleEntryEqualityComparer()
				{
				}

				public bool Equals(RuleEntry x, RuleEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					return ReferenceEquals(x.Rule, y.Rule);
				}

				public int GetHashCode(RuleEntry obj)
				{
					return obj.Rule.GetHashCode();
				}
			}
		}

		protected abstract class RuleEntryContext
		{
		}
	}
}