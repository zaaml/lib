// <copyright file="MouseButtons.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Input
{
  internal struct MouseButtons
  {
    private byte _packedValue;

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedEnumItemDefinition<MouseButtonStateKind> LeftButton;
      public static readonly PackedEnumItemDefinition<MouseButtonStateKind> RightButton;
      public static readonly PackedEnumItemDefinition<MouseButtonStateKind> MiddleButton;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        LeftButton = allocator.AllocateEnumItem<MouseButtonStateKind>();
        RightButton = allocator.AllocateEnumItem<MouseButtonStateKind>();
        MiddleButton = allocator.AllocateEnumItem<MouseButtonStateKind>();
      }

      #endregion
    }

    public static MouseButtons CreateButtonsState(MouseButtonStateKind left, MouseButtonStateKind right, MouseButtonStateKind middle)
    {
      return new MouseButtons
      {
        LeftButton = left,
        RightButton = right,
        MiddleButton = middle
      };
    }

    public MouseButtonStateKind LeftButton
    {
      get => PackedDefinition.LeftButton.GetValue(_packedValue);
      private set => PackedDefinition.LeftButton.SetValue(ref _packedValue, value);
    }

    public MouseButtonStateKind RightButton
    {
      get => PackedDefinition.RightButton.GetValue(_packedValue);
      private set => PackedDefinition.RightButton.SetValue(ref _packedValue, value);
    }

    public MouseButtonStateKind MiddleButton
    {
      get => PackedDefinition.MiddleButton.GetValue(_packedValue);
      private set => PackedDefinition.MiddleButton.SetValue(ref _packedValue, value);
    }
  }
}