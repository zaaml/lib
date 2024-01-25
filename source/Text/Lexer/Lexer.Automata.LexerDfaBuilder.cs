// <copyright file="Lexer.Automata.LexerDfaBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed partial class LexerDfaBuilder
			{
				private const int NullSaveIndexOffset = 0x10000000;

				private readonly Stack<DfaBuilderKey> _builderKeyPool = new();
				private readonly Dictionary<LexerDfaNode, LexerDfaState> _detachedStates = new();
				private readonly Dictionary<(Node, LexerDfaTransition, ExecutionPath), LexerDfaNode> _dfaNodesDictionary = new();
				private readonly Dictionary<DfaTransitionCollection, DfaTransitionCollection> _frozenLazyTransitionsDictionary = new();
				private readonly Dictionary<DfaNodeCollection, DfaNodeCollection> _frozenNodesDictionary = new();
				private readonly Dictionary<DfaStateKey, LexerDfaState> _frozenStatesDictionary = new();
				private readonly List<LexerDfaNode> _initialStateNodes;
				private readonly List<LexerDfaNode> _noOpInitialStateNodes;
				private readonly object _syncRoot = new();
				private readonly int _transitionCount;
				public readonly LexerDfaState InitialState;
				public readonly LexerDfaState NoOpInitialState;
				public LexerDfaState[] StateRepository = new LexerDfaState[128];

				public LexerDfaBuilder(IEnumerable<LexerSyntax> rules, LexerAutomata automata)
				{
					_initialStateNodes = new List<LexerDfaNode>();
					_noOpInitialStateNodes = new List<LexerDfaNode>();

					foreach (var subGraph in rules.Select(automata.EnsureSubGraphProtected))
					{
						if (subGraph.SyntaxGraph.CanSimulateDfa == false)
							throw new InvalidOperationException($"FiniteState {subGraph.Syntax} can not be simulated as DFA");

						if (subGraph.SyntaxGraph.HasOperandNodes == false)
							_noOpInitialStateNodes.Add(CreateDfaNode(subGraph.SyntaxGraph.BeginNode, new LexerDfaTransition(subGraph, _transitionCount), null));
						else
							_initialStateNodes.Add(CreateDfaNode(subGraph.SyntaxGraph.BeginNode, new LexerDfaTransition(subGraph, _transitionCount), null));

						_transitionCount++;
					}

					FastLookup = true;

					var builderKey = GetBuilderKey();

					InitialState = builderKey.Build(_initialStateNodes);
					NoOpInitialState = builderKey.Build(_noOpInitialStateNodes);

					ReleaseBuilderKey(builderKey);

					if (FastLookup)
						BuildFastLookup(InitialState);
				}

				public bool FastLookup { get; }

				private int StateCount { get; set; }

				private LexerDfaState CreateDfaState(LexerDfaNode[] nodes, LexerDfaTransition[] lazyTransitions, LexerDfaTransition successTransition, LexerDfaTransition prevSuccessTransition, int hashCode)
				{
					return new LexerDfaState(nodes, lazyTransitions, successTransition, prevSuccessTransition, hashCode, this);
				}

				public LexerDfaState Build(int operand, LexerDfaState currentState)
				{
					lock (_syncRoot)
					{
						var dfaState = currentState.TryGetState(operand);

						if (dfaState != null)
							return dfaState;

						dfaState = BuildImpl(operand, currentState);

						currentState.SetState(operand, dfaState);

						return dfaState;
					}
				}

				private void BuildFastLookup(LexerDfaState initialState)
				{
					if (initialState.Break)
						return;

					var builderKey = GetBuilderKey();
					var hashSet = new HashSet<LexerDfaState>();
					var queue = new Queue<LexerDfaState>();

					hashSet.Add(initialState);
					queue.Enqueue(initialState);

					while (queue.Count > 0)
					{
						var state = queue.Dequeue();
						var nextBreakCount = 0;

						for (var i = 0; i < LexerDfaState.ArrayLimit; i++)
						{
							var next = state.TryGetState(i);

							if (next == null)
							{
								next = builderKey.Build(i, state);
								state.SetState(i, next);
							}

							if (next.Break)
							{
								if (next.SuccessSubGraph == state.SuccessSubGraph && next.SavePointer == false)
									nextBreakCount++;

								continue;
							}

							if (hashSet.Add(next))
								queue.Enqueue(next);
						}

						if (nextBreakCount == LexerDfaState.ArrayLimit && state.SuccessSubGraph != null && state.SavePointer)
							state.FastNoNext = true;
					}

					foreach (var dfaState in hashSet)
					{
						if (dfaState.Array == null)
							continue;

						for (var i = 0; i < LexerDfaState.ArraySentinel; i++)
						{
							var next = dfaState.Array[i];

							if (next.FastNoNext)
								dfaState.Array[i] = next.NullSaveState;
						}
					}

					ReleaseBuilderKey(builderKey);
				}

				private LexerDfaState BuildImpl(int operand, LexerDfaState currentState)
				{
					var builderKey = GetBuilderKey();
					var dfaState = builderKey.Build(operand, currentState);

					ReleaseBuilderKey(builderKey);

					return dfaState;
				}

				public LexerDfaState BuildPredicateState(int operand, LexerDfaState currentState, bool predicateResult)
				{
					lock (_syncRoot)
					{
						var dfaState = operand == 0 ? currentState.TryGetPredicateState(predicateResult) : currentState.TryGetPredicateState(operand, predicateResult);

						if (dfaState != null)
							return dfaState;

						var builderKey = GetBuilderKey();
						var state = builderKey.BuildPredicateState(operand, currentState, predicateResult);

						ReleaseBuilderKey(builderKey);

						if (operand == 0)
							currentState.SetPredicateState(state, predicateResult);
						else
							currentState.SetPredicateState(operand, state, predicateResult);

						if (FastLookup)
							BuildFastLookup(state);

						return state;
					}
				}

				private LexerDfaNode CreateDfaNode(Node node, LexerDfaTransition transition, ExecutionPath predicatePath)
				{
					var tuple = (node, transition, predicatePath);

					if (_dfaNodesDictionary.TryGetValue(tuple, out var dfaNode))
						return dfaNode;

					return _dfaNodesDictionary[tuple] = new LexerDfaNode(node, transition, predicatePath);
				}

				private DfaBuilderKey GetBuilderKey()
				{
					return _builderKeyPool.Count > 0 ? _builderKeyPool.Pop() : new DfaBuilderKey(this, _transitionCount);
				}

				public LexerDfaState GetByIndex(int id)
				{
					return StateRepository[id];
				}

				public static int GetNullIndex(bool save, int stateIndex)
				{
					return save ? -stateIndex - NullSaveIndexOffset : -stateIndex;
				}

				public static int GetNullOwnerIndex(int index)
				{
					return index + NullSaveIndexOffset < 0 ? -(index + NullSaveIndexOffset) : -index;
				}

				public int Register(LexerDfaState dfaState)
				{
					var stateIndex = StateCount;

					StateRepository[stateIndex] = dfaState;
					StateCount++;

					if (StateCount == StateRepository.Length)
						Array.Resize(ref StateRepository, StateCount * 2);

					return stateIndex;
				}

				private void ReleaseBuilderKey(DfaBuilderKey dfaBuilderKey)
				{
					_builderKeyPool.Push(dfaBuilderKey);
				}

				public LexerDfaState GetDetachedState(LexerDfaState currentState, LexerDfaNode node)
				{
					lock (_detachedStates)
					{
						if (_detachedStates.TryGetValue(node, out var state))
							return state;

						state = BuildDetachedLazyState(currentState, node);

						_detachedStates.Add(node, state);

						return state;
					}
				}

				private LexerDfaState BuildDetachedLazyState(LexerDfaState currentState, LexerDfaNode node)
				{
					var builderKey = GetBuilderKey();
					var state = builderKey.BuildPredicateDetachedLazy(currentState, node);

					ReleaseBuilderKey(builderKey);

					return state;
				}

				private sealed class DfaNodeCollection : CollectionKey<LexerDfaNode, DfaNodeCollection>
				{
				}

				private sealed class DfaTransitionCollection : CollectionKey<LexerDfaTransition, DfaTransitionCollection>
				{
				}
			}

			private abstract class CollectionKey<TElement, TCollection> where TElement : IEquatable<TElement> where TCollection : CollectionKey<TElement, TCollection>, new()
			{
				private int _count;
				private TElement[] _elements;
				private int _elementsHashCode;

				public int Count => _count;

				public TElement[] Elements => _elements;

				public int ElementsHashCode => _elementsHashCode;

				public void Add(TElement element)
				{
					ArrayUtils.EnsureArrayLength(ref _elements, Count + 1, true);

					_elements[_count++] = element;

					unchecked
					{
						_elementsHashCode *= 397;
						_elementsHashCode ^= element.GetHashCode();
					}
				}

				private static bool AreEqual(CollectionKey<TElement, TCollection> collection1, CollectionKey<TElement, TCollection> collection2)
				{
					var arrayLength1 = collection1.Count;
					var arrayLength2 = collection2.Count;

					if (arrayLength1 != arrayLength2)
						return false;

					var array1 = collection1.Elements;
					var array2 = collection2.Elements;

					for (var i = 0; i < arrayLength1; i++)
						if (array1[i].Equals(array2[i]) == false)
							return false;

					return true;
				}

				public void Clear()
				{
					_count = 0;
					_elementsHashCode = 0;
				}

				public override bool Equals(object obj)
				{
					return AreEqual(this, (CollectionKey<TElement, TCollection>)obj);
				}

				public TCollection Freeze()
				{
					var collection = new TCollection
					{
						_elements = Count == 0 ? Array.Empty<TElement>() : new TElement[Count],
						_elementsHashCode = ElementsHashCode,
						_count = Count
					};

					if (_elements != null)
						Array.Copy(_elements, collection._elements, Count);

					return collection;
				}

				public override int GetHashCode()
				{
					// ReSharper disable once NonReadonlyMemberInGetHashCode
					return ElementsHashCode;
				}
			}
		}
	}
}