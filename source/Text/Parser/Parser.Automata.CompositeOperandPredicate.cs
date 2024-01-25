// <copyright file="Parser._.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Parser<TGrammar, TToken>
	{
		private sealed partial class ParserAutomata
		{
			private sealed class CompositeOperandPredicate
			{
				public CompositeOperandPredicate(Grammar<TGrammar, TToken>.ParserGrammar.TokenSymbol tokenSymbol)
				{
					ConnectedTokenCount = tokenSymbol.Token.Productions[0].Symbols.Count;
				}

				public int ConnectedTokenCount { get; }

				public bool Predicate(AutomataContext context)
				{
					var instructionStream = context.Process.InstructionStream;
					var instructionPointer = context.Process.InstructionPointer;
					var currentPointer = instructionPointer - ConnectedTokenCount;
					var currentLexeme = instructionStream.PeekInstruction(currentPointer);

					for (var i = 1; i < ConnectedTokenCount; i++)
					{
						var nextLexeme = instructionStream.PeekInstruction(currentPointer + i);

						if (currentLexeme.End != nextLexeme.Start)
							return false;

						currentLexeme = nextLexeme;
					}

					return true;
				}
			}
		}
	}
}