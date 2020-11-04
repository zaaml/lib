// <copyright file="Automata.ILBuilderContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		#region Nested Types

		protected struct ILBuilderContext
		{
			private readonly LocalBuilder _contextLocal;
			public readonly DynamicMethod DynMethod;
			private readonly LocalBuilder _stackLocal;
			//public readonly int StartOffset;

			public ILBuilderContext(FieldInfo contextFieldInfo)
			{
				Values = new List<object>();
				ValuesMap = new Dictionary<object, int>();
				DynMethod = new DynamicMethod("Execute", typeof(Node), new[] {typeof(ExecutionPath), typeof(Process), typeof(AutomataStack), typeof(object[])}, typeof(ExecutionPath), true);

				DynMethod.DefineParameter(1, ParameterAttributes.None, "executionPath");
				DynMethod.DefineParameter(2, ParameterAttributes.None, "process");
				DynMethod.DefineParameter(3, ParameterAttributes.None, "stack");
				DynMethod.DefineParameter(4, ParameterAttributes.None, "values");

				IL = DynMethod.GetILGenerator();

				_contextLocal = IL.DeclareLocal(typeof(AutomataContext));
				_stackLocal = IL.DeclareLocal(typeof(AutomataStack));

				// var contextLocal = process._context;
				EmitLdProcess();
				IL.Emit(OpCodes.Ldfld, contextFieldInfo);
				IL.Emit(OpCodes.Stloc, _contextLocal);

				// var stackLocal = stack;
				IL.Emit(OpCodes.Ldarg_2);
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

				IL.Emit(OpCodes.Ldarg_3);
				IL.Emit(OpCodes.Ldc_I4, index);
				IL.Emit(OpCodes.Ldelem_Ref);
			}
		}

		#endregion
	}
}