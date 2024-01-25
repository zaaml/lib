// <copyright file="Parser.Automata.ParserProcess.IL.Debug.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private partial class ParserProcess
			{
				private void DebugPostConsumeLexerValue(ProductionArgument argument)
				{
				}

				private void DebugPostConsumeParserValue(ProductionArgument argument)
				{
				}

				private void DebugPostProductionEnter(ParserProduction production)
				{
				}

				private void DebugPostProductionLeave(ParserProduction production)
				{
				}

				private void DebugPostSyntaxEnter(SyntaxEntry syntaxEntry)
				{
				}

				private void DebugPostSyntaxLeave(SyntaxEntry syntaxEntry)
				{
				}

				private void DebugPreConsumeLexerValue(ProductionArgument argument)
				{
				}

				private void DebugPreConsumeParserValue(ProductionArgument argument)
				{
				}

				private void DebugPreProductionEnter(ParserProduction production)
				{
				}

				private void DebugPreProductionLeave(ParserProduction production)
				{
				}

				private void DebugPreSyntaxEnter(SyntaxEntry syntaxEntry)
				{
				}

				private void DebugPreSyntaxLeave(SyntaxEntry syntaxEntry)
				{
				}

				private partial class ParserILGenerator
				{
					private static readonly MethodInfo DebugPreRuleEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreSyntaxEnter), IPNP);
					private static readonly MethodInfo DebugPostRuleEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostSyntaxEnter), IPNP);

					private static readonly MethodInfo DebugPreRuleLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreSyntaxLeave), IPNP);
					private static readonly MethodInfo DebugPostRuleLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostSyntaxLeave), IPNP);

					private static readonly MethodInfo DebugPreProductionEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreProductionEnter), IPNP);
					private static readonly MethodInfo DebugPostProductionEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostProductionEnter), IPNP);

					private static readonly MethodInfo DebugPreProductionLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreProductionLeave), IPNP);
					private static readonly MethodInfo DebugPostProductionLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostProductionLeave), IPNP);

					private static readonly MethodInfo DebugPreConsumeParserValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreConsumeParserValue), IPNP);
					private static readonly MethodInfo DebugPostConsumeParserValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostConsumeParserValue), IPNP);

					private static readonly MethodInfo DebugPreConsumeLexerValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreConsumeLexerValue), IPNP);
					private static readonly MethodInfo DebugPostConsumeLexerValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostConsumeLexerValue), IPNP);


					private void EmitDebugPostConsumeLexerValue(ILContext context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostConsumeLexerValueMethodInfo);
					}

					private void EmitDebugPostConsumeParserValue(ILContext context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostConsumeParserValueMethodInfo);
					}

					private void EmitDebugPostProductionEnter(ILContext context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPostProductionEnterMethodInfo);
					}

					private void EmitDebugPostProductionLeave(ILContext context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPostProductionLeaveMethodInfo);
					}

					private void EmitDebugPostSyntaxEnter(ILContext context, SyntaxEntry syntaxEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(syntaxEntry);
						context.IL.Emit(OpCodes.Call, DebugPostRuleEnterMethodInfo);
					}

					private void EmitDebugPostSyntaxLeave(ILContext context, SyntaxEntry syntaxEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(syntaxEntry);
						context.IL.Emit(OpCodes.Call, DebugPostRuleLeaveMethodInfo);
					}

					private void EmitDebugPreConsumeLexerValue(ILContext context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreConsumeLexerValueMethodInfo);
					}

					private void EmitDebugPreConsumeParserValue(ILContext context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreConsumeParserValueMethodInfo);
					}

					private void EmitDebugPreProductionEnter(ILContext context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPreProductionEnterMethodInfo);
					}

					private void EmitDebugPreProductionLeave(ILContext context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPreProductionLeaveMethodInfo);
					}

					private void EmitDebugPreSyntaxEnter(ILContext context, SyntaxEntry syntaxEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(syntaxEntry);
						context.IL.Emit(OpCodes.Call, DebugPreRuleEnterMethodInfo);
					}

					private void EmitDebugPreSyntaxLeave(ILContext context, SyntaxEntry syntaxEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(syntaxEntry);
						context.IL.Emit(OpCodes.Call, DebugPreRuleLeaveMethodInfo);
					}
				}
			}
		}
	}
}