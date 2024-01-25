// <copyright file="LexemeSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal abstract class LexemeSource<TToken> : IDisposable, IEnumerable<Lexeme<TToken>> where TToken : unmanaged, Enum
	{
		protected LexemeSource(TextSpan textSourceSpan, LexemeSourceOptions options)
		{
			TextSourceSpan = textSourceSpan;
			Options = options;
		}

		public LexemeSourceOptions Options { get; }

		public TextSpan TextSourceSpan { get; }

		protected abstract void DisposeCore();

		public LexemeEnumerator<TToken> GetEnumerator()
		{
			return new LexemeEnumerator<TToken>(this);
		}

		public string GetText(Lexeme<TToken> lexeme)
		{
			return TextSourceSpan.GetText(lexeme.Start, lexeme.End - lexeme.Start);
		}

		internal int Read(ref int position, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength)
		{
			return ReadCore(ref position, lexemesBuffer, operandsBuffer, bufferOffset, bufferLength);
		}

		protected abstract int ReadCore(ref int position, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength);

		public Lexeme<TToken>[] ToArray()
		{
			return ToList().ToArray();
		}

		public List<Lexeme<TToken>> ToList()
		{
			return GetEnumerator().Enumerate().ToList();
		}

		public void Dispose()
		{
			DisposeCore();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator<Lexeme<TToken>> IEnumerable<Lexeme<TToken>>.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}