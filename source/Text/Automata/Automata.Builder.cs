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

		private HashSet<Rule> Rules { get; } = new HashSet<Rule>();

		protected void AddRule(Rule rule, Production production)
		{
			rule.Productions.Add(production);

			Rules.Add(rule);
		}

		protected void AddRule(Rule rule, params Production[] productions)
		{
			AddRule(rule, productions.AsEnumerable());
		}

		protected void AddRule(Rule rule, IEnumerable<Production> productions)
		{
			foreach (var production in productions)
				AddRule(rule, production);
		}

		protected Production CreateProduction(params Entry[] entries)
		{
			return new Production(entries);
		}

		protected RuleEntry Inline(IEnumerable<Entry> entries)
		{
			var internalState = new InternalState("Internal_" + _inlineStateCounter++);

			AddRule(internalState, new Production(entries));

			return new RuleEntry(internalState);
		}

		protected RuleEntry Inline(params Entry[] entries)
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

		protected static SingleMatchEntry Single(TOperand operand)
		{
			return new SingleMatchEntry(operand);
		}
	}
}