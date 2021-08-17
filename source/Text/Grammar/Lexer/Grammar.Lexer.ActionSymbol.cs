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
				public ActionSymbol(Lexer<TToken>.ActionEntry action)
				{
					Action = action;
				}

				public Lexer<TToken>.ActionEntry Action { get; }
			}
		}
	}
}