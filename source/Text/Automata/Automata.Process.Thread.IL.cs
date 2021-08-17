// <copyright file="Automata.Process.Thread.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Reflection.Emit;
using Zaaml.Core.Reflection;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		partial class Process
		{
			internal partial struct Thread
			{
				private static readonly MethodInfo TryEnterPrecedenceMethodInfo = typeof(Thread).GetMethod(nameof(TryEnterPrecedence), BF.IPNP);
				private static readonly MethodInfo LeavePrecedenceMethodInfo = typeof(Thread).GetMethod(nameof(LeavePrecedence), BF.IPNP);

				internal static class ThreadILGenerator
				{
					public static void EmitEnterPrecedenceNode(ILContext context, PrecedenceEnterNode precedenceEnterNode)
					{
						context.EmitLdThread();
						context.IL.Emit(OpCodes.Ldc_I4, precedenceEnterNode.Id);
						context.IL.Emit(OpCodes.Call, TryEnterPrecedenceMethodInfo);
					}

					public static void EmitLeavePrecedenceNode(ILContext context, PrecedenceLeaveNode precedenceLeaveNode)
					{
						context.EmitLdThread();
						context.IL.Emit(OpCodes.Ldc_I4, precedenceLeaveNode.Id);
						context.IL.Emit(OpCodes.Call, LeavePrecedenceMethodInfo);
					}
				}
			}
		}
	}
}