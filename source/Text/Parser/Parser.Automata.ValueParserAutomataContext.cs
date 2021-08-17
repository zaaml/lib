// <copyright file="Parser.Automata.ValueParserAutomataContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable StaticMemberInGenericType

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			static ParserAutomata()
			{
				StaticLexemes = new string[InstructionsRange.Maximum + 1];
			}

			private abstract class ValueAutomataContext : ParserAutomataContext
			{
				private readonly Stack<ParserAutomataContextState> _parserAutomataContextStatesPool = new();

				protected ValueAutomataContext(ParserRule rule, LexemeSource<TToken> lexemeSource, ParserContext parserContext, ProcessKind processKind, Parser<TGrammar, TToken> parser, ParserAutomata parserAutomata)
					: base(rule, lexemeSource, parserContext, processKind, parser, parserAutomata)
				{
				}

				protected sealed override AutomataContextState CloneContextState(AutomataContextState contextState)
				{
					if (contextState == null)
						return null;

					var parserContextState = (ParserAutomataContextState)contextState;
					var cloneContextState = _parserAutomataContextStatesPool.Count > 0 ? _parserAutomataContextStatesPool.Pop() : new ParserAutomataContextState();
					var parserContextClone = parserContextState.ParserContext.Clone();

					parserContextClone.ParserAutomataContext = this;
					cloneContextState.ParserContext = parserContextClone;

					return cloneContextState;
				}

				protected sealed override AutomataContextState CreateContextState()
				{
					if (ParserContext == null)
						return null;

					var contextState = _parserAutomataContextStatesPool.Count > 0 ? _parserAutomataContextStatesPool.Pop() : new ParserAutomataContextState();

					contextState.ParserContext = ParserContext;

					return contextState;
				}

				protected sealed override void DisposeContextState(AutomataContextState contextState)
				{
					if (contextState == null)
						return;

					var parserContextState = (ParserAutomataContextState)contextState;

					parserContextState.ParserContext.Dispose();
					parserContextState.ParserContext = null;

					_parserAutomataContextStatesPool.Push(parserContextState);
				}

				public TResult GetResult<TResult>()
				{
					return ((ParserProcess)Process).GetResult<TResult>();
				}
			}
		}
	}
}