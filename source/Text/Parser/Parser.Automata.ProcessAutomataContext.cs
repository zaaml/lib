// <copyright file="Parser.Automata.ProcessAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ProcessAutomataContext : ParserAutomataContext
			{
				public ProcessAutomataContext(ParserRule rule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata)
					: base(rule, lexemeSource, parserContext, processKind, parser, parserAutomata)
				{
				}
			}
		}
	}
}