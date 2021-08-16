// <copyright file="Grammar.ParserProductionBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal partial struct ParserProductionBuilder
		{
			private ParserEntry _entry0;
			private ParserEntry _entry1;
			private ParserEntry _entry2;
			private ParserEntry _entry3;
			private List<ParserEntry> _collection;
			private int _entryCount;

			internal ParserProductionBuilder(ParserEntry parserEntry)
			{
				_collection = null;
				_entry0 = parserEntry;
				_entry1 = null;
				_entry2 = null;
				_entry3 = null;

				_entryCount = 1;
			}

			private ParserEntry this[int index]
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

			public static implicit operator ParserProduction(ParserProductionBuilder parserProductionBuilder)
			{
				return new ParserProduction(parserProductionBuilder.CreateArray());
			}

			public static ParserProductionBuilder operator +(ParserProductionBuilder op1, ParserProductionBuilder op2)
			{
				var result = new ParserProductionBuilder();

				for (var i = 0; i < op1._entryCount; i++)
					result.Add(op1[i]);

				for (var j = 0; j < op2._entryCount; j++)
					result.Add(op2[j]);

				return result;
			}

			private ParserProductionBuilder CreateQuantifier(QuantifierKind kind, QuantifierMode mode)
			{
				var parserRule = new ParserRule
				{
					IsInline = true
				};

				parserRule.Productions.Add(new ParserProduction(CreateArray()));

				return new ParserProductionBuilder(new ParserQuantifierEntry(new ParserRuleEntry(parserRule), kind, mode));
			}

			public ParserProductionBuilder ZeroOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return CreateQuantifier(QuantifierKind.ZeroOrMore, mode);
			}

			public ParserProductionBuilder ZeroOrOne(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return CreateQuantifier(QuantifierKind.ZeroOrOne, mode);
			}

			public ParserProductionBuilder OneOrMore(QuantifierMode mode = QuantifierMode.Greedy)
			{
				return CreateQuantifier(QuantifierKind.OneOrMore, mode);
			}

			public static ParserProductionBuilder operator +(ParserProductionBuilder op1, TokenInterProductionBuilder op2)
			{
				return op1 + op2.AsParserProductionBuilder();
			}

			public static ParserProductionBuilder operator +(TokenInterProductionBuilder op1, ParserProductionBuilder op2)
			{
				return op1.AsParserProductionBuilder() + op2;
			}

			public static ParserProductionBuilder operator +(ParserProductionBuilder op1, TokenInterEntry op2)
			{
				return op1 + new ParserProductionBuilder(op2.CreateParserEntry());
			}

			public static ParserProductionBuilder operator +(TokenInterEntry op1, ParserProductionBuilder op2)
			{
				return new ParserProductionBuilder(op1.CreateParserEntry()) + op2;
			}

			public static ParserProductionBuilder operator +(ParserProductionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return op1 + new ParserProductionBuilder(op2.AsFragment().CreateParserEntry());
			}

			public static ParserProductionBuilder operator +(TokenInterProductionCollectionBuilder op1, ParserProductionBuilder op2)
			{
				return new ParserProductionBuilder(op1.AsFragment().CreateParserEntry()) + op2;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionBuilder op1, ParserProductionBuilder op2)
			{
				return new ParserProductionCollectionBuilder(new ParserProduction(op1.CreateArray())) | op2;
			}

			public static ParserProductionCollectionBuilder operator |(TokenInterProductionBuilder op1, ParserProductionBuilder op2)
			{
				return new ParserProductionCollectionBuilder() | op1.AsParserProductionBuilder() | op2;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionBuilder op1, TokenInterProductionBuilder op2)
			{
				return new ParserProductionCollectionBuilder() | op1 | op2.AsParserProductionBuilder();
			}

			public static ParserProductionCollectionBuilder operator |(TokenInterProductionCollectionBuilder op1, ParserProductionBuilder op2)
			{
				return new ParserProductionCollectionBuilder() | new ParserProductionBuilder(op1.AsFragment().CreateParserEntry()) | op2;
			}

			public static ParserProductionCollectionBuilder operator |(ParserProductionBuilder op1, TokenInterProductionCollectionBuilder op2)
			{
				return new ParserProductionCollectionBuilder() | op1 | new ParserProductionBuilder(op2.AsFragment().CreateParserEntry());
			}

			private const int Capacity = 4;

			public void Add(ParserEntry parserEntry)
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
					_collection = new List<ParserEntry>(Capacity);

					for (var i = 0; i < _entryCount; i++)
						_collection.Add(this[i]);
				}
			}

			internal ParserEntry[] CreateArray()
			{
				var entryArray = new ParserEntry[_entryCount];

				for (var i = 0; i < _entryCount; i++)
					entryArray[i] = this[i];

				return entryArray;
			}
		}
	}
}