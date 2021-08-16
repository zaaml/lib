// <copyright file="Grammar.TokenInterProductionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal partial struct TokenInterProductionBuilder
		{
			private TokenInterEntry _entry0;
			private TokenInterEntry _entry1;
			private TokenInterEntry _entry2;
			private TokenInterEntry _entry3;
			private List<TokenInterEntry> _collection;
			private int _entryCount;

			public TokenInterProductionBuilder(TokenInterEntry tokenRule)
			{
				_collection = null;
				_entry0 = tokenRule;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				_entryCount = 1;
			}

			private TokenInterEntry this[int index]
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

			private const int Capacity = 4;

			private void Add(TokenInterEntry parserEntry)
			{
				if (_entryCount >= Capacity)
				{
					_collection.Add(parserEntry);

					_entryCount++;

					return;
				}

				if (_entryCount == 0)
					_entry0 = parserEntry;
				else if (_entryCount == 1)
					_entry1 = parserEntry;
				else if (_entryCount == 2)
					_entry2 = parserEntry;
				else if (_entryCount == 3)
					_entry3 = parserEntry;

				_entryCount++;

				if (_entryCount == Capacity)
				{
					_collection = new List<TokenInterEntry>(Capacity);

					for (var i = 0; i < _entryCount; i++)
						_collection.Add(this[i]);
				}
			}

			internal TokenInterEntry[] CreateArray()
			{
				var entryArray = new TokenInterEntry[_entryCount];

				for (var i = 0; i < _entryCount; i++)
					entryArray[i] = this[i];

				return entryArray;
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionBuilder op1, TokenRule op2)
			{
				return op1 + new TokenInterProductionBuilder(new TokenInterRuleEntry(op2));
			}

			public static TokenInterProductionBuilder operator +(TokenRule op1, TokenInterProductionBuilder op2)
			{
				return new TokenInterProductionBuilder(new TokenInterRuleEntry(op1)) + op2;
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionBuilder op1, TokenInterEntry op2)
			{
				return op1 + new TokenInterProductionBuilder(op2);
			}

			public static TokenInterProductionBuilder operator +(TokenInterEntry op1, TokenInterProductionBuilder op2)
			{
				return new TokenInterProductionBuilder(op1) + op2;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionBuilder op1, TokenRule op2)
			{
				return new TokenInterProductionCollectionBuilder() | op1 | new TokenInterProductionBuilder(new TokenInterRuleEntry(op2));
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenRule op1, TokenInterProductionBuilder op2)
			{
				return new TokenInterProductionCollectionBuilder() | new TokenInterProductionBuilder(new TokenInterRuleEntry(op1)) + op2;
			}

			public static TokenInterProductionBuilder operator +(TokenInterProductionBuilder op1, TokenInterProductionBuilder op2)
			{
				var result = new TokenInterProductionBuilder();

				for (var i = 0; i < op1._entryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2._entryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			public static TokenInterProductionCollectionBuilder operator |(TokenInterProductionBuilder op1, TokenInterProductionBuilder op2)
			{
				return new TokenInterProductionCollectionBuilder() | op1 | op2;
			}

			public TokenInterFragment AsFragment()
			{
				var fragment = new TokenInterFragment();

				fragment.Productions.Add(new TokenInterProduction(CreateArray()));

				return fragment;
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

			public ParserEntry AsParserEntry()
			{
				return AsFragment().CreateParserEntry();
			}

			public ParserProductionBuilder AsParserProductionBuilder()
			{
				var parserProductionBuilder = new ParserProductionBuilder();

				foreach (var entry in CreateArray()) 
					parserProductionBuilder.Add(entry.CreateParserEntry());

				return parserProductionBuilder;
			}
		}
	}
}