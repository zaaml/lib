// <copyright file="Grammar.Parser.ExternalTokenSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal abstract class ExternalTokenSymbol : ExternalSymbol
			{
			}

			protected internal sealed class ExternalTokenSymbol<TExternalGrammar, TExternalToken> : ExternalTokenSymbol
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
			{
				public ExternalTokenSymbol(Grammar<TExternalGrammar, TExternalToken>.LexerGrammar.TokenSyntax externalToken)
				{
					ExternalToken = externalToken;
					ArgumentName = externalToken.Name;
				}

				public override Type ExternalGrammarType => ExternalToken.GrammarType;

				public Grammar<TExternalGrammar, TExternalToken>.LexerGrammar.TokenSyntax ExternalToken { get; }

				public override Type ExternalTokenType => ExternalToken.TokenType;
			}
		}
	}
}