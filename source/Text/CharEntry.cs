// <copyright file="CharEntry.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

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

		public static CharEntry Parse(string charLiteral)
		{
			string charStr;

			if (charLiteral.StartsWith("'") && charLiteral.EndsWith("'") ||
			    charLiteral.StartsWith("\"") && charLiteral.EndsWith("\""))
			{
				charStr = charLiteral.Substring(1, charLiteral.Length - 2);
			}
			else
				throw new InvalidOperationException();

			var c = ParseChar(charStr, out var unicode);

			return new CharEntry(c, unicode);
		}

		public static char ParseChar(string charStr, out bool unicode)
		{
			var index = 0;

			return ParseChar(charStr, charStr.Length, ref index, out unicode);
		}

		private static int HexCharToDecimal(char c)
		{
			return c switch
			{
				>= '0' and <= '9' => c - '0',
				>= 'a' and <= 'f' => c - 'a' + 10,
				>= 'A' and <= 'F' => c - 'A' + 10,
				_ => -1
			};
		}

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
				case '\'': return '\'';
				case '-': return '-';
			}

			throw new ArgumentOutOfRangeException(nameof(escape));
		}

		internal static char ParseChar(string charSetString, int endIndex, ref int index, out bool unicode)
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

					return (char)decimalValue;
				}

				index++;

				unicode = false;

				return GetEscapeChar(nextChar);
			}

			index++;

			unicode = false;

			return c;
		}

		public override string Format(bool set)
		{
			if (set)
			{
				return Char switch
				{
					'\a' => "\\a",
					'\b' => "\\b",
					'\f' => "\\f",
					'\n' => "\\n",
					'\r' => "\\r",
					'\t' => "\\t",
					'\v' => "\\v",
					'\\' => "\\\\",
					'\'' => "\\'",
					_ => Unicode ? $"\\u{(int)Char:x4}" : $"{Char}"
				};
			}

			return Char switch
			{
				'\a' => "'\\a'",
				'\b' => "'\\b'",
				'\f' => "'\\f'",
				'\n' => "'\\n'",
				'\r' => "'\\r'",
				'\t' => "'\\t'",
				'\v' => "'\\v'",
				'\\' => "'\\\\'",
				'\'' => "'\\''",
				_ => Unicode ? $"'\\u{(int)Char:x4}'" : $"'{Char}'"
			};
		}
	}
}