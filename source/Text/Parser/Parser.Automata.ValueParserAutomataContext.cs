// <copyright file="Parser.Automata.ValueParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			static ParserAutomata()
			{
				StaticLexemes = new string[InstructionsRange.Maximum + 1];
			}

			private abstract class ValueAutomataContext : ParserAutomataContext
			{
				protected ValueAutomataContext(ParserSyntax rule, LexemeSource<TToken> lexemeSource, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata)
					: base(rule, lexemeSource, processKind, parser, parserAutomata)
				{
				}

				public TResult GetResult<TResult>()
				{
					return ((ParserProcess)Process).GetResult<TResult>();
				}
			}
		}
	}
}