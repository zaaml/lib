// <copyright file="Grammar.ParserPrimitiveEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract class ParserPrimitiveEntry : ParserEntry
		{
			#region Methods

			public ParserQuantifierEntry AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierHelper.AtLeast(count), mode);
			}

			public ParserQuantifierEntry Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierHelper.Between(from, to), mode);
			}

			public ParserQuantifierEntry Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierHelper.Exact(count), mode);
			}

			public ParserQuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierKind.OneOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierKind.ZeroOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(this, QuantifierKind.ZeroOrOne, mode);
			}

			#endregion
		}

		#endregion
	}
}