// <copyright file="Grammar.Lexer.ActionSyntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class ActionSyntax : Syntax
			{
				public ActionSyntax([CallerMemberName] string name = null) : base(name)
				{
				}

				public Lexer<TToken>.ActionEntry ActionEntry { get; private set; }

				public void Bind<TLexer>(Func<TLexer, Lexer<TGrammar, TToken>.LexerAction> actionFactory) where TLexer : Lexer<TGrammar, TToken>
				{
					ActionEntry = new Lexer<TToken>.ActionEntry(l =>
					{
						var action = actionFactory((TLexer)l);

						action();
					});
				}
			}
		}
	}
}