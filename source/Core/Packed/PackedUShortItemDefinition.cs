// <copyright file="PackedUShortItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Packed
{
  internal class PackedUShortItemDefinition : PackedItemDefinition<ushort>
  {
    #region Ctors

    internal PackedUShortItemDefinition(int bitOffset)
      : base(bitOffset, 16)
    {
    }

    #endregion

    #region  Methods

    protected override ushort Convert(ulong value)
    {
      return (ushort) (value & 0xFFFF);
    }

    protected override ulong Convert(ushort value)
    {
      return ((ulong) value) & 0xFFFF;
    }

    #endregion
  }
}