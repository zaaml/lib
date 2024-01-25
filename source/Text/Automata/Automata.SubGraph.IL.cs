// <copyright file="Automata.SubGraph.IL.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private protected partial class SubGraph
		{
			internal static class ILGenerator
			{
				private static readonly FieldInfo LeaveNodeFieldInfo = typeof(SubGraph).GetField(nameof(LeaveNode), IPNP);

				public static void EmitLoadLeaveNode(Process.ILContext automataILBuilder)
				{
					automataILBuilder.IL.Emit(OpCodes.Ldfld, LeaveNodeFieldInfo);
				}
			}
		}
	}
}