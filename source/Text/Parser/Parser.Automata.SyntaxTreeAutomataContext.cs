// <copyright file="Parser.Automata.SyntaxTreeAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class SyntaxTreeAutomataContext : ValueAutomataContext
			{
				public SyntaxTreeAutomataContext(ParserRule rule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata)
					: base(rule, lexemeSource, parserContext, processKind, parser, parserAutomata)
				{
				}
			}
		}
	}
}