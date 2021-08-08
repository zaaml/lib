// <copyright file="Grammar.PatternBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken> where TToken : unmanaged, Enum
	{
		protected internal struct PatternBuilder
		{
			private TokenEntry _entry0;
			private TokenEntry _entry1;
			private TokenEntry _entry2;
			private TokenEntry _entry3;
			private List<TokenEntry> _collection;
			private int _entryCount;

			internal PatternBuilder(TokenEntry parserEntry)
			{
				_collection = null;
				_entry0 = parserEntry;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				_entryCount = 1;
			}

			private TokenEntry this[int index]
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

			public static implicit operator PatternBuilder(TokenFragment fragment)
			{
				return new PatternBuilder(fragment);
			}

			public static implicit operator TokenPattern(PatternBuilder builder)
			{
				return new TokenPattern(builder.CreateArray());
			}

			//public static implicit operator TokenFragment(PatternBuilder builder)
			//{
			//	return new TokenPattern(builder.CreateArray());
			//}

			public static PatternBuilder operator +(PatternBuilder op1, PatternBuilder op2)
			{
				var result = new PatternBuilder();

				for (var i = 0; i < op1._entryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2._entryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			private const int Capacity = 4;

			private void Add(TokenEntry parserEntry)
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
					_collection = new List<TokenEntry>(Capacity);

					for (var i = 0; i < _entryCount; i++)
						_collection.Add(this[i]);
				}
			}

			internal TokenEntry[] CreateArray()
			{
				var entryArray = new TokenEntry[_entryCount];

				for (var i = 0; i < _entryCount; i++)
					entryArray[i] = this[i];

				return entryArray;
			}

			public static PatternCollectionBuilder operator |(PatternBuilder op1, PatternBuilder op2)
			{
				return new PatternCollectionBuilder(op1.CreateArray()) | op2;
			}

			public static PatternBuilder operator +(PatternBuilder op1, PatternCollectionBuilder op2)
			{
				return op1 + op2.AsFragment();
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
		}
	}
}