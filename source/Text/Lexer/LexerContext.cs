// <copyright file="LexerContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal abstract class LexerContext<TToken> : IDisposable where TToken : unmanaged, Enum
	{
		#region Ctors

		protected LexerContext(LexemeSource<TToken> lexemeSource)
		{
			LexemeSource = lexemeSource;
		}

		#endregion

		#region Properties

		internal ILexerAutomataContextInterface LexerAutomataContext { get; set; }

		public int TextPointer { get; internal set; }

		public LexemeSource<TToken> LexemeSource { get; }

		#endregion

		#region Methods

		public abstract LexerContext<TToken> Clone();

		#endregion

		#region Interface Implementations

		#region IDisposable

		public abstract void Dispose();

		#endregion

		#endregion
	}
}