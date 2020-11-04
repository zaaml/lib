// <copyright file="Grammar.TokenPrimitiveEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class TokenPrimitiveEntry : TokenEntry
		{
			#region Methods

			public QuantifierEntry AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierHelper.AtLeast(count), mode);
			}

			public QuantifierEntry Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierHelper.Between(from, to), mode);
			}

			public QuantifierEntry Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierHelper.Exact(count), mode);
			}

			public QuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.OneOrMore, mode);
			}

			public QuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.ZeroOrMore, mode);
			}

			public QuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(this, QuantifierKind.ZeroOrOne, mode);
			}

			#endregion
		}

		#endregion
	}
}