// <copyright file="Lexer.Automata.LexerDfaState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerDfaState : DfaState<LexerDfaState>
			{
				private static readonly LexerDfaState EmptyState = new();

				public readonly TToken Token;
				public readonly int TokenCode;

				private LexerDfaState()
				{
				}

				private LexerDfaState(LexerDfaState state, bool save) : base(state, save)
				{
					if (SuccessSubGraph == null)
						return;

					var lexerStateRule = ((LexerRule)SuccessSubGraph.Rule).TokenRule;

					Token = lexerStateRule.Token;
					TokenCode = lexerStateRule.TokenCode;
				}

				public LexerDfaState(DfaNode[] nodes, DfaTransition[] lazyTransitions, DfaTransition successTransition, DfaTransition prevSuccessTransition, int hashCode, LexerDfaBuilder builder, bool build = true,
					LexerDfaState[] array = null) : base(nodes, lazyTransitions, successTransition,
					prevSuccessTransition, hashCode, builder)
				{
					if (SuccessSubGraph == null)
						return;

					var lexerStateRule = ((LexerRule)SuccessSubGraph.Rule).TokenRule;

					Token = lexerStateRule.Token;
					TokenCode = lexerStateRule.TokenCode;
				}

				protected override LexerDfaState CreateEmptyState()
				{
					return EmptyState;
				}

				protected override LexerDfaState CreateNullState(bool save)
				{
					return new LexerDfaState(this, save);
				}

				protected override bool GetSkip(SubGraph subGraph)
				{
					if (subGraph == null)
						return false;

					var lexerStateRule = ((LexerRule)SuccessSubGraph.Rule).TokenRule;

					return lexerStateRule.Skip;
				}

				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				public static int ReadFastNoPredicate(LexerDfaState initial, ref ReadOnlySpan<char> span, ref int instructionPointer, ref Lexeme<TToken> lexeme, ref int operand)
				{
					break_skip:

					var lexemeEnd = instructionPointer;
					var current = initial;
					var index = instructionPointer - 1;

					for (var i = instructionPointer; i < span.Length; i++)
					{
						var ch = span[i];

						index++;

						var d = ch - ArraySentinel;
						var dfaIndex = ((d >> 31) & d) + ArraySentinel;

						current = current.Array[dfaIndex];

						switch (current.Switch)
						{
							case SwitchSave:

								lexemeEnd = index;

								break;

							case SwitchBreakTake:

								lexemeEnd++;

								goto switch_break_take;

							case SwitchBreakTakeSave:

								lexemeEnd = index + 1;

								goto switch_break_take;

							case SwitchBreakSkip:

								instructionPointer = lexemeEnd + 1;

								goto break_skip;

							case SwitchBreakSkipSave:

								instructionPointer = index + 1;

								goto break_skip;
						}
					}

					throw new InvalidOperationException("Impossible flow.");

					switch_break_take:

					operand = current.TokenCode;
					lexeme.TokenField = current.Token;
					lexeme.StartField = instructionPointer;
					lexeme.EndField = lexemeEnd;

					instructionPointer = lexemeEnd;

					return 1;
				}
			}
		}
	}
}