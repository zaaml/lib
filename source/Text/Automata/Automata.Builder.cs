// <copyright file="Automata.Builder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal abstract partial class Automata<TInstruction, TOperand>
	{
		private int _inlineStateCounter;

		private HashSet<Syntax> Syntaxes { get; } = new();

		protected void AddSyntax(Syntax rule, Production production)
		{
			rule.Productions.Add(production);

			Syntaxes.Add(rule);
		}

		protected void AddSyntax(Syntax rule, params Production[] productions)
		{
			AddSyntax(rule, productions.AsEnumerable());
		}

		protected void AddSyntax(Syntax rule, IEnumerable<Production> productions)
		{
			foreach (var production in productions)
				AddSyntax(rule, production);
		}

		protected Production CreateProduction(params Entry[] entries)
		{
			return new Production(entries);
		}

		protected SyntaxEntry Inline(IEnumerable<Entry> entries)
		{
			var internalState = new InternalState("Internal_" + _inlineStateCounter++);

			AddSyntax(internalState, new Production(entries));

			return new SyntaxEntry(internalState);
		}

		protected SyntaxEntry Inline(params Entry[] entries)
		{
			return Inline(entries.AsEnumerable());
		}

		protected static RangeMatchEntry Range(TOperand from, TOperand to)
		{
			return new RangeMatchEntry(from, to);
		}

		protected static SetMatchEntry Set(params PrimitiveMatchEntry[] primitiveMatches)
		{
			return new SetMatchEntry(primitiveMatches);
		}

		protected static OperandMatchEntry Single(TOperand operand)
		{
			return new OperandMatchEntry(operand);
		}
	}
}