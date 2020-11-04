// <copyright file="Grammar.ParserProductionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken> where TToken : unmanaged, Enum
	{
		#region Nested Types

		protected internal class ParserProductionCollection : List<ParserProduction>
		{
			#region Methods

			public void FromBuilder(ParserProductionCollectionBuilder builder)
			{
				for (var i = 0; i < builder.EntryCount; i++)
					Add(builder[i]);
			}

			public static implicit operator ParserProductionCollection(ParserProduction parserProduction)
			{
				return new ParserProductionCollection().WithProduction(parserProduction);
			}

			public static implicit operator ParserProductionCollection(ParserProductionBuilder builder)
			{
				return new ParserProductionCollection().WithProduction(builder);
			}

			public static implicit operator ParserProductionCollection(ParserRule parserRule)
			{
				return new ParserProductionCollection().WithProduction(parserRule);
			}

			public static implicit operator ParserProductionCollection(ParserEntry parserEntry)
			{
				return new ParserProductionCollection().WithProduction(new ParserProduction(new[] {parserEntry}));
			}

			public static implicit operator ParserProductionCollection(ParserProductionCollectionBuilder builder)
			{
				var collection = new ParserProductionCollection();

				for (var i = 0; i < builder.EntryCount; i++)
					collection.Add(builder[i]);

				return collection;
			}

			private ParserProductionCollection WithProduction(ParserProduction parserProduction)
			{
				Add(parserProduction);

				return this;
			}

			#endregion
		}

		#endregion
	}
}