// <copyright file="Grammar.Lexer.QuantifierSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class QuantifierSymbol : Symbol
			{
				public QuantifierSymbol(PrimitiveSymbol symbol, QuantifierKind kind, QuantifierMode mode)
				{
					Symbol = symbol;
					Kind = kind;
					Mode = mode;
					Range = QuantifierHelper.GetRange(kind);
				}

				public QuantifierSymbol(PrimitiveSymbol symbol, Interval<int> range, QuantifierMode mode)
				{
					Symbol = symbol;
					Kind = QuantifierHelper.GetKind(range);
					Range = range;
					Mode = mode;
				}

				public QuantifierKind Kind { get; }

				public QuantifierMode Mode { get; }

				public Interval<int> Range { get; }

				public PrimitiveSymbol Symbol { get; }
			}
		}
	}
}