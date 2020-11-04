// <copyright file="Automata.DfaBuilder.DfaBuilderKey.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		private protected abstract partial class DfaBuilder<TDfaState> where TDfaState : DfaState<TDfaState>
		{
			#region Nested Types

			private sealed class DfaBuilderKey : DfaStateKey
			{
				#region Fields

				private readonly DfaBuilder<TDfaState> _builder;
				private readonly DfaTransitionCollection _lazyTransitionsCollection = new DfaTransitionCollection();
				private readonly bool[] _lazyTransitionsState;
				private readonly DfaNodeCollection _nodesCollection = new DfaNodeCollection();
				private readonly HashSet<Node> _nodeSet = new HashSet<Node>();
				private DfaNodeCollection _frozenNodes;
				private DfaTransitionCollection _frozenTransitions;
				private DfaTransition _prevSuccessTransition;
				private int _stateHashCode;
				private DfaTransition _successTransition;

				#endregion

				#region Ctors

				public DfaBuilderKey(DfaBuilder<TDfaState> builder, int transitionsCount)
				{
					_builder = builder;
					_lazyTransitionsState = new bool[transitionsCount];
				}

				#endregion

				#region Methods

				private void AddLazyTransition(DfaTransition transition)
				{
					_lazyTransitionsCollection.Add(transition);
					_lazyTransitionsState[transition.LazyIndex] = true;
				}

				private void AddNode(DfaNode dfaNode)
				{
					_nodesCollection.Add(dfaNode);
				}

				private void AddNode(Node node, DfaTransition transition, ExecutionPath predicatePath)
				{
					if (_nodeSet.Add(node) == false)
						return;

					var dfaNode = _builder.CreateDfaNode(node, transition, predicatePath);

					_nodesCollection.Add(dfaNode);

					if (predicatePath == null && (node is ReturnStateNode || node.ReturnPath.IsInvalid == false))
						_successTransition = transition;
				}

				public TDfaState Build(int operand, TDfaState currentState)
				{
					_prevSuccessTransition = currentState.PrevSuccessTransition;

					foreach (var lazyTransition in currentState.LazyTransitions)
						AddLazyTransition(lazyTransition);

					foreach (var dfaNode in currentState.Nodes)
					{
						var transition = dfaNode.Transition;
						var lazyIndex = transition.LazyIndex;

						if (ReferenceEquals(transition, _prevSuccessTransition) && _lazyTransitionsState[lazyIndex])
							continue;

						var passLazyNode = _lazyTransitionsState[lazyIndex];
						var node = (Node) dfaNode.Node;
						var executionPaths = operand == -1 ? node.GetExecutionPaths() : node.GetExecutionPaths(operand);

						foreach (var executionPath in executionPaths)
						{
							AddNode(executionPath.Output, transition, executionPath.IsPredicate ? executionPath : null);

							passLazyNode |= executionPath.PassLazyNode;
						}

						if (_lazyTransitionsState[lazyIndex] == passLazyNode)
							continue;

						AddLazyTransition(transition);
					}

					_prevSuccessTransition = _successTransition ?? _prevSuccessTransition;

					return BuildState() ?? currentState.NullState;
				}

				public TDfaState Build(IEnumerable<DfaNode> nodes)
				{
					foreach (var dfaNode in nodes)
						AddNode(dfaNode);

					var state = BuildState();

					Clear();

					return state;
				}

				public TDfaState BuildPredicateState(int operand, TDfaState state, bool predicateResult)
				{
					_prevSuccessTransition = state.PrevSuccessTransition;
					_successTransition = state.SuccessTransition;

					foreach (var lazyTransition in state.LazyTransitions)
						AddLazyTransition(lazyTransition);

					var predicateFound = false;

					foreach (var dfaNode in state.Nodes)
					{
						var node = (Node) dfaNode.Node;
						var predicateExecutionPath = (ExecutionPath) dfaNode.ExecutionPathObject;

						if (predicateExecutionPath == null || predicateFound)
						{
							AddNode(dfaNode);

							continue;
						}

						predicateFound = true;

						if (predicateResult == false)
							continue;

						var transition = dfaNode.Transition;
						var lazyIndex = transition.LazyIndex;

						if (ReferenceEquals(transition, _prevSuccessTransition) && _lazyTransitionsState[lazyIndex])
							continue;

						if (node.ReturnPath.IsInvalid == false)
						{
							AddNode(node.ReturnPath.Output, transition, null);

							_successTransition = transition;
						}

						var passLazyNode = _lazyTransitionsState[lazyIndex];
						var executionPaths = operand == -1 ? node.GetExecutionPaths() : node.GetExecutionPaths(operand);

						foreach (var executionPath in executionPaths)
						{
							AddNode(executionPath.Output, transition, executionPath.IsPredicate ? executionPath : null);

							passLazyNode |= executionPath.PassLazyNode;
						}

						if (_lazyTransitionsState[lazyIndex] == passLazyNode)
							continue;

						AddLazyTransition(transition);
					}

					_prevSuccessTransition = _successTransition ?? _prevSuccessTransition;

					return BuildState() ?? state.NullState;
				}

				private TDfaState BuildState()
				{
					try
					{
						if (_builder._frozenNodesDictionary.TryGetValue(_nodesCollection, out _frozenNodes) == false)
						{
							_frozenNodes = _nodesCollection.Freeze();
							_builder._frozenNodesDictionary[_frozenNodes] = _frozenNodes;
						}

						if (_builder._frozenLazyTransitionsDictionary.TryGetValue(_lazyTransitionsCollection, out _frozenTransitions) == false)
						{
							_frozenTransitions = _lazyTransitionsCollection.Freeze();
							_builder._frozenLazyTransitionsDictionary[_frozenTransitions] = _frozenTransitions;
						}

						_stateHashCode = 0;

						unchecked
						{
							_stateHashCode *= 397;
							_stateHashCode ^= _frozenNodes.ElementsHashCode;

							_stateHashCode *= 397;
							_stateHashCode ^= _frozenTransitions.ElementsHashCode;

							_stateHashCode *= 397;
							_stateHashCode ^= _successTransition?.GetHashCode() ?? 0;

							_stateHashCode *= 397;
							_stateHashCode ^= _prevSuccessTransition?.GetHashCode() ?? 0;
						}

						if (_builder._frozenStatesDictionary.TryGetValue(this, out var frozen))
							return frozen;

						frozen = _builder.CreateDfaState(_frozenNodes.Elements, _frozenTransitions.Elements, _successTransition, _prevSuccessTransition, _stateHashCode);

						var stateKey = new DfaFrozenStateKey(frozen);
						var stateValue = _nodesCollection.Count == 0 ? null : frozen;

						_builder._frozenStatesDictionary[stateKey] = stateValue;

						return stateValue;
					}
					finally
					{
						Clear();
					}
				}

				private void Clear()
				{
					_stateHashCode = 0;
					_successTransition = null;
					_prevSuccessTransition = null;

					_nodesCollection.Clear();
					_nodeSet.Clear();

					if (_lazyTransitionsCollection.Count > 0)
						Array.Clear(_lazyTransitionsState, 0, _lazyTransitionsState.Length);

					_lazyTransitionsCollection.Clear();
				}

				protected override int GetKeyHashCode()
				{
					return _stateHashCode;
				}

				protected override DfaTransition[] GetLazyTransitions()
				{
					return _frozenTransitions.Elements;
				}

				protected override DfaNode[] GetNodes()
				{
					return _frozenNodes.Elements;
				}

				protected override DfaTransition GetPrevSuccessTransition()
				{
					return _prevSuccessTransition;
				}

				protected override DfaTransition GetSuccessTransition()
				{
					return _successTransition;
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}