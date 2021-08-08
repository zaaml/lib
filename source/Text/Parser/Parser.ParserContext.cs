// <copyright file="Parser.ParserContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal interface IParserAutomataContextInterface
	{
		int TextPosition { get; set; }
	}

	internal abstract class ParserContext : IDisposable
	{
		protected ParserContext(TextSpan textSourceSpan)
		{
			TextSourceSpan = textSourceSpan;
		}

		internal IParserAutomataContextInterface ParserAutomataContext { get; set; }

		public int TextPointer
		{
			get => ParserAutomataContext.TextPosition;
			set => ParserAutomataContext.TextPosition = value;
		}

		public TextSpan TextSourceSpan { get; }

		public abstract ParserContext Clone();

		public abstract void Dispose();
	}

	internal abstract partial class Parser<TGrammar, TToken>
	{
		protected virtual ParserContext CreateContext(LexemeSource<TToken> lexemeSource)
		{
			return null;
		}

		private protected virtual SyntaxFactory CreateSyntaxFactoryInternal()
		{
			return null;
		}
	}
}