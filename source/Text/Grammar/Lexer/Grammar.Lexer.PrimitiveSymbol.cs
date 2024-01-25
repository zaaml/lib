// <copyright file="Grammar.Lexer.PrimitiveSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal abstract class PrimitiveSymbol : Symbol
			{
				public QuantifierSymbol AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierHelper.AtLeast(count), mode);
				}

				public QuantifierSymbol Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierHelper.Between(from, to), mode);
				}

				public QuantifierSymbol Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierHelper.Exact(count), mode);
				}

				public QuantifierSymbol OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierKind.OneOrMore, mode);
				}

				public QuantifierSymbol ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierKind.ZeroOrMore, mode);
				}

				public QuantifierSymbol ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(this, QuantifierKind.ZeroOrOne, mode);
				}
			}
		}
	}
}