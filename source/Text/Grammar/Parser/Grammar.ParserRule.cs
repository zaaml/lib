// <copyright file="Grammar.ParserRule.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal partial class ParserRule
		{
			internal ParserRule()
			{
				GrammarType = GetGrammarType();
			}

			public Grammar<TToken> Grammar => Get<TToken>(GrammarType);

			private Type GrammarType { get; }

			internal bool IsInline { get; set; }

			internal bool AggressiveInlining { get; set; }

			public string Name { get; internal set; }

			public virtual Type NodeType => null;

			public ParserProductionCollection Productions { get; } = new ParserProductionCollection();

			public void AddProduction(ParserProduction parserProduction)
			{
				Productions.Add(parserProduction);
			}

			public ParserRuleEntry Bind(string name)
			{
				return new ParserRuleEntry(this)
				{
					Name = name
				};
			}

			public ParserRuleEntry Return()
			{
				return new ParserRuleEntry(this)
				{
					Name = Name,
					IsReturn = true
				};
			}

			public ParserQuantifierEntry AtLeast(int count)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierHelper.AtLeast(count), QuantifierMode.Greedy);
			}

			public ParserQuantifierEntry Between(int from, int to)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierHelper.Between(from, to), QuantifierMode.Greedy);
			}

			internal string EnsureName()
			{
				return Name;
			}

			public ParserQuantifierEntry Exact(int count)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierHelper.Exact(count), QuantifierMode.Greedy);
			}

			public ParserQuantifierEntry OneOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierKind.OneOrMore, quantifierMode);
			}

			public override string ToString()
			{
				return Name ?? base.ToString();
			}

			public ParserQuantifierEntry ZeroOrMore(QuantifierMode quantifierMode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierKind.ZeroOrMore, quantifierMode);
			}

			public ParserQuantifierEntry ZeroOrOne(QuantifierMode quantifierMode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(new ParserRuleEntry(this), QuantifierKind.ZeroOrOne, quantifierMode);
			}
		}
	}
}