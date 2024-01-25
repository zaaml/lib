// <copyright file="Parser.Automata.CompositeOperandEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class CompositeOperandEntry : ValueEntry, IParserEntry
			{
				public CompositeOperandEntry(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenSymbol)
				{
					TokenSymbol = tokenSymbol;

					var production = tokenSymbol.Token.Productions[0];

					TokenCode = production.TokenCode;
					SimpleTokenCount = production.Symbols.Count;
				}

				public int TokenCode { get; }

				public int SimpleTokenCount { get; }

				protected override string DebuggerDisplay => $"{TokenSymbol.Token}";

				public Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol TokenSymbol { get; }

				public Grammar<TGrammar, TToken>.ParserGrammar.Symbol GrammarSymbol => TokenSymbol;

				public ProductionArgument ProductionArgument { get; set; }

				public IParserEntry Source { get; private set; }

				public Entry Clone()
				{
					return new CompositeOperandEntry(TokenSymbol)
					{
						Source = this
					};
				}
			}
		}
	}
}