// <copyright file="Lexer.Automata.LexerState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerState : FiniteState
			{
				public readonly Grammar<TToken>.TokenRule Rule;

				public LexerState(Grammar<TToken>.TokenRule tokenRule) : base(tokenRule.Name)
				{
					Rule = tokenRule;
				}

				public LexerState(string name) : base(name)
				{
				}
			}
		}
	}
}