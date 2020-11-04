// <copyright file="PackedByteItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Packed
{
  internal class PackedByteItemDefinition : PackedItemDefinition<byte>
  {
    #region Ctors

    internal PackedByteItemDefinition(int bitOffset)
      : base(bitOffset, 8)
    {
    }

    #endregion

    #region  Methods

    protected override byte Convert(ulong value)
    {
      return (byte) (value & 0xFF);
    }

    protected override ulong Convert(byte value)
    {
      return (ulong) value & 0xFF;
    }

    #endregion
  }
}