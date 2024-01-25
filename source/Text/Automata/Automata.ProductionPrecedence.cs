// <copyright file="Automata.ProductionPrecedence.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private List<PrecedenceDefinition> PrecedenceDefinitions { get; } = new();

		protected int CreatePrecedencePredicateId()
		{
			var id = PrecedenceDefinitions.Count;

			var precedenceDefinition = new PrecedenceDefinition(id);

			PrecedenceDefinitions.Add(precedenceDefinition);

			return precedenceDefinition.Id;
		}

		private readonly struct PrecedenceDefinition
		{
			public PrecedenceDefinition(int id)
			{
				Id = id;
			}

			public readonly int Id;
		}

		protected sealed class PrecedencePredicate
		{
			public readonly int Id;
			public readonly bool Level;
			public readonly int Precedence;

			public PrecedencePredicate(int id, int precedence, bool level)
			{
				Id = id;
				Precedence = precedence;
				Level = level;
			}
		}
	}
}