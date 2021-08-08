// <copyright file="Grammar.PatternCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class PatternCollection
		{
			internal List<TokenPattern> Patterns { get; } = new List<TokenPattern>();

			public static implicit operator PatternCollection(TokenPattern transition)
			{
				return new PatternCollection().WithPattern(transition);
			}

			public static implicit operator PatternCollection(PatternBuilder builder)
			{
				return new PatternCollection().WithPattern(builder);
			}

			public static implicit operator PatternCollection(TokenEntry parserEntry)
			{
				return new PatternCollection().WithPattern(new TokenPattern(new[] { parserEntry }));
			}

			public static implicit operator PatternCollection(PatternCollectionBuilder parserEntry)
			{
				var collection = new PatternCollection();

				for (var i = 0; i < parserEntry.EntryCount; i++)
					collection.Patterns.Add(new TokenPattern(parserEntry[i]));

				return collection;
			}

			private PatternCollection WithPattern(TokenPattern transition)
			{
				Patterns.Add(transition);

				return this;
			}
		}
	}
}