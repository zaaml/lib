// <copyright file="Parser.Automata.VisitorAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class VisitorAutomataContext : ValueAutomataContext
			{
				public VisitorAutomataContext(Visitor visitor, ParserSyntax rule, LexemeSource<TToken> lexemeSource, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata)
					: base(rule, lexemeSource, processKind, parser, parserAutomata)
				{
					Visitor = visitor;
				}

				public Visitor Visitor { get; }
			}
		}
	}
}