// <copyright file="Parser.Automata.ParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Threading;
using Zaaml.Core;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class ParserAutomataContextState : AutomataContextState
			{
				public ParserContext ParserContext;
			}

			private abstract partial class ParserAutomataContext : AutomataContext, IParserAutomataContextInterface, IDisposable
			{
				private readonly ThreadLocal<Dictionary<Type, ExternalParserResources>> _threadLocalExternalParserResourcesDictionary = new(() => new Dictionary<Type, ExternalParserResources>());

				private Dictionary<Type, ExternalParserResources> _externalParserResourcesDictionary;

				protected ParserAutomataContext(ParserRule rule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata) : base(rule, parserAutomata)
				{
					_externalParserResourcesDictionary = _threadLocalExternalParserResourcesDictionary.Value;

					LexemeSource = lexemeSource;
					TextSourceSpan = lexemeSource.TextSourceSpan;
					ParserContext = parserContext;
					ProcessKind = processKind;
					Parser = parser;

					if (ParserContext != null)
						ParserContext.ParserAutomataContext = this;

					Process = new ParserProcess(this);
				}

				public ParserAutomata ParserAutomata => (ParserAutomata)Automata;

				public LexemeSource<TToken> LexemeSource { get; }

				public Parser<TGrammar, TToken> Parser { get; }

				public ParserContext ParserContext { get; }

				public override Process Process { get; }

				public override ProcessKind ProcessKind { get; }

				private int TextPosition => Process.InstructionStreamPosition;

				public TextSpan TextSourceSpan { get; }

				public bool CallExternalLexer<TExternalGrammar, TExternalToken>(ExternalLexerInvokeInfo<TExternalGrammar, TExternalToken> invokeInfo, out Lexeme<TExternalToken> result)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum 
				{
					var offset = TextPosition;
					var textSource = LexemeSource.TextSourceSpan.Slice(offset);
					var lexer = invokeInfo.Lexer;
					var lexemeSource = lexer.GetLexemeSource(textSource);
					var enumerator = lexemeSource.GetEnumerator();

					result = default;

					try
					{
						if (enumerator.MoveNext() == false)
							return false;

						throw Error.Refactoring;
						//if (lexer.GetIntValue(result.Token) != lexer.GetIntValue(invokeInfo.SyntaxToken.Token))
						//	return false;

						//result = enumerator.Current;
						//result.StartField += offset;
						//result.EndField += offset;

						//LexemeSource.Position = offset + result.End - result.Start;

						//Process.AdvanceInstructionPosition();

						//return true;
					}
					catch
					{
						return false;
					}
					finally
					{
						enumerator.Dispose();
						lexemeSource.Dispose();
					}
				}

				public PredicateResult CallExternalParser<TExternalGrammar, TExternalToken>(ExternalParserInvokeInfo<TExternalGrammar, TExternalToken> invokeInfo)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum
				{
					var offset = TextPosition;
					var textSource = LexemeSource.TextSourceSpan.Slice(offset);
					var externalAutomata = invokeInfo.Parser.Automata;
					var lexemeSource = invokeInfo.Lexer.GetLexemeSource(textSource);
					var externalContext = invokeInfo.Parser.CreateContext(lexemeSource);
					var externalAutomataContext = externalAutomata.CreateProcessContext(invokeInfo.SyntaxNode, lexemeSource, externalContext, ProcessKind.SubProcess, invokeInfo.Parser);
					var externalResult = externalAutomataContext.Process.Run();
					var poolCollection = GetExternalParserResources<TExternalGrammar, TExternalToken>();
					var callSubParserContext = poolCollection.ExternalParserContextPool.Get().Mount(this, invokeInfo, lexemeSource, offset, externalContext, externalAutomataContext, externalResult);

					try
					{
						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.SuccessAutomataResult successAutomataResult)
						{
							Process.AdvanceInstructionPosition(offset + successAutomataResult.InstructionPosition);

							return poolCollection.ExternalParserPredicateResultPool.Get().Mount(null);
						}

						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.ExceptionAutomataResult exceptionAutomataResult)
							return null;

						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult forkResult)
						{
							var firstBranch = poolCollection.ExternalParserForkBranchPool.Get().Mount(callSubParserContext, forkResult, true);
							var secondBranch = poolCollection.ExternalParserForkBranchPool.Get().Mount(callSubParserContext, forkResult, false);

							return poolCollection.ExternalParserForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
						}

						throw new InvalidOperationException();
					}
					finally
					{
						callSubParserContext.ReleaseReference();
					}
				}

				public PredicateResult<TExternalNode> CallValueExternalParser<TExternalGrammar, TExternalToken, TExternalNode>(
					ExternalParserInvokeInfo<TExternalGrammar, TExternalToken, TExternalNode> invokeInfo)
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> 
					where TExternalToken : unmanaged, Enum 
					where TExternalNode :  class
				{
					var offset = TextPosition;
					var textSource = TextSourceSpan.Slice(offset);
					var subAutomata = invokeInfo.Parser.Automata;
					var lexemeSource = invokeInfo.Lexer.GetLexemeSource(textSource);
					var externalContext = invokeInfo.Parser.CreateContext(lexemeSource);
					var externalAutomataContext = subAutomata.CreateSyntaxTreeContext(invokeInfo.SyntaxNode, lexemeSource, externalContext, ProcessKind.SubProcess, invokeInfo.Parser);
					var externalResult = externalAutomataContext.Process.Run();
					var poolCollection = GetExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode>();
					var externalParserContext = poolCollection.ExternalParserContextPool.Get().Mount(this, invokeInfo, lexemeSource, offset, externalContext, externalAutomataContext, externalResult);

					try
					{
						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.SuccessAutomataResult successAutomataResult)
						{
							Process.AdvanceInstructionPosition(offset + successAutomataResult.InstructionPosition);

							return poolCollection.ExternalParserPredicateResultPool.Get().Mount(externalAutomataContext.GetResult<TExternalNode>());
						}

						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.ExceptionAutomataResult exceptionAutomataResult)
							return null;

						if (externalResult is Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult forkResult)
						{
							var popResult = Process.IsMainThread;
							var firstBranch = poolCollection.ExternalParserForkBranchPool.Get().Mount(externalParserContext, forkResult, true, popResult);
							var secondBranch = poolCollection.ExternalParserForkBranchPool.Get().Mount(externalParserContext, forkResult, false, popResult);

							return poolCollection.ExternalParserForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
						}

						throw new InvalidOperationException();
					}
					catch
					{
						return null;
					}
					finally
					{
						externalParserContext.ReleaseReference();
					}
				}

				private ExternalParserResources<TExternalGrammar, TExternalToken> GetExternalParserResources<TExternalGrammar, TExternalToken>()
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> where TExternalToken : unmanaged, Enum
				{
					return GetExternalParserResources<ExternalParserResources<TExternalGrammar, TExternalToken>>();
				}

				private ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode> GetExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode>()
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> 
					where TExternalToken : unmanaged, Enum
					where TExternalNode : class
				{
					return GetExternalParserResources<ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode>>();
				}

				private T GetExternalParserResources<T>() where T : ExternalParserResources, new()
				{
					var type = typeof(T);

					if (_externalParserResourcesDictionary.TryGetValue(type, out var poolCollectionRaw))
						return (T)poolCollectionRaw;

					var poolCollection = new T();

					_externalParserResourcesDictionary.Add(type, poolCollection);

					return poolCollection;
				}

				public virtual void Dispose()
				{
					_externalParserResourcesDictionary = null;

					if (ParserContext == null)
						return;

					ParserContext.Dispose();
					ParserContext.ParserAutomataContext = null;
				}

				int IParserAutomataContextInterface.TextPosition
				{
					get => TextPosition;
					set => Process.AdvanceInstructionPosition(value);
				}
			}
		}
	}
}