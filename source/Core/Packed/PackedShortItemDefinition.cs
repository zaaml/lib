// <copyright file="PackedShortItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Packed
{
  internal class PackedShortItemDefinition : PackedItemDefinition<short>
  {
    #region Ctors

    internal PackedShortItemDefinition(int bitOffset)
      : base(bitOffset, 16)
    {
    }

    #endregion

    #region  Methods

    protected override short Convert(ulong value)
    {
      return (short) (value & 0xFFFF);
    }

    protected override ulong Convert(short value)
    {
      return ((ulong) value) & 0xFFFF;
    }

    #endregion
  }
}