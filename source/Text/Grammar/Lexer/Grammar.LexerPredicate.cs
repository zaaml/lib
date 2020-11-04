// <copyright file="Grammar.LexerPredicate.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class LexerPredicate : TokenEntry
		{
			#region Ctors

			public LexerPredicate(Lexer<TToken>.PredicateEntry predicateEntry)
			{
				PredicateEntry = predicateEntry;
			}

			#endregion

			#region Properties

			public Lexer<TToken>.PredicateEntry PredicateEntry { get; }

			#endregion
		}

		#endregion
	}
}