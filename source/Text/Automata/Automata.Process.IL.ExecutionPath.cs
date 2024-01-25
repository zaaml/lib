// <copyright file="Automata.Process.IL.ExecutionPath.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			private static Type ProcessType => typeof(Process);
			private static Type ThreadContextType => typeof(ThreadContext);

			private protected partial class ProcessILGenerator
			{
				private static Type ProcessILGeneratorType => typeof(ProcessILGenerator);

				#region Static Fields and Constants

				private static readonly MethodInfo DebugNodeMethodInfo = ProcessILGeneratorType.GetMethod(nameof(DebugNode), SPNP);
				private static readonly MethodInfo DebugExecutionPathMethodInfo = ProcessILGeneratorType.GetMethod(nameof(DebugExecutionPath), SPNP);
				private static readonly MethodInfo CallPredicateMethodInfo = ProcessType.GetMethod(nameof(CallPredicate), IPNP);
				private static readonly MethodInfo ShouldPopPredicateResultMethodInfo = ProcessType.GetMethod(nameof(ShouldPopPredicateResult), IPNP);

				private static readonly MethodInfo PredicateResultIsForkMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.IsFork), IPNP);
				private static readonly MethodInfo PredicateResultDisposeMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.Dispose), IPNP);

				#endregion

				#region Methods

				protected virtual void EmitEnterSyntaxEntry(ILContext ilBuilderContext, SyntaxEntry syntaxEntry)
				{
				}

				protected virtual void EmitEnterProduction(ILContext ilBuilderContext, Production production)
				{
				}

				protected virtual void EmitInvoke(ILContext ilBuilderContext, MatchEntry matchEntry)
				{
				}

				protected virtual void EmitValue(ILContext ilBContext, ValueEntry valueEntry)
				{
				}

				protected virtual void EmitConsumePredicateResult(ILContext ilBuilderContext, LocalBuilder resultLocal, PredicateEntryBase predicateEntryBase)
				{
				}

				protected virtual void EmitLeaveSyntaxEntry(ILContext ilBuilderContext, SyntaxEntry syntaxEntry)
				{
				}

				protected virtual void EmitBeginExecutionPath(ILContext ilBuilderContext, int stackDelta)
				{
				}

				protected virtual void EmitLeaveProduction(ILContext ilBuilderContext, Production production)
				{
				}

				private bool IsOverriden(string methodName)
				{
					var methodInfo = GetType().GetMethod(methodName, IPNP);

					// ReSharper disable once PossibleNullReferenceException
					var isOverriden = methodInfo.DeclaringType != typeof(ProcessILGenerator);

					return isOverriden;
				}

#if DEBUG_IL
			private static int DebugNodeCount;
			private static int DebugExecutionPathCount;

			private static void DebugExecutionPath(ExecutionPath executionPath)
			{
				DebugExecutionPathCount++;
			}

			private static void DebugNode(Node node, ExecutionPath executionPath)
			{
				DebugNodeCount++;
			}

			[Conditional("DEBUG_IL")]
			private void EmitDebugNode(ILContext ilBuilderContext, Node node, ExecutionPath executionPath)
			{
				EmitLdValue(node);
				EmitLdValue(executionPath);
				IL.Emit(OpCodes.Call, DebugNodeMethodInfo);
			}

			[Conditional("DEBUG_IL")]
			private void EmitDebugExecutionPath(ILContext ilBuilderContext, ExecutionPath executionPath)
			{
				EmitLdValue(executionPath);
				IL.Emit(OpCodes.Call, DebugExecutionPathMethodInfo);
			}
#else

				[SuppressMessage("ReSharper", "UnusedParameter.Local")]
				private static void DebugExecutionPath(ExecutionPath executionPath)
				{
				}

				[SuppressMessage("ReSharper", "UnusedParameter.Local")]
				private static void DebugNode(Node node, ExecutionPath executionPath)
				{
				}

				[Conditional("DEBUG_IL")]
				private void EmitDebugNode(ILContext ilBuilderContext, Node node, ExecutionPath executionPath)
				{
					ilBuilderContext.EmitLdValue(node);
					ilBuilderContext.EmitLdValue(executionPath);
					ilBuilderContext.IL.Emit(OpCodes.Call, DebugNodeMethodInfo);
				}

				[Conditional("DEBUG_IL")]
				private void EmitDebugExecutionPath(ILContext ilBuilderContext, ExecutionPath executionPath)
				{
					ilBuilderContext.EmitLdValue(executionPath);
					ilBuilderContext.IL.Emit(OpCodes.Call, DebugExecutionPathMethodInfo);
				}
#endif

				#endregion

				#region Interface Implementations

				#region Automata<TInstruction,TOperand>.IILBuilder

				public ExecutionPathMethodDelegate Build(ExecutionPathMethodKind kind, ExecutionPath executionPath)
				{
					switch (kind)
					{
						case ExecutionPathMethodKind.Main:
						{
							var dynamicMethod = BuildMain(executionPath, out var closure);

							return (ExecutionPathMethodDelegate)dynamicMethod.CreateDelegate(typeof(ExecutionPathMethodDelegate), closure);
						}
						case ExecutionPathMethodKind.Parallel:
						{
							var dynamicMethod = BuildParallel(executionPath, out var closure);

							return (ExecutionPathMethodDelegate)dynamicMethod.CreateDelegate(typeof(ExecutionPathMethodDelegate), closure);
						}
						default:
							throw new ArgumentOutOfRangeException(nameof(kind));
					}
				}

				private DynamicMethod BuildParallel(ExecutionPath executionPath, out object[] closure)
				{
					var ilBuilderContext = new ILContext(executionPath, ExecutionPathMethodKind.Parallel);

					EmitEnqueueParallelPath(ilBuilderContext);

					for (var index = 0; index < executionPath.Nodes.Length; index++)
					{
						var node = executionPath.Nodes[index];

						switch (node)
						{
							case EnterSyntaxNode enterStateNode:
							{
								AutomataStack.ILGenerator.EmitPush(ilBuilderContext, enterStateNode.SubGraph);

								break;
							}

							case ActionNode actionNode:
							{
								var action = actionNode.ActionEntry.Action;

								if (action.Target != null)
								{
									ilBuilderContext.EmitLdValue(action.Target);
									ilBuilderContext.EmitLdContext();
									ilBuilderContext.IL.Emit(OpCodes.Callvirt, action.Method);
								}
								else
								{
									ilBuilderContext.EmitLdContext();
									ilBuilderContext.IL.Emit(OpCodes.Call, action.Method);
								}

								break;
							}

							case OperandNode operandNode:
							{
								//_instructionPointer.MoveNext();
								ilBuilderContext.EmitMoveInstructionPointer();

								break;
							}

							case PrecedenceEnterNode precedenceEnterNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var successLabel = ilBuilderContext.IL.DefineLabel();

								Thread.ThreadILGenerator.EmitEnterPrecedenceNode(ilBuilderContext, precedenceEnterNode);
								ilBuilderContext.IL.Emit(OpCodes.Brtrue, successLabel);
								EmitGetUnexpectedNode(ilBuilderContext);
								ilBuilderContext.IL.Emit(OpCodes.Ret);
								ilBuilderContext.IL.MarkLabel(successLabel);

								break;
							}

							case PrecedenceLeaveNode precedenceLeaveNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								Thread.ThreadILGenerator.EmitLeavePrecedenceNode(ilBuilderContext, precedenceLeaveNode);

								break;
							}

							case PredicateNode predicateNode:
							{
								//if (predicate.Predicate.Predicate(Context) == false)
								//	return null;

								var predicate = predicateNode.PredicateEntry;
								var trueLabel = ilBuilderContext.IL.DefineLabel();
								var forkLabel = ilBuilderContext.IL.DefineLabel();
								var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(PredicateResult));

								ilBuilderContext.EmitLdProcess();
								ilBuilderContext.EmitExecutionPath();
								ilBuilderContext.IL.Emit(OpCodes.Call, CallPredicateMethodInfo);

								ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);

								ilBuilderContext.IL.Emit(OpCodes.Brtrue, forkLabel);

								EmitGetUnexpectedNode(ilBuilderContext);

								ilBuilderContext.IL.Emit(OpCodes.Ret);

								ilBuilderContext.IL.MarkLabel(forkLabel);

								// ForkPredicate
								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultIsForkMethodInfo);
								ilBuilderContext.IL.Emit(OpCodes.Brfalse, trueLabel);

								EmitBuildForkNode(ilBuilderContext, index, resultLocal);

								ilBuilderContext.IL.Emit(OpCodes.Ret);

								ilBuilderContext.IL.MarkLabel(trueLabel);

								if (predicate.ConsumeResult)
									EmitEnqueuePredicateResult(ilBuilderContext, resultLocal);

								break;
							}

							case BeginSyntaxNode beginSyntaxNode:
							{
								if (beginSyntaxNode.SyntaxGraph.Syntax.CollapseBacktracking)
									EmitEnterForkFrame(ilBuilderContext);

								break;
							}

							case ReturnSyntaxNode returnNode:
							{
									//return stack.Pop().LeaveNode;

									//if (returnNode.SyntaxGraph.Syntax.CollapseBacktracking)
									//{
									//	ilBuilderContext.EmitLdThreadContext();
									//	ilBuilderContext.IL.Emit(OpCodes.Call, ThreadContext.CompleteBlockMethodInfo);
									//}

									AutomataStack.ILGenerator.EmitPopLeaveNode(ilBuilderContext);

								if (returnNode.SyntaxGraph.Syntax.CollapseBacktracking)
										EmitLeaveForkFrame(ilBuilderContext);

								if (index == executionPath.Nodes.Length - 1)
								{
									ilBuilderContext.IL.Emit(OpCodes.Ret);

									closure = ilBuilderContext.Values.ToArray();

									return ilBuilderContext.DynMethod;
								}

								ilBuilderContext.IL.Emit(OpCodes.Pop);

								break;
							}
						}
					}

					if (executionPath.Output == null)
						throw new InvalidOperationException();

					ilBuilderContext.EmitLdValue(executionPath.Output);

					ilBuilderContext.IL.Emit(OpCodes.Ret);

					closure = ilBuilderContext.Values.ToArray();

					return ilBuilderContext.DynMethod;
				}

				private protected virtual Type ILBuilderType => typeof(AutomataContext);

				internal Type ILBuilderTypeInternal => ILBuilderType;

				private DynamicMethod BuildMain(ExecutionPath executionPath, out object[] closure)
				{
					var ilBuilderContext = new ILContext(executionPath, ExecutionPathMethodKind.Main);

					if (executionPath.StackDepth > 0)
					{
						if (IsOverriden(nameof(EmitBeginExecutionPath)))
							EmitBeginExecutionPath(ilBuilderContext, executionPath.StackDepth);

						AutomataStack.ILGenerator.EmitEnsureStackDepth(ilBuilderContext, executionPath.StackDepth);
					}

					{
						if (executionPath.PathSourceNode is LeaveSyntaxNode leaveRuleNode)
						{
							EmitDebugNode(ilBuilderContext, executionPath.PathSourceNode, executionPath);

							var subGraph = leaveRuleNode.SubGraph;

							if (subGraph.SyntaxEntry != null)
							{
								if (IsOverriden(nameof(EmitLeaveSyntaxEntry)))
									EmitLeaveSyntaxEntry(ilBuilderContext, subGraph.SyntaxEntry);
							}
						}
					}

					EmitDebugExecutionPath(ilBuilderContext, executionPath);

					for (var index = 0; index < executionPath.Nodes.Length; index++)
					{
						var node = executionPath.Nodes[index];

						switch (node)
						{
							case LeaveSyntaxNode leaveRuleNode:
							{
								EmitDebugNode(ilBuilderContext, executionPath.PathSourceNode, executionPath);

								var subGraph = leaveRuleNode.SubGraph;

								if (subGraph.SyntaxEntry != null)
								{
									if (IsOverriden(nameof(EmitLeaveSyntaxEntry)))
										EmitLeaveSyntaxEntry(ilBuilderContext, subGraph.SyntaxEntry);
								}

								break;
							}

							case ActionNode actionNode:
							{
								var action = actionNode.ActionEntry.Action;

								if (action.Target != null)
								{
									ilBuilderContext.EmitLdValue(action.Target);
									ilBuilderContext.EmitLdContext();
									ilBuilderContext.IL.Emit(OpCodes.Callvirt, action.Method);
								}
								else
								{
									ilBuilderContext.EmitLdContext();
									ilBuilderContext.IL.Emit(OpCodes.Call, action.Method);
								}

								break;
							}

							case BeginSyntaxNode beginRuleNode:
							{
								if (beginRuleNode.SyntaxGraph.Syntax.CollapseBacktracking)
									EmitEnterForkFrame(ilBuilderContext);
								
								EmitDebugNode(ilBuilderContext, node, executionPath);

								break;
							}

							case PrecedenceEnterNode precedenceEnterNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var successLabel = ilBuilderContext.IL.DefineLabel();

								Thread.ThreadILGenerator.EmitEnterPrecedenceNode(ilBuilderContext, precedenceEnterNode);
								ilBuilderContext.IL.Emit(OpCodes.Brtrue, successLabel);
								EmitGetUnexpectedNode(ilBuilderContext);
								ilBuilderContext.IL.Emit(OpCodes.Ret);
								ilBuilderContext.IL.MarkLabel(successLabel);

								break;
							}

							case PrecedenceLeaveNode precedenceLeaveNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								Thread.ThreadILGenerator.EmitLeavePrecedenceNode(ilBuilderContext, precedenceLeaveNode);

								break;
							}

							case BeginProductionNode beginProductionNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								if (IsOverriden(nameof(EmitEnterProduction)))
									EmitEnterProduction(ilBuilderContext, beginProductionNode.Production);

								break;
							}

							case EndProductionNode endProductionNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								if (IsOverriden(nameof(EmitLeaveProduction)))
									EmitLeaveProduction(ilBuilderContext, endProductionNode.Production);

								break;
							}

							case EnterSyntaxNode enterRuleNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var subGraph = enterRuleNode.SubGraph;

								if (IsOverriden(nameof(EmitEnterSyntaxEntry)))
									EmitEnterSyntaxEntry(ilBuilderContext, subGraph.SyntaxEntry);

								//stack.Push(subGraph);
								AutomataStack.ILGenerator.EmitPush(ilBuilderContext, subGraph);

								break;
							}

							case InlineEnterSyntaxNode inlineEnterRuleNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var ruleEntry = inlineEnterRuleNode.RuleEntry;

								if (IsOverriden(nameof(EmitEnterSyntaxEntry)))
									EmitEnterSyntaxEntry(ilBuilderContext, ruleEntry);

								break;
							}

							case InlineLeaveSyntaxNode inlineLeaveRuleNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var ruleEntry = inlineLeaveRuleNode.RuleEntry;

								if (IsOverriden(nameof(EmitLeaveSyntaxEntry)))
									EmitLeaveSyntaxEntry(ilBuilderContext, ruleEntry);

								break;
							}

							case OperandNode operandNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								if (IsOverriden(nameof(EmitInvoke)))
									EmitInvoke(ilBuilderContext, operandNode.MatchEntry);

								ilBuilderContext.EmitMoveInstructionPointer();

								break;
							}

							case PredicateNode predicateNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								//if (predicate.Predicate.Predicate(Context) == false)
								//	return null;

								var returnPredicateLabel = ilBuilderContext.IL.DefineLabel();
								var callPredicateLabel = ilBuilderContext.IL.DefineLabel();
								var predicate = predicateNode.PredicateEntry;
								var consumeResult = predicate.ConsumeResult && IsOverriden(nameof(EmitConsumePredicateResult));
								var trueLabel = ilBuilderContext.IL.DefineLabel();
								var forkLabel = ilBuilderContext.IL.DefineLabel();
								var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(PredicateResult));

								EmitLdExecuteThreadQueue(ilBuilderContext);

								ilBuilderContext.IL.Emit(OpCodes.Brfalse, callPredicateLabel);

								// Get Result from Process Result Queue
								if (consumeResult)
								{
									// if (predicate.PopResult == false)
									//   return

									ilBuilderContext.EmitLdProcess();
									ilBuilderContext.EmitExecutionPath();
									ilBuilderContext.IL.Emit(OpCodes.Call, ShouldPopPredicateResultMethodInfo);
									ilBuilderContext.IL.Emit(OpCodes.Brfalse, returnPredicateLabel);

									EmitDequePredicateResult(ilBuilderContext);

									ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);

									EmitConsumePredicateResult(ilBuilderContext, resultLocal, predicateNode.PredicateEntry.GetActualPredicateEntry());

									ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
									ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultDisposeMethodInfo);
								}

								ilBuilderContext.IL.Emit(OpCodes.Br, returnPredicateLabel);

								ilBuilderContext.IL.MarkLabel(callPredicateLabel);

								ilBuilderContext.EmitLdProcess();
								ilBuilderContext.EmitExecutionPath();
								ilBuilderContext.IL.Emit(OpCodes.Call, CallPredicateMethodInfo);

								ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);

								ilBuilderContext.IL.Emit(OpCodes.Brtrue, forkLabel);

								EmitGetUnexpectedNode(ilBuilderContext);

								ilBuilderContext.IL.Emit(OpCodes.Ret);

								ilBuilderContext.IL.MarkLabel(forkLabel);

								// ForkPredicate
								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultIsForkMethodInfo);
								ilBuilderContext.IL.Emit(OpCodes.Brfalse, trueLabel);

								EmitBuildForkNode(ilBuilderContext, index, resultLocal);

								ilBuilderContext.IL.Emit(OpCodes.Ret);

								ilBuilderContext.IL.MarkLabel(trueLabel);

								if (consumeResult)
								{
									EmitConsumePredicateResult(ilBuilderContext, resultLocal, predicateNode.PredicateEntry.GetActualPredicateEntry());

									ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
									ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultDisposeMethodInfo);
								}
								else
								{
									ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
									ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultDisposeMethodInfo);
								}

								ilBuilderContext.IL.MarkLabel(returnPredicateLabel);

								break;
							}

							case ValueNode valueNode:
							{
								if (IsOverriden(nameof(EmitValue)))
									EmitValue(ilBuilderContext, valueNode.ValueEntry);

								break;
							}

							case ReturnSyntaxNode returnSyntaxNode:
							{
								EmitDebugNode(ilBuilderContext, node, executionPath);

								var lastNode = index == executionPath.Nodes.Length - 1;
								var leaveNodeLocal = lastNode ? ilBuilderContext.IL.DeclareLocal(typeof(Node)) : null;

								if (lastNode)
								{
									AutomataStack.ILGenerator.EmitPopLeaveNode(ilBuilderContext);

									ilBuilderContext.IL.Emit(OpCodes.Stloc, leaveNodeLocal);
								}
								else
									AutomataStack.ILGenerator.EmitPopNoRet(ilBuilderContext);

								if (returnSyntaxNode.SyntaxGraph.Syntax.CollapseBacktracking)
										EmitLeaveForkFrame(ilBuilderContext);

								if (lastNode)
								{
									ilBuilderContext.IL.Emit(OpCodes.Ldloc, leaveNodeLocal);
									ilBuilderContext.IL.Emit(OpCodes.Ret);

									closure = ilBuilderContext.Values.ToArray();

									return ilBuilderContext.DynMethod;
								}

								break;
							}
						}
					}

					if (executionPath.Output == null)
						throw new InvalidOperationException();

					ilBuilderContext.EmitLdValue(executionPath.Output);

					ilBuilderContext.IL.Emit(OpCodes.Ret);

					closure = ilBuilderContext.Values.ToArray();

					return ilBuilderContext.DynMethod;
				}

				#endregion

				#endregion
			}
		}
	}
}