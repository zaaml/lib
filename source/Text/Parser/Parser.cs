// <copyright file="Parser.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Threading;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TToken> : ParserBase where TToken : unmanaged, Enum
	{
		protected Parser(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
		}

		public IServiceProvider ServiceProvider { get; }
	}

	internal partial class Parser<TGrammar, TToken> : Parser<TToken>
		where TGrammar : Grammar<TGrammar, TToken>
		where TToken : unmanaged, Enum
	{
		private static ParserAutomata _automataStatic;

		public Parser(IServiceProvider serviceProvider) : base(serviceProvider)
		{
		}

		protected virtual bool AllowParallel => false;

		private ParserAutomata Automata => AutomataStatic;

		private static ParserAutomata AutomataStatic => _automataStatic ??= AutomataManager.Get<ParserAutomata>();

		protected virtual void BuildNode(NodeContext nodeContext)
		{
		}

		internal void BuildNodeInternal(object node, TextSpan textSpan)
		{
			BuildNode(new NodeContext(node, textSpan));
		}

		internal static void BuildFullDfa()
		{
			AutomataStatic.BuildFullDfa();
		}

		protected TResult ParseCore<TResult, TNode>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default)
			where TNode : class
		{
			return ParseInternal(visitor, syntax, lexemeSource, cancellationToken);
		}

		protected virtual TNode ParseCore<TNode>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default) where TNode : class
		{
			return ParseInternal<TNode>(syntax, lexemeSource, cancellationToken);
		}

		private ExternalParseResult<TNode> ParseExternal<TNode>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default) where TNode : class
		{
			return Automata.ParseExternal(syntax, lexemeSource, this, cancellationToken);
		}

		private protected TResult ParseInternal<TResult>(Visitor<TResult> visitor, Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default)
		{
			return Automata.Parse(visitor, syntax, lexemeSource, this, cancellationToken);
		}

		private protected TResult ParseInternal<TResult>(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default)
		{
			return Automata.Parse<TResult>(syntax, lexemeSource, this, cancellationToken);
		}

		private protected void ParseInternal(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax syntax, LexemeSource<TToken> lexemeSource, CancellationToken cancellationToken = default)
		{
			Automata.Parse(syntax, lexemeSource, this, cancellationToken);
		}

		public readonly ref struct SyntaxNodeParser<TNode> where TNode : class
		{
			public Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> Syntax { get; }

			private Func<IServiceProvider, Lexer<TGrammar, TToken>> LexerFactory { get; }

			private Func<IServiceProvider, Parser<TGrammar, TToken>> ParserFactory { get; }

			public SyntaxNodeParser(Grammar<TGrammar, TToken>.ParserGrammar.NodeSyntax<TNode> syntax, Func<IServiceProvider, Lexer<TGrammar, TToken>> lexerFactory, Func<IServiceProvider, Parser<TGrammar, TToken>> parserFactory)
			{
				Syntax = syntax;
				LexerFactory = lexerFactory;
				ParserFactory = parserFactory;
			}

			public TNode Parse(string nodeString, IServiceProvider serviceProvider = null, CancellationToken cancellationToken = default)
			{
				return Parse(new TextSpan(nodeString), serviceProvider, cancellationToken);
			}

			public TNode Parse(TextSpan text, IServiceProvider serviceProvider = null, CancellationToken cancellationToken = default)
			{
				using var lexemeStream = LexerFactory(serviceProvider).GetLexemeSource(text, new LexemeSourceOptions(true));

				return ParserFactory(serviceProvider).ParseCore(Syntax, lexemeStream, cancellationToken);
			}

			internal ExternalParseResult<TNode> ParseExternal(TextSpan text, IServiceProvider serviceProvider = null, CancellationToken cancellationToken = default)
			{
				using var lexemeStream = LexerFactory(serviceProvider).GetLexemeSource(text, new LexemeSourceOptions(true));

				return ParserFactory(serviceProvider).ParseExternal(Syntax, lexemeStream, cancellationToken);
			}
		}

		internal abstract class ExternalParseResult<TNode>
		{
		}

		internal sealed class SuccessExternalParseResult<TNode> : ExternalParseResult<TNode>
		{
			public SuccessExternalParseResult(TNode value, int textPosition)
			{
				Value = value;
				TextPosition = textPosition;
			}

			public int TextPosition { get; }
			public TNode Value { get; }
		}

		internal sealed class ExceptionExternalParseResult<TNode> : ExternalParseResult<TNode>
		{
			public ExceptionExternalParseResult(Exception exception)
			{
				Exception = exception;
			}

			public Exception Exception { get; }
		}

		internal class ForkExternalParseResult<TNode> : ExternalParseResult<TNode>
		{
		}

		internal delegate bool ParserPredicate();

		internal delegate bool ParserPredicate<TValue>(out TValue value);

		internal delegate void ParserAction();

		internal delegate void ParserAction<TValue>(out TValue value);
	}
}