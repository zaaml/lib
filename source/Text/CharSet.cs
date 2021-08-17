// <copyright file="CharSet.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Text
{
	internal class CharSet
	{
		public CharSet(CharSetEntry[] entries, bool except = false)
		{
			Entries = entries;
			Except = except;
		}

		public CharSetEntry[] Entries { get; }

		public bool Except { get; }

		public bool IsPrimitive => Except == false && Entries.Length == 1;

		private static char GetMinChar(CharSetEntry entry)
		{
			if (entry is CharRangeEntry range)
				return range.Min.Char;

			return ((CharEntry)entry).Char;
		}

		public CharSet Inverse()
		{
			var listRanges = new List<CharSetEntry>();
			var current = char.MinValue;

			foreach (var entry in Entries.OrderBy(GetMinChar))
			{
				if (entry is CharRangeEntry range)
				{
					if (range.Min.Char > current)
					{
						var next = (char)(range.Min.Char - 1);

						if (current < next)
							listRanges.Add(new CharRangeEntry(new CharEntry(current), new CharEntry(next)));
					}

					current = (char)(range.Max.Char + 1);

					continue;
				}

				var charEntry = (CharEntry)entry;
				var prev = (char)(charEntry.Char - 1);

				if (current < prev)
					listRanges.Add(new CharRangeEntry(new CharEntry(current), new CharEntry(prev)));

				current = (char)(charEntry.Char + 1);
			}

			if (current < char.MaxValue)
				listRanges.Add(new CharRangeEntry(new CharEntry(current), new CharEntry(char.MaxValue)));

			return new CharSet(listRanges.ToArray(), !Except);
		}

		public static CharSet Parse(string charSetString)
		{
			if ((charSetString.StartsWith("[") && charSetString.EndsWith("]")) == false)
				throw new InvalidOperationException();

			var index = 1;
			var entriesList = new List<CharSetEntry>();
			var endIndex = charSetString.Length - 1;

			while (index < endIndex)
			{
				var c = CharEntry.ParseChar(charSetString, endIndex, ref index, out var cu);

				if (charSetString[index] == '-')
				{
					index++;

					var next = CharEntry.ParseChar(charSetString, endIndex, ref index, out var ncu);

					entriesList.Add(new CharRangeEntry(new CharEntry(c, cu), new CharEntry(next, ncu)));
				}
				else
					entriesList.Add(new CharEntry(c, cu));
			}

			return new CharSet(entriesList.ToArray());
		}
	}
}