// <copyright file="Automata.Process.IL.Context.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal readonly struct ILContext
			{
				private readonly ExecutionPathBase _executionPath;
				private readonly ExecutionPathMethodKind _executionPathMethodKind;
				private readonly ProcessILGenerator _processILGenerator;
				private readonly LocalBuilder _contextLocal;
				public readonly DynamicMethod DynMethod;

				private readonly LocalBuilder _stackLocal;
				//public readonly int StartOffset;

				internal ILContext(ExecutionPathBase executionPath, ExecutionPathMethodKind executionPathMethodKind, ProcessILGenerator processILGenerator)
				{
					_executionPath = executionPath;
					_executionPathMethodKind = executionPathMethodKind;
					_processILGenerator = processILGenerator;
					Values = new List<object>();
					ValuesMap = new Dictionary<object, int>();
					DynMethod = new DynamicMethod("Execute", typeof(Node), new[] { typeof(ExecutionPath), typeof(Process), typeof(Thread).MakeByRefType(), typeof(ThreadContext).MakeByRefType(), typeof(object[]) }, typeof(ExecutionPath), true);

					DynMethod.DefineParameter(1, ParameterAttributes.None, "executionPath");
					DynMethod.DefineParameter(2, ParameterAttributes.None, "process");
					DynMethod.DefineParameter(3, ParameterAttributes.None, "thread");
					DynMethod.DefineParameter(4, ParameterAttributes.None, "threadContext");
					DynMethod.DefineParameter(5, ParameterAttributes.None, "closure");

					IL = DynMethod.GetILGenerator();

					_contextLocal = IL.DeclareLocal(typeof(AutomataContext));
					_stackLocal = IL.DeclareLocal(typeof(AutomataStack));

					// var contextLocal = process._context;
					_processILGenerator.EmitLdContext(this);

					IL.Emit(OpCodes.Stloc, _contextLocal);

					// var stackLocal = stack;

					//_processILGenerator.EmitLdStack(this);
					IL.Emit(OpCodes.Ldarg_2);
					IL.Emit(OpCodes.Ldfld, Thread.StackField);

					IL.Emit(OpCodes.Stloc, _stackLocal);

					//StartOffset = IL.ILOffset;
				}

				public ILGenerator IL { get; }

				public List<object> Values { get; }

				private Dictionary<object, int> ValuesMap { get; }

				public void EmitExecutionPath()
				{
					IL.Emit(OpCodes.Ldarg_0);
				}

				public void EmitLdContext()
				{
					IL.Emit(OpCodes.Ldloc, _contextLocal);
				}

				public void EmitLdProcess()
				{
					IL.Emit(OpCodes.Ldarg_1);
				}

				public void EmitLdThread()
				{
					IL.Emit(OpCodes.Ldarg_2);
				}

				public void EmitLdStack()
				{
					IL.Emit(OpCodes.Ldloc, _stackLocal);
				}

				public void EmitLdValue(object value)
				{
					if (ValuesMap.TryGetValue(value, out var index) == false)
					{
						Values.Add(value);
						index = Values.Count - 1;
						ValuesMap[value] = index;
					}

					IL.Emit(OpCodes.Ldarg, 4);
					IL.Emit(OpCodes.Ldc_I4, index);
					IL.Emit(OpCodes.Ldelem_Ref);
				}

				public void EmitMoveInstructionPointer()
				{
					IL.Emit(OpCodes.Ldarg_3);
					IL.Emit(OpCodes.Ldarg_3);
					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamPointerFieldInfo);
					IL.Emit(OpCodes.Ldc_I4_1);
					IL.Emit(OpCodes.Add);
					IL.Emit(OpCodes.Stfld, ThreadContext.InstructionStreamPointerFieldInfo);
				}

				public void EmitGetInstruction()
				{
					IL.Emit(OpCodes.Ldarg_3);
					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamFieldInfo);
					IL.Emit(OpCodes.Ldarg_3);
					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamPointerFieldInfo);
					IL.Emit(OpCodes.Call, InstructionStream.PeekInstructionMethodInfo);
				}
			}
		}
	}
}