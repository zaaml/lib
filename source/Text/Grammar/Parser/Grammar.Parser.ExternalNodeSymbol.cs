// <copyright file="Grammar.Parser.ExternalNodeSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal abstract class ExternalNodeSymbol : ExternalSymbol
			{
				public abstract Type ExternalNodeType { get; }
			}

			protected internal sealed class ExternalNodeSymbol<TExternalGrammar, TExternalToken> : ExternalNodeSymbol
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
			{
				public ExternalNodeSymbol(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax externalParserNode)
				{
					ExternalParserNode = externalParserNode;
					ArgumentName = externalParserNode.EnsureName();
				}

				public override Type ExternalGrammarType => ExternalParserNode.GrammarType;

				public Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax ExternalParserNode { get; }

				public override Type ExternalTokenType => ExternalParserNode.TokenType;

				public override Type ExternalNodeType => null;
			}

			protected internal sealed class ExternalNodeSymbol<TExternalGrammar, TExternalToken, TExternalNode> : ExternalNodeSymbol
				where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
				where TExternalToken : unmanaged, Enum
				where TExternalNode : class
			{
				public ExternalNodeSymbol(Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> externalParserNode)
				{
					ExternalParserNode = externalParserNode;
					ArgumentName = externalParserNode.EnsureName();
				}

				public override Type ExternalNodeType => typeof(TExternalNode);

				public override Type ExternalGrammarType => ExternalParserNode.GrammarType;

				public Grammar<TExternalGrammar, TExternalToken>.ParserGrammar.NodeSyntax<TExternalNode> ExternalParserNode { get; }

				public override Type ExternalTokenType => ExternalParserNode.TokenType;
			}
		}
	}
}