// <copyright file="Automata.ProductionPrecedence.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private List<PrecedenceDefinition> PrecedenceDefinitions { get; } = new();

		protected int CreatePrecedencePredicateId(int precedenceCount)
		{
			var id = 0;

			if (PrecedenceDefinitions.Count > 0)
			{
				var prevDefinition = PrecedenceDefinitions[PrecedenceDefinitions.Count - 1];

				id = prevDefinition.Id + prevDefinition.PrecedenceCount + 1;
			}

			var precedenceDefinition = new PrecedenceDefinition(id, precedenceCount);

			PrecedenceDefinitions.Add(precedenceDefinition);

			return precedenceDefinition.Id;
		}

		private readonly struct PrecedenceDefinition
		{

			public PrecedenceDefinition(int id, int precedenceCount)
			{
				Id = id;
				PrecedenceCount = precedenceCount;
			}

			public readonly int Id;
			public readonly int PrecedenceCount;
		}

		protected sealed class PrecedencePredicate
		{
			public PrecedencePredicate(int id, int precedence, bool level)
			{
				Id = id;
				Precedence = precedence;
				Level = level;
			}

			public int Id { get; }

			public int Precedence { get; }

			public bool Level { get; }
		}
	}
}