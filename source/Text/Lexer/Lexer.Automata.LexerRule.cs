// <copyright file="Lexer.Automata.LexerRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
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
				public LexerRule(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax) : base(syntax.Name)
				{
					Syntax = syntax;
				}

				public LexerRule(Grammar<TGrammar, TToken>.LexerGrammar.Syntax syntax, Grammar<TGrammar, TToken>.LexerGrammar.Production production) : base(syntax.Name)
				{
					Syntax = syntax;
					Token = production.Token;
					TokenCode = production.TokenCode;
				}

				public bool IsTrivia => Syntax is Grammar<TGrammar, TToken>.LexerGrammar.TriviaSyntax;
				
				public bool IsToken => Syntax is Grammar<TGrammar, TToken>.LexerGrammar.TokenSyntax;

				public bool IsFragment => Syntax is Grammar<TGrammar, TToken>.LexerGrammar.FragmentSyntax;
				
				private Grammar<TGrammar, TToken>.LexerGrammar.Syntax Syntax { get; }

				public TToken Token { get; }

				public int TokenCode { get; }
			}
		}
	}
}