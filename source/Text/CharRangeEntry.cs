// <copyright file="CharRangeEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal class CharRangeEntry : CharSetEntry
	{
		#region Ctors

		public CharRangeEntry(char minChar, char maxChar)
		{
			MinChar = minChar;
			MaxChar = maxChar;
		}

		public CharRangeEntry(char minChar, bool minCharUnicode, char maxChar, bool maxCharUnicode)
		{
			MinChar = minChar;
			MinCharUnicode = minCharUnicode;
			MaxChar = maxChar;
			MaxCharUnicode = maxCharUnicode;
		}

		#endregion

		#region Properties

		public char MaxChar { get; }

		public bool MaxCharUnicode { get; }

		public char MinChar { get; }

		public bool MinCharUnicode { get; }

		#endregion
	}
}