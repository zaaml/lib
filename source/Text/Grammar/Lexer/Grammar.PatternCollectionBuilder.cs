// <copyright file="Grammar.PatternCollectionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal struct PatternCollectionBuilder
		{
			private TokenEntry[] _entry0;
			private TokenEntry[] _entry1;
			private TokenEntry[] _entry2;
			private TokenEntry[] _entry3;
			private List<TokenEntry[]> _collection;
			private const int Capacity = 4;

			internal PatternCollectionBuilder(TokenEntry[] parserEntry)
			{
				_collection = null;
				_entry0 = parserEntry;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				EntryCount = 1;
			}

			private void Add(TokenEntry[] parserEntry)
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
					_collection = new List<TokenEntry[]>(Capacity);

					for (var i = 0; i < EntryCount; i++)
						_collection.Add(this[i]);
				}
			}

			public int EntryCount { get; private set; }

			internal TokenEntry[] this[int index]
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

			public static PatternCollectionBuilder operator |(PatternCollectionBuilder op1, PatternCollectionBuilder op2)
			{
				var result = new PatternCollectionBuilder();

				for (var i = 0; i < op1.EntryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2.EntryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			public static implicit operator PatternCollectionBuilder(TokenEntry entry)
			{
				return new PatternCollectionBuilder(new[] { entry });
			}

			public static PatternCollectionBuilder operator |(PatternCollectionBuilder op1, PatternBuilder op2)
			{
				return op1 | new PatternCollectionBuilder(op2.CreateArray());
			}

			public TokenFragment AsFragment()
			{
				return new TokenFragment(this);
			}

			public QuantifierEntry ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(new TokenFragment(this)), QuantifierKind.ZeroOrMore, mode);
			}

			public QuantifierEntry ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(new TokenFragment(this)), QuantifierKind.ZeroOrOne, mode);
			}

			public QuantifierEntry OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return new QuantifierEntry(new TokenFragmentEntry(new TokenFragment(this)), QuantifierKind.OneOrMore, mode);
			}

			#endregion
		}
	}
}