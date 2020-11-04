// <copyright file="CharSet.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.Text
{
	internal class CharSet
	{
		#region Ctors

		public CharSet(CharSetEntry[] entries, bool except = false)
		{
			Entries = entries;
			Except = except;
		}

		#endregion

		#region Properties

		public CharSetEntry[] Entries { get; }

		public bool Except { get; }

		#endregion

		#region Methods

		private static char GetEscapeChar(char escape)
		{
			switch (escape)
			{
				case 'a': return '\a';
				case 'b': return '\b';
				case 'f': return '\f';
				case 'n': return '\n';
				case 'r': return '\r';
				case 't': return '\t';
				case 'v': return '\v';
				case '\\': return '\\';
				case '-': return '-';
				case '\'': return '\'';
			}

			throw new ArgumentOutOfRangeException(nameof(escape));
		}

		private static int HexCharToDecimal(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';

			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;

			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;

			return -1;
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
				var c = ParseChar(charSetString, endIndex, ref index, out var cu);

				if (charSetString[index] == '-')
				{
					index++;

					var next = ParseChar(charSetString, endIndex, ref index, out var ncu);

					entriesList.Add(new CharRangeEntry(c, cu, next, ncu));
				}
				else
					entriesList.Add(new CharEntry(c, cu));
			}

			return new CharSet(entriesList.ToArray());
		}

		public static char ParseChar(string charStr, out bool unicode)
		{
			var index = 0;

			return ParseChar(charStr, charStr.Length, ref index, out unicode);
		}

		private static char ParseChar(string charSetString, int endIndex, ref int index, out bool unicode)
		{
			var c = charSetString[index];

			if (c == '\\')
			{
				if (index + 1 >= endIndex)
					throw new InvalidOperationException();

				var nextChar = charSetString[++index];

				if (nextChar == 'u')
				{
					var unicodeDigitCounter = 0;
					var decimalValue = 0;

					while (index + 1 < endIndex)
					{
						var decVal = HexCharToDecimal(charSetString[index + 1]);

						if (decVal == -1)
							break;

						decimalValue = (decimalValue << 4) + decVal;

						unicodeDigitCounter++;
						index++;
					}

					if (unicodeDigitCounter > 4)
						throw new InvalidOperationException();

					index++;

					unicode = true;

					return (char) decimalValue;
				}

				index++;

				unicode = false;

				return GetEscapeChar(nextChar);
			}

			index++;

			unicode = false;

			return c;
		}

		#endregion
	}
}