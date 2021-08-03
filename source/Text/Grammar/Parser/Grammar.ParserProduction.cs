// <copyright file="Grammar.ParserProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class ParserProduction
		{
			public static readonly ParserProduction Empty = new ParserProduction();

			public ParserProduction(ParserEntry[] entries)
			{
				Entries = entries;
			}

			private ParserProduction()
			{
				Entries = new ParserEntry[0];
			}

			public ParserEntry[] Entries { get; }

			internal string Name { get; set; }

			public ParserProductionBinding ProductionBinding { get; internal set; }

			internal bool Unwrap { get; set; }

			public static implicit operator ParserProduction(Parser<TToken>.PredicateEntry parserPredicateEntry)
			{
				return new ParserProduction(new ParserEntry[] {new ParserPredicate(parserPredicateEntry)});
			}

			public static implicit operator ParserProduction(Parser<TToken>.ActionEntry parserActionEntry)
			{
				return new ParserProduction(new ParserEntry[] {new ParserAction(parserActionEntry)});
			}

			public static implicit operator ParserProduction(TokenRule tokenRule)
			{
				return new ParserProduction(new ParserEntry[] {tokenRule});
			}

			public static implicit operator ParserProduction(TokenInterProductionBuilder builder)
			{
				return new ParserProduction(new[] {builder.AsParserEntry()});
			}

			public static implicit operator ParserProduction(TokenInterProductionCollectionBuilder builder)
			{
				return new ParserProduction(new[] {builder.AsFragment().CreateParserEntry()});
			}

			public static implicit operator ParserProduction(TokenInterEntry entry)
			{
				return new ParserProduction(new[] {entry.CreateParserEntry()});
			}

			public override string ToString()
			{
				return Name;
			}
		}
	}
}