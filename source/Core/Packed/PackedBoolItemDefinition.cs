// <copyright file="PackedBoolItemDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Packed
{
  internal class PackedBoolItemDefinition : PackedItemDefinition<bool>
  {
    #region Ctors

    internal PackedBoolItemDefinition(int bitOffset)
      : base(bitOffset, 1)
    {
    }

    #endregion

    #region  Methods

    protected override bool Convert(ulong value)
    {
      return value == 1;
    }

    protected override ulong Convert(bool value)
    {
      return value ? 1ul : 0;
    }

    #endregion
  }
}