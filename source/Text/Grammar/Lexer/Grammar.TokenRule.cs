// <copyright file="Grammar.TokenRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class TokenRule : ParserPrimitiveEntry
		{
			#region Fields

			public readonly TToken Token;

			internal int TokenCode;

			#endregion

			#region Ctors

			internal TokenRule(TToken token)
			{
				Token = token;
				GrammarType = GetGrammarType();
			}

			#endregion

			#region Properties

			public Grammar<TToken> Grammar => Get<TToken>(GrammarType);

			private Type GrammarType { get; }

			public PatternCollection Pattern { get; set; }

			public bool Skip { get; set; }

			#endregion

			#region Methods

			public ParserTokenRuleEntry Bind(string name)
			{
				return new ParserTokenRuleEntry(this)
				{
					Name = name
				};
			}

			public ParserTokenRuleEntry Converter<TEntryType>()
			{
				return new ParserTokenRuleEntry(this)
				{
					ConverterType = typeof(TEntryType)
				};
			}

			internal string EnsureName()
			{
				return Name;
			}

			public static TokenInterProductionBuilder operator +(TokenRule op1, TokenRule op2)
			{
				return new TokenInterProductionBuilder() + op1 + op2;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenRule op1, TokenRule op2)
			{
				return new TokenInterProductionCollectionBuilder() | op1 | op2;
			}

			public ParserTokenRuleEntry Pointer()
			{
				return new ParserTokenRuleEntry(this)
				{
					ConverterType = typeof(LexemePointerConverter)
				};
			}

			#endregion
		}

		#endregion
	}
}