// <copyright file="Grammar.Lexer.Syntax.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			public abstract class Syntax : GrammarSyntax<Syntax, Production, Symbol>
			{
				protected Syntax(string name) : base(name)
				{
				}
			}
		}
	}
}