// <copyright file="Lexer.Automata.LexerRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerSyntax : Syntax
			{
				public LexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax grammarSyntax) : base(grammarSyntax.Name)
				{
					GrammarSyntax = grammarSyntax;
				}

				public LexerSyntax(Grammar<TGrammar, TToken>.LexerGrammar.Syntax grammarSyntax, Grammar<TGrammar, TToken>.LexerGrammar.Production production) : base(grammarSyntax.Name)
				{
					GrammarSyntax = grammarSyntax;
					Token = production.Token;
					TokenCode = production.TokenCode;
				}

				private Grammar<TGrammar, TToken>.LexerGrammar.Syntax GrammarSyntax { get; }

				public bool IsFragment => GrammarSyntax is Grammar<TGrammar, TToken>.LexerGrammar.FragmentSyntax;

				public bool IsToken => GrammarSyntax is Grammar<TGrammar, TToken>.LexerGrammar.TokenSyntax;

				public bool IsTrivia => GrammarSyntax is Grammar<TGrammar, TToken>.LexerGrammar.TriviaSyntax;

				public TToken Token { get; }

				public int TokenCode { get; }
			}
		}
	}
}