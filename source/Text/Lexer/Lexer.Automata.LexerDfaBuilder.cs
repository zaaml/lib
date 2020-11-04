// <copyright file="Lexer.Automata.LexerDfaBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerDfaBuilder : DfaBuilder<LexerDfaState>
			{
				public LexerDfaBuilder(IEnumerable<LexerState> states, Automata<char, int> automata) : base(states, automata)
				{
				}

				protected override LexerDfaState CreateDfaState(DfaNode[] nodes, DfaTransition[] lazyTransitions, DfaTransition successTransition, DfaTransition prevSuccessTransition, int hashCode)
				{
					return new LexerDfaState(nodes, lazyTransitions, successTransition, prevSuccessTransition, hashCode, this);
				}
			}
		}
	}
}