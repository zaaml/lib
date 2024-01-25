// <copyright file="CharUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.Core.Utils
{
	internal static class CharUtils
	{
		public static int HexCharToDecimal(char c)
		{
			return c switch
			{
				>= '0' and <= '9' => c - '0',
				>= 'a' and <= 'f' => c - 'a' + 10,
				>= 'A' and <= 'F' => c - 'A' + 10,
				_ => -1
			};
		}

		public static char GetEscapeChar(char escape)
		{
			return escape switch
			{
				'a' => '\a',
				'b' => '\b',
				'f' => '\f',
				'n' => '\n',
				'r' => '\r',
				't' => '\t',
				'v' => '\v',
				'\\' => '\\',
				'\'' => '\'',
				_ => throw new ArgumentOutOfRangeException(nameof(escape))
			};
		}
	}
}