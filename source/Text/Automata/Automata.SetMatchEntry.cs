// <copyright file="Automata.SetMatchEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

// ReSharper disable ForCanBeConvertedToForeach

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected class SetMatchEntry : PrimitiveMatchEntry
		{
			public SetMatchEntry(IEnumerable<PrimitiveMatchEntry> matches)
			{
				Matches = matches.ToArray();
			}

			protected override string DebuggerDisplay
			{
				get
				{
					var setStr = string.Join(",", Matches.Select(m => m.ToString()));

					return $"{{{setStr}}}";
				}
			}

			public static IEqualityComparer<SetMatchEntry> EqualityComparer => SetMatchEntryEqualityComparer.Instance;

			public PrimitiveMatchEntry[] Matches { get; }

			public override bool Match(TOperand operand)
			{
				for (var index = 0; index < Matches.Length; index++)
				{
					var match = Matches[index];

					if (match.Match(operand))
						return true;
				}

				return false;
			}

			public override bool Match(int operand)
			{
				for (var index = 0; index < Matches.Length; index++)
				{
					var match = Matches[index];

					if (match.Match(operand))
						return true;
				}

				return false;
			}

			private sealed class SetMatchEntryEqualityComparer : IEqualityComparer<SetMatchEntry>
			{
				public static readonly SetMatchEntryEqualityComparer Instance = new();

				private SetMatchEntryEqualityComparer()
				{
				}

				public bool Equals(SetMatchEntry x, SetMatchEntry y)
				{
					if (ReferenceEquals(x, y)) return true;
					if (ReferenceEquals(x, null)) return false;
					if (ReferenceEquals(y, null)) return false;
					if (x.GetType() != y.GetType()) return false;

					if (x.Matches.Length != y.Matches.Length)
						return false;

					var equalityComparer = EntryEqualityComparer.Instance;

					for (var i = 0; i < x.Matches.Length; i++)
					{
						var xi = x.Matches[i];
						var yi = y.Matches[i];

						if (equalityComparer.Equals(xi, yi) == false)
							return false;
					}

					return true;
				}

				public int GetHashCode(SetMatchEntry obj)
				{
					unchecked
					{
						var hashCode = 0;
						var equalityComparer = EntryEqualityComparer.Instance;

						for (var i = 0; i < obj.Matches.Length; i++)
							hashCode = (hashCode * 397) ^ equalityComparer.GetHashCode(obj.Matches[i]);

						return hashCode;
					}
				}
			}
		}
	}
}