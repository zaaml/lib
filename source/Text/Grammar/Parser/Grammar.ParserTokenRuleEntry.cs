// <copyright file="Grammar.ParserTokenRuleEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class ParserTokenRuleEntry : ParserPrimitiveEntry
		{
			#region Ctors

			public ParserTokenRuleEntry(TokenRule tokenRule)
			{
				TokenRule = tokenRule;
			}

			#endregion

			#region Properties

			public Type ConverterType { get; internal set; }

			public TokenRule TokenRule { get; }

			#endregion

			#region Methods

			public ParserTokenRuleEntry Bind(string name)
			{
				Name = name;

				return this;
			}

			public ParserTokenRuleEntry Converter<TEntryType>()
			{
				ConverterType = typeof(TEntryType);

				return this;
			}

			public static implicit operator ParserTokenRuleEntry(TokenRule tokenRule)
			{
				return new ParserTokenRuleEntry(tokenRule);
			}

			public ParserTokenRuleEntry Pointer()
			{
				ConverterType = typeof(LexemePointerConverter);

				return this;
			}

			#endregion
		}

		#endregion
	}
}