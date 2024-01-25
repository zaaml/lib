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
		private protected abstract class ExecutionMethodBuilder
		{
			protected ExecutionMethodBuilder(int builderIndex)
			{
				BuilderIndex = builderIndex;
			}

			public int BuilderIndex { get; }

			public abstract void BuildExecutionMethods(ExecutionPath executionPath);
		}

		partial class Process
		{
			private static ProcessILGenerator StaticILGenerator { get; set; }

			private protected partial class ProcessILGenerator
			{
				private static readonly MethodInfo ExecuteThreadQueueMethodInfo = ProcessType.GetMethod(nameof(GetExecuteThreadQueue), IPNP);
				private static readonly MethodInfo ProcessEnqueuePredicateResultMethodInfo = ProcessType.GetMethod(nameof(EnqueuePredicateResult), IPNP);
				private static readonly MethodInfo ProcessDequePredicateResultMethodInfo = ProcessType.GetMethod(nameof(DequeuePredicateResult), IPNP);

				private static readonly FieldInfo UnexpectedNodeFieldInfo = typeof(UnexpectedNode).GetField(nameof(UnexpectedNode.Instance), SPNP);

				private static readonly MethodInfo BuildForkNodeMethodInfo = ProcessType.GetMethod(nameof(BuildForkNode), IPNP);
				private static readonly MethodInfo EnqueueParallelPathMethodInfo = ThreadContextType.GetMethod(nameof(ThreadContext.EnqueueParallelPath), IPNP);

				public static readonly FieldInfo ProcessThreadsFieldInfo = ProcessType.GetField(nameof(_threads), IPNP);

				private static readonly MethodInfo EnterForkFrameMethodInfo = ProcessType.GetMethod(nameof(EnterForkFrame), IPNP);
				private static readonly MethodInfo LeaveForkFrameMethodInfo = ProcessType.GetMethod(nameof(LeaveForkFrame), IPNP);

				public readonly int MainExecutionMethodIndex;
				public readonly int ParallelExecutionMethodIndex;

				public ProcessILGenerator(Automata<TInstruction, TOperand> automata)
				{
					Automata = automata;
					ExecutionMethodBuilder = Automata.RegisterExecutionMethodBuilder(i => new GeneratorExecutionMethodBuilder(i, this));
					MainExecutionMethodIndex = ExecutionMethodBuilder.BuilderIndex;
					ParallelExecutionMethodIndex = ExecutionMethodBuilder.BuilderIndex + 1;
				}

				private GeneratorExecutionMethodBuilder ExecutionMethodBuilder { get; }

				private Automata<TInstruction, TOperand> Automata { get; }

				public void EmitEnterForkFrame(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, EnterForkFrameMethodInfo);
				}

				public void EmitLeaveForkFrame(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, LeaveForkFrameMethodInfo);
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
					context.EmitLdThreadContext();
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
					context.IL.Emit(OpCodes.Ldsfld, UnexpectedNodeFieldInfo);
				}

				public void EmitLdExecuteThreadQueue(ILContext context)
				{
					context.EmitLdProcess();
					context.IL.Emit(OpCodes.Call, ExecuteThreadQueueMethodInfo);
				}

				private sealed class GeneratorExecutionMethodBuilder : ExecutionMethodBuilder
				{
					private readonly ProcessILGenerator _ilGenerator;

					public GeneratorExecutionMethodBuilder(int builderIndex, ProcessILGenerator ilGenerator) : base(builderIndex)
					{
						_ilGenerator = ilGenerator;
						LazyMainExecutionMethodDelegate = LazyMainExecutionMethod;
						LazyParallelExecutionMethodDelegate = LazyParallelExecutionMethod;
					}

					private ExecutionPathMethodDelegate LazyMainExecutionMethodDelegate { get; }

					private ExecutionPathMethodDelegate LazyParallelExecutionMethodDelegate { get; }

					private int MainIndex => BuilderIndex * 2;

					private int ParallelIndex => MainIndex + 1;

					private Node LazyMainExecutionMethod(ExecutionPath executionPath, Process process)
					{
						var method = executionPath.IsForkExecutionPath ? ExecuteForkPathMainDelegate : _ilGenerator.Build(ExecutionPathMethodKind.Main, executionPath);
						var node = method(executionPath, process);

						executionPath.BindExecutionMethod(MainIndex, method);

						return node;
					}

					private Node LazyParallelExecutionMethod(ExecutionPath executionPath, Process process)
					{
						var method = executionPath.IsForkExecutionPath ? ExecuteForkPathParallelDelegate : _ilGenerator.Build(ExecutionPathMethodKind.Parallel, executionPath);
						var node = method(executionPath, process);

						executionPath.BindExecutionMethod(ParallelIndex, method);

						return node;
					}

					public override void BuildExecutionMethods(ExecutionPath executionPath)
					{
						executionPath.BindExecutionMethod(MainIndex, LazyMainExecutionMethodDelegate);
						executionPath.BindExecutionMethod(ParallelIndex, LazyParallelExecutionMethodDelegate);
					}
				}
			}
		}
	}
}