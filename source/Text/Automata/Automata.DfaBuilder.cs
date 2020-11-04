// <copyright file="Automata.DfaBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Utils;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected abstract partial class DfaBuilder<TDfaState> where TDfaState : DfaState<TDfaState>
		{
			#region Fields

			private readonly Stack<DfaBuilderKey> _builderKeyPool = new Stack<DfaBuilderKey>();
			private readonly Dictionary<(Node, DfaTransition, ExecutionPath), DfaNode> _dfaNodesDictionary = new Dictionary<(Node, DfaTransition, ExecutionPath), DfaNode>();
			private readonly Dictionary<DfaTransitionCollection, DfaTransitionCollection> _frozenLazyTransitionsDictionary = new Dictionary<DfaTransitionCollection, DfaTransitionCollection>();
			private readonly Dictionary<DfaNodeCollection, DfaNodeCollection> _frozenNodesDictionary = new Dictionary<DfaNodeCollection, DfaNodeCollection>();
			private readonly Dictionary<DfaStateKey, TDfaState> _frozenStatesDictionary = new Dictionary<DfaStateKey, TDfaState>();
			private readonly List<DfaNode> _initialStateNodes;
			private readonly List<DfaNode> _noOpInitialStateNodes;
			private readonly object _syncRoot = new object();
			private readonly int _transitionCount;
			public readonly TDfaState InitialState;
			public readonly TDfaState NoOpInitialState;
			private TDfaState[] StateRepository = new TDfaState[128];

			#endregion

			#region Ctors

			protected DfaBuilder(IEnumerable<FiniteState> states, Automata<TInstruction, TOperand> automata)
			{
				_initialStateNodes = new List<DfaNode>();
				_noOpInitialStateNodes = new List<DfaNode>();

				foreach (var subGraph in states.Select(automata.EnsureSubGraph))
				{
					if (subGraph.Graph.CanSimulateDfa == false)
						throw new InvalidOperationException($"FiniteState {subGraph.State} can not be simulated as DFA");

					if (subGraph.Graph.HasOperandNodes == false)
						_noOpInitialStateNodes.Add(CreateDfaNode(subGraph.Graph.BeginNode, new DfaTransition(subGraph, _transitionCount), null));
					else
						_initialStateNodes.Add(CreateDfaNode(subGraph.Graph.BeginNode, new DfaTransition(subGraph, _transitionCount), null));

					_transitionCount++;
				}

				FastLookup = _initialStateNodes.Count < 100;

				var builderKey = GetBuilderKey();

				InitialState = builderKey.Build(_initialStateNodes);
				NoOpInitialState = builderKey.Build(_noOpInitialStateNodes);

				ReleaseBuilderKey(builderKey);

				if (FastLookup)
					BuildFastLookup(InitialState);
			}

			#endregion

			#region Properties

			public bool FastLookup { get; }

			private int StateCount { get; set; }

			#endregion

			#region Methods

			public TDfaState Build(int operand, TDfaState currentState)
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

			public TDfaState Build(TDfaState currentState)
			{
				lock (_syncRoot)
				{
					var dfaState = currentState.EndState;

					if (dfaState != null)
						return dfaState;

					dfaState = BuildImpl(-1, currentState);

					currentState.EndState = dfaState;

					return dfaState;
				}
			}

			private void BuildFastLookup(TDfaState initialState)
			{
				if (initialState.Break)
					return;

				var builderKey = GetBuilderKey();
				var hashSet = new HashSet<TDfaState>();
				var queue = new Queue<TDfaState>();

				hashSet.Add(initialState);
				queue.Enqueue(initialState);

				while (queue.Count > 0)
				{
					var state = queue.Dequeue();

					for (var i = 0; i < DfaState<TDfaState>.ArrayLimit; i++)
					{
						var next = state.TryGetState(i);

						if (next == null)
						{
							next = builderKey.Build(i, state);
							state.SetState(i, next);
						}

						if (next.Break)
							continue;

						if (hashSet.Add(next))
							queue.Enqueue(next);
					}
				}

				ReleaseBuilderKey(builderKey);
			}

			private TDfaState BuildImpl(int operand, TDfaState currentState)
			{
				var builderKey = GetBuilderKey();
				var dfaState = builderKey.Build(operand, currentState);

				ReleaseBuilderKey(builderKey);

				return dfaState;
			}

			public TDfaState BuildPredicateState(int operand, TDfaState currentState, bool predicateResult)
			{
				lock (_syncRoot)
				{
					var dfaState = operand == -1 ? currentState.TryGetPredicateState(predicateResult) : currentState.TryGetPredicateState(operand, predicateResult);

					if (dfaState != null)
						return dfaState;

					var builderKey = GetBuilderKey();
					var state = builderKey.BuildPredicateState(operand, currentState, predicateResult);

					ReleaseBuilderKey(builderKey);

					if (operand == -1)
						currentState.SetPredicateState(state, predicateResult);
					else
						currentState.SetPredicateState(operand, state, predicateResult);

					if (FastLookup)
						BuildFastLookup(state);

					return state;
				}
			}

			private DfaNode CreateDfaNode(Node node, DfaTransition transition, ExecutionPath predicatePath)
			{
				var tuple = (node, transition, predicatePath);

				if (_dfaNodesDictionary.TryGetValue(tuple, out var dfaNode))
					return dfaNode;

				return _dfaNodesDictionary[tuple] = new DfaNode(node, transition, predicatePath);
			}

			protected abstract TDfaState CreateDfaState(DfaNode[] nodes, DfaTransition[] lazyTransitions, DfaTransition successTransition, DfaTransition prevSuccessTransition, int hashCode);

			private DfaBuilderKey GetBuilderKey()
			{
				return _builderKeyPool.Count > 0 ? _builderKeyPool.Pop() : new DfaBuilderKey(this, _transitionCount);
			}

			public TDfaState GetByIndex(int id)
			{
				return StateRepository[id];
			}

			public int Register(TDfaState dfaState)
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

			#endregion

			#region Nested Types

			private sealed class DfaNodeCollection : CollectionKey<DfaNode, DfaNodeCollection>
			{
			}

			private sealed class DfaTransitionCollection : CollectionKey<DfaTransition, DfaTransitionCollection>
			{
			}

			#endregion
		}

		private abstract class CollectionKey<TElement, TCollection> where TElement : IEquatable<TElement> where TCollection : CollectionKey<TElement, TCollection>, new()
		{
			#region Fields

			private int _count;
			private TElement[] _elements;
			private int _elementsHashCode;

			#endregion

			#region Properties

			public int Count => _count;

			public TElement[] Elements => _elements;

			public int ElementsHashCode => _elementsHashCode;

			#endregion

			#region Methods

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

			#endregion
		}

		#endregion
	}
}