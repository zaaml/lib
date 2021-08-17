// <copyright file="Grammar.Lexer.CharSetSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class CharSetSymbol : MatchSymbol
			{
				public CharSetSymbol(IEnumerable<PrimitiveMatchSymbol> matches)
				{
					Matches = matches;
				}

				public IEnumerable<PrimitiveMatchSymbol> Matches { get; }
			}
		}
	}
}