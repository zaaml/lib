// <copyright file="Grammar.TokenInterProductionCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class TokenInterProductionCollection : List<TokenInterProduction>
		{
			public void FromBuilder(TokenInterProductionCollectionBuilder builder)
			{
				for (var i = 0; i < builder.EntryCount; i++)
					Add(builder[i]);
			}
		}
	}
}