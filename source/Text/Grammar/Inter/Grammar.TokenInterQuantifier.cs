// <copyright file="Grammar.TokenInterQuantifier.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed class TokenInterQuantifier : TokenInterEntry
		{
			#region Ctors

			public TokenInterQuantifier(TokenInterPrimitiveEntry primitiveEntry, Interval<int> range, QuantifierMode mode)
			{
				PrimitiveEntry = primitiveEntry;
				Kind = QuantifierHelper.GetKind(range);
				Range = range;
				Mode = mode;
			}

			public TokenInterQuantifier(TokenInterPrimitiveEntry primitiveEntry, QuantifierKind kind, QuantifierMode mode = QuantifierMode.Greedy)
			{
				PrimitiveEntry = primitiveEntry;
				Range = QuantifierHelper.GetRange(kind);
				Kind = kind;
				Mode = mode;
			}

			#endregion

			#region Properties

			public QuantifierKind Kind { get; }

			public QuantifierMode Mode { get; }

			public TokenInterPrimitiveEntry PrimitiveEntry { get; }

			public Interval<int> Range { get; }

			#endregion

			#region Methods

			private TokenInterFragment AsFragment()
			{
				var parserFragment = new TokenInterFragment();

				parserFragment.Productions.Add(new TokenInterProduction(new TokenInterEntry[] {this}));

				return parserFragment;
			}


			public TokenInterQuantifier AtLeast(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierHelper.AtLeast(count), mode);
			}

			public TokenInterQuantifier Between(int from, int to, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierHelper.Between(from, to), mode);
			}

			public override ParserEntry CreateParserEntry()
			{
				return new ParserQuantifierEntry((ParserPrimitiveEntry) PrimitiveEntry.CreateParserEntry(), Kind, Mode);
			}

			public TokenInterQuantifier Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierHelper.Exact(count), mode);
			}

			public TokenInterQuantifier OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.OneOrMore))
					return new TokenInterQuantifier(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.OneOrMore), Mode);

				return new TokenInterQuantifier(AsFragment(), QuantifierKind.OneOrMore, mode);
			}

			public static implicit operator TokenInterProduction(TokenInterQuantifier entry)
			{
				return new TokenInterProduction(new TokenInterEntry[] {entry});
			}

			public TokenInterQuantifier ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrMore))
					return new TokenInterQuantifier(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrMore), Mode);

				return new TokenInterQuantifier(AsFragment(), QuantifierKind.ZeroOrMore, mode);
			}

			public TokenInterQuantifier ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				if (Mode == mode && mode == QuantifierMode.Greedy && QuantifierHelper.CanCollapse(Kind, QuantifierKind.ZeroOrOne))
					return new TokenInterQuantifier(PrimitiveEntry, QuantifierHelper.Collapse(Kind, QuantifierKind.ZeroOrOne), Mode);

				return new TokenInterQuantifier(AsFragment(), QuantifierKind.ZeroOrOne, mode);
			}

			#endregion
		}

		#endregion
	}
}