// <copyright file="Parser.Automata.ParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Pools;


// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	[AttributeUsage(AttributeTargets.Parameter)]
	public abstract class LexemeBaseAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class LexemeDoubleAttribute : LexemeBaseAttribute
	{
	}

	internal abstract class LexemeTypeConverter<TToken, TOutput> where TToken : unmanaged, Enum
	{
		#region Methods

		protected abstract TOutput Convert(ref Lexeme<TToken> lexeme);

		internal TOutput ConvertInternal(ref Lexeme<TToken> lexeme)
		{
			return Convert(ref lexeme);
		}

		#endregion
	}

	internal sealed class LexemePointerConverter
	{
	}

	internal abstract partial class Parser<TGrammar, TToken>
	{
		#region Nested Types

		private sealed partial class ParserAutomata
		{
			#region Nested Types

			private interface IParserILBuilder
			{
				#region Methods

				void EmitGetInstruction(ILBuilderContext ilBuilderContext);

				void EmitGetInstructionText(ILBuilderContext ilBuilderContext);

				void EmitGetInstructionToken(ILBuilderContext ilBuilderContext);

				void EmitLdTextSourceSpan(ILBuilderContext ilBuilderContext);

				#endregion
			}

			private sealed class ParserAutomataContextState : AutomataContextState
			{
				#region Fields

				public ParserContext ParserContext;

				#endregion
			}

			private abstract class ParserAutomataContext : AutomataContext, IParserAutomataContextInterface, IDisposable
			{
				protected readonly ParserAutomata Automata;

				#region Static Fields and Constants

				private static readonly MethodInfo DebugMethodInfo = typeof(ParserAutomataContext).GetMethod(nameof(Debug), BindingFlags.Instance | BindingFlags.NonPublic);

				#endregion

				#region Fields

				private readonly ThreadLocal<Dictionary<Type, SubParserPoolCollection>> _threadLocalPoolDictionary = new ThreadLocal<Dictionary<Type, SubParserPoolCollection>>(() => new Dictionary<Type, SubParserPoolCollection>());
				private Dictionary<Type, SubParserPoolCollection> _poolDictionary;
				protected LexemeSource<TToken> LexemeSource;
				protected ParserContext ParserContext;
				protected TextSourceSpan TextSourceSpan;

				#endregion

				#region Ctors

				protected ParserAutomataContext(ParserState state, ParserAutomata parserAutomata) : base(state)
				{
					Automata = parserAutomata;
				}

				#endregion

				#region Properties

				private int TextPointer => InstructionStreamPosition;

				#endregion

				#region Methods

				public bool CallSubLexer<TSubToken>(SubLexerInvokeInfo<TSubToken> invokeInfo, out Lexeme<TSubToken> result) where TSubToken : unmanaged, Enum
				{
					var offset = TextPointer;
					var textSource = LexemeSource.TextSourceSpan.Slice(offset);
					var lexer = invokeInfo.Lexer;
					var lexemeSource = lexer.GetLexemeSource(textSource);
					var enumerator = lexemeSource.GetEnumerator();

					result = default;

					try
					{
						if (enumerator.MoveNext() == false)
							return false;

						if (lexer.GetIntValue(result.Token) != lexer.GetIntValue(invokeInfo.Rule.Token))
							return false;

						result = enumerator.Current;
						result.StartField += offset;
						result.EndField += offset;

						LexemeSource.Position = offset + result.End - result.Start;

						AdvanceInstructionPosition();

						return true;
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

				public PredicateResult CallSubParser<TSubGrammar, TSubToken>(SubParserInvokeInfo<TSubGrammar, TSubToken> invokeInfo) where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					var offset = TextPointer;
					var textSource = LexemeSource.TextSourceSpan.Slice(offset);
					var subAutomata = invokeInfo.Parser.Automata;
					var lexemeSource = invokeInfo.Lexer.GetLexemeSource(textSource);
					var subParserContext = invokeInfo.Parser.CreateContext(lexemeSource);
					var subParserAutomataContext = subAutomata.GetParserState(invokeInfo.Rule).MountProcessContext(subAutomata, lexemeSource, subParserContext, invokeInfo.Parser);
					var subParseResult = subAutomata.PartialRunCore(lexemeSource, subParserAutomataContext);
					var poolCollection = GetSubParserPoolCollection<TSubGrammar, TSubToken>();
					var callSubParserContext = poolCollection.CallSubParserContextPool.Get().Mount(this, invokeInfo, lexemeSource, offset, subParserContext, subParserAutomataContext, subParseResult);

					try
					{
						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.SuccessAutomataResult successAutomataResult)
						{
							LexemeSource.Position = offset + successAutomataResult.InstructionPosition;

							AdvanceInstructionPosition();

							return poolCollection.LocalResultPool.Get().Mount(null);
						}

						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.ExceptionAutomataResult exceptionAutomataResult)
							return null;

						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult forkResult)
						{
							var firstBranch = poolCollection.ForkBranchPool.Get().Mount(callSubParserContext, forkResult, true);
							var secondBranch = poolCollection.ForkBranchPool.Get().Mount(callSubParserContext, forkResult, false);

							return poolCollection.ForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
						}

						throw new InvalidOperationException();
					}
					finally
					{
						callSubParserContext.ReleaseReference();
					}
				}

				public PredicateResult<TSubNode> CallValueSubParser<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(SubParserInvokeInfo<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> invokeInfo)
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					var offset = TextPointer;
					var textSource = LexemeSource.TextSourceSpan.Slice(offset);
					var subAutomata = invokeInfo.Parser.Automata;
					var lexemeSource = invokeInfo.Lexer.GetLexemeSource(textSource);
					var subParserContext = invokeInfo.Parser.CreateContext(lexemeSource);
					var subParserAutomataContext = subAutomata.GetParserState(invokeInfo.Rule).MountSyntaxTreeContext(subAutomata, lexemeSource, subParserContext, invokeInfo.Parser);
					var subParseResult = subAutomata.PartialRunCore(lexemeSource, subParserAutomataContext);
					var poolCollection = GetSubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>();
					var callSubParserContext = poolCollection.CallSubParserContextPool.Get().Mount(this, invokeInfo, lexemeSource, offset, subParserContext, subParserAutomataContext, subParseResult);

					try
					{
						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.SuccessAutomataResult successAutomataResult)
						{
							LexemeSource.Position = offset + successAutomataResult.InstructionPosition;

							AdvanceInstructionPosition();

							return poolCollection.LocalResultPool.Get().Mount(subParserAutomataContext.GetResult<TSubNode>());
						}

						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.ExceptionAutomataResult exceptionAutomataResult)
							return null;

						if (subParseResult is Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult forkResult)
						{
							var popResult = IsMainThread;
							var firstBranch = poolCollection.ForkBranchPool.Get().Mount(callSubParserContext, forkResult, true, popResult);
							var secondBranch = poolCollection.ForkBranchPool.Get().Mount(callSubParserContext, forkResult, false, popResult);

							return poolCollection.ForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
						}

						throw new InvalidOperationException();
					}
					catch
					{
						return null;
					}
					finally
					{
						callSubParserContext.ReleaseReference();
					}
				}

				private void Debug()
				{
				}

				private static void EmitDebug(ILBuilderContext ilBuilderContext)
				{
					ilBuilderContext.EmitLdContext();
					ilBuilderContext.IL.Emit(OpCodes.Call, DebugMethodInfo);
				}

				private T GetPoolCollection<T>() where T : SubParserPoolCollection, new()
				{
					var poolType = typeof(T);

					if (_poolDictionary.TryGetValue(poolType, out var poolCollectionRaw))
						return (T) poolCollectionRaw;

					var poolCollection = new T();

					_poolDictionary.Add(poolType, poolCollection);

					return poolCollection;
				}

				private SubParserPoolCollection<TSubGrammar, TSubToken> GetSubParserPoolCollection<TSubGrammar, TSubToken>()
					where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					return GetPoolCollection<SubParserPoolCollection<TSubGrammar, TSubToken>>();
				}

				private SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> GetSubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>()
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					return GetPoolCollection<SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>>();
				}

				public virtual void Mount(LexemeSource<TToken> lexemeSource, ParserContext parserContext, Parser<TGrammar, TToken> parser)
				{
					_poolDictionary = _threadLocalPoolDictionary.Value;

					LexemeSource = lexemeSource;
					TextSourceSpan = lexemeSource.TextSourceSpan;
					ParserContext = parserContext;

					if (ParserContext != null)
						ParserContext.ParserAutomataContext = this;
				}

				#endregion

				#region Interface Implementations

				#region IDisposable

				public virtual void Dispose()
				{
					TextSourceSpan = TextSourceSpan.Empty;
					LexemeSource = null;
					_poolDictionary = null;

					if (ParserContext == null)
						return;

					ParserContext.Dispose();

					ParserContext.ParserAutomataContext = null;
					ParserContext = null;
				}

				#endregion

				#region IParserAutomataContextInterface

				int IParserAutomataContextInterface.TextPointer
				{
					get => TextPointer;
					set
					{
						LexemeSource.Position = value;

						AdvanceInstructionPosition();
					}
				}

				#endregion

				#endregion

				#region Nested Types

				private abstract class SubParserPoolCollection
				{
				}

				private sealed class SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> : SubParserPoolCollection
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					#region Fields

					public readonly Pool<CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>> CallSubParserContextPool;
					public readonly Pool<CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>> ForkBranchPool;
					public readonly Pool<CallSubParserForkPredicateResult<TSubNode>> ForkPredicateResultPool;
					public readonly Pool<LocalParserPredicateResult<TSubNode>> LocalResultPool;
					public readonly Pool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>> PredicateResultPool;

					#endregion

					#region Ctors

					public SubParserPoolCollection()
					{
						ForkBranchPool = new Pool<CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>>(p => new CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(this));
						CallSubParserContextPool = new Pool<CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>>(p => new CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(this));
						ForkPredicateResultPool = new Pool<CallSubParserForkPredicateResult<TSubNode>>(p => new CallSubParserForkPredicateResult<TSubNode>(p));
						PredicateResultPool = new Pool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>>(p => new CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>(p));
						LocalResultPool = new Pool<LocalParserPredicateResult<TSubNode>>(p => new LocalParserPredicateResult<TSubNode>(p));
					}

					#endregion
				}

				private sealed class SubParserPoolCollection<TSubGrammar, TSubToken> : SubParserPoolCollection
					where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					#region Fields

					public readonly Pool<CallSubParserContext<TSubGrammar, TSubToken>> CallSubParserContextPool;
					public readonly Pool<CallSubParserForkBranch<TSubGrammar, TSubToken>> ForkBranchPool;
					public readonly Pool<CallSubParserForkPredicateResult> ForkPredicateResultPool;
					public readonly Pool<LocalParserPredicateResult<object>> LocalResultPool;
					public readonly Pool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken>> PredicateResultPool;

					#endregion

					#region Ctors

					public SubParserPoolCollection()
					{
						ForkBranchPool = new Pool<CallSubParserForkBranch<TSubGrammar, TSubToken>>(p => new CallSubParserForkBranch<TSubGrammar, TSubToken>(this));
						CallSubParserContextPool = new Pool<CallSubParserContext<TSubGrammar, TSubToken>>(p => new CallSubParserContext<TSubGrammar, TSubToken>(this));
						ForkPredicateResultPool = new Pool<CallSubParserForkPredicateResult>(p => new CallSubParserForkPredicateResult(p));
						PredicateResultPool = new Pool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken>>(p => new CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken>(p));
						LocalResultPool = new Pool<LocalParserPredicateResult<object>>(p => new LocalParserPredicateResult<object>(p));
					}

					#endregion
				}

				private sealed class CallSubParserForkPredicateResult : ForkPredicateResult
				{
					#region Fields

					private readonly IPool<CallSubParserForkPredicateResult> _pool;

					#endregion

					#region Ctors

					public CallSubParserForkPredicateResult(IPool<CallSubParserForkPredicateResult> pool)
					{
						_pool = pool;
					}

					#endregion

					#region Methods

					internal override void Dispose()
					{
						base.Dispose();

						_pool.Release(this);
					}

					public CallSubParserForkPredicateResult Mount(PredicateEntryBase first, PredicateEntryBase second)
					{
						First = first;
						Second = second;

						return this;
					}

					#endregion
				}

				private sealed class CallSubParserForkPredicateResult<TResult> : ForkPredicateResult<TResult>
				{
					#region Fields

					private readonly IPool<CallSubParserForkPredicateResult<TResult>> _pool;

					#endregion

					#region Ctors

					public CallSubParserForkPredicateResult(IPool<CallSubParserForkPredicateResult<TResult>> pool)
					{
						_pool = pool;
					}

					#endregion

					#region Methods

					internal override void Dispose()
					{
						base.Dispose();

						_pool.Release(this);
					}

					public CallSubParserForkPredicateResult<TResult> Mount(PredicateEntryBase first, PredicateEntryBase second)
					{
						First = first;
						Second = second;

						return this;
					}

					#endregion
				}

				private abstract class CallSubParserContext : IDisposable
				{
					#region Fields

					private ReferenceCounter ReferenceCount = new ReferenceCounter();

					#endregion

					#region Methods

					protected void AddReferenceCore()
					{
						ReferenceCount.AddReference();
					}

					protected void ReleaseReferenceCore()
					{
						if (ReferenceCount.ReleaseReference() == 0)
							Dispose();
					}

					#endregion

					#region Interface Implementations

					#region IDisposable

					public abstract void Dispose();

					#endregion

					#endregion
				}

				private sealed class CallSubParserContext<TSubGrammar, TSubToken> : CallSubParserContext
					where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					#region Fields

					private readonly SubParserPoolCollection<TSubGrammar, TSubToken> _poolCollection;

					#endregion

					#region Ctors

					public CallSubParserContext(SubParserPoolCollection<TSubGrammar, TSubToken> poolCollection)
					{
						_poolCollection = poolCollection;
					}

					#endregion

					#region Properties

					private LexemeSource<TSubToken> LexemeSource { get; set; }

					public int Offset { get; private set; }

					public ParserAutomataContext ParserAutomataContext { get; private set; }

					private Parser<TSubGrammar, TSubToken>.ParserAutomata.ProcessAutomataContext SubParserAutomataContext { get; set; }

					private ParserContext SubParserContext { get; set; }

					private Automata<Lexeme<TSubToken>, TSubToken>.AutomataResult SubParseResult { get; set; }

					public SubParserInvokeInfo<TSubGrammar, TSubToken> SubParserInvokeInfo { get; private set; }

					#endregion

					#region Methods

					public CallSubParserContext<TSubGrammar, TSubToken> AddReference()
					{
						AddReferenceCore();

						return this;
					}

					public override void Dispose()
					{
						LexemeSource = LexemeSource.DisposeExchange();
						SubParserContext = SubParserContext.DisposeExchange();
						SubParserAutomataContext = SubParserAutomataContext.DisposeExchange();
						SubParseResult = SubParseResult.DisposeExchange();

						_poolCollection.CallSubParserContextPool.Release(this);
					}

					public CallSubParserContext<TSubGrammar, TSubToken> Mount(ParserAutomataContext parserAutomataContext, SubParserInvokeInfo<TSubGrammar, TSubToken> subParserInvokeInfo, LexemeSource<TSubToken> lexemeSource, int offset,
						ParserContext subParserContext, Parser<TSubGrammar, TSubToken>.ParserAutomata.ProcessAutomataContext subParserAutomataContext, Automata<Lexeme<TSubToken>, TSubToken>.AutomataResult subParseResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						SubParseResult = subParseResult;
						SubParserContext = subParserContext;
						SubParserInvokeInfo = subParserInvokeInfo;
						ParserAutomataContext = parserAutomataContext;
						SubParserAutomataContext = subParserAutomataContext;

						return AddReference();
					}

					public void ReleaseReference()
					{
						ReleaseReferenceCore();
					}

					#endregion
				}

				private sealed class CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> : CallSubParserContext
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					#region Fields

					private readonly SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> _poolCollection;

					#endregion

					#region Ctors

					public CallSubParserContext(SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> poolCollection)
					{
						_poolCollection = poolCollection;
					}

					#endregion

					#region Properties

					private LexemeSource<TSubToken> LexemeSource { get; set; }

					public int Offset { get; private set; }

					public ParserAutomataContext ParserAutomataContext { get; private set; }

					public Parser<TSubGrammar, TSubToken>.ParserAutomata.SyntaxTreeAutomataContext SubParserAutomataContext { get; private set; }

					private ParserContext SubParserContext { get; set; }

					private Automata<Lexeme<TSubToken>, TSubToken>.AutomataResult SubParseResult { get; set; }

					public SubParserInvokeInfo<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> SubParserInvokeInfo { get; private set; }

					#endregion

					#region Methods

					public CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> AddReference()
					{
						AddReferenceCore();

						return this;
					}

					public override void Dispose()
					{
						LexemeSource = LexemeSource.DisposeExchange();
						SubParserContext = SubParserContext.DisposeExchange();
						SubParserAutomataContext = SubParserAutomataContext.DisposeExchange();
						SubParseResult = SubParseResult.DisposeExchange();
						_poolCollection.CallSubParserContextPool.Release(this);
					}

					public CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> Mount(ParserAutomataContext parserAutomataContext, SubParserInvokeInfo<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> subParserInvokeInfo,
						LexemeSource<TSubToken> lexemeSource, int offset,
						ParserContext subParserContext,
						Parser<TSubGrammar, TSubToken>.ParserAutomata.SyntaxTreeAutomataContext subParserAutomataContext, Automata<Lexeme<TSubToken>, TSubToken>.AutomataResult subParseResult)
					{
						Offset = offset;
						LexemeSource = lexemeSource;
						SubParseResult = subParseResult;
						SubParserContext = subParserContext;
						SubParserInvokeInfo = subParserInvokeInfo;
						ParserAutomataContext = parserAutomataContext;
						SubParserAutomataContext = subParserAutomataContext;

						return AddReference();
					}

					public void ReleaseReference()
					{
						ReleaseReferenceCore();
					}

					#endregion
				}

				private sealed class CallSubParserForkBranch<TSubGrammar, TSubToken> : PredicateEntryBase, IDisposable where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					#region Fields

					private readonly SubParserPoolCollection<TSubGrammar, TSubToken> _poolCollection;
					private bool _finish;

					#endregion

					#region Ctors

					public CallSubParserForkBranch(SubParserPoolCollection<TSubGrammar, TSubToken> poolCollection)
					{
						_poolCollection = poolCollection;
					}

					#endregion

					#region Properties

					public CallSubParserContext<TSubGrammar, TSubToken> CallSubParserContext { get; private set; }

					internal override bool ConsumeResult => false;

					public Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult SubParseResult { get; private set; }

					#endregion

					#region Methods

					internal override PredicateEntryBase GetActualPredicateEntry()
					{
						return CallSubParserContext.SubParserInvokeInfo.PredicateEntry;
					}

					public CallSubParserForkBranch<TSubGrammar, TSubToken> Mount(CallSubParserContext<TSubGrammar, TSubToken> callSubParserContext, Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult subParseResult, bool finish)
					{
						CallSubParserContext = callSubParserContext.AddReference();
						SubParseResult = subParseResult.AddReference();
						_finish = finish;

						return this;
					}

					protected override PredicateResult Pass(AutomataContext context)
					{
						if (_finish)
						{
							CallSubParserContext.ParserAutomataContext.LexemeSource.Position = CallSubParserContext.Offset + SubParseResult.InstructionStreamPosition;
							CallSubParserContext.ParserAutomataContext.AdvanceInstructionPosition();

							return _poolCollection.PredicateResultPool.Get().Mount(this);
						}

						var result = SubParseResult.RunSecond();

						try
						{
							if (result is Automata<Lexeme<TSubToken>, TSubToken>.SuccessAutomataResult localResult)
							{
								CallSubParserContext.ParserAutomataContext.LexemeSource.Position = CallSubParserContext.Offset + localResult.InstructionPosition;
								CallSubParserContext.ParserAutomataContext.AdvanceInstructionPosition();

								return _poolCollection.PredicateResultPool.Get().Mount(this);
							}

							if (result is Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult forkResult)
							{
								var firstBranch = _poolCollection.ForkBranchPool.Get().Mount(CallSubParserContext, forkResult, true);
								var secondBranch = _poolCollection.ForkBranchPool.Get().Mount(CallSubParserContext, forkResult, false);

								return _poolCollection.ForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
							}

							return null;
						}
						finally
						{
							result.Dispose();
						}
					}

					#endregion

					#region Interface Implementations

					#region IDisposable

					public void Dispose()
					{
						SubParseResult = SubParseResult.DisposeExchange();
						CallSubParserContext.ReleaseReference();
						CallSubParserContext = null;
						_poolCollection.ForkBranchPool.Release(this);
					}

					#endregion

					#endregion
				}

				private sealed class CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken> : PredicateResult
					where TSubGrammar : Grammar<TSubToken> where TSubToken : unmanaged, Enum
				{
					#region Fields

					private readonly IPool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken>> _pool;

					private CallSubParserForkBranch<TSubGrammar, TSubToken> _forkBranch;

					#endregion

					#region Ctors

					public CallSubParserForkBranchPredicateResult(IPool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken>> pool)
					{
						_pool = pool;
					}

					#endregion

					#region Methods

					internal override void Dispose()
					{
						_forkBranch = null;

						_pool.Release(this);
					}

					public CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken> Mount(CallSubParserForkBranch<TSubGrammar, TSubToken> branch)
					{
						_forkBranch = branch;

						return this;
					}

					#endregion
				}

				private sealed class CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> : PredicateResult<TSubNode>
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					#region Fields

					private readonly IPool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>> _pool;
					private CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> _forkBranch;

					#endregion

					#region Ctors

					public CallSubParserForkBranchPredicateResult(IPool<CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase>> pool)
					{
						_pool = pool;
					}

					#endregion

					#region Methods

					internal override void Dispose()
					{
						base.Dispose();

						_forkBranch = null;
						_pool.Release(this);
					}

					protected override TSubNode GetResultCore()
					{
						if (_forkBranch.Finish)
							_forkBranch.SubParseResult.RunFirst().Dispose();

						return _forkBranch.CallSubParserContext.SubParserAutomataContext.GetResult<TSubNode>();
					}

					public CallSubParserForkBranchPredicateResult<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> Mount(CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> forkBranch)
					{
						_forkBranch = forkBranch;

						return this;
					}

					#endregion
				}

				private sealed class CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> : PredicateEntryBase, IDisposable
					where TSubGrammar : Grammar<TSubToken, TSubNodeBase> where TSubToken : unmanaged, Enum where TSubNode : TSubNodeBase where TSubNodeBase : class
				{
					#region Fields

					private readonly SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> _poolCollection;
					private bool _popResult;

					#endregion

					#region Ctors

					public CallSubParserForkBranch(SubParserPoolCollection<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> poolCollection)
					{
						_poolCollection = poolCollection;
					}

					#endregion

					#region Properties

					public CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> CallSubParserContext { get; private set; }

					internal override bool ConsumeResult => true;

					public bool Finish { get; private set; }

					internal override bool PopResult => _popResult;

					public Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult SubParseResult { get; private set; }

					#endregion

					#region Methods

					internal override PredicateEntryBase GetActualPredicateEntry()
					{
						return CallSubParserContext.SubParserInvokeInfo.PredicateEntry;
					}

					public CallSubParserForkBranch<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> Mount(CallSubParserContext<TSubGrammar, TSubToken, TSubNode, TSubNodeBase> callSubParserContext,
						Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult subParseResult, bool finish, bool popResult)
					{
						CallSubParserContext = callSubParserContext.AddReference();
						SubParseResult = subParseResult.AddReference();

						Finish = finish;
						_popResult = popResult;

						return this;
					}

					protected override PredicateResult Pass(AutomataContext context)
					{
						if (Finish)
						{
							CallSubParserContext.ParserAutomataContext.LexemeSource.Position = CallSubParserContext.Offset + SubParseResult.InstructionStreamPosition;
							CallSubParserContext.ParserAutomataContext.AdvanceInstructionPosition();

							return _poolCollection.PredicateResultPool.Get().Mount(this);
						}

						var result = SubParseResult.RunSecond();

						try
						{
							if (result is Automata<Lexeme<TSubToken>, TSubToken>.SuccessAutomataResult localResult)
							{
								CallSubParserContext.ParserAutomataContext.LexemeSource.Position = CallSubParserContext.Offset + localResult.InstructionPosition;
								CallSubParserContext.ParserAutomataContext.AdvanceInstructionPosition();

								return _poolCollection.PredicateResultPool.Get().Mount(this);
							}

							if (result is Automata<Lexeme<TSubToken>, TSubToken>.ForkAutomataResult forkResult)
							{
								var firstBranch = _poolCollection.ForkBranchPool.Get().Mount(CallSubParserContext, forkResult, true, false);
								var secondBranch = _poolCollection.ForkBranchPool.Get().Mount(CallSubParserContext, forkResult, false, false);

								return _poolCollection.ForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
							}

							return null;
						}
						finally
						{
							result.Dispose();
						}
					}

					#endregion

					#region Interface Implementations

					#region IDisposable

					public void Dispose()
					{
						SubParseResult = SubParseResult.DisposeExchange();
						CallSubParserContext.ReleaseReference();
						CallSubParserContext = null;
						_poolCollection.ForkBranchPool.Release(this);
					}

					#endregion

					#endregion
				}

				protected sealed class LocalParserPredicateResult<TResult> : PredicateResult<TResult>
				{
					#region Fields

					private readonly IPool<LocalParserPredicateResult<TResult>> _pool;

					private TResult _result;

					#endregion

					#region Ctors

					public LocalParserPredicateResult(IPool<LocalParserPredicateResult<TResult>> pool)
					{
						_pool = pool;
					}

					#endregion

					#region Methods

					internal override void Dispose()
					{
						base.Dispose();

						_result = default;
						_pool.Release(this);
					}

					protected override TResult GetResultCore()
					{
						return _result;
					}

					public LocalParserPredicateResult<TResult> Mount(TResult result)
					{
						_result = result;

						return this;
					}

					#endregion
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}