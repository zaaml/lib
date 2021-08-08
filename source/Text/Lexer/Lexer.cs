// <copyright file="Lexer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using Zaaml.Core.Converters;

namespace Zaaml.Text
{
	internal abstract class Lexer<TToken> where TToken : unmanaged, Enum
	{
		internal int GetIntValue(TToken token)
		{
			return (int)EnumConverter<TToken>.Convert(token);
		}

		public LexemeSource<TToken> GetLexemeSource(string text, IServiceProvider serviceProvider = null)
		{
			return GetLexemeSourceCore(new StringTextSource(text).GetTextSpan(), serviceProvider);
		}

		public LexemeSource<TToken> GetLexemeSource(TextSpan textSourceSpan, IServiceProvider serviceProvider = null)
		{
			return GetLexemeSourceCore(textSourceSpan, serviceProvider);
		}

		protected abstract LexemeSource<TToken> GetLexemeSourceCore(TextSpan textSourceSpan, IServiceProvider serviceProvider);

		public class PredicateEntry
		{
			public PredicateEntry(Func<LexerContext<TToken>, bool> predicate)
			{
				Predicate = predicate;
			}

			public Func<LexerContext<TToken>, bool> Predicate { get; }

			public static implicit operator Grammar<TToken>.PatternCollection(PredicateEntry parserEntry)
			{
				var patternCollection = new Grammar<TToken>.PatternCollection();

				patternCollection.Patterns.Add(new Grammar<TToken>.TokenPattern(new Grammar<TToken>.TokenEntry[] { parserEntry }));

				return patternCollection;
			}
		}

		public class ActionEntry
		{
			public ActionEntry(Action<LexerContext<TToken>> action)
			{
				Action = action;
			}

			public Action<LexerContext<TToken>> Action { get; }
		}
	}

	internal partial class Lexer<TGrammar, TToken> : Lexer<TToken> where TGrammar : Grammar<TToken> where TToken : unmanaged, Enum
	{
		protected virtual LexerContext<TToken> CreateContext(LexemeSource<TToken> lexemeSource)
		{
			return null;
		}

		protected override LexemeSource<TToken> GetLexemeSourceCore(TextSpan textSourceSpan, IServiceProvider serviceProvider)
		{
			return new LexemeSourceImpl(this, textSourceSpan, serviceProvider);
		}

		internal static void RentLexemeBuffers(int bufferLength, out Lexeme<TToken>[] lexemesBuffer, out int[] operandsBuffer)
		{
			lexemesBuffer = ArrayPool<Lexeme<TToken>>.Shared.Rent(bufferLength);
			operandsBuffer = ArrayPool<int>.Shared.Rent(bufferLength);
		}

		internal static void ReturnLexemeBuffers(Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer)
		{
			ArrayPool<Lexeme<TToken>>.Shared.Return(lexemesBuffer, true);
			ArrayPool<int>.Shared.Return(operandsBuffer, true);
		}

		private sealed class LexemeSourceImpl : LexemeSource<TToken>
		{
			private readonly LexerProcess _lexerProcess;

			public LexemeSourceImpl(Lexer<TGrammar, TToken> lexer, TextSpan textSourceSpan, IServiceProvider serviceProvider) : base(textSourceSpan, serviceProvider)
			{
				_lexerProcess = new LexerProcess(lexer, this);
			}

			internal override int Position
			{
				get => _lexerProcess.TextPointer;
				set => _lexerProcess.TextPointer = value;
			}

			protected override void DisposeCore()
			{
				_lexerProcess.Dispose();
			}

			protected override int ReadCore(Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength, bool skipLexemes)
			{
				return _lexerProcess.RunLexer(lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, skipLexemes);
			}
		}

		private class LexerProcess : IDisposable
		{
			private readonly LexerAutomata.LexerProcess _process;

			public LexerProcess(Lexer<TGrammar, TToken> lexer, LexemeSource<TToken> lexemeSource)
			{
				var textSourceSpan = lexemeSource.TextSourceSpan;
				var lexerContext = lexer.CreateContext(lexemeSource);

				_process = new LexerAutomata.SpanProcess(textSourceSpan, lexer, lexerContext);

				//if (textSourceSpan.TextSource is StringTextSource stringTextSource)
				//	_process = new LexerAutomata.SpanProcess(textSourceSpan, lexer, lexerContext);
				//else
				//	_process = new LexerAutomata.ReaderProcess(textSource, lexer, lexerContext);
			}

			public int TextPointer
			{
				get => _process.TextPointer;
				set => _process.TextPointer = value;
			}

			public TextSpan TextSourceSpan => _process.TextSourceSpan;

			public int RunLexer(Lexeme<TToken>[] lexemes, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
			{
				return _process.Run(lexemes, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
			}

			public void Dispose()
			{
				_process.Dispose();
			}
		}
	}
}