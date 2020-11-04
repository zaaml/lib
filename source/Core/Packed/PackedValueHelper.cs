// <copyright file="PackedValueHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Runtime.CompilerServices;

#if DEBUG
using System;
#endif

namespace Zaaml.Core.Packed
{
  //TODO Create Unit Tests
  internal static class PackedValueHelper
  {
#region Static Fields and Constants

	  public const int ByteBitCount = 8;
	  public const int UShortBitCount = 16;
	  public const int UIntBitCount = 32;
	  public const int ULongBitCount = 64;

#endregion

#region  Methods

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte GetByteValue(byte packedValue, int bitOffset, int bitCount, int mask)
    {
	    unchecked
	    {
#if DEBUG
		    if (bitOffset + bitCount > ByteBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

		    return (byte) (packedValue >> bitOffset & mask);
	    }
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort GetUShortValue(ushort packedValue, int bitOffset, int bitCount, int mask)
    {
	    unchecked
			{
#if DEBUG
		    if (bitOffset + bitCount > UShortBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

		    return (ushort) (packedValue >> bitOffset & mask);
	    }
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint GetUIntValue(uint packed, int bitOffset, int bitCount, uint mask)
    {
#if DEBUG
		    if (bitOffset + bitCount > UIntBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

	    return packed >> bitOffset & mask;
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong GetULongValue(ulong packed, int bitOffset, int bitCount, ulong mask)
    {
#if DEBUG
		    if (bitOffset + bitCount > ULongBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

	    return packed >> bitOffset & mask;
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte SetByteValue(byte packed, byte value, int bitOffset, int bitCount, int mask, int exmask)
    {
	    unchecked
			{
#if DEBUG
		    if (bitOffset + bitCount > ByteBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

		    return (byte) (packed & exmask | (value & mask) << bitOffset);
	    }
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ushort SetUShortValue(ushort packed, ushort value, int bitOffset, int bitCount, int mask, int exmask)
    {
	    unchecked
			{
#if DEBUG
		    if (bitOffset + bitCount > UShortBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

		    return (ushort) (packed & exmask | (value & mask) << bitOffset);
	    }
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint SetUIntValue(uint packed, uint value, int bitOffset, int bitCount, uint mask, uint exmask)
    {
#if DEBUG
		    if (bitOffset + bitCount > UIntBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

	    return packed & exmask | (value & mask) << bitOffset;
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong SetULongValue(ulong packed, ulong value, int bitOffset, int bitCount, ulong mask, ulong exmask)
    {
#if DEBUG
		    if (bitOffset + bitCount > ULongBitCount)
			    throw new ArgumentOutOfRangeException();
#endif

	    return packed & exmask | (value & mask) << bitOffset;
    }

#endregion
  }

  internal abstract class PackedItemDefinition<T> : PackedItemDefinition
  {
	  public const int ByteBitCount = 8;
	  public const int UShortBitCount = 16;
	  public const int UIntBitCount = 32;
	  public const int ULongBitCount = 64;

    // ReSharper disable InconsistentNaming
		private readonly int _mask_byte;
	  private readonly int _exmask_byte;

	  private readonly int _mask_ushort;
	  private readonly int _exmask_ushort;

		private readonly uint _mask_uint;
	  private readonly uint _exmask_uint;

	  private readonly ulong _mask_ulong;
	  private readonly ulong _exmask_ulong;
    // ReSharper restore InconsistentNaming

    #region Ctors

    protected PackedItemDefinition(int bitOffset, int bitCount)
      : base(bitOffset, bitCount)
    {
			// Byte
	    _mask_byte = byte.MaxValue >> ByteBitCount - bitCount;
	    _exmask_byte = ~(_mask_byte << bitOffset);

			// UShort
	    _mask_ushort = ushort.MaxValue >> UShortBitCount - bitCount;
	    _exmask_ushort = ~(_mask_ushort << bitOffset);

			// UInt
	    _mask_uint = uint.MaxValue >> UIntBitCount - bitCount;
	    _exmask_uint = ~(_mask_uint << bitOffset);

			// ULong
	    _mask_ulong = ulong.MaxValue >> ULongBitCount - bitCount;
	    _exmask_ulong = ~(_mask_ulong << bitOffset);
		}

		#endregion

		#region  Methods

		protected abstract T Convert(ulong value);

    protected abstract ulong Convert(T value);

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(ulong packedValue)
    {
      return Convert(PackedValueHelper.GetULongValue(packedValue, BitOffset, BitCount, _mask_ulong));
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(uint packedValue)
    {
      return Convert(PackedValueHelper.GetUIntValue(packedValue, BitOffset, BitCount, _mask_uint));
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(ushort packedValue)
    {
      return Convert(PackedValueHelper.GetUShortValue(packedValue, BitOffset, BitCount, _mask_ushort));
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetValue(byte packedValue)
    {
      return Convert(PackedValueHelper.GetByteValue(packedValue, BitOffset, BitCount, _mask_byte));
    }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(ref ulong packedValue, T value)
	  {
		  unchecked
		  {
				var convert = Convert(value);

			  Validate(convert);

			  packedValue = PackedValueHelper.SetULongValue(packedValue, convert, BitOffset, BitCount, _mask_ulong, _exmask_ulong);
			}
	  }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(ref uint packedValue, T value)
	  {
		  unchecked
			{
			  var convert = (uint)Convert(value);

			  Validate(convert);

			  packedValue = PackedValueHelper.SetUIntValue(packedValue, convert, BitOffset, BitCount, _mask_uint, _exmask_uint);
		  }
	  }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(ref ushort packedValue, T value)
	  {
		  unchecked
			{
			  var convert = (ushort)Convert(value);

			  Validate(convert);

			  packedValue = PackedValueHelper.SetUShortValue(packedValue, convert, BitOffset, BitCount, _mask_ushort, _exmask_ushort);
		  }
	  }

	  [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(ref byte packedValue, T value)
    {
	    unchecked
			{
				var convert = (byte)Convert(value);

		    Validate(convert);

		    packedValue = PackedValueHelper.SetByteValue(packedValue, convert, BitOffset, BitCount, _mask_byte, _exmask_byte);
	    }
    }

#endregion
  }
}