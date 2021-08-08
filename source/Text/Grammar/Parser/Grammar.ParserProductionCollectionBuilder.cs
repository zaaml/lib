// <copyright file="Grammar.ParserProductionCollectionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal partial struct ParserProductionCollectionBuilder
		{
			private ParserProduction _entry0;
			private ParserProduction _entry1;
			private ParserProduction _entry2;
			private ParserProduction _entry3;
			private List<ParserProduction> _collection;
			private const int Capacity = 4;

			internal ParserProductionCollectionBuilder(ParserProduction parserEntry)
			{
				_collection = null;
				_entry0 = parserEntry;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				EntryCount = 1;
			}

			private void Add(ParserProduction parserEntry)
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
					_collection = new List<ParserProduction>(Capacity);

					for (var i = 0; i < EntryCount; i++)
						_collection.Add(this[i]);
				}
			}

			public int EntryCount { get; private set; }

			internal ParserProduction this[int index]
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

			public static ParserProductionCollectionBuilder operator |(ParserProductionCollectionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				var result = new ParserProductionCollectionBuilder();

				for (var i = 0; i < op1.EntryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2.EntryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionCollectionBuilder op1, ParserProductionBuilder op2)
			{
				return op1 | new ParserProductionCollectionBuilder(new ParserProduction(op2.CreateArray()));
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return new ParserProductionCollectionBuilder(new ParserProduction(op1.CreateArray())) | op2;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionCollectionBuilder op1, TokenInterProductionBuilder op2)
			{
				return op1 | new ParserProductionCollectionBuilder(op2.AsParserProductionBuilder());
			}

			public static ParserProductionCollectionBuilder operator |(TokenInterProductionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return new ParserProductionCollectionBuilder(op1.AsParserProductionBuilder()) | op2;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionCollectionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1 | new ParserProductionCollectionBuilder(new ParserProduction(new[] { op2.AsFragment().CreateParserEntry() }));
			}

			public static ParserProductionCollectionBuilder operator |(TokenInterProductionCollectionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return new ParserProductionCollectionBuilder(new ParserProduction(new[] { op1.AsFragment().CreateParserEntry() })) | op2;
			}

			public static ParserProductionBuilder operator +(ParserProductionCollectionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return op1.AsFragment() + op2.AsFragment();
			}

			public static ParserProductionBuilder operator +(ParserProductionCollectionBuilder op1, ParserProductionBuilder op2)
			{
				return op1.AsFragment() + op2;
			}

			public static ParserProductionBuilder operator +(ParserProductionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return op1 + op2.AsFragment();
			}

			public static ParserProductionBuilder operator +(ParserProductionCollectionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1.AsFragment() + new ParserProductionBuilder(op2.AsFragment().CreateParserEntry());
			}

			public static ParserProductionBuilder operator +(TokenInterProductionCollectionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return new ParserProductionBuilder(op1.AsFragment().CreateParserEntry()) + op2.AsFragment();
			}

			public static ParserProductionBuilder operator +(ParserProductionCollectionBuilder op1, TokenInterProductionBuilder op2)
			{
				return op1.AsFragment() + op2.AsParserProductionBuilder();
			}

			public static ParserProductionBuilder operator +(TokenInterProductionBuilder op1, ParserProductionCollectionBuilder op2)
			{
				return op1.AsParserProductionBuilder() + op2.AsFragment();
			}

			public static ParserProductionBuilder operator +(ParserProductionCollectionBuilder op1, TokenRule op2)
			{
				return op1.AsFragment() + new ParserProductionBuilder(new ParserTokenRuleEntry(op2));
			}

			public static ParserProductionBuilder operator +(TokenRule op1, ParserProductionCollectionBuilder op2)
			{
				return new ParserProductionBuilder(new ParserTokenRuleEntry(op1)) + op2.AsFragment();
			}

			public static implicit operator ParserProduction(ParserProductionCollectionBuilder builder)
			{
				return builder.AsFragment();
			}

			public ParserFragment Bind(string name)
			{
				var parserFragment = AsFragment();

				parserFragment.Name = name;

				return parserFragment;
			}

			public ParserFragment AsFragment()
			{
				var parserFragment = new ParserFragment(true);

				parserFragment.Productions.FromBuilder(this);

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

			public ParserQuantifierEntry Exact(int count, QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierHelper.Exact(count), mode);
			}

			public ParserQuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.OneOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.ZeroOrMore, mode);
			}

			public ParserQuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new ParserQuantifierEntry(AsFragment(), QuantifierKind.ZeroOrOne, mode);
			}

			#endregion
		}
	}
}