// <copyright file="Automata.Process.IL.Context.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			internal readonly struct ILContext
			{
				private static readonly FieldInfo ContextFieldInfo = ProcessType.GetField(nameof(_context), BF.IPNP | BF.GF);

				private readonly ExecutionPathBase _executionPath;
				private readonly ExecutionPathMethodKind _executionPathMethodKind;
				
				
				public readonly DynamicMethod DynMethod;
				private readonly Lazy<LocalBuilder> _contextLocalLazy;
				private readonly Lazy<LocalBuilder> _stackLocalLazy;
				private readonly Lazy<LocalBuilder> _threadContextLocalLazy;
				private readonly Lazy<LocalBuilder> _threadLocalLazy;
				private readonly Lazy<LocalBuilder> _threadForkLazy;
				//public readonly int StartOffset;

				internal ILContext(ExecutionPathBase executionPath, ExecutionPathMethodKind executionPathMethodKind)
				{
					_executionPath = executionPath;
					_executionPathMethodKind = executionPathMethodKind;

					Values = new List<object>();
					ValuesMap = new Dictionary<object, int>();
					DynMethod = new DynamicMethod("Execute", typeof(Node), new[] { typeof(object[]), typeof(ExecutionPath), typeof(Process) }, typeof(ExecutionPath), true);

					DynMethod.DefineParameter(1, ParameterAttributes.None, "executionPath");
					DynMethod.DefineParameter(2, ParameterAttributes.None, "process");
					DynMethod.DefineParameter(3, ParameterAttributes.None, "closure");

					IL = DynMethod.GetILGenerator();

					var il = IL;

					_contextLocalLazy = new Lazy<LocalBuilder>(() =>
					{
						var local = il.DeclareLocal(typeof(AutomataContext));

						//EmitLdProcess();
						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Ldfld, ContextFieldInfo);
						il.Emit(OpCodes.Stloc, local);

						return local;
					});

					_threadForkLazy = new Lazy<LocalBuilder>(() =>
					{
						var threadsCollectionLocal = il.DeclareLocal(typeof(ThreadCollection).MakeByRefType());
						var threadForkLocal = il.DeclareLocal(typeof(ThreadFork).MakeByRefType());

						il.Emit(OpCodes.Ldarg_2);
						il.Emit(OpCodes.Ldflda, ProcessILGenerator.ProcessThreadsFieldInfo);
						il.Emit(OpCodes.Stloc, threadsCollectionLocal);

						il.Emit(OpCodes.Ldloc, threadsCollectionLocal);
						il.Emit(OpCodes.Ldfld, ThreadCollection.ThreadsField);
						il.Emit(OpCodes.Ldloc, threadsCollectionLocal);
						il.Emit(OpCodes.Ldfld, ThreadCollection.ThreadsHeadField);
						il.Emit(OpCodes.Ldelema, typeof(ThreadFork));

						il.Emit(OpCodes.Stloc, threadForkLocal);

						return threadForkLocal;
					});

					var threadForkLazy = _threadForkLazy;

					_threadContextLocalLazy = new Lazy<LocalBuilder>(() =>
					{
						var local = il.DeclareLocal(typeof(ThreadContext).MakeByRefType());

						//il.Emit(OpCodes.Ldarg_2);
						//il.Emit(OpCodes.Call, ProcessILGenerator.ProcessGetThreadContextMethodInfo);
						//il.Emit(OpCodes.Stloc, local);

						il.Emit(OpCodes.Ldloc, threadForkLazy.Value);
						il.Emit(OpCodes.Ldflda, ThreadFork.ContextField);
						il.Emit(OpCodes.Stloc, local);

						return local;
					});

					_threadLocalLazy = new Lazy<LocalBuilder>(() =>
					{
						var local = il.DeclareLocal(typeof(Thread).MakeByRefType());

						//il.Emit(OpCodes.Ldarg_2);
						//il.Emit(OpCodes.Call, ProcessILGenerator.ProcessGetThreadMethodInfo);
						//il.Emit(OpCodes.Stloc, local);

						il.Emit(OpCodes.Ldloc, threadForkLazy.Value);
						il.Emit(OpCodes.Ldflda, ThreadFork.ThreadField);
						il.Emit(OpCodes.Stloc, local);

						return local;
					});

					var threadLocalLazy = _threadLocalLazy;

					_stackLocalLazy = new Lazy<LocalBuilder>(() =>
					{
						var local = il.DeclareLocal(typeof(AutomataStack));

						il.Emit(OpCodes.Ldloc, threadForkLazy.Value);
						il.Emit(OpCodes.Ldfld, Thread.StackField);
						il.Emit(OpCodes.Stloc, local);

						return local;
					});

					//StartOffset = IL.ILOffset;
				}

				public ILGenerator IL { get; }

				public List<object> Values { get; }

				private Dictionary<object, int> ValuesMap { get; }

				public void EmitExecutionPath()
				{
					IL.Emit(OpCodes.Ldarg_1);
				}

				public void EmitLdContext()
				{
					//IL.Emit(OpCodes.Ldloc, _contextLocalLazy.Value);

					EmitLdProcess();

					IL.Emit(OpCodes.Ldfld, ContextFieldInfo);
				}

				public void EmitLdProcess()
				{
					IL.Emit(OpCodes.Ldarg_2);
				}

				public void EmitLdThread()
				{
					//IL.Emit(OpCodes.Ldloc, _threadLocalLazy.Value);

					IL.Emit(OpCodes.Ldloc, _threadForkLazy.Value);
					IL.Emit(OpCodes.Ldflda, ThreadFork.ThreadField);
				}

				public void EmitLdThreadContext()
				{
					//IL.Emit(OpCodes.Ldloc, _threadContextLocalLazy.Value);

					IL.Emit(OpCodes.Ldloc, _threadForkLazy.Value);
					IL.Emit(OpCodes.Ldflda, ThreadFork.ContextField);
				}

				public void EmitLdStack()
				{
					//IL.Emit(OpCodes.Ldloc, _stackLocalLazy.Value);

					EmitLdThread();
					IL.Emit(OpCodes.Ldfld, Thread.StackField);
				}

				public void EmitLdValue(object value)
				{
					if (ValuesMap.TryGetValue(value, out var index) == false)
					{
						Values.Add(value);
						index = Values.Count - 1;
						ValuesMap[value] = index;
					}

					IL.Emit(OpCodes.Ldarg_0);
					IL.Emit(OpCodes.Ldc_I4, index);
					IL.Emit(OpCodes.Ldelem_Ref);
				}

				public void EmitMoveInstructionPointer()
				{
					EmitLdThreadContext();
					EmitLdThreadContext();

					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamPointerFieldInfo);
					IL.Emit(OpCodes.Ldc_I4_1);
					IL.Emit(OpCodes.Add);
					IL.Emit(OpCodes.Stfld, ThreadContext.InstructionStreamPointerFieldInfo);
				}

				public void EmitGetInstruction()
				{
					EmitLdThreadContext();
					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamFieldInfo);
					EmitLdThreadContext();
					IL.Emit(OpCodes.Ldfld, ThreadContext.InstructionStreamPointerFieldInfo);
					IL.Emit(OpCodes.Call, InstructionStream.PeekInstructionMethodInfo);
				}
			}
		}
	}
}