// <copyright file="Automata.DfaState.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// ReSharper disable ForCanBeConvertedToForeach

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected abstract class DfaState<TDfaState> where TDfaState : DfaState<TDfaState>
		{
			#region Static Fields and Constants

			public const int ArrayLimit = 127;

			#endregion

			#region Fields

			public readonly TDfaState[] Array;
			public readonly bool Break;
			public readonly DfaBuilder<TDfaState> Builder;
			public readonly bool Continue;
			public readonly DfaDictionary<TDfaState> Dictionary;
			public readonly TDfaState[] FalsePredicateArray;
			public readonly Dictionary<int, TDfaState> FalsePredicateDictionary;
			public readonly int HashCode;
			public readonly int Index;
			public readonly DfaTransition[] LazyTransitions;
			public readonly DfaNode[] Nodes;
			public readonly TDfaState NullState;
			public readonly object Predicate;
			public readonly DfaTransition PrevSuccessTransition;
			public readonly bool SavePointer;
			public readonly SubGraphBase SuccessSubGraph;
			public readonly DfaTransition SuccessTransition;
			public readonly TDfaState[] TruePredicateArray;
			public readonly Dictionary<int, TDfaState> TruePredicateDictionary;
			public TDfaState EndState;
			public TDfaState FalsePredicateEndState;
			public TDfaState TruePredicateEndState;

			#endregion

			#region Ctors

			protected DfaState()
			{
			}

			protected DfaState(TDfaState state)
			{
				Nodes = state.Nodes;
				LazyTransitions = state.LazyTransitions;

				HashCode = state.HashCode;
				SuccessSubGraph = state.PrevSuccessTransition?.SubGraph;
				Break = true;
				Builder = state.Builder;
			}

			protected DfaState(DfaNode[] nodes, DfaTransition[] lazyTransitions, DfaTransition successTransition, DfaTransition prevSuccessTransition, int hashCode, DfaBuilder<TDfaState> builder)
			{
				Index = builder.Register((TDfaState) this);
				Array = builder.FastLookup ? new TDfaState[ArrayLimit] : System.Array.Empty<TDfaState>();

				Dictionary = new DfaDictionary<TDfaState>();
				Builder = builder;

				Nodes = nodes;
				LazyTransitions = lazyTransitions;
				HashCode = hashCode;
				SuccessTransition = successTransition;
				PrevSuccessTransition = prevSuccessTransition;
				SuccessSubGraph = SuccessTransition?.SubGraph;

				if (SuccessSubGraph != null)
					SavePointer = true;

				if (nodes.Length == 1 && SuccessTransition != null)
				{
					var dfaNode = Nodes[0];
					var node = (Node) dfaNode.Node;

					if ((node is ReturnRuleNode || node.ReturnPath.IsInvalid) && node.ExecutionPaths.Length == 0)
						Break = true;
				}

				Predicate = Nodes.FirstOrDefault(a => a.ExecutionPathObject != null)?.ExecutionPathObject;

				if (Predicate != null)
				{
					TruePredicateArray = builder.FastLookup ? new TDfaState[ArrayLimit] : System.Array.Empty<TDfaState>();
					TruePredicateDictionary = new Dictionary<int, TDfaState>();

					FalsePredicateArray = builder.FastLookup ? new TDfaState[ArrayLimit] : System.Array.Empty<TDfaState>();
					FalsePredicateDictionary = new Dictionary<int, TDfaState>();
				}

				// ReSharper disable once VirtualMemberCallInConstructor
				NullState = CreateNullState();

				if (SavePointer == false && Break == false && Predicate == null)
					Continue = true;
			}

			#endregion

			#region Methods

			protected abstract TDfaState CreateEmptyState();

			protected abstract TDfaState CreateNullState();

			public TDfaState EvalPredicates(int operand, AutomataContext context)
			{
				var predicate = ((ExecutionPath) Predicate).Predicate;
				var predicateResult = predicate.PassInternal(context) != null;

				return TryGetPredicateState(operand, predicateResult) ?? Builder.BuildPredicateState(operand, (TDfaState) this, predicateResult);
			}

			public TDfaState EvalPredicates(AutomataContext context)
			{
				var predicate = ((ExecutionPath) Predicate).Predicate;
				var predicateResult = predicate.PassInternal(context) != null;

				return TryGetPredicateState(predicateResult) ?? Builder.BuildPredicateState(-1, (TDfaState) this, predicateResult);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetPredicateState(int operand, TDfaState state, bool predicateResult)
			{
				if (predicateResult)
				{
					if (operand < TruePredicateArray.Length)
						TruePredicateArray[operand] = state;
					else
						TruePredicateDictionary[operand] = state;
				}
				else
				{
					if (operand < FalsePredicateArray.Length)
						FalsePredicateArray[operand] = state;
					else
						FalsePredicateDictionary[operand] = state;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetPredicateState(TDfaState state, bool predicateResult)
			{
				if (predicateResult)
					TruePredicateEndState = state;
				else
					FalsePredicateEndState = state;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetState(int operand, TDfaState state)
			{
				if (operand < Array.Length)
					Array[operand] = state;
				else
					Dictionary.Add(operand, state);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TDfaState TryGetPredicateState(int operand, bool predicateResult)
			{
				if (predicateResult)
				{
					if (operand < TruePredicateArray.Length)
						return TruePredicateArray[operand];

					return TruePredicateDictionary.TryGetValue(operand, out var result) ? result : null;
				}
				else
				{
					if (operand < FalsePredicateArray.Length)
						return FalsePredicateArray[operand];

					return FalsePredicateDictionary.TryGetValue(operand, out var result) ? result : null;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TDfaState TryGetPredicateState(bool predicateResult)
			{
				return predicateResult ? TruePredicateEndState : FalsePredicateEndState;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public TDfaState TryGetState(int operand)
			{
				if (operand < Array.Length)
					return Array[operand];

				return Dictionary.TryGetValue(operand, out var result) ? result : null;
			}

			#endregion
		}

		//public sealed class DfaDictionary<TValue>  : Dictionary<int, TValue>
		public sealed class DfaDictionary<TValue>  : DfaDictionaryEx<TValue>
		{
		}

		public class DfaDictionaryEx<TValue>
		{
			private int[] _buckets;
			private Entry[] _entries;

			public DfaDictionaryEx(int capacity = 16)
			{
				_buckets = new int[capacity];
				_entries = new Entry[capacity];
			}

			public void Add(int key, TValue value)
			{
				this[key] = value;
			}

			private ref TValue this[int key]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					var entries = _entries;
					var entryIndex = _buckets[key % _buckets.Length] - 1;

					while (entryIndex != -1)
					{
						if (entries[entryIndex].Key == key)
							return ref entries[entryIndex].Value;

						entryIndex = entries[entryIndex].Next;
					}

					if (Count == entries.Length)
						entries = Resize();

					entryIndex = Count++;
					entries[entryIndex].Key = key;

					var bucket = key % _buckets.Length;

					entries[entryIndex].Next = _buckets[bucket] - 1;
					_buckets[bucket] = entryIndex + 1;

					return ref entries[entryIndex].Value;
				}
			}

			private int Count { get; set; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetValue(int key, out TValue value)
			{
				var entries = _entries;
				var entryIndex = _buckets[key % _buckets.Length] - 1;

				while (entryIndex != -1)
				{
					if (entries[entryIndex].Key == key)
					{
						value = entries[entryIndex].Value;

						return true;
					}

					entryIndex = entries[entryIndex].Next;
				}

				value = default;

				return false;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private Entry[] Resize()
			{
				var count = Count;
				var entries = new Entry[count * 2];

				Array.Copy(_entries, 0, entries, 0, count);

				_entries = entries;

				var newBuckets = new int[count * 2];

				_buckets = newBuckets;

				for (var i = 0; i < count;)
				{
					var bucketIndex = entries[i].Key % _buckets.Length;

					entries[i].Next = newBuckets[bucketIndex] - 1;
					newBuckets[bucketIndex] = ++i;
				}

				return entries;
			}

			private struct Entry
			{
				public int Key;
				public TValue Value;
				public int Next;
			}
		}

		#endregion
	}
}