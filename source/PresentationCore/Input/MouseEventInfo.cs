// <copyright file="MouseEventInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Packed;

namespace Zaaml.PresentationCore.Input
{
  internal struct MouseEventInfo
  {
    private uint _packedValue;

    public Point ScreenPosition { get; private set; }

    public MouseEventKind EventKind
    {
      get => PackedDefinition.EventKind.GetValue(_packedValue);
      private set => PackedDefinition.EventKind.SetValue(ref _packedValue, value);
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

    public MouseButtonKind ChangedButton
    {
      get => PackedDefinition.ChangedButton.GetValue(_packedValue);
      private set => PackedDefinition.ChangedButton.SetValue(ref _packedValue, value);
    }

    public MouseEventAreaKind AreaKind
    {
      get => PackedDefinition.AreaKind.GetValue(_packedValue);
      private set => PackedDefinition.AreaKind.SetValue(ref _packedValue, value);
    }

    public IntPtr Handle { get; private set; }

    public static MouseEventInfo CreateMouseMoveInfo(IntPtr handle, Point screenPosition, MouseButtons buttons, MouseEventAreaKind areaKind)
    {
      return new MouseEventInfo
      {
        Handle = handle,
        EventKind = MouseEventKind.Move,
        ScreenPosition = screenPosition,
        AreaKind = areaKind
      }.WithButtons(buttons);
    }

    public static MouseEventInfo CreateMouseEnterInfo(IntPtr handle, Point screenPosition, MouseButtons buttons, MouseEventAreaKind areaKind)
    {
      return new MouseEventInfo
      {
        Handle = handle,
        EventKind = MouseEventKind.Enter,
        ScreenPosition = screenPosition,
        AreaKind = areaKind
      }.WithButtons(buttons);
    }

    public static MouseEventInfo CreateMouseLeaveInfo(IntPtr handle, Point screenPosition, MouseButtons buttons, MouseEventAreaKind areaKind)
    {
      return new MouseEventInfo
      {
        Handle = handle,
        EventKind = MouseEventKind.Leave,
        ScreenPosition = screenPosition,
        AreaKind = areaKind
      }.WithButtons(buttons);
    }

    private MouseEventInfo WithButtons(MouseButtons buttons)
    {
      LeftButton = buttons.LeftButton;
      RightButton = buttons.RightButton;

      return this;
    }

    public static MouseEventInfo CreateMouseButtonInfo(IntPtr handle, Point screenPosition, MouseButtons buttons, MouseButtonKind changedButton, MouseEventAreaKind areaKind)
    {
      return new MouseEventInfo
      {
        Handle = handle,
        EventKind = MouseEventKind.Button,
        ScreenPosition = screenPosition,
        ChangedButton = changedButton,
        AreaKind = areaKind
      }.WithButtons(buttons);
    }

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedEnumItemDefinition<MouseEventKind> EventKind;
      public static readonly PackedEnumItemDefinition<MouseButtonStateKind> LeftButton;
      public static readonly PackedEnumItemDefinition<MouseButtonStateKind> RightButton;
      public static readonly PackedEnumItemDefinition<MouseButtonKind> ChangedButton;
      public static readonly PackedEnumItemDefinition<MouseEventAreaKind> AreaKind;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        EventKind = allocator.AllocateEnumItem<MouseEventKind>();
        LeftButton = allocator.AllocateEnumItem<MouseButtonStateKind>();
        RightButton = allocator.AllocateEnumItem<MouseButtonStateKind>();
        ChangedButton = allocator.AllocateEnumItem<MouseButtonKind>();
        AreaKind = allocator.AllocateEnumItem<MouseEventAreaKind>();
      }

      #endregion
    }
  }
}