// <copyright file="Grammar.TokenInterProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed class TokenInterProduction
		{
			public TokenInterProduction(TokenInterEntry[] entries)
			{
				Entries = entries;
			}

			public TokenInterEntry[] Entries { get; }
		}
	}
}