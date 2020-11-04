// <copyright file="CharEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Text
{
	internal class CharEntry : CharSetEntry
	{
		#region Ctors

		public CharEntry(char c)
		{
			Char = c;
		}

		public CharEntry(char c, bool unicode)
		{
			Char = c;
			Unicode = unicode;
		}

		#endregion

		#region Properties

		public char Char { get; }

		public bool Unicode { get; }

		#endregion
	}
}