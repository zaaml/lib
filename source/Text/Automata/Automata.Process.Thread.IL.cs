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
			partial struct Thread
			{
				private static readonly MethodInfo TryEnterPrecedenceMethodInfo = typeof(Thread).GetMethod(nameof(TryEnterPrecedence), BF.IPNP);
				private static readonly MethodInfo TryEnterPrecedenceCodeMethodInfo = typeof(Thread).GetMethod(nameof(TryEnterPrecedenceCode), BF.IPNP);
				private static readonly MethodInfo LeavePrecedenceMethodInfo = typeof(Thread).GetMethod(nameof(LeavePrecedence), BF.IPNP);
				private static readonly MethodInfo LeavePrecedenceCodeMethodInfo = typeof(Thread).GetMethod(nameof(LeavePrecedenceCode), BF.IPNP);

				internal static class ThreadILGenerator
				{
					public static void EmitEnterPrecedenceNode(ILContext context, PrecedenceEnterNode precedenceEnterNode)
					{
						var precedenceCode = GetPrecedenceCode(precedenceEnterNode.Precedence);

						context.EmitLdThread();
						context.IL.Emit(OpCodes.Ldc_I4, precedenceCode);
						context.IL.Emit(OpCodes.Call, TryEnterPrecedenceCodeMethodInfo);
					}

					private static int GetPrecedenceCode(PrecedencePredicate precedence)
					{
						var precedenceId = precedence.Id;
						var precedenceValue = precedence.Precedence;
						var precedenceLevel = precedence.Level;

						var precedenceCode = (precedenceId << 16) | precedenceValue;

						if (precedenceLevel)
							precedenceCode |= 0x1000000;

						return precedenceCode;
					}

					public static void EmitLeavePrecedenceNode(ILContext context, PrecedenceLeaveNode precedenceLeaveNode)
					{
						var precedenceCode = GetPrecedenceCode(precedenceLeaveNode.Precedence);

						context.EmitLdThread();
						context.IL.Emit(OpCodes.Ldc_I4, precedenceCode);
						context.IL.Emit(OpCodes.Call, LeavePrecedenceCodeMethodInfo);
					}
				}
			}
		}
	}
}