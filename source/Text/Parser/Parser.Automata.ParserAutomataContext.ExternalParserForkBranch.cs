// <copyright file="Parser.Automata.ParserAutomataContext.ExternalParserForkBranch.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Extensions;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private abstract partial class ParserAutomataContext
			{
				private sealed class ExternalParserForkBranch<TExternalGrammar, TExternalToken> : PredicateEntryBase, IDisposable 
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken> where TExternalToken : unmanaged, Enum
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken> _externalParserResources;
					private bool _finish;

					public ExternalParserForkBranch(ExternalParserResources<TExternalGrammar, TExternalToken> externalParserResources)
					{
						_externalParserResources = externalParserResources;
					}

					internal override bool ConsumeResult => false;

					public ExternalParserContext<TExternalGrammar, TExternalToken> ExternalContext { get; private set; }

					public Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult ExternalResult { get; private set; }

					internal override PredicateEntryBase GetActualPredicateEntry()
					{
						return ExternalContext.ExternalParserInvokeInfo.PredicateEntry;
					}

					public ExternalParserForkBranch<TExternalGrammar, TExternalToken> Mount(ExternalParserContext<TExternalGrammar, TExternalToken> externalContext, Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult externalResult,
						bool finish)
					{
						ExternalContext = externalContext.AddReference();
						ExternalResult = externalResult.AddReference();
						_finish = finish;

						return this;
					}

					protected override PredicateResult Pass(AutomataContext context)
					{
						if (_finish)
						{
							ExternalContext.InternalAutomataContext.Process.AdvanceInstructionPosition(ExternalContext.Offset + ExternalResult.InstructionStreamPosition);

							return _externalParserResources.ExternalParserForkBranchPredicateResultPool.Get().Mount(this);
						}

						var result = ExternalResult.RunSecond();

						try
						{
							switch (result)
							{
								case Automata<Lexeme<TExternalToken>, TExternalToken>.SuccessAutomataResult localResult:

									ExternalContext.InternalAutomataContext.Process.AdvanceInstructionPosition(ExternalContext.Offset + localResult.InstructionPosition);

									return _externalParserResources.ExternalParserForkBranchPredicateResultPool.Get().Mount(this);

								case Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult forkResult:
								{
									var firstBranch = _externalParserResources.ExternalParserForkBranchPool.Get().Mount(ExternalContext, forkResult, true);
									var secondBranch = _externalParserResources.ExternalParserForkBranchPool.Get().Mount(ExternalContext, forkResult, false);

									return _externalParserResources.ExternalParserForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
								}

								default:
									return null;
							}
						}
						finally
						{
							result.Dispose();
						}
					}

					public void Dispose()
					{
						ExternalResult = ExternalResult.DisposeExchange();
						ExternalContext.ReleaseReference();
						ExternalContext = null;
						_externalParserResources.ExternalParserForkBranchPool.Release(this);
					}
				}

				private sealed class ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode> : PredicateEntryBase, IDisposable
					where TExternalGrammar : Grammar<TExternalGrammar, TExternalToken>
					where TExternalToken : unmanaged, Enum 
					where TExternalNode : class
				{
					private readonly ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode> _resources;
					private bool _popResult;

					public ExternalParserForkBranch(ExternalParserResources<TExternalGrammar, TExternalToken, TExternalNode> resources)
					{
						_resources = resources;
					}

					internal override bool ConsumeResult => true;

					public ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode> ExternalContext { get; private set; }

					public Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult ExternalResult { get; private set; }

					public bool Finish { get; private set; }

					internal override bool PopResult => _popResult;

					internal override PredicateEntryBase GetActualPredicateEntry()
					{
						return ExternalContext.ExternalParserInvokeInfo.PredicateEntry;
					}

					public ExternalParserForkBranch<TExternalGrammar, TExternalToken, TExternalNode> Mount(ExternalParserContext<TExternalGrammar, TExternalToken, TExternalNode> externalParserContext,
						Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult subParseResult, bool finish, bool popResult)
					{
						ExternalContext = externalParserContext.AddReference();
						ExternalResult = subParseResult.AddReference();

						Finish = finish;
						_popResult = popResult;

						return this;
					}

					protected override PredicateResult Pass(AutomataContext context)
					{
						if (Finish)
						{
							ExternalContext.InternalAutomataContext.Process.AdvanceInstructionPosition(ExternalContext.Offset + ExternalResult.InstructionStreamPosition);

							return _resources.ExternalParserForkBranchPredicateResultPool.Get().Mount(this);
						}

						var result = ExternalResult.RunSecond();

						try
						{
							if (result is Automata<Lexeme<TExternalToken>, TExternalToken>.SuccessAutomataResult localResult)
							{
								ExternalContext.InternalAutomataContext.Process.AdvanceInstructionPosition(ExternalContext.Offset + localResult.InstructionPosition);

								return _resources.ExternalParserForkBranchPredicateResultPool.Get().Mount(this);
							}

							if (result is Automata<Lexeme<TExternalToken>, TExternalToken>.ForkAutomataResult forkResult)
							{
								var firstBranch = _resources.ExternalParserForkBranchPool.Get().Mount(ExternalContext, forkResult, true, false);
								var secondBranch = _resources.ExternalParserForkBranchPool.Get().Mount(ExternalContext, forkResult, false, false);

								return _resources.ExternalParserForkPredicateResultPool.Get().Mount(firstBranch, secondBranch);
							}

							return null;
						}
						finally
						{
							result.Dispose();
						}
					}

					public void Dispose()
					{
						ExternalResult = ExternalResult.DisposeExchange();
						ExternalContext.ReleaseReference();
						ExternalContext = null;
						_resources.ExternalParserForkBranchPool.Release(this);
					}
				}
			}
		}
	}
}