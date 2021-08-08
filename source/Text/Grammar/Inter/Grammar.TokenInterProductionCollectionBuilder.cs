// <copyright file="Grammar.TokenInterProductionCollectionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal struct TokenInterProductionCollectionBuilder
		{
			private TokenInterProduction _entry0;
			private TokenInterProduction _entry1;
			private TokenInterProduction _entry2;
			private TokenInterProduction _entry3;
			private List<TokenInterProduction> _collection;
			private const int Capacity = 4;

			internal TokenInterProductionCollectionBuilder(TokenInterProduction parserEntry)
			{
				_collection = null;
				_entry0 = parserEntry;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				EntryCount = 1;
			}

			private void Add(TokenInterProduction parserEntry)
			{
				if (EntryCount >= Capacity)
				{
					_collection.Add(parserEntry);

					EntryCount++;

					return;
				}

				if (EntryCount == 0)
					_entry0 = parserEntry;
				else if (EntryCount == 1)
					_entry1 = parserEntry;
				else if (EntryCount == 2)
					_entry2 = parserEntry;
				else if (EntryCount == 3)
					_entry3 = parserEntry;

				EntryCount++;

				if (EntryCount == Capacity)
				{
					_collection = new List<TokenInterProduction>(Capacity);

					for (var i = 0; i < EntryCount; i++)
						_collection.Add(this[i]);
				}
			}

			public int EntryCount { get; private set; }

			internal TokenInterProduction this[int index]
			{
				get
				{
					if (index > 3)
						return _collection[index];

					if (index == 0)
						return _entry0;

					if (index == 1)
						return _entry1;

					if (index == 2)
						return _entry2;

					if (index == 3)
						return _entry3;

					throw new ArgumentOutOfRangeException();
				}
			}

			#region Methods

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionCollectionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				var result = new TokenInterProductionCollectionBuilder();

				for (var i = 0; i < op1.EntryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2.EntryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionCollectionBuilder op1, TokenInterProductionBuilder op2)
			{
				return op1 | new TokenInterProductionCollectionBuilder(new TokenInterProduction(op2.CreateArray()));
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return new TokenInterProductionCollectionBuilder(new TokenInterProduction(op1.CreateArray())) | op2;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionCollectionBuilder op1, TokenRule op2)
			{
				return op1 | new TokenInterProductionCollectionBuilder(new TokenInterProduction(new TokenInterEntry[] { new TokenInterRuleEntry(op2) }));
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenRule op1, TokenInterProductionCollectionBuilder op2)
			{
				return new TokenInterProductionCollectionBuilder(new TokenInterProduction(new TokenInterEntry[] { new TokenInterRuleEntry(op1) })) | op2;
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionCollectionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1.AsFragment() + op2.AsFragment();
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionCollectionBuilder op1, TokenInterProductionBuilder op2)
			{
				return new TokenInterProductionBuilder(op1.AsFragment()) + op2;
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionCollectionBuilder op1, TokenRule op2)
			{
				return op1.AsFragment() + op2;
			}

			public static TokenInterProductionBuilder operator +(TokenRule op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1 + op2.AsFragment();
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1 + new TokenInterProductionBuilder(op2.AsFragment());
			}

			public static implicit operator TokenInterProduction(TokenInterProductionCollectionBuilder builder)
			{
				return builder.AsFragment();
			}

			public TokenInterFragment AsFragment()
			{
				var parserFragment = new TokenInterFragment();

				parserFragment.Productions.FromBuilder(this);

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

			public TokenInterQuantifier Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierHelper.Exact(count), mode);
			}

			public TokenInterQuantifier OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierKind.OneOrMore, mode);
			}

			public TokenInterQuantifier ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierKind.ZeroOrMore, mode);
			}

			public TokenInterQuantifier ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new TokenInterQuantifier(AsFragment(), QuantifierKind.ZeroOrOne, mode);
			}

			#endregion
		}
	}
}