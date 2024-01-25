// <copyright file="Automata.Process.ProcessResources.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			private sealed class ProcessResources
			{
				private static readonly ThreadLocal<Dictionary<Automata<TInstruction, TOperand>, ProcessResources>> ThreadLocalDictionary = new(() => new Dictionary<Automata<TInstruction, TOperand>, ProcessResources>());

				public readonly ArrayPool<int> IntArrayPool = ArrayPool<int>.Create(1024 * 1024 * 4, 256);
				public readonly ArrayPool<int> StackArrayPool = ArrayPool<int>.Create(1024 * 1024 * 4, 256);
				public readonly Pool<NfaTransitionBuilder> NfaTransitionBuilderPool;
				public readonly MemorySpanAllocator<int> DynamicMemorySpanAllocator;

				public readonly Pool<ExceptionAutomataResult> ExceptionAutomataResultPool;
				public readonly Pool<ExecutionStream> ExecutionStreamPool;
				public readonly Pool<ForkAutomataResult> ForkAutomataResultPool;
				public readonly Pool<ExecutionPath> ForkExecutionPathPool;
				public readonly Pool<ForkNode> ForkNodePool;
				public readonly Pool<PredicateNode> ForkPredicateNodePool;
				public readonly Pool<InstructionStream> InstructionStreamPool;
				public readonly PrecedenceContextPool PrecedencePool;
				public readonly MemorySpanAllocator<int> PrecedenceSpanAllocator;
				public readonly Pool<PredicateResultStream> PredicateResultStreamPool;
				public readonly AutomataStackPool StackPool;
				public readonly MemorySpanAllocator<int> StackSpanAllocator;
				public readonly Pool<SuccessAutomataResult> SuccessAutomataResultPool;
				public readonly Pool<ExecutionRailBuilder> ThreadTransitionPool;
				public readonly AutomataStackCleaner.NodePool AutomataStackCleanNodePool = new();
				public readonly PrecedenceStackCleaner.NodePool PrecedenceStackCleanerNodePool = new();
				public readonly PrecedenceContextCleaner.NodePool PrecedenceContextCleanerNodePool = new();
				public readonly ResetForkNodePool ResetForkNodePool = new();

				private ProcessResources(Automata<TInstruction, TOperand> automata)
				{
					DynamicMemorySpanAllocator = MemorySpanAllocator.Create(IntArrayPool, false);
					PrecedenceSpanAllocator = MemorySpanAllocator.Create(IntArrayPool, false);
					StackSpanAllocator = MemorySpanAllocator.Create(StackArrayPool, false);

					StackPool = new AutomataStackPool(automata, StackSpanAllocator);
					PrecedencePool = new PrecedenceContextPool(automata, PrecedenceSpanAllocator);
					InstructionStreamPool = automata?.CreateInstructionStreamPool() ?? new Pool<InstructionStream>(p => new InstructionStream(p));
					ExecutionStreamPool = new Pool<ExecutionStream>(p => new ExecutionStream(DynamicMemorySpanAllocator, p));
					PredicateResultStreamPool = new Pool<PredicateResultStream>(p => new PredicateResultStream(MemorySpanAllocator<PredicateResult>.Shared, p));
					ForkExecutionPathPool = new Pool<ExecutionPath>(p => new ExecutionPath(automata, p));
					ForkPredicateNodePool = new Pool<PredicateNode>(p => new PredicateNode(automata, p));
					ForkNodePool = new Pool<ForkNode>(p => new ForkNode(automata, p));
					SuccessAutomataResultPool = new Pool<SuccessAutomataResult>(p => new SuccessAutomataResult(p));
					ExceptionAutomataResultPool = new Pool<ExceptionAutomataResult>(p => new ExceptionAutomataResult(p));
					ForkAutomataResultPool = new Pool<ForkAutomataResult>(p => new ForkAutomataResult(p));
					NfaTransitionBuilderPool = new Pool<NfaTransitionBuilder>(p => new NfaTransitionBuilder(automata, p));
					ThreadTransitionPool = new Pool<ExecutionRailBuilder>(p => new ExecutionRailBuilder(p));
				}

				public static ProcessResources Get(Automata<TInstruction, TOperand> automata)
				{
					var threadLocalDictionary = ThreadLocalDictionary.Value;

					if (threadLocalDictionary.TryGetValue(automata, out var resources) == false)
						threadLocalDictionary[automata] = resources = new ProcessResources(automata);

					return resources;
				}
			}
		}
	}
}