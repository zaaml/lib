// <copyright file="Automata.DfaBuilder.DfaBuilderKey.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		private protected partial class LexerAutomata
		{
			private sealed partial class LexerDfaBuilder
			{
				private sealed class DfaBuilderKey : DfaStateKey
				{
					private readonly LexerDfaBuilder _builder;
					private readonly DfaTransitionCollection _lazyTransitionsCollection = new();
					private readonly bool[] _lazyTransitionsState;
					private readonly DfaNodeCollection _nodesCollection = new();
					private readonly HashSet<Node> _nodeSet = new();
					private DfaNodeCollection _frozenNodes;
					private DfaTransitionCollection _frozenTransitions;
					private LexerDfaTransition _prevSuccessTransition;
					private int _stateHashCode;
					private LexerDfaTransition _successTransition;

					public DfaBuilderKey(LexerDfaBuilder builder, int transitionsCount)
					{
						_builder = builder;
						_lazyTransitionsState = new bool[transitionsCount];
					}

					private void AddLazyTransition(LexerDfaTransition transition)
					{
						_lazyTransitionsCollection.Add(transition);
						_lazyTransitionsState[transition.LazyIndex] = true;
					}

					private void AddNode(LexerDfaNode dfaNode)
					{
						_nodesCollection.Add(dfaNode);
					}

					private void AddNode(Node node, LexerDfaTransition transition, ExecutionPath predicatePath)
					{
						if (_nodeSet.Add(node) == false)
							return;

						var dfaNode = _builder.CreateDfaNode(node, transition, predicatePath);

						_nodesCollection.Add(dfaNode);

						if (predicatePath == null && (node is ReturnSyntaxNode || node.HasReturn))
							_successTransition = transition;
					}

					private LexerDfaState BuildDetached(int operand, DetachedStateLazy detachedStateLazy)
					{
						AddPostPredicateNode(operand, detachedStateLazy.Node);

						return BuildState() ?? detachedStateLazy.State.NullState;
					}

					public LexerDfaState Build(int operand, LexerDfaState currentState)
					{
						if (currentState.Nodes.Length == 1 && currentState.Nodes[0].Node is DataNode<DetachedStateLazy> detachedState)
							return BuildDetached(operand, detachedState.Data);

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
							var node = dfaNode.Node;
							var executionPaths = node.GetExecutionPaths(operand);

							foreach (var executionPath in executionPaths)
							{
								if (executionPath.OutputReturn)
								{
									if (node is PredicateNode && operand == 0)
										AddNode(executionPath.Output, transition, null);

									continue;
								}

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

					public LexerDfaState Build(IEnumerable<LexerDfaNode> nodes)
					{
						foreach (var dfaNode in nodes)
							AddNode(dfaNode);
						
						return BuildState();
					}

					public LexerDfaState BuildPredicateDetachedLazy(LexerDfaState state, LexerDfaNode dfaNode)
					{
						var targetNode = dfaNode.Node.OutEdges.Single().TargetNode;

						AddNode(new LexerDfaNode(targetNode, dfaNode.Transition, null));
						
						if (dfaNode.Node.HasReturn) 
							_successTransition = dfaNode.Transition;

						return BuildState();
					}

					private readonly struct DetachedStateLazy
					{
						public DetachedStateLazy(LexerDfaState state, LexerDfaNode node)
						{
							State = state;
							Node = node;
						}

						public readonly LexerDfaState State;
						public readonly LexerDfaNode Node;
					}

					public LexerDfaState BuildPredicateState(int operand, LexerDfaState state, bool predicateResult)
					{
						_prevSuccessTransition = state.PrevSuccessTransition;
						_successTransition = state.SuccessTransition;

						foreach (var lazyTransition in state.LazyTransitions)
							AddLazyTransition(lazyTransition);

						var predicateFound = false;

						foreach (var dfaNode in state.Nodes)
						{
							var predicateExecutionPath = dfaNode.ExecutionPath;

							if (predicateExecutionPath == null || predicateFound)
							{
								AddNode(dfaNode);

								continue;
							}

							predicateFound = true;

							if (predicateResult == false)
								continue;

							AddPostPredicateNode(operand, dfaNode);
						}

						_prevSuccessTransition = _successTransition ?? _prevSuccessTransition;

						return BuildState() ?? state.NullState;
					}

					private void AddPostPredicateNode(int operand, LexerDfaNode dfaNode)
					{
						var transition = dfaNode.Transition;
						var lazyIndex = transition.LazyIndex;

						if (ReferenceEquals(transition, _prevSuccessTransition) && _lazyTransitionsState[lazyIndex])
							return;

						var node = dfaNode.Node;

						if (node.HasReturn)
						{
							AddNode(node.ReturnPaths[0].Output, transition, null);

							_successTransition = transition;
						}

						var passLazyNode = _lazyTransitionsState[lazyIndex];
						var executionPaths = node.GetExecutionPaths(operand);

						foreach (var executionPath in executionPaths)
						{
							if (executionPath.OutputReturn)
								continue;

							AddNode(executionPath.Output, transition, executionPath.IsPredicate ? executionPath : null);

							passLazyNode |= executionPath.PassLazyNode;
						}

						if (_lazyTransitionsState[lazyIndex] == passLazyNode)
							return;

						AddLazyTransition(transition);
					}

					private LexerDfaState BuildState()
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

					protected override LexerDfaTransition[] GetLazyTransitions()
					{
						return _frozenTransitions.Elements;
					}

					protected override LexerDfaNode[] GetNodes()
					{
						return _frozenNodes.Elements;
					}

					protected override LexerDfaTransition GetPrevSuccessTransition()
					{
						return _prevSuccessTransition;
					}

					protected override LexerDfaTransition GetSuccessTransition()
					{
						return _successTransition;
					}
				}
			}
		}
	}
}