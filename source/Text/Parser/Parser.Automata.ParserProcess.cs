// <copyright file="Parser.Automata.ParserProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private ParserProcess.ParserILGenerator ILGenerator { get; } = new();

			private sealed partial class ParserProcess : Process
			{
				private readonly ParserAutomataContext _context;
				private readonly ParserProduction[] _productions;

				public readonly Converter<Lexeme<TToken>, string> LexemeStringConverter;
				public readonly Converter<Lexeme<TToken>, TToken> LexemeTokenConverter;
				private ProductionEntity[] _productionEntityStack;
				private int _productionEntityStackTail;
				private SyntaxNodeFactory _syntaxTreeFactory;
				private TextSpan _textSpan;

				public ParserProcess(ParserAutomataContext context) : base(new LexemeStreamInstructionReader(context.LexemeSource), context, context.ParserAutomata.ILGenerator)
				{
					_context = context;
					_productions = context.ParserAutomata.Productions.ToArray();
					_textSpan = context.TextSourceSpan;
					_productionEntityStack = ArrayPool<ProductionEntity>.Shared.Rent(0x10000);
					_syntaxTreeFactory = context.Parser.CreateSyntaxFactoryInternal();
					_syntaxTreeFactory?.AttachParserContextInternal(context.ParserContext);

					LexemeStringConverter = lexeme => _textSpan.GetText(lexeme.StartField, lexeme.EndField - lexeme.StartField);
					LexemeTokenConverter = lexeme => lexeme.TokenField;
				}

				private void ConsumeProductionEntity(int entryIndex)
				{
					var value = _productionEntityStack[_productionEntityStackTail].Result;

					_productionEntityStack[_productionEntityStackTail - 1].Arguments[entryIndex].ConsumeValue(value);
				}

				protected override void Dispose()
				{
					while (_productionEntityStackTail >= 0)
					{
						_productionEntityStack[_productionEntityStackTail]?.Reset();
						_productionEntityStack[_productionEntityStackTail--] = null;
					}

					_productionEntityStackTail = 0;

					_syntaxTreeFactory?.DetachParserContextInternal(_context.ParserContext);
					_syntaxTreeFactory = _syntaxTreeFactory.DisposeExchange();

					ArrayPool<ProductionEntity>.Shared.Return(_productionEntityStack, true);

					base.Dispose();
				}

				private void EnsureProductionEntityStackDepth(int delta)
				{
					if (_productionEntityStackTail + delta >= _productionEntityStack.Length)
						ArrayUtils.ExpandArrayLength(ref _productionEntityStack, true);
				}

				private void EnterLeftFactoringProduction(int productionIndex)
				{
					_productionEntityStack[_productionEntityStackTail] = _productions[productionIndex].RentEntity();
				}

				private void EnterLeftRecursionProduction(int productionIndex)
				{
					var parserProduction = _productions[productionIndex];

					_productionEntityStack[_productionEntityStackTail] = parserProduction.RentEntity();
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void EnterProduction(int productionIndex)
				{
					_productionEntityStack[_productionEntityStackTail] = _productions[productionIndex].RentEntity();
				}

				internal string GetInstructionText()
				{
					ref var lexeme = ref GetInstructionOperand(out var operand);

					if (operand < 0)
						return null;

					return StaticLexemes[operand] ?? _textSpan.GetText(lexeme.StartField, lexeme.EndField - lexeme.StartField);
				}

				public string GetLexemeString(ref Lexeme<TToken> lexeme)
				{
					return lexeme.StartField == lexeme.EndField ? null : _textSpan.GetTextInternal(lexeme.StartField, lexeme.EndField - lexeme.StartField);
				}

				private protected override void ConsumePredicateResult(PredicateResult predicateResult, PredicateEntryBase predicateEntry)
				{
					var argument = ((IParserEntry)predicateEntry.GetActualPredicateEntry()).ProductionArgument;

					if (argument is not (ParserProductionArgument or LexerProductionArgument))
						return;

					var entityArgument = _productionEntityStack[_productionEntityStackTail].Arguments[argument.ArgumentIndex];
					var result = predicateResult.GetResult();

					entityArgument.ConsumeValue(result);
				}

				protected override void OnFinished()
				{
					base.OnFinished();

					if (_productionEntityStackTail != 0)
						throw new InvalidOperationException();

					Result = _productionEntityStack[0].Result;
				}

				private object Result { get; set; }

				public TResult GetResult<TResult>()
				{
					return (TResult)Result;
				}

				private void LeaveLeftFactoringProduction(int productionIndex)
				{
					var postfix = _productionEntityStack[_productionEntityStackTail];
					var prefixCount = 0;
					var actualProduction = postfix.ParserProduction;
					var sourceProduction = postfix.ParserProduction;

					while (sourceProduction.SourceProduction != null)
					{
						prefixCount++;
						sourceProduction = sourceProduction.SourceProduction;
						actualProduction = actualProduction?.ActualProduction;
					}

					if (prefixCount == 0)
					{
						postfix.CreateEntityInstance(this);

						return;
					}

					if (_productionEntityStack[_productionEntityStackTail - prefixCount].ParserProduction == actualProduction || actualProduction == null)
						return;

					var actualEntity = actualProduction.RentEntity();

					foreach (var postfixArgument in postfix.Arguments)
					{
						var originalArgument = postfixArgument.Argument;

						while (originalArgument.OriginalArgument != null) 
							originalArgument = originalArgument.OriginalArgument;

						var actualArgument = actualEntity.Arguments[originalArgument.ArgumentIndex];

						postfixArgument.TransferValue(actualArgument);
					}

					postfix.Return();

					for (var i = 0; i < prefixCount; i++)
					{
						var prefix = _productionEntityStack[_productionEntityStackTail - i - 1];

						for (var index = 0; index < prefix.Arguments.Length - 1; index++)
						{
							var prefixArgument = prefix.Arguments[index];
							var originalArgument = prefixArgument.Argument;

							while (originalArgument.OriginalArgument != null)
								originalArgument = originalArgument.OriginalArgument;

							var actualArgument = actualEntity.Arguments[originalArgument.ArgumentIndex];

							prefixArgument.TransferValue(actualArgument);
						}

						_productionEntityStack[_productionEntityStackTail - i] = prefix;
					}

					_productionEntityStack[_productionEntityStackTail - prefixCount] = actualEntity;
				}

				private void LeaveLeftRecursionProduction(int productionIndex)
				{
					var tail = _productionEntityStack[_productionEntityStackTail];

					if (tail.ParserProduction.SourceProduction == null)
					{
						if (tail.ParserProduction.Binder is not LeftRecursionBinder)
							tail.CreateEntityInstance(this);

						return;
					}

					if (tail.ParserProduction.Binder is LeftRecursionBinder { Kind: LeftRecursionBinderKind.Recurse})
					{
						var actualTail = tail.CreateSourceEntity();
						var head = _productionEntityStack[_productionEntityStackTail - 1];
						var actualHead = head.ParserProduction.Binder is LeftRecursionBinder ? head.CreateSourceEntity() : head;

						actualHead.CreateEntityInstance(this);

						actualTail.Arguments[0].ConsumeValue(actualHead.Result);

						_productionEntityStack[_productionEntityStackTail - 1] = actualTail;
						_productionEntityStack[_productionEntityStackTail] = actualHead;
					}
					else if (tail.ParserProduction.Binder is LeftRecursionBinder { Kind: LeftRecursionBinderKind.Tail})
					{
						var actualTail = tail.CreateSourceEntity();

						while (actualTail.ParserProduction.SourceProduction != null) 
							actualTail = actualTail.CreateSourceEntity();

						actualTail.CreateEntityInstance(this);

						_productionEntityStack[_productionEntityStackTail] = actualTail;
					}
					else if (tail.ParserProduction.Binder is LeftRecursionBinder { Kind: LeftRecursionBinderKind.Indirect })
					{
						var sourceEntity = tail.ParserProduction.SourceProduction.RentEntity();

						tail.TransferValues(sourceEntity);

						sourceEntity.CreateEntityInstance(this);

						var actualEntity = tail.ParserProduction.ActualProduction.RentEntity();

						sourceEntity.TransferValues(actualEntity);

						actualEntity.Arguments[0].ConsumeValue(sourceEntity.Result);

						sourceEntity.Return();

						actualEntity.CreateEntityInstance(this);

						_productionEntityStack[_productionEntityStackTail] = actualEntity;
					}
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				private void LeaveProduction(int productionIndex)
				{
					_productionEntityStack[_productionEntityStackTail].CreateEntityInstance(this);
				}

				private void LeaveRuleEntry()
				{
					_productionEntityStack[_productionEntityStackTail--].Return();
				}

				private ProductionEntity PeekProductionEntity()
				{
					return _productionEntityStack[_productionEntityStackTail];
				}
			}
		}
	}
}