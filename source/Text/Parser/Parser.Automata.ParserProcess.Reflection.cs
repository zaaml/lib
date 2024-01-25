// <copyright file="Parser.Automata.ParserQuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using static Zaaml.Core.Reflection.BF;

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private partial class ParserProcess
			{
				public static Type ParserProcessType => typeof(ParserProcess);

				public static readonly FieldInfo TextSourceSpanFieldInfo = ParserProcessType.GetField(nameof(_textSpan), IPNP);

				public static readonly FieldInfo LexemeStartFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.StartField), IPNP);
				public static readonly FieldInfo LexemeEndFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.EndField), IPNP);
				public static readonly FieldInfo LexemeTokenFieldInfo = typeof(Lexeme<TToken>).GetField(nameof(Lexeme<TToken>.TokenField), IPNP);
				public static readonly FieldInfo ProductionEntityArgumentsFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Arguments), IPNP);

				public static readonly FieldInfo ProductionEntityStackFieldInfo = ParserProcessType.GetField(nameof(_productionEntityStack), IPNP);
				public static readonly FieldInfo ProductionEntityStackTailFieldInfo = ParserProcessType.GetField(nameof(_productionEntityStackTail), IPNP);

				public static readonly FieldInfo ProductionEntityResultFieldInfo = typeof(ProductionEntity).GetField(nameof(ProductionEntity.Result), IPNP);
				public static readonly FieldInfo SyntaxTreeFactoryFieldInfo = ParserProcessType.GetField(nameof(_syntaxTreeFactory), IPNP);

				public static readonly FieldInfo LexemeStringConverterFieldInfo = ParserProcessType.GetField(nameof(LexemeStringConverter), IPNP);
				public static readonly FieldInfo LexemeTokenConverterFieldInfo = ParserProcessType.GetField(nameof(LexemeTokenConverter), IPNP);
				public static readonly FieldInfo SyntaxTokenConverterFieldInfo = ParserProcessType.GetField(nameof(SyntaxTokenConverter), IPNP);
				public static readonly FieldInfo NullableSyntaxTokenConverterFieldInfo = ParserProcessType.GetField(nameof(NullableSyntaxTokenConverter), IPNP);

				public static readonly MethodInfo GetInstructionReferenceMethodInfo = ParserProcessType.GetProperty(nameof(InstructionReference), IPNP)?.GetGetMethod();
				public static readonly MethodInfo GetInstructionTextMethodInfo = ParserProcessType.GetMethod(nameof(GetInstructionText), IPNP);
				public static readonly MethodInfo GetCompositeTokenLexemeMethodInfo = ParserProcessType.GetMethod(nameof(GetCompositeTokenLexeme), IPNP);
				public static readonly MethodInfo GetInstructionMethodInfo = ParserProcessType.GetProperty(nameof(Instruction), IPNP)?.GetGetMethod();
				public static readonly MethodInfo EnsureProductionEntityStackDepthMethodInfo = ParserProcessType.GetMethod(nameof(EnsureProductionEntityStackDepth), IPNP);
				public static readonly MethodInfo PeekProductionEntityMethodInfo = ParserProcessType.GetMethod(nameof(PeekProductionEntity), IPNP);
				public static readonly MethodInfo PredicateResultGetResultMethodInfo = typeof(PredicateResult).GetMethod(nameof(PredicateResult.GetResult), IPNP);
				public static readonly MethodInfo EnterProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterProduction), IPNP);
				public static readonly MethodInfo ConsumeProductionEntityMethodInfo = ParserProcessType.GetMethod(nameof(ConsumeProductionEntity), IPNP);
				public static readonly MethodInfo LeaveRuleEntryMethodInfo = ParserProcessType.GetMethod(nameof(LeaveRuleEntry), IPNP);

				public static readonly MethodInfo GetLexemeTextMethodInfo = ParserProcessType.GetMethod(nameof(GetLexemeString), IPNP);

				public static readonly MethodInfo ProductionEntityReturnMethodInfo = typeof(ProductionEntity).GetMethod(nameof(ProductionEntity.Return), IPNP);

				public static readonly MethodInfo EnterLeftRecursionProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterLeftRecursionProduction), IPNP);
				public static readonly MethodInfo EnterLeftFactoringProductionMethodInfo = ParserProcessType.GetMethod(nameof(EnterLeftFactoringProduction), IPNP);

				public static readonly MethodInfo LeaveLeftFactoringProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveLeftFactoringProduction), IPNP);
				public static readonly MethodInfo LeaveLeftRecursionProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveLeftRecursionProduction), IPNP);
				public static readonly MethodInfo LeaveProductionMethodInfo = ParserProcessType.GetMethod(nameof(LeaveProduction), IPNP);
				
			}
		}
	}
}