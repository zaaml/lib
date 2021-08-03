// <copyright file="Lexer.Automata.LexerDfaState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed class LexerDfaState : DfaState<LexerDfaState>
			{
				private static readonly LexerDfaState EmptyState = new LexerDfaState();

				public readonly bool Skip;
				public readonly TToken Token;
				public readonly int TokenCode;

				private LexerDfaState()
				{
				}

				private LexerDfaState(LexerDfaState state) : base(state)
				{
					if (SuccessSubGraph == null)
						return;

					var lexerStateRule = ((LexerRule) SuccessSubGraph.State).TokenRule;

					Token = lexerStateRule.Token;
					TokenCode = lexerStateRule.TokenCode;
					Skip = lexerStateRule.Skip;
				}

				public LexerDfaState(DfaNode[] nodes, DfaTransition[] lazyTransitions, DfaTransition successTransition, DfaTransition prevSuccessTransition, int hashCode, LexerDfaBuilder builder, bool build = true,
					LexerDfaState[] array = null) : base(nodes, lazyTransitions, successTransition,
					prevSuccessTransition, hashCode, builder)
				{
					if (SuccessSubGraph == null)
						return;

					var lexerStateRule = ((LexerRule) SuccessSubGraph.State).TokenRule;

					Token = lexerStateRule.Token;
					TokenCode = lexerStateRule.TokenCode;
					Skip = lexerStateRule.Skip;
				}

				protected override LexerDfaState CreateEmptyState()
				{
					return EmptyState;
				}

				protected override LexerDfaState CreateNullState()
				{
					return new LexerDfaState(this);
				}
			}
		}
	}
}