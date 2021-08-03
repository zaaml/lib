// <copyright file="Automata.AutomataStack.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Zaaml.Core;
using Zaaml.Core.Pools;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private sealed class AutomataExecutionContext : PoolSharedObject<AutomataExecutionContext>
		{
			public AutomataStack Stack { get; private set; }
			
			public Node Node { get; private set; }
			
			public InstructionStream InstructionStream { get; private set; }
			
			public int InstructionPointer { get; private set; }

			private AutomataExecutionContext(IPool<AutomataExecutionContext> pool) : base(pool)
			{
			}

			public AutomataExecutionContext Mount(InstructionStream instructionStream, int instructionPointer, Node node, AutomataStack stack)
			{
				InstructionStream = instructionStream.AddReference();
				InstructionPointer = instructionPointer;
				Node = node;
				Stack = stack.AddReference();

				return AddReference();
			}

			public void Release()
			{
				InstructionStream.ReleaseReference();
				Stack.ReleaseReference();
				Node = null;
				InstructionStream = null;
				Stack = null;
				InstructionPointer = 0;
			}

			public AutomataExecutionContext Clone()
			{
				var clone = Pool.Get().AddReference();

				clone.Stack = Stack.Clone();
				clone.InstructionStream = InstructionStream.AddReference();
				clone.InstructionPointer = InstructionPointer;
				clone.Node = Node;

				return clone;
			}

			public void Eval(ExecutionPath executionPath)
			{
				var pointer = InstructionPointer;

				Node = Stack.Eval(executionPath);
				InstructionStream.Move(executionPath.LookAheadMatch.Length, ref pointer);
				InstructionPointer = pointer;
			}

			public void Sync(InstructionStream instructionStream, int instructionPointer, Node node, AutomataStack stack)
			{
				InstructionStream = instructionStream;
				InstructionPointer = instructionPointer;
				Node = node;
				Stack.Sync(stack);
			}

			public void EvalReturn(int depth)
			{
				Eval(Node.ReturnPathSafe);

				while (--depth > 0) 
					Eval(Stack.PeekLeaveNode().ReturnPathSafe);
			}

			public int ReturnDepth => Node.HasReturnPathSafe ? 1 + Stack.ReturnDepth : 0;
		}

		// ReSharper disable once MemberCanBeProtected.Local
		private sealed class AutomataStack : PoolSharedObject<AutomataStack>
		{
			private const int DefaultCapacity = 8;
			private readonly Automata<TInstruction, TOperand> _automata;
			private int _hashCode;
			private int _hashCodeDepth;
			private bool _hashCodeDirty;
			private int _returnDepth;
			public int[] Array;
			public int Count;
			private readonly List<SubGraph> _automataSubGraphRegistry;

			public AutomataStack(Automata<TInstruction, TOperand> automata, Pool<AutomataStack> pool) : base(pool)
			{
				_automata = automata;
				_automataSubGraphRegistry = _automata._subGraphRegistry;
				Array = new int[DefaultCapacity];
			}

			private AutomataStack(AutomataStack source) : base(source.Pool)
			{
				_automata = source._automata;
				_automataSubGraphRegistry = _automata._subGraphRegistry;
				_hashCode = source.GetHashCode();

				Count = source._hashCodeDepth;
				Array = new int[Count];

				_hashCodeDirty = false;
				_hashCodeDepth = Count;
				_returnDepth = source._returnDepth;

				System.Array.Copy(source.Array, source.Count - Count, Array, 0, Count);
			}

			public IEnumerable<string> DebugItems => Array.Take(Count).Select(id => _automataSubGraphRegistry[id].ToString());

			public string DebugView => string.Join("\n", DebugItems);

			private static bool AreEqual(AutomataStack collection1, AutomataStack collection2)
			{
				var span1 = new Span<int>(collection1.Array, collection1.Count - collection1._hashCodeDepth, collection1._hashCodeDepth);
				var span2 = new Span<int>(collection2.Array, collection2.Count - collection2._hashCodeDepth, collection2._hashCodeDepth);

				return span1.SequenceEqual(span2);
			}

			public void Clear()
			{
				_hashCode = 0;
				_returnDepth = 0;
				_hashCodeDepth = 0;
				_hashCodeDirty = false;

				Count = 0;
			}

			public void FromSpan(Span<int> span)
			{
				var capacity = Array.Length;

				if (span.Length >= capacity)
				{
					capacity *= 2;

					while (span.Length >= capacity) 
						capacity *= 2;

					Resize(capacity);
				}

				Count = span.Length;

				span.CopyTo(new Span<int>(Array, 0, Count));
				_hashCodeDirty = true;
			}

			public AutomataStack Clone()
			{
				var clone = Pool.Get().AddReference();

				clone.CopyFrom(this);

				return clone;
			}

			public int ReturnDepth
			{
				get
				{
					if (_hashCodeDirty)
						EvalHashCode();

					return _returnDepth;
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void CopyFrom(AutomataStack stack)
			{
				if (Array.Length < stack.Array.Length)
					Array = new int[stack.Array.Length];

				Count = stack.Count;

				_hashCode = stack._hashCode;
				_hashCodeDirty = stack._hashCodeDirty;
				_returnDepth = stack._returnDepth;
				_hashCodeDepth = stack._hashCodeDepth;

				System.Array.Copy(stack.Array, Array, Count);
			}

			public void Deserialize(int[] binaryStack)
			{
				Count = binaryStack.Length - 1;

				if (Array.Length < Count)
					EnsureCapacity(Count);

				for (var index = 1; index < binaryStack.Length; index++)
				{
					var subGraphId = binaryStack[index];

					Array[index - 1] = subGraphId;
				}
			}

			private void EnsureCapacity(int requiredCapacity)
			{
				var capacity = Array.Length;

				while (capacity < requiredCapacity)
					capacity <<= 1;

				if (capacity != Array.Length)
					Resize(capacity);
			}

			public override bool Equals(object obj)
			{
				return AreEqual(this, (AutomataStack) obj);
			}

			public Node Eval(ExecutionPath path)
			{
				Node output = null;

				while (Count + path.StackEvalDelta >= Array.Length)
					Resize(Array.Length * 2);

				foreach (var node in path.EnterReturnNodes)
				{
					if (node is EnterRuleNode enterStateNode)
						Array[Count++] = enterStateNode.SubGraph.Id;
					else
						output = _automataSubGraphRegistry[Array[--Count]].LeaveNode;
				}

				if (path.Output is not ReturnRuleNode)
					output = path.Output;

				_hashCodeDirty = true;

				return output;
			}

			private void EvalHashCode()
			{
				_returnDepth = 0;
				_hashCodeDepth = 0;
				_hashCode = 0;

				var retBarrierDistance = 0;

				unchecked
				{
					var span = new Span<int>(Array, 0, Count);
					var subGraphRegistry = _automataSubGraphRegistry;
					var returnEndFound = false;

					for (var index = span.Length - 1; index >= 0; index--)
					{
						var id = span[index];
						var subGraph = subGraphRegistry[id];

						_hashCode *= 397;
						_hashCode ^= id;
						_hashCodeDepth++;

						if (returnEndFound == false && subGraph.LeaveNode.HasReturnPathSafe)
						{
							_returnDepth++;

							continue;
						}

						returnEndFound = true;

						if (--retBarrierDistance <= 0)
							break;

						//if (subGraph.DfaBarrier)
						//	break;
					}
				}

				_hashCodeDirty = false;
			}

			public AutomataStack Freeze()
			{
				return new AutomataStack(this).AddReference();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SubGraph Get(int index) => _automataSubGraphRegistry[Array[index]];

			public override int GetHashCode()
			{
				if (_hashCodeDirty) 
					EvalHashCode();

				return _hashCode;
			}

			protected override void OnReleased()
			{
				_hashCodeDirty = true;
				Count = 0;

				base.OnReleased();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SubGraph Peek()
			{
#if DEBUG
				if (Count == 0)
					Error.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
#endif
				return _automataSubGraphRegistry[Array[Count - 1]];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public SubGraph Pop()
			{
				_hashCodeDirty = true;

#if DEBUG
				if (Count == 0)
					Error.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyStack);
#endif
				return _automataSubGraphRegistry[Array[--Count]];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Push(SubGraph item)
			{
				_hashCodeDirty = true;

				if (Count < Array.Length)
					Array[Count++] = item.Id;
				else
					PushResize(item);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void PushSafeId(int id)
			{
				Array[Count++] = id;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void PopSafe()
			{
				Count--;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node PopSafeNode()
			{
				return _automataSubGraphRegistry[Array[--Count]].LeaveNode;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Node PeekLeaveNode()
			{
				return _automataSubGraphRegistry[Array[Count]].LeaveNode;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void EnsureDelta(int delta)
			{
				_hashCodeDirty = true;

				while (Count + delta >= Array.Length)
					Resize(Array.Length * 2);
			}

			private void PushResize(SubGraph item)
			{
				Resize(2 * Array.Length);

				Array[Count++] = item.Id;
			}

			private void Resize(int newSize)
			{
				var newArray = new int[newSize];

				System.Array.Copy(Array, 0, newArray, 0, Count);

				Array = newArray;
			}

			public int[] Serialize()
			{
				var binaryStack = new int[Count + 1];
				var hashCode = 0;

				for (var i = 0; i < Count; i++)
				{
					var id = Array[i];

					binaryStack[i + 1] = id;
					hashCode *= 397;
					hashCode ^= id;
				}

				binaryStack[0] = hashCode;

				return binaryStack;
			}

			public void Sync(AutomataStack stack)
			{
				CopyFrom(stack);
			}
		}
	}
}