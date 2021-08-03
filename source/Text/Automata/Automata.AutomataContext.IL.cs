// <copyright file="Automata.AutomataContext.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected abstract partial class AutomataContext : IILBuilder
		{
			#region Static Fields and Constants

			private static readonly MethodInfo PushMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.Push), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo PushSafeIdMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.PushSafeId), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo EnsureDeltaMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.EnsureDelta), BindingFlags.Instance | BindingFlags.Public);
			
			private static readonly MethodInfo MoveNextMethodInfo = typeof(Process).GetMethod(nameof(Process.MoveInstructionPointer), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo PopMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.Pop), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo PopSafeMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.PopSafe), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo PopSafeNodeMethodInfo = typeof(AutomataStack).GetMethod(nameof(AutomataStack.PopSafeNode), BindingFlags.Instance | BindingFlags.Public);

			private static readonly FieldInfo LeaveNodeFieldInfo = typeof(SubGraph).GetField(nameof(SubGraph.LeaveNode), BindingFlags.Instance | BindingFlags.Public);
			private static readonly MethodInfo ProcessEnqueuePredicateResultMethodInfo = typeof(Process).GetMethod(nameof(Process.EnqueuePredicateResult), BindingFlags.Instance | BindingFlags.NonPublic);
			private static readonly MethodInfo ProcessDequePredicateResultMethodInfo = typeof(Process).GetMethod(nameof(Process.DequePredicateResult), BindingFlags.Instance | BindingFlags.NonPublic);
			private static readonly MethodInfo DebugNodeMethodInfo = typeof(AutomataContext).GetMethod(nameof(DebugNode), BindingFlags.Static | BindingFlags.NonPublic);
			private static readonly MethodInfo DebugExecutionPathMethodInfo = typeof(AutomataContext).GetMethod(nameof(DebugExecutionPath), BindingFlags.Static | BindingFlags.NonPublic);
			private static readonly MethodInfo PredicateResultIsForkMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.IsFork), BindingFlags.Instance | BindingFlags.NonPublic);
			private static readonly MethodInfo BuildForkNodeMethodInfo = typeof(Process).GetMethod("BuildForkNode", BindingFlags.Instance | BindingFlags.NonPublic);
			private static readonly MethodInfo CallPredicateMethodInfo = typeof(AutomataContext).GetMethod(nameof(CallPredicate), BindingFlags.Instance | BindingFlags.NonPublic);
			private static readonly MethodInfo ShouldPopPredicateResultMethodInfo = typeof(AutomataContext).GetMethod(nameof(ShouldPopPredicateResult), BindingFlags.Instance | BindingFlags.NonPublic);

			private static readonly MethodInfo PredicateResultDisposeMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.Dispose), BindingFlags.Instance | BindingFlags.NonPublic);

			private static readonly FieldInfo ContextFieldInfo = typeof(Process).GetField(nameof(Process.Context), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);
			private static readonly FieldInfo ExecuteThreadQueueFieldInfo = typeof(Process).GetField(nameof(Process.ExecuteThreadQueue), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetField);

			#endregion

			#region Methods

			protected virtual void BuildEnterRuleEntry(ILBuilderContext ilBuilderContext, RuleEntry ruleEntry)
			{
			}

			protected virtual void BuildEnterProduction(ILBuilderContext ilBuilderContext, Production production)
			{
			}

			protected virtual void BuildInvoke(ILBuilderContext ilBuilderContext, MatchEntry matchEntry, bool main)
			{
			}

			protected virtual void BuildConsumePredicateResult(ILBuilderContext ilBuilderContext, LocalBuilder resultLocal, PredicateEntryBase predicateEntryBase)
			{
			}

			protected virtual void BuildLeaveRuleEntry(ILBuilderContext ilBuilderContext, RuleEntry ruleEntry)
			{
			}

			protected virtual void BuildBeginExecutionPath(ILBuilderContext ilBuilderContext, int stackDelta)
			{
			}

			protected virtual void BuildLeaveProduction(ILBuilderContext ilBuilderContext, Production production)
			{
			}

			private bool IsOverriden(string methodName)
			{
				var methodInfo = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

				// ReSharper disable once PossibleNullReferenceException
				return methodInfo.DeclaringType != typeof(AutomataContext);
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
			private void EmitDebugNode(ILBuilderContext ilBuilderContext, Node node, ExecutionPath executionPath)
			{
				ilBuilderContext.EmitLdValue(node);
				ilBuilderContext.EmitLdValue(executionPath);
				ilBuilderContext.IL.Emit(OpCodes.Call, DebugNodeMethodInfo);
			}

			[Conditional("DEBUG_IL")]
			private void EmitDebugExecutionPath(ILBuilderContext ilBuilderContext, ExecutionPath executionPath)
			{
				ilBuilderContext.EmitLdValue(executionPath);
				ilBuilderContext.IL.Emit(OpCodes.Call, DebugExecutionPathMethodInfo);
			}
#endif

			#endregion

			#region Interface Implementations

			#region Automata<TInstruction,TOperand>.IILBuilder

			Func<Process, AutomataStack, object[], Node> IILBuilder.BuildMain(ExecutionPath executionPath, out object[] closure)
			{
				var mainMethod = BuildMain(executionPath, out closure);

				return (Func<Process, AutomataStack, object[], Node>) mainMethod.CreateDelegate(typeof(Func<Process, AutomataStack, object[], Node>), executionPath);
			}

			Func<Process, AutomataStack, object[], Node> IILBuilder.BuildParallel(ExecutionPath executionPath, out object[] closure)
			{
				var parallelMethod = BuildParallel(executionPath, out closure);

				return (Func<Process, AutomataStack, object[], Node>) parallelMethod.CreateDelegate(typeof(Func<Process, AutomataStack, object[], Node>), executionPath);
			}

			private DynamicMethod BuildParallel(ExecutionPath executionPath, out object[] closure)
			{
				var ilBuilderContext = new ILBuilderContext(ContextFieldInfo);

				for (var index = 0; index < executionPath.Nodes.Length; index++)
				{
					var node = executionPath.Nodes[index];

					switch (node)
					{
						case EnterRuleNode enterStateNode:
						{
							var subGraph = enterStateNode.SubGraph;

							//stack.Push(subGraph);
							ilBuilderContext.EmitLdStack();
							ilBuilderContext.EmitLdValue(subGraph);
							ilBuilderContext.IL.Emit(OpCodes.Call, PushMethodInfo);

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
							if (IsOverriden(nameof(BuildInvoke)))
								BuildInvoke(ilBuilderContext, operandNode.MatchEntry, false);

							//_instructionPointer.MoveNext();
							ilBuilderContext.EmitLdProcess();
							ilBuilderContext.IL.Emit(OpCodes.Call, MoveNextMethodInfo);

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

							ilBuilderContext.EmitLdContext();
							ilBuilderContext.EmitExecutionPath();
							ilBuilderContext.IL.Emit(OpCodes.Call, CallPredicateMethodInfo);

							ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);

							ilBuilderContext.IL.Emit(OpCodes.Brtrue, forkLabel);

							ilBuilderContext.IL.Emit(OpCodes.Ldnull);
							ilBuilderContext.IL.Emit(OpCodes.Ret);

							ilBuilderContext.IL.MarkLabel(forkLabel);

							// ForkPredicate
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultIsForkMethodInfo);
							ilBuilderContext.IL.Emit(OpCodes.Brfalse, trueLabel);
							ilBuilderContext.EmitLdProcess();
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, index);
							ilBuilderContext.EmitExecutionPath();
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Call, BuildForkNodeMethodInfo);
							ilBuilderContext.IL.Emit(OpCodes.Ret);

							ilBuilderContext.IL.MarkLabel(trueLabel);

							if (predicate.ConsumeResult)
							{
								ilBuilderContext.EmitLdProcess();
								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Call, ProcessEnqueuePredicateResultMethodInfo);
							}

							break;
						}

						case ReturnRuleNode _:
						{
							//return stack.Pop().LeaveNode;
							ilBuilderContext.EmitLdStack();
							ilBuilderContext.IL.Emit(OpCodes.Call, PopMethodInfo);
							ilBuilderContext.IL.Emit(OpCodes.Ldfld, LeaveNodeFieldInfo);

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

				ilBuilderContext.EmitLdValue(executionPath.Output);

				ilBuilderContext.IL.Emit(OpCodes.Ret);

				closure = ilBuilderContext.Values.ToArray();

				return ilBuilderContext.DynMethod;
			}

			private protected virtual Type ILBuilderType => typeof(AutomataContext);

			internal Type ILBuilderTypeInternal => ILBuilderType;

			private PredicateResult CallPredicate(ExecutionPath executionPath)
			{
				var predicateNode = (PredicateNode) executionPath.Nodes[executionPath.Nodes.Length - 1];

				return predicateNode.PredicateEntry.PassInternal(this);
			}

			private bool ShouldPopPredicateResult(ExecutionPath executionPath)
			{
				var predicateNode = (PredicateNode) executionPath.Nodes[executionPath.Nodes.Length - 1];

				return predicateNode.PredicateEntry.PopResult;
			}

			private DynamicMethod BuildMain(ExecutionPath executionPath, out object[] closure)
			{
				var ilBuilderContext = new ILBuilderContext(ContextFieldInfo);

				if (executionPath.EnterReturnNodes.Length > 0)
				{
					if (IsOverriden(nameof(BuildBeginExecutionPath)))
						BuildBeginExecutionPath(ilBuilderContext, executionPath.StackEvalDelta);

					ilBuilderContext.EmitLdStack();
					ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, executionPath.StackEvalDelta);
					ilBuilderContext.IL.Emit(OpCodes.Call, EnsureDeltaMethodInfo);
				}

				{
					if (executionPath.PathSourceNode is LeaveRuleNode leaveStateNode)
					{
						EmitDebugNode(ilBuilderContext, executionPath.PathSourceNode, executionPath);

						var subGraph = leaveStateNode.SubGraph;

						if (subGraph.RuleEntry != null)
						{
							if (IsOverriden(nameof(BuildLeaveRuleEntry)))
								BuildLeaveRuleEntry(ilBuilderContext, subGraph.RuleEntry);
						}
					}
				}

				EmitDebugExecutionPath(ilBuilderContext, executionPath);

				for (var index = 0; index < executionPath.Nodes.Length; index++)
				{
					var node = executionPath.Nodes[index];

					switch (node)
					{
						case LeaveRuleNode leaveStateNode:
						{
							EmitDebugNode(ilBuilderContext, executionPath.PathSourceNode, executionPath);

							var subGraph = leaveStateNode.SubGraph;

							if (subGraph.RuleEntry != null)
							{
								if (IsOverriden(nameof(BuildLeaveRuleEntry)))
									BuildLeaveRuleEntry(ilBuilderContext, subGraph.RuleEntry);
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

						case BeginRuleNode beginStateNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							break;
						}

						case BeginProductionNode beginProductionNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							if (IsOverriden(nameof(BuildEnterProduction)))
								BuildEnterProduction(ilBuilderContext, beginProductionNode.Production);

							break;
						}

						case EndProductionNode endProductionNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							if (IsOverriden(nameof(BuildLeaveProduction)))
								BuildLeaveProduction(ilBuilderContext, endProductionNode.Production);

							break;
						}

						case EnterRuleNode enterStateNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							var subGraph = enterStateNode.SubGraph;

							if (IsOverriden(nameof(BuildEnterRuleEntry)))
								BuildEnterRuleEntry(ilBuilderContext, subGraph.RuleEntry);

							//stack.Push(subGraph);

							ilBuilderContext.EmitLdStack();
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, subGraph.Id);
							ilBuilderContext.IL.Emit(OpCodes.Call, PushSafeIdMethodInfo);


							break;
						}

						case InlineEnterRuleNode inlineEnterStateNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							var stateEntry = inlineEnterStateNode.RuleEntry;

							if (IsOverriden(nameof(BuildEnterRuleEntry)))
								BuildEnterRuleEntry(ilBuilderContext, stateEntry);

							break;
						}

						case InlineLeaveRuleNode inlineLeaveStateNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							var stateEntry = inlineLeaveStateNode.RuleEntry;

							if (IsOverriden(nameof(BuildLeaveRuleEntry)))
								BuildLeaveRuleEntry(ilBuilderContext, stateEntry);

							break;
						}

						case OperandNode operandNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);

							if (IsOverriden(nameof(BuildInvoke)))
								BuildInvoke(ilBuilderContext, operandNode.MatchEntry, true);

							//_instructionPointer.MoveNext();
							ilBuilderContext.EmitLdProcess();
							ilBuilderContext.IL.Emit(OpCodes.Call, MoveNextMethodInfo);

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
							var consumeResult = predicate.ConsumeResult && IsOverriden(nameof(BuildConsumePredicateResult));
							var trueLabel = ilBuilderContext.IL.DefineLabel();
							var forkLabel = ilBuilderContext.IL.DefineLabel();
							var resultLocal = ilBuilderContext.IL.DeclareLocal(typeof(PredicateResult));

							ilBuilderContext.EmitLdProcess();
							ilBuilderContext.IL.Emit(OpCodes.Ldfld, ExecuteThreadQueueFieldInfo);
							ilBuilderContext.IL.Emit(OpCodes.Brfalse, callPredicateLabel);

							// Get Result from Process Result Queue
							if (consumeResult)
							{
								// if (predicate.PopResult == false)
								//   return

								ilBuilderContext.EmitLdContext();
								ilBuilderContext.EmitExecutionPath();
								ilBuilderContext.IL.Emit(OpCodes.Call, ShouldPopPredicateResultMethodInfo);
								ilBuilderContext.IL.Emit(OpCodes.Brfalse, returnPredicateLabel);

								ilBuilderContext.EmitLdProcess();
								ilBuilderContext.IL.Emit(OpCodes.Call, ProcessDequePredicateResultMethodInfo);
								ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);

								BuildConsumePredicateResult(ilBuilderContext, resultLocal, predicateNode.PredicateEntry.GetActualPredicateEntry());

								ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
								ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultDisposeMethodInfo);
							}

							ilBuilderContext.IL.Emit(OpCodes.Br, returnPredicateLabel);

							ilBuilderContext.IL.MarkLabel(callPredicateLabel);

							ilBuilderContext.EmitLdContext();
							ilBuilderContext.EmitExecutionPath();
							ilBuilderContext.IL.Emit(OpCodes.Call, CallPredicateMethodInfo);

							ilBuilderContext.IL.Emit(OpCodes.Stloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);

							ilBuilderContext.IL.Emit(OpCodes.Brtrue, forkLabel);
							ilBuilderContext.IL.Emit(OpCodes.Ldnull);
							ilBuilderContext.IL.Emit(OpCodes.Ret);

							ilBuilderContext.IL.MarkLabel(forkLabel);

							// ForkPredicate
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Callvirt, PredicateResultIsForkMethodInfo);
							ilBuilderContext.IL.Emit(OpCodes.Brfalse, trueLabel);
							ilBuilderContext.EmitLdProcess();
							ilBuilderContext.IL.Emit(OpCodes.Ldc_I4, index);
							ilBuilderContext.EmitExecutionPath();
							ilBuilderContext.IL.Emit(OpCodes.Ldloc, resultLocal);
							ilBuilderContext.IL.Emit(OpCodes.Call, BuildForkNodeMethodInfo);
							ilBuilderContext.IL.Emit(OpCodes.Ret);

							ilBuilderContext.IL.MarkLabel(trueLabel);

							if (consumeResult)
							{
								BuildConsumePredicateResult(ilBuilderContext, resultLocal, predicateNode.PredicateEntry.GetActualPredicateEntry());

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

						case ReturnRuleNode returnStateNode:
						{
							EmitDebugNode(ilBuilderContext, node, executionPath);
							
							var lastNode = index == executionPath.Nodes.Length - 1;
							var leaveNodeLocal = lastNode ? ilBuilderContext.IL.DeclareLocal(typeof(Node)) : null;


							ilBuilderContext.EmitLdStack();
							
							if (lastNode)
							{
								ilBuilderContext.IL.Emit(OpCodes.Call, PopSafeNodeMethodInfo);
								ilBuilderContext.IL.Emit(OpCodes.Stloc, leaveNodeLocal);
							}
							else
							{
								ilBuilderContext.IL.Emit(OpCodes.Call, PopSafeMethodInfo);
							}
							
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

				ilBuilderContext.EmitLdValue(executionPath.Output);

				ilBuilderContext.IL.Emit(OpCodes.Ret);

				closure = ilBuilderContext.Values.ToArray();

				return ilBuilderContext.DynMethod;
			}

			#endregion

			#endregion
		}

		#endregion
	}
}