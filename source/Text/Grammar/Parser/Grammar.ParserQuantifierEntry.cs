// <copyright file="Grammar.ParserQuantifierEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class ParserQuantifierEntry : ParserEntry
		{
			public ParserQuantifierEntry(ParserPrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = kind;
				Range = QuantifierHelper.GetRange(kind);
				Mode = mode;
			}

			public ParserQuantifierEntry(ParserPrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = QuantifierHelper.GetKind(range);
				Range = range;
				Mode = mode;
			}

			private QuantifierKind Kind { get; }

			public QuantifierMode Mode { get; }

			public ParserPrimitiveEntry PrimitiveEntry { get; }

			public Interval<int> Range { get; }

			private ParserFragment AsFragment()
			{
				var parserFragment = new ParserFragment(true);

				parserFragment.Productions.Add(new ParserProduction(new ParserEntry[] { this }));

				return parserFragment;
			}

			public ParserQuantifierEntry AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierHelper.AtLeast(count), mode);
			}

			public ParserQuantifierEntry Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierHelper.Between(from, to), mode);
			}

			public ParserQuantifierEntry Bind(string name)
			{
				return new ParserQuantifierEntry(PrimitiveEntry, Kind, Mode)
				{
					Name = name
				};
			}

			public ParserQuantifierEntry Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierHelper.Exact(count), mode);
			}

			public ParserQuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.OneOrMore))
					return new ParserQuantifierEntry(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.OneOrMore), Mode);

				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.OneOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrMore))
					return new ParserQuantifierEntry(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrMore), Mode);

				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.ZeroOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrOne))
					return new ParserQuantifierEntry(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrOne), Mode);

				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.ZeroOrOne, mode);
			}
		}
	}
}