// <copyright file="Automata.Pool.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected sealed class DfaState
		{
			private readonly Dictionary<int, DfaState> _dfa2Dfa = new();
			private readonly Dictionary<int, NfaState> _dfa2Nfa = new();
			public readonly DfaBuilder Builder;
			public readonly bool Closed;
			public readonly int Index;
			public readonly Node Node;
			public readonly MemorySpan<int> Stack;
			public readonly DfaState SourceDfa;
			public int Depth;
			private int _dfaLockCounter;
			private int _nfaLockCounter;

			public DfaState(DfaBuilder builder, int index, Node node, MemorySpan<int> stack, DfaState sourceDfa)
			{
				Builder = builder;
				Index = index;
				Node = node;
				Stack = stack;
				SourceDfa = sourceDfa;
				Depth = sourceDfa?.Depth ?? node.DfaDepth;

				if (node.HasReturn)
				{
					foreach (var id in stack.SpanSafe)
					{
						if (id >= SubGraphIdMask) 
							continue;

						Closed = true;
							
						break;
					}
				}
				else
					Closed = true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DfaState Expand(Span<int> stack)
			{
				return Closed ? this : Builder.GetDfa(Node, stack, SourceDfa, false);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DfaState GetNextDfa(ReadOnlyMemorySpan<int> executionRail)
			{
				var nextDfa = this;

				foreach (var executionPath in executionRail)
					nextDfa = nextDfa.GetNextDfa(executionPath);

				return nextDfa;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public DfaState GetNextDfa(int executionPath)
			{
				if (Interlocked.Increment(ref _dfaLockCounter) == 1)
				{
					if (_dfa2Dfa.TryGetValue(executionPath, out var nextDfa) == false)
						nextDfa = GetNextDfaShared(executionPath);

					Interlocked.Decrement(ref _dfaLockCounter);

					return nextDfa;
				}

				{
					var nextDfa = GetNextDfaShared(executionPath);

					Interlocked.Decrement(ref _dfaLockCounter);

					return nextDfa;
				}
			}

			private DfaState GetNextDfaShared(int executionPath)
			{
				lock (_dfa2Dfa)
				{
					if (_dfa2Dfa.TryGetValue(executionPath, out var nextDfa))
						return nextDfa;

					_dfa2Dfa.Add(executionPath, nextDfa = Builder.BuildNextDfa(this, executionPath));

					return nextDfa;
				}
			}

			private NfaState GetNfaShared(int operand)
			{
				lock (_dfa2Nfa)
				{
					if (_dfa2Nfa.TryGetValue(operand, out var nfa))
						return nfa;

					_dfa2Nfa.Add(operand, nfa = Builder.BuildNfa(this, operand));

					return nfa;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public NfaState GetNfa(int operand)
			{
#if DEBUG
				if (Closed == false)
					throw new InvalidOperationException();
#endif

				if (Interlocked.Increment(ref _nfaLockCounter) == 1)
				{
					if (_dfa2Nfa.TryGetValue(operand, out var nfa) == false)
						nfa = GetNfaShared(operand);

					Interlocked.Decrement(ref _nfaLockCounter);

					return nfa;
				}

				{
					var nfa = GetNfaShared(operand);

					Interlocked.Decrement(ref _nfaLockCounter);

					return nfa;
				}
			}

			public DfaState WithDepth(int i)
			{
				Depth = i;

				return this;
			}
		}
	}
}