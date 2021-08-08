// <copyright file="ArrayUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Utils
{
	internal static class ArrayUtils
	{
		internal const int MaxArrayLength = 0X7FEFFFFF;
		internal const int DefaultCapacity = 8;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AddList<T>(T value, ref T[] array, ref int count)
		{
			if (count >= array.Length)
				EnsureArrayLength(ref array, count + 1, true);

			array[count++] = value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ClearList<T>(ref T[] array, ref int count, bool clean = true)
		{
			if (clean)
				Array.Clear(array, 0, count);

			count = 0;
		}

		public static void EnsureArrayLength<T>(ref T[] array, int length)
		{
			length = BitUtils.Power2Ceiling(length);

			if (array == null)
			{
				array = new T[length];

				return;
			}

			if (length <= array.Length)
				return;

			array = new T[length];
		}

		public static void EnsureArrayLength<T>(ref T[] array, int length, bool copyItems)
		{
			length = BitUtils.Power2Ceiling(length);

			if (array == null)
			{
				array = new T[length];

				return;
			}

			if (length <= array.Length)
				return;

			var newArray = new T[length];

			if (copyItems)
				Array.Copy(array, newArray, array.Length);

			array = newArray;
		}

		public static void EnsureArrayLength<T>(ref T[] array, int length, ArrayPool<T> pool, bool copyItems, bool clearArray)
		{
			length = BitUtils.Power2Ceiling(length);

			if (array == null)
			{
				array = pool.Rent(length);

				return;
			}

			if (length <= array.Length)
				return;
			
			var newArray = pool.Rent(length);

			if (copyItems)
				Array.Copy(array, newArray, array.Length);

			pool.Return(array, clearArray);

			array = newArray;
		}

		public static void ExpandArrayLength<T>(ref T[] array, bool copyItems)
		{
			EnsureArrayLength(ref array, array.Length + 1, copyItems);
		}

		public static void ExpandArrayLength<T>(ref T[] array, ArrayPool<T> pool, bool copyItems, bool clearArray)
		{
			EnsureArrayLength(ref array, array.Length + 1, pool, copyItems, clearArray);
		}

		public static void Fill<T>(T[] array, T value)
		{
#if NETCOREAPP
			Array.Fill(array, value);
#else
			for (var i = 0; i < array.Length; i++)
				array[i] = value;
#endif
		}

		public static void Fill<T>(T[] array, T value, int startIndex, int length)
		{
#if NETCOREAPP
			Array.Fill(array, value, startIndex, length);
#else
			for (var i = startIndex; i < startIndex + length; i++)
				array[i] = value;
#endif
		}
	}
}