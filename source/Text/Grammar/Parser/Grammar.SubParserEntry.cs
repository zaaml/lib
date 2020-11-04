// <copyright file="Grammar.SubParserEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal abstract partial class SubParserEntry : ParserPrimitiveEntry
		{
			#region Properties

			internal abstract Type GrammarType { get; }

			#endregion
		}

		protected internal sealed class SubParserEntry<TSubToken> : SubParserEntry where TSubToken : unmanaged, Enum
		{
			#region Ctors

			public SubParserEntry(Grammar<TSubToken>.ParserRule subParserRule)
			{
				SubParserRule = subParserRule;
			}

			#endregion

			#region Properties

			internal override Type GrammarType => SubParserRule.Grammar.GetType();

			public Grammar<TSubToken>.ParserRule SubParserRule { get; }

			#endregion
		}

		protected internal sealed class SubParserEntry<TSubToken, TSubNode, TSubNodeBase> : SubParserEntry where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
		{
			#region Ctors

			public SubParserEntry(Grammar<TSubToken, TSubNodeBase>.ParserRule<TSubNode> subParserRule)
			{
				SubParserRule = subParserRule;
				Name = subParserRule.EnsureName();
			}

			#endregion

			#region Properties

			internal override Type GrammarType => SubParserRule.Grammar.GetType();

			public Grammar<TSubToken, TSubNodeBase>.ParserRule<TSubNode> SubParserRule { get; }

			#endregion
		}

		#endregion
	}
}