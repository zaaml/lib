// <copyright file="Grammar.CharEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal partial class Grammar<TToken>
	{
		protected internal sealed partial class CharEntry : PrimitiveMatchEntry
		{
			public CharEntry(char @char)
			{
				Char = @char;
			}

			public char Char { get; }
		}
	}
}