// <copyright file="LexerContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract class LexerContext<TToken> : IDisposable where TToken : unmanaged, Enum
	{
		protected LexerContext(LexemeSource<TToken> lexemeSource)
		{
			LexemeSource = lexemeSource;
		}

		public LexemeSource<TToken> LexemeSource { get; }

		internal ILexerAutomataContextInterface LexerAutomataContext { get; set; }

		public int TextPointer { get; internal set; }

		public abstract LexerContext<TToken> Clone();

		public abstract void Dispose();
	}
}