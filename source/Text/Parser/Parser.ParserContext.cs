// <copyright file="Parser.ParserContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal interface IParserAutomataContextInterface
	{
		#region Properties

		int TextPointer { get; set; }

		#endregion
	}

	internal abstract class ParserContext : IDisposable
	{
		#region Ctors

		protected ParserContext(TextSpan textSourceSpan)
		{
			TextSourceSpan = textSourceSpan;
		}

		#endregion

		#region Properties

		internal IParserAutomataContextInterface ParserAutomataContext { get; set; }

		public int TextPointer
		{
			get => ParserAutomataContext.TextPointer;
			set => ParserAutomataContext.TextPointer = value;
		}

		public TextSpan TextSourceSpan { get; }

		#endregion

		#region Methods

		public abstract ParserContext Clone();

		#endregion

		#region Interface Implementations

		#region IDisposable

		public abstract void Dispose();

		#endregion

		#endregion
	}

	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Methods

		protected virtual ParserContext CreateContext(LexemeSource<TToken> lexemeSource)
		{
			return null;
		}

		private protected virtual SyntaxFactory CreateSyntaxFactoryInternal()
		{
			return null;
		}

		#endregion
	}
}