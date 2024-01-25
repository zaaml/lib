// <copyright file="Grammar.Lexer.CharSetSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class LexerGrammar
		{
			protected internal sealed class CharSetSymbol : PrimitiveMatchSymbol
			{
				public CharSetSymbol(IEnumerable<PrimitiveMatchSymbol> matches)
				{
					Matches = new ReadOnlyCollection<PrimitiveMatchSymbol>(matches.ToList());
				}

				public IReadOnlyCollection<PrimitiveMatchSymbol> Matches { get; }
			}
		}
	}
}