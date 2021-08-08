// <copyright file="Grammar.SubLexerEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal abstract partial class ExternalLexerEntry : ParserPrimitiveEntry
		{
			public abstract Type GrammarType { get; }

			public abstract Type TokenType { get; }
		}

		protected internal sealed class ExternalLexerEntry<TExternalToken> : ExternalLexerEntry where TExternalToken : unmanaged, Enum
		{
			public ExternalLexerEntry(Grammar<TExternalToken>.TokenRule externalLexerRule)
			{
				ExternalLexerRule = externalLexerRule;
				Name = externalLexerRule.EnsureName();
			}

			public override Type GrammarType => ExternalLexerRule.Grammar?.GetType();

			public Grammar<TExternalToken>.TokenRule ExternalLexerRule { get; }

			public override Type TokenType => typeof(TExternalToken);
		}
	}
}