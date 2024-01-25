// <copyright file="Parser.ParserContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal interface IParserAutomataContextInterface
	{
		int TextPosition { get; set; }
	}

	internal partial class Parser<TGrammar, TToken>
	{
		private protected virtual SyntaxNodeFactory CreateSyntaxFactoryInternal()
		{
			return null;
		}
	}
}