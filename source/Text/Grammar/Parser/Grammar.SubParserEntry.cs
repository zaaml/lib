// <copyright file="Grammar.SubParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal abstract partial class ExternalParserEntry : ParserPrimitiveEntry
		{
			internal abstract Type GrammarType { get; }
		}

		protected internal sealed class ExternalParserEntry<TExternalToken> : ExternalParserEntry where TExternalToken : unmanaged, Enum
		{
			public ExternalParserEntry(Grammar<TExternalToken>.ParserRule externalParserRule)
			{
				ExternalParserRule = externalParserRule;
			}

			internal override Type GrammarType => ExternalParserRule.Grammar.GetType();

			public Grammar<TExternalToken>.ParserRule ExternalParserRule { get; }
		}

		protected internal sealed class ExternalParserEntry<TExternalToken, TExternalNode, TExternalNodeBase> : ExternalParserEntry where TExternalToken : unmanaged, Enum where TExternalNode : TExternalNodeBase where TExternalNodeBase : class
		{
			public ExternalParserEntry(Grammar<TExternalToken, TExternalNodeBase>.ParserRule<TExternalNode> externalParserRule)
			{
				ExternalParserRule = externalParserRule;
				Name = externalParserRule.EnsureName();
			}

			internal override Type GrammarType => ExternalParserRule.Grammar.GetType();

			public Grammar<TExternalToken, TExternalNodeBase>.ParserRule<TExternalNode> ExternalParserRule { get; }
		}
	}
}