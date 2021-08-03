// <copyright file="Lexer.Automata.LexerState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerRule : Rule
			{
				public readonly Grammar<TToken>.TokenRule TokenRule;

				public LexerRule(Grammar<TToken>.TokenRule tokenRule) : base(tokenRule.Name)
				{
					TokenRule = tokenRule;
				}

				public LexerRule(string name) : base(name)
				{
				}
			}
		}
	}
}