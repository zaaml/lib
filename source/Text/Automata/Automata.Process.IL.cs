// <copyright file="Automata.Process.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

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
			internal partial class ProcessILGenerator
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

				public void Emit(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ProcessDequePredicateResultMethodInfo);
				}

				public void EmitBuildForkNode(ILContext context, int nodeIndex, LocalBuilder resultLocal)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldc_I4, nodeIndex);
					context.EmitExecutionPath();
					context.IL.Emit(OpCodes.Ldloc, resultLocal);
					context.IL.Emit(OpCodes.Call, BuildForkNodeMethodInfo);
				}

				public void EmitDequePredicateResult(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ProcessDequePredicateResultMethodInfo);
				}

				public void EmitEnqueueParallelPath(ILContext context)
				{
					context.EmitLdProcess();
					context.EmitExecutionPath();
					context.IL.Emit(OpCodes.Call, EnqueueParallelPathMethodInfo);
				}

				public void EmitEnqueuePredicateResult(ILContext context, LocalBuilder resultLocal)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldloc, resultLocal);
					context.IL.Emit(OpCodes.Call, ProcessEnqueuePredicateResultMethodInfo);
				}

				public void EmitGetUnexpectedNode(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, GetUnexpectedNodeMethodInfo);
				}

				public void EmitLdContext(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Ldfld, ContextFieldInfo);
				}

				public void EmitLdExecuteThreadQueue(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ExecuteThreadQueueMethodInfo);
				}

				public void EmitLdStack(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, GetAutomataStackMethodInfo);
				}

				public void EmitMoveNext(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, MoveNextMethodInfo);
				}
			}
		}
	}
}