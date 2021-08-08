// <copyright file="Grammar.TokenFragmentEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class TokenFragmentEntry : TokenPrimitiveEntry
		{
			public TokenFragmentEntry(TokenFragment fragment)
			{
				Fragment = fragment;
			}

			public TokenFragment Fragment { get; }
		}
	}
}