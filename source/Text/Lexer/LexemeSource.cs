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
		protected LexemeSource(TextSource textSource, IServiceProvider serviceProvider)
		{
			TextSource = textSource;
			ServiceProvider = serviceProvider;
		}

		internal abstract int Position { get; set; }

		public TextSource TextSource { get; }

		public IServiceProvider ServiceProvider { get; }

		protected abstract void DisposeCore();

		public LexemeEnumerator<TToken> GetEnumerator()
		{
			return new LexemeEnumerator<TToken>(this);
		}

		public string GetText(Lexeme<TToken> lexeme)
		{
			return TextSource.GetText(lexeme.Start, lexeme.End);
		}

		internal int Read(Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength, bool skipLexemes)
		{
			return ReadCore(lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, skipLexemes);
		}

		protected abstract int ReadCore(Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength, bool skipLexemes);

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