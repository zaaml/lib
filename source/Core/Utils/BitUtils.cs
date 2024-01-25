// <copyright file="BitUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

namespace Zaaml.Core.Utils
{
	internal static class BitUtils
	{
		public static int LeadingZeroCount(ulong input)
		{
			if (input == 0)
				return 64;

			ulong n = 1;

			if (input >> 32 == 0)
			{
				n += 32;
				input <<= 32;
			}

			if (input >> 48 == 0)
			{
				n += 16;
				input <<= 16;
			}

			if (input >> 56 == 0)
			{
				n += 8;
				input <<= 8;
			}

			if (input >> 60 == 0)
			{
				n += 4;
				input <<= 4;
			}

			if (input >> 62 == 0)
			{
				n += 2;
				input <<= 2;
			}

			n -= input >> 63;

			return (int)n;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateLeft(ulong value, int bitsCount)
		{
			return (value << bitsCount) | (value >> (64 - bitsCount));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateLeft(uint value, int bitsCount)
		{
			return (value << bitsCount) | (value >> (32 - bitsCount));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint RotateRight(uint value, int bitsCount)
		{
			return (value >> bitsCount) | (value << (32 - bitsCount));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong RotateRight(ulong value, int bitsCount)
		{
			return (value >> bitsCount) | (value << (64 - bitsCount));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(int value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(uint value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(short value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(ushort value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(byte value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(long value)
		{
			return MostSignificantPosition((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int MostSignificantPosition(ulong input)
		{
			return 63 - LeadingZeroCount(input);
		}

		public static int NextPower2(int input)
		{
			if (input < 0)
				return 0;

			--input;
			input |= input >> 1;
			input |= input >> 2;
			input |= input >> 4;
			input |= input >> 8;
			input |= input >> 16;

			return input + 1;
		}

		public static int Power2Ceiling(int input)
		{
			var nextPower2 = NextPower2(input);

			return input == nextPower2 >> 1 ? input : nextPower2;
		}
	}
}