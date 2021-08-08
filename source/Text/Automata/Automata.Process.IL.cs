// <copyright file="Automata.Process.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private Process.ProcessILGenerator ILGenerator { get; } = new();

		partial class Process
		{
			public partial class ProcessILGenerator
			{
				private static readonly FieldInfo ContextFieldInfo = ProcessType.GetField(nameof(_context), IPNP | GF);
				private static readonly MethodInfo ExecuteThreadQueueMethodInfo = ProcessType.GetMethod(nameof(GetExecuteThreadQueue), IPNP);
				private static readonly MethodInfo MoveNextMethodInfo = ProcessType.GetMethod(nameof(MoveInstructionPointer), IPNP);
				private static readonly MethodInfo GetAutomataStackMethodInfo = ProcessType.GetMethod(nameof(GetAutomataStack), IPNP);
				private static readonly MethodInfo ProcessEnqueuePredicateResultMethodInfo = ProcessType.GetMethod(nameof(EnqueuePredicateResult), IPNP);
				private static readonly MethodInfo ProcessDequePredicateResultMethodInfo = ProcessType.GetMethod(nameof(DequeuePredicateResult), IPNP);
				private static readonly MethodInfo GetUnexpectedNodeMethodInfo = ProcessType.GetMethod(nameof(GetUnexpectedNode), IPNP);
				private static readonly MethodInfo BuildForkNodeMethodInfo = ProcessType.GetMethod(nameof(BuildForkNode), IPNP);
				private static readonly MethodInfo EnqueueParallelPathMethodInfo = ProcessType.GetMethod(nameof(EnqueueParallelPath), IPNP);

				public readonly ExecutionPathMethodCollection MainExecutionMethods;
				public readonly ExecutionPathMethodCollection ParallelExecutionMethods;

				public ProcessILGenerator()
				{
					MainExecutionMethods = new ExecutionPathMethodCollection(ExecutionPathMethodKind.Main, this);
					ParallelExecutionMethods = new ExecutionPathMethodCollection(ExecutionPathMethodKind.Parallel, this);
				}

				public void Emit(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ProcessDequePredicateResultMethodInfo);
				}

				public void EmitBuildForkNode(Context context, int nodeIndex, LocalBuilder resultLocal)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldc_I4, nodeIndex);
					context.EmitExecutionPath();
					context.IL.Emit(OpCodes.Ldloc, resultLocal);
					context.IL.Emit(OpCodes.Call, BuildForkNodeMethodInfo);
				}

				public void EmitDequePredicateResult(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ProcessDequePredicateResultMethodInfo);
				}

				public void EmitEnqueueParallelPath(Context context)
				{
					context.EmitLdProcess();
					context.EmitExecutionPath();
					context.IL.Emit(OpCodes.Call, EnqueueParallelPathMethodInfo);
				}

				public void EmitEnqueuePredicateResult(Context context, LocalBuilder resultLocal)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldloc, resultLocal);
					context.IL.Emit(OpCodes.Call, ProcessEnqueuePredicateResultMethodInfo);
				}

				public void EmitGetUnexpectedNode(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, GetUnexpectedNodeMethodInfo);
				}

				public void EmitLdContext(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldfld, ContextFieldInfo);
				}

				public void EmitLdExecuteThreadQueue(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ExecuteThreadQueueMethodInfo);
				}

				private void EmitLdStack(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, GetAutomataStackMethodInfo);
				}

				public void EmitMoveNext(Context context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, MoveNextMethodInfo);
				}
			}
		}
	}
}