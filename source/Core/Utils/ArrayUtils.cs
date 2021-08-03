// <copyright file="ArrayUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Runtime.CompilerServices;

namespace Zaaml.Core.Utils
{
	internal static class ArrayUtils
	{
		#region Static Fields and Constants

		internal const int MaxArrayLength = 0X7FEFFFFF;
		internal const int DefaultCapacity = 8;

		#endregion

		#region  Methods

		public static void Fill<T>(T[] array, T value)
		{
#if NETCOREAPP3_1
			Array.Fill(array, value);
#else
			for (var i = 0; i < array.Length; i++)
				array[i] = value;			
#endif
		}

		public static void Fill<T>(T[] array, T value, int startIndex, int length)
		{
#if NETCOREAPP3_1
			Array.Fill(array, value, startIndex, length);
#else
			for (var i = startIndex; i < startIndex + length; i++)
				array[i] = value;
#endif
		}

		public static void ExpandArrayLength<T>(ref T[] array, bool copyItems)
		{
			EnsureArrayLength(ref array, array.Length * 2, copyItems);
		}

		public static void EnsureArrayLength<T>(ref T[] array, int minLength, bool copyItems)
		{
			if (array == null)
			{
				array = new T[minLength];

				return;
			}

			if (array.Length >= minLength)
				return;

			var newCapacity = array.Length == 0 ? DefaultCapacity : array.Length * 2;

			if ((uint) newCapacity > MaxArrayLength)
				newCapacity = MaxArrayLength;

			if (newCapacity < minLength)
				newCapacity = minLength;

			var newArray = new T[newCapacity];

			if (copyItems)
				for (var index = 0; index < array.Length; index++)
					newArray[index] = array[index];

			array = newArray;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ClearList<T>(ref T[] array, ref int count, bool clean = true)
		{
			if (clean)
				Array.Clear(array, 0, count);

			count = 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void AddList<T>(T value, ref T[] array, ref int count)
		{
			if (count >= array.Length) 
				EnsureArrayLength(ref array, count * 2, true);

			array[count++] = value;
		}

#endregion
	}
}