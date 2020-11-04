// <copyright file="Grammar.CharEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		#region Nested Types

		protected internal sealed partial class CharEntry : PrimitiveMatchEntry
		{
			#region Ctors

			public CharEntry(char @char)
			{
				Char = @char;
			}

			#endregion

			#region Properties

			public char Char { get; }

			#endregion
		}

		#endregion
	}
}