// <copyright file="UnicodeUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Core.Utils
{
	internal static class UnicodeUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void GetUtf16Surrogates(uint value, out char highSurrogate, out char lowSurrogate)
		{
			highSurrogate = (char) ((value + ((0xD800u - 0x40u) << 10)) >> 10);
			lowSurrogate = (char) ((value & 0x3FFu) + 0xDC00u);
		}

		public static bool IsUtf16SurrogatePair(uint value)
		{
			return value > 0xFFFFu;
		}
	}
}