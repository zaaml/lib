// <copyright file="Grammar.Lexer.TerminalSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal abstract class TerminalSymbol : PrimitiveSymbol
			{
			}
		}
	}
}