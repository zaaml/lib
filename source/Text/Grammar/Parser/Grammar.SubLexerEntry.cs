// <copyright file="Grammar.SubLexerEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract partial class SubLexerEntry : ParserPrimitiveEntry
		{
			#region Properties

			public abstract Type GrammarType { get; }

			public abstract Type TokenType { get; }

			#endregion
		}

		protected internal sealed class SubLexerEntry<TSubToken> : SubLexerEntry where TSubToken : unmanaged, Enum
		{
			#region Ctors

			public SubLexerEntry(Grammar<TSubToken>.TokenRule subLexerRule)
			{
				SubLexerRule = subLexerRule;
				Name = subLexerRule.EnsureName();
			}

			#endregion

			#region Properties

			public override Type GrammarType => SubLexerRule.Grammar?.GetType();

			public Grammar<TSubToken>.TokenRule SubLexerRule { get; }

			public override Type TokenType => typeof(TSubToken);

			#endregion
		}

		#endregion
	}
}