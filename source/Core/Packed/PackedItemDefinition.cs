// <copyright file="PackedItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core.Packed
{
  internal class PackedItemDefinition
  {
    #region Fields

    private readonly ulong _maxValue;

    public readonly int BitCount;
    public readonly int BitOffset;

    #endregion

    #region Ctors

    internal PackedItemDefinition(int bitOffset, int bitCount)
    {
      BitOffset = bitOffset;
      BitCount = bitCount;

      _maxValue = ~0ul >> (64 - bitCount);
    }

    #endregion

    #region  Methods

		[Conditional("DEBUG")]
    protected void Validate(ulong value)
    {
      if (value > _maxValue)
        throw new ArgumentOutOfRangeException();
    }

    #endregion
  }
}