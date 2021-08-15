// <copyright file="Parser.Automata.ParserProcess.IL.Debug.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Reflection;
using System.Reflection.Emit;
using static Zaaml.Core.Reflection.BF;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.Text
{
	internal abstract partial class Parser<TGrammar, TToken>
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

				private void DebugPostRuleEnter(RuleEntry ruleEntry)
				{
				}

				private void DebugPostRuleLeave(RuleEntry ruleEntry)
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

				private void DebugPreRuleEnter(RuleEntry ruleEntry)
				{
				}

				private void DebugPreRuleLeave(RuleEntry ruleEntry)
				{
				}

				public partial class ParserILGenerator
				{
					private static readonly MethodInfo DebugPreRuleEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreRuleEnter), IPNP);
					private static readonly MethodInfo DebugPostRuleEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostRuleEnter), IPNP);

					private static readonly MethodInfo DebugPreRuleLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreRuleLeave), IPNP);
					private static readonly MethodInfo DebugPostRuleLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostRuleLeave), IPNP);

					private static readonly MethodInfo DebugPreProductionEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreProductionEnter), IPNP);
					private static readonly MethodInfo DebugPostProductionEnterMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostProductionEnter), IPNP);

					private static readonly MethodInfo DebugPreProductionLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreProductionLeave), IPNP);
					private static readonly MethodInfo DebugPostProductionLeaveMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostProductionLeave), IPNP);

					private static readonly MethodInfo DebugPreConsumeParserValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreConsumeParserValue), IPNP);
					private static readonly MethodInfo DebugPostConsumeParserValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostConsumeParserValue), IPNP);

					private static readonly MethodInfo DebugPreConsumeLexerValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPreConsumeLexerValue), IPNP);
					private static readonly MethodInfo DebugPostConsumeLexerValueMethodInfo = ParserProcessType.GetMethod(nameof(DebugPostConsumeLexerValue), IPNP);


					private void EmitDebugPostConsumeLexerValue(Context context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostConsumeLexerValueMethodInfo);
					}

					private void EmitDebugPostConsumeParserValue(Context context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostConsumeParserValueMethodInfo);
					}

					private void EmitDebugPostProductionEnter(Context context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPostProductionEnterMethodInfo);
					}

					private void EmitDebugPostProductionLeave(Context context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPostProductionLeaveMethodInfo);
					}

					private void EmitDebugPostRuleEnter(Context context, RuleEntry ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostRuleEnterMethodInfo);
					}

					private void EmitDebugPostRuleLeave(Context context, RuleEntry ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPostRuleLeaveMethodInfo);
					}

					private void EmitDebugPreConsumeLexerValue(Context context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreConsumeLexerValueMethodInfo);
					}

					private void EmitDebugPreConsumeParserValue(Context context, ProductionArgument ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreConsumeParserValueMethodInfo);
					}

					private void EmitDebugPreProductionEnter(Context context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPreProductionEnterMethodInfo);
					}

					private void EmitDebugPreProductionLeave(Context context, ParserProduction parserProduction)
					{
						context.EmitLdProcess();
						context.EmitLdValue(parserProduction);
						context.IL.Emit(OpCodes.Call, DebugPreProductionLeaveMethodInfo);
					}

					private void EmitDebugPreRuleEnter(Context context, RuleEntry ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreRuleEnterMethodInfo);
					}

					private void EmitDebugPreRuleLeave(Context context, RuleEntry ruleEntry)
					{
						context.EmitLdProcess();
						context.EmitLdValue(ruleEntry);
						context.IL.Emit(OpCodes.Call, DebugPreRuleLeaveMethodInfo);
					}
				}
			}
		}
	}
}