// <copyright file="Grammar.TokenFragmentEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class TokenFragmentEntry : TokenPrimitiveEntry
		{
			#region Ctors

			public TokenFragmentEntry(TokenFragment fragment)
			{
				Fragment = fragment;
			}

			#endregion

			#region Properties

			public TokenFragment Fragment { get; }

			#endregion
		}

		#endregion
	}
}