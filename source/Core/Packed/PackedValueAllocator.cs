// <copyright file="PackedValueAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Packed
{
  internal class PackedValueAllocator
  {
    #region Fields

    private int _bitOffset;

    #endregion

    #region  Methods

    public PackedBoolItemDefinition AllocateBoolItem()
    {
      return IncrementOffset(new PackedBoolItemDefinition(_bitOffset));
    }

    public PackedByteItemDefinition AllocateByteItem()
    {
      return IncrementOffset(new PackedByteItemDefinition(_bitOffset));
    }

    public PackedEnumItemDefinition<TEnum> AllocateEnumItem<TEnum>() where TEnum : struct, Enum, IConvertible
    {
      return IncrementOffset(new PackedEnumItemDefinition<TEnum>(_bitOffset));
    }

    public PackedShortItemDefinition AllocateShortItem()
    {
      return IncrementOffset(new PackedShortItemDefinition(_bitOffset));
    }

    public PackedUIntItemDefinition AllocateUIntItem(uint maxValue)
    {
      return IncrementOffset(new PackedUIntItemDefinition(_bitOffset, BitUtils.SignificantBitCount(maxValue)));
    }

    public PackedUShortItemDefinition AllocateUShortItem()
    {
      return IncrementOffset(new PackedUShortItemDefinition(_bitOffset));
    }


    public PackedValueAllocator Clone()
    {
      return new PackedValueAllocator
      {
        _bitOffset = _bitOffset
      };
    }

    private TDefinition IncrementOffset<TDefinition>(TDefinition itemDefinition) where TDefinition : PackedItemDefinition
    {
      _bitOffset += itemDefinition.BitCount;
      return itemDefinition;
    }

    #endregion
  }
}