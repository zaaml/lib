// <copyright file="Grammar.Lexer.ActionSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class ActionSymbol : Symbol
			{
				public ActionSymbol(ActionSyntax actionSyntax)
				{
					ActionSyntax = actionSyntax;
				}

				public Lexer<TToken>.ActionEntry Action => ActionSyntax.ActionEntry;

				public ActionSyntax ActionSyntax { get; }
			}
		}
	}
}