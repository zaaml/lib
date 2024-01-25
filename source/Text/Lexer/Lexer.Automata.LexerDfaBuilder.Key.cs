// <copyright file="Automata.DfaBuilder.Key.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed partial class LexerDfaBuilder
			{
				private abstract class DfaStateKey
				{
					private bool Equals(DfaStateKey other)
					{
						return ReferenceEquals(GetNodes(), other.GetNodes()) &&
						       ReferenceEquals(GetLazyTransitions(), other.GetLazyTransitions()) &&
						       ReferenceEquals(GetSuccessTransition(), other.GetSuccessTransition()) &&
						       ReferenceEquals(GetPrevSuccessTransition(), other.GetPrevSuccessTransition());
					}

					public override bool Equals(object obj)
					{
						return Equals((DfaStateKey)obj);
					}

					public override int GetHashCode()
					{
						return GetKeyHashCode();
					}

					protected abstract int GetKeyHashCode();

					protected abstract LexerDfaTransition[] GetLazyTransitions();

					protected abstract LexerDfaNode[] GetNodes();

					protected abstract LexerDfaTransition GetPrevSuccessTransition();

					protected abstract LexerDfaTransition GetSuccessTransition();
				}

				private sealed class DfaFrozenStateKey : DfaStateKey
				{
					private readonly LexerDfaState _state;

					public DfaFrozenStateKey(LexerDfaState state)
					{
						_state = state;
					}

					protected override int GetKeyHashCode()
					{
						return _state.HashCode;
					}

					protected override LexerDfaTransition[] GetLazyTransitions()
					{
						return _state.LazyTransitions;
					}

					protected override LexerDfaNode[] GetNodes()
					{
						return _state.Nodes;
					}

					protected override LexerDfaTransition GetPrevSuccessTransition()
					{
						return _state.PrevSuccessTransition;
					}

					protected override LexerDfaTransition GetSuccessTransition()
					{
						return _state.SuccessTransition;
					}
				}
			}
		}
	}
}