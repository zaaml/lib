// <copyright file="Automata.Process.AutomataStack.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class Process
		{
			// ReSharper disable once MemberCanBeProtected.Local
			internal sealed partial class AutomataStack
			{
				[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
				internal static class ILGenerator
				{
					private static readonly Type AutomataStackType = typeof(AutomataStack);

					private static readonly MethodInfo PushIntMethodInfo = AutomataStackType.GetMethod(nameof(Push), IPNP, null, new[] { typeof(int) }, null);
					private static readonly MethodInfo EnsureDepthMethodInfo = AutomataStackType.GetMethod(nameof(EnsureDepth), IPNP);

					private static readonly MethodInfo PopLeaveNodeMethodInfo = AutomataStackType.GetMethod(nameof(PopLeaveNode), IPNP);
					private static readonly MethodInfo PopNoRetMethodInfo = AutomataStackType.GetMethod(nameof(PopNoReturn), IPNP);

					[Conditional("AUTOMATA_VERIFY_STACK")]
					public static void EmitEnsureStackDepth(ILContext context, int stackDepth)
					{
						context.EmitLdStack();
						context.IL.Emit(OpCodes.Ldc_I4, stackDepth);
						context.IL.Emit(OpCodes.Call, EnsureDepthMethodInfo);
					}

					public static void EmitPopLeaveNode(ILContext context)
					{
						// stack.Pop().LeaveNode;
						context.EmitLdStack();
						context.IL.Emit(OpCodes.Call, PopLeaveNodeMethodInfo);
					}

					public static void EmitPopNoRet(ILContext context)
					{
						context.EmitLdStack();
						context.IL.Emit(OpCodes.Call, PopNoRetMethodInfo);
					}

					public static void EmitPush(ILContext context, SubGraph subGraph)
					{
#if false
						//stack.Push(subGraph);

						context.EmitLdStack();
						context.EmitLdValue(subGraph);
						context.IL.Emit(OpCodes.Call, PushMethodInfo);
#else
						//stack.PushId(subGraph.Id);

						context.EmitLdStack();
						context.IL.Emit(OpCodes.Ldc_I4, subGraph.RId);
						context.IL.Emit(OpCodes.Call, PushIntMethodInfo);
#endif
					}
				}
			}
		}
	}
}