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
		protected Lexer(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		private ILexerContext Context { get; set; }

		protected TextPoint Position
		{
			get { return Context.Position; }
			set { Context.Position = value; }
		}

		public IServiceProvider ServiceProvider { get; }

		protected TextSpan Text => Context.Text;

		internal void AttachContext(ILexerContext context)
		{
			if (ReferenceEquals(Context, null) == false)
				throw new InvalidOperationException();

			Context = context;
		}

		internal void DetachContext(ILexerContext context)
		{
			if (ReferenceEquals(Context, context) == false)
				throw new InvalidOperationException();

			Context = null;
		}

		internal int GetIntValue(TToken token)
		{
			return (int)EnumConverter<TToken>.Convert(token);
		}

		public LexemeSource<TToken> GetLexemeSource(TextSpan textSourceSpan, LexemeSourceOptions options = default)
		{
			return GetLexemeSourceCore(textSourceSpan, options);
		}

		protected abstract LexemeSource<TToken> GetLexemeSourceCore(TextSpan textSourceSpan, LexemeSourceOptions options);

		protected abstract class LexerContext
		{
			public abstract TextPoint Position { get; }

			public abstract TextSpan Text { get; }

			public abstract TToken Token { get; }
		}

		internal interface ILexerContext
		{
			TextPoint Position { get; set; }

			TextSpan Text { get; }
		}

		public class PredicateEntry
		{
			public PredicateEntry(Func<Lexer<TToken>, bool> predicate)
			{
				Predicate = predicate;
			}

			public Func<Lexer<TToken>, bool> Predicate { get; }
		}

		public class ActionEntry
		{
			public ActionEntry(Action<Lexer<TToken>> action)
			{
				Action = action;
			}

			public Action<Lexer<TToken>> Action { get; }
		}
	}

	internal partial class Lexer<TGrammar, TToken>
		: Lexer<TToken> where TGrammar : Grammar<TGrammar, TToken> where TToken : unmanaged, Enum
	{
		public Lexer(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected override LexemeSource<TToken> GetLexemeSourceCore(TextSpan textSourceSpan, LexemeSourceOptions options)
		{
			return new LexemeSourceImpl(this, textSourceSpan, options);
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

			public LexemeSourceImpl(Lexer<TGrammar, TToken> lexer, TextSpan textSourceSpan, LexemeSourceOptions options) : base(textSourceSpan, options)
			{
				_lexerProcess = new LexerProcess(lexer, this);
			}

			protected override void DisposeCore()
			{
				_lexerProcess.Dispose();
			}

			protected override int ReadCore(ref int position, Lexeme<TToken>[] lexemesBuffer, int[] operandsBuffer, int bufferOffset, int bufferLength)
			{
				return _lexerProcess.RunLexer(ref position, lexemesBuffer, operandsBuffer, bufferOffset, bufferLength, Options.SkipTrivia);
			}
		}

		private class LexerProcess : IDisposable
		{
			private readonly LexerAutomata.LexerProcess _process;

			public LexerProcess(Lexer<TGrammar, TToken> lexer, LexemeSource<TToken> lexemeSource)
			{
				_process = new LexerAutomata.SpanProcess(lexemeSource.TextSourceSpan, lexer);
			}

			public TextSpan TextSourceSpan => _process.TextSourceSpan;

			public int RunLexer(ref int instructionPointer, Lexeme<TToken>[] lexemes, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
			{
				return _process.Run(ref instructionPointer, lexemes, operands, lexemesBufferOffset, lexemesBufferSize, skipLexemes);
			}

			public void Dispose()
			{
				_process.Dispose();
			}
		}

		internal delegate bool LexerPredicate();

		internal delegate bool LexerPredicate<TValue>(out TValue value);

		internal delegate void LexerAction();

		internal delegate void LexerAction<TValue>(out TValue value);
	}
}