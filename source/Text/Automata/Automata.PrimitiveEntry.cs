// <copyright file="Automata.PrimitiveEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		protected abstract class PrimitiveEntry : Entry
		{
			public static implicit operator PrimitiveEntry(Rule state)
			{
				return new RuleEntry(state);
			}

			public static implicit operator PrimitiveEntry(TOperand input)
			{
				return new SingleMatchEntry(input);
			}
		}

		protected abstract class SemanticPredicate : PrimitiveEntry
		{
		}

		protected sealed class PreviousMatchSemanticPredicate : SemanticPredicate
		{
			public SetMatchEntry Match { get; }

			public PreviousMatchSemanticPredicate(SetMatchEntry match)
			{
				Match = match;
			}

			protected override string DebuggerDisplay => "PreviousMatchSemanticPredicate";
		}

		protected abstract class PrecedenceEntry : SemanticPredicate
		{
			public PrecedencePredicate PrecedencePredicate { get; }
			
			protected PrecedenceEntry(PrecedencePredicate precedencePredicate)
			{
				PrecedencePredicate = precedencePredicate;
			}
		}

		protected sealed class EnterPrecedenceEntry : PrecedenceEntry
		{
			public EnterPrecedenceEntry(PrecedencePredicate precedencePredicate) : base(precedencePredicate)
			{
			}

			protected override string DebuggerDisplay => "EnterPrecedence";
		}

		protected sealed class LeavePrecedenceEntry : PrecedenceEntry
		{
			public LeavePrecedenceEntry(PrecedencePredicate precedencePredicate) : base(precedencePredicate)
			{
			}

			protected override string DebuggerDisplay => "LeavePrecedence";
		}
	}
}