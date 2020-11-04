// <copyright file="Grammar.TokenInterProduction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed class TokenInterProduction
		{
			#region Ctors

			public TokenInterProduction(TokenInterEntry[] entries)
			{
				Entries = entries;
			}

			#endregion

			#region Properties

			public TokenInterEntry[] Entries { get; }

			#endregion
		}

		#endregion
	}
}