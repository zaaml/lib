// <copyright file="Grammar.Parser.QuantifierSymbol.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal abstract partial class Grammar<TGrammar, TToken>
	{
		public partial class ParserGrammar
		{
			protected internal sealed class QuantifierSymbol : Symbol
			{
				public QuantifierSymbol(PrimitiveSymbol symbol, QuantifierKind kind, QuantifierMode mode)
				{
					Symbol = symbol;
					Kind = kind;
					Range = QuantifierHelper.GetRange(kind);
					Mode = mode;
				}

				public QuantifierSymbol(PrimitiveSymbol symbol, Interval<int> range, QuantifierMode mode)
				{
					Symbol = symbol;
					Kind = QuantifierHelper.GetKind(range);
					Range = range;
					Mode = mode;
				}

				private QuantifierKind Kind { get; }

				public QuantifierMode Mode { get; }

				public Interval<int> Range { get; }

				public PrimitiveSymbol Symbol { get; }

				private FragmentSyntax AsFragment()
				{
					var parserFragment = new FragmentSyntax("Internal", true);

					parserFragment.AddProduction(new Production(new Symbol[] { this }));

					return parserFragment;
				}

				private FragmentSymbol AsFragmentEntry()
				{
					return new FragmentSymbol(AsFragment());
				}

				public QuantifierSymbol AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(AsFragmentEntry(), QuantifierHelper.AtLeast(count), mode);
				}

				public QuantifierSymbol Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(AsFragmentEntry(), QuantifierHelper.Between(from, to), mode);
				}

				public QuantifierSymbol Bind(string name)
				{
					return new QuantifierSymbol(Symbol, Kind, Mode)
					{
						ArgumentName = name
					};
				}

				public QuantifierSymbol Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
				{
					return new QuantifierSymbol(AsFragmentEntry(), QuantifierHelper.Exact(count), mode);
				}

				public QuantifierSymbol OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.OneOrMore))
						return new QuantifierSymbol(Symbol, QuantifierHelper.Collapse(Kind, QuantifierKind.OneOrMore), Mode);

					return new QuantifierSymbol(AsFragmentEntry(), QuantifierKind.OneOrMore, mode);
				}

				public QuantifierSymbol ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
				{
					if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrMore))
						return new QuantifierSymbol(Symbol, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrMore), Mode);

					return new QuantifierSymbol(AsFragmentEntry(), QuantifierKind.ZeroOrMore, mode);
				}

				public QuantifierSymbol ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
				{
					if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrOne))
						return new QuantifierSymbol(Symbol, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrOne), Mode);

					return new QuantifierSymbol(AsFragmentEntry(), QuantifierKind.ZeroOrOne, mode);
				}
			}
		}
	}
}