// <copyright file="Lexer.Automata.ReaderProcess.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal partial class Lexer<TGrammar, TToken>
	{
		#region Nested Types

		private protected partial class LexerAutomata
		{
			#region Nested Types

			//public sealed class ReaderProcess : LexerProcess
			//{
			//	#region Fields

			//	private readonly LexerDfaState _dfaInitialState;
			//	private readonly InstructionStream _instructionStream;
			//	private int _instructionPointer;

			//	#endregion

			//	#region Ctors

			//	public ReaderProcess(TextSource textSource, Lexer<TGrammar, TToken> lexer, LexerContext<TToken> lexerContext) : base(textSource)
			//	{
			//		_dfaInitialState = lexer.Automata._dfaBuilder.InitialState;
			//		_instructionStream = new InstructionStream().AddReference().Mount(new TextInstructionReader(textSource.CreateReader()), lexer.Automata);
			//		_instructionPointer = InstructionStream.InitializeInstructionPointer();

			//		Context = new LexerAutomataContext();

			//		Context.Mount(textSource, lexerContext);
			//	}

			//	#endregion

			//	#region Properties

			//	private LexerAutomataContext Context { get; }

			//	public override int TextPointer
			//	{
			//		get => _instructionPointer;
			//		set => _instructionPointer = value;
			//	}

			//	#endregion

			//	#region Methods

			//	public override void Dispose()
			//	{
			//		_instructionStream.ReleaseReference();
			//	}

			//	public override int Run(Lexeme<TToken>[] lexemesBuffer, int[] operands, int lexemesBufferOffset, int lexemesBufferSize, bool skipLexemes)
			//	{
			//		throw new RefactoringException("InstructionPointer refactoring");
			//		//if (_instructionPointer < 0)
			//		//	return 0;

			//		//var iLexeme = lexemesBufferOffset;
			//		//var instructionPointer = _instructionPointer;
			//		//var lexemeStart = instructionPointer;

			//		//while (iLexeme < lexemesBufferSize && _instructionQueue.ReadOperand(ref instructionPointer) >= 0)
			//		//{
			//		//	var successInstructionPointer = instructionPointer;
			//		//	var dfaState = _dfaInitialState;
			//		//	var operand = _instructionQueue.PeekOperand(instructionPointer);

			//		//	while (operand >= 0)
			//		//	{
			//		//		if (operand < LexerDfaState.ArrayLimit)
			//		//			dfaState = dfaState.Array[operand];
			//		//		else
			//		//		{
			//		//			if (dfaState.Dictionary.TryGetValue(operand, out var result))
			//		//				dfaState = result;
			//		//			else
			//		//			{
			//		//				var prevState = dfaState;

			//		//				dfaState = dfaState.Builder.Build(operand, dfaState);
			//		//				prevState.SetState(operand, dfaState);
			//		//			}
			//		//		}

			//		//		operand = _instructionQueue.ReadOperand(ref instructionPointer);

			//		//		if (dfaState.Continue)
			//		//			continue;

			//		//		if (dfaState.Break)
			//		//		{
			//		//			if (dfaState.SavePointer)
			//		//				successInstructionPointer = instructionPointer;

			//		//			break;
			//		//		}

			//		//		if (dfaState.SavePointer)
			//		//			successInstructionPointer = instructionPointer;
			//		//	}

			//		//	if (dfaState.SuccessSubGraph == null)
			//		//	{
			//		//		_instructionQueue.LockPointer(instructionPointer);
			//		//		_instructionQueue.ReleasePointerReference(_instructionPointer);
			//		//		_instructionPointer = instructionPointer;

			//		//		return iLexeme - lexemesBufferOffset;
			//		//	}

			//		//	if (skipLexemes == false || dfaState.Skip == false)
			//		//	{
			//		//		ref var lexeme = ref lexemesBuffer[iLexeme];

			//		//		lexeme.Token = dfaState.Token;
			//		//		lexeme.Start = lexemeStart;
			//		//		lexeme.End = successInstructionPointer;

			//		//		operands[iLexeme] = dfaState.TokenCode;
			//		//		iLexeme++;
			//		//	}

			//		//	lexemeStart = successInstructionPointer;
			//		//	instructionPointer = successInstructionPointer;
			//		//}

			//		//_instructionQueue.LockPointer(instructionPointer);
			//		//_instructionQueue.ReleasePointerReference(_instructionPointer);
			//		//_instructionPointer = instructionPointer;

			//		//return iLexeme - lexemesBufferOffset;
			//	}

			//	#endregion
			//}

			#endregion
		}

		#endregion
	}
}