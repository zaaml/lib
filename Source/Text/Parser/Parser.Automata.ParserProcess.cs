// <copyright file="Parser.Automata.ParserProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Collections.Generic;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
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
				private SyntaxFactory _syntaxTreeFactory;
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
						_productionEntityStack[_productionEntityStackTail].Reset();
						_productionEntityStackTail--;
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

				public TResult GetResult<TResult>()
				{
					if (_productionEntityStackTail != 0)
						throw new InvalidOperationException();

					return (TResult)_productionEntityStack[0].Result;
				}

				private void LeaveLeftFactoringProduction(int productionIndex)
				{
					var postfix = _productionEntityStack[_productionEntityStackTail];

					if (postfix.ParserProduction.OriginalProduction != null)
					{
						var actualProduction = postfix.ParserProduction;

						while (actualProduction.OriginalProduction != null)
							actualProduction = actualProduction.OriginalProduction;

						var actualEntity = actualProduction.RentEntity();

						foreach (var postfixArgument in postfix.Arguments)
						{
							var actualArgument = actualEntity.Arguments[postfixArgument.Argument.OriginalArgument.ArgumentIndex];

							postfixArgument.TransferValue(actualArgument);
						}

						postfix.Return();

						_productionEntityStack[_productionEntityStackTail] = postfix = actualEntity;
					}

					var prefix = _productionEntityStack[_productionEntityStackTail - 1];

					if (prefix.ParserProduction.LeftFactorProduction == false)
					{
						postfix.CreateEntityInstance(this);

						return;
					}

					foreach (var prefixArgument in prefix.Arguments)
					{
						var actualArgument = postfix.Arguments[prefixArgument.Argument.OriginalArgument.ArgumentIndex];

						prefixArgument.TransferValue(actualArgument);
					}

					_productionEntityStack[_productionEntityStackTail - 1] = postfix;
					_productionEntityStack[_productionEntityStackTail] = prefix;
				}

				private void LeaveLeftRecursionProduction(int productionIndex)
				{
					var tail = _productionEntityStack[_productionEntityStackTail];

					if (tail.ParserProduction.OriginalProduction == null)
					{
						if (tail.ParserProduction.Binder is not LeftRecursionBinder)
							tail.CreateEntityInstance(this);

						return;
					}

					if (tail.ParserProduction.Binder is LeftRecursionBinder { Recursive: true })
					{
						var actualTail = LeftRecursionUnwind(tail);

						var head = _productionEntityStack[_productionEntityStackTail - 1];

						var actualHead = LeftRecursionUnwind(head);

						actualHead.CreateEntityInstance(this);

						actualTail.Arguments[0].ConsumeValue(actualHead.Result);

						_productionEntityStack[_productionEntityStackTail - 1] = actualTail;
						_productionEntityStack[_productionEntityStackTail] = actualHead;
					}
					else
					{
						var actualTail = LeftRecursionUnwind(tail);

						actualTail.CreateEntityInstance(this);

						_productionEntityStack[_productionEntityStackTail] = actualTail;
					}
				}

				private void LeaveProduction(int productionIndex)
				{
					_productionEntityStack[_productionEntityStackTail].CreateEntityInstance(this);
				}

				private void LeaveRuleEntry()
				{
					_productionEntityStack[_productionEntityStackTail--].Return();
				}

				private ProductionEntity LeftRecursionUnwind(ProductionEntity recursionEntity)
				{
					var actualEntity = recursionEntity.ParserProduction.OriginalProduction.RentEntity();

					foreach (var tailArgument in recursionEntity.Arguments)
					{
						var actualArgument = actualEntity.Arguments[tailArgument.Argument.OriginalArgument.ArgumentIndex];

						tailArgument.TransferValue(actualArgument);
					}

					return actualEntity;
				}

				private void OnAfterConsumeValue(int entryIndex)
				{
					_productionEntityStack[_productionEntityStackTail].OnAfterConsumeValue(entryIndex);
				}

				private ProductionEntity PeekProductionEntity()
				{
					return _productionEntityStack[_productionEntityStackTail];
				}
			}
		}
	}
}