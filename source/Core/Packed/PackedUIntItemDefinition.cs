// <copyright file="PackedUIntItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Packed
{
  internal class PackedUIntItemDefinition : PackedItemDefinition<uint>
  {
    #region Ctors

    internal PackedUIntItemDefinition(int bitOffset, int bitCount)
      : base(bitOffset, bitCount)
    {
    }

    #endregion

    #region  Methods

    protected override uint Convert(ulong value)
    {
      return (uint) value;
    }

    protected override ulong Convert(uint value)
    {
      return value;
    }

    #endregion
  }
}