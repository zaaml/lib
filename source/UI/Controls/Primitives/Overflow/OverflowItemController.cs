// <copyright file="OverflowItemController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core.Packed;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Controls.Primitives.Overflow
{
  public sealed class OverflowItemController<TItem> : IDisposable 
	  where TItem : System.Windows.Controls.Control, IOverflowableItem
  {
    #region Fields

    private uint _packedValue;

    #endregion

    #region Ctors

    public OverflowItemController(TItem item)
    {
      Item = item;

      VisibleHost = new OverflowItem<TItem>(this, OverflowItemKind.Visible);
      OverflowHost = new OverflowItem<TItem>(this, OverflowItemKind.Overflow);
    }

    #endregion

    #region Properties

    private bool IsAttached
    {
      get => PackedDefinition.IsAttached.GetValue(_packedValue);
      set
      {
        if (IsAttached == value)
          return;

        PackedDefinition.IsAttached.SetValue(ref _packedValue, value);
        IsDirty = true;

        UpdateHost();
      }
    }

    private bool IsDirty
    {
      get => PackedDefinition.IsDirty.GetValue(_packedValue);
      set
      {
        if (IsDirty == value)
          return;

        PackedDefinition.IsDirty.SetValue(ref _packedValue, value);
      }
    }

    public bool IsOverflow
    {
      get => PackedDefinition.IsOverflow.GetValue(_packedValue);
      set
      {
        if (IsOverflow == value)
          return;

        PackedDefinition.IsOverflow.SetValue(ref _packedValue, value);
        IsDirty = true;

        UpdateHost();
      }
    }

    public TItem Item { get; private set; }

    public OverflowItem<TItem> OverflowHost { get; }

    private byte SuspendCount
    {
      get => PackedDefinition.SuspendCount.GetValue(_packedValue);
      set => PackedDefinition.SuspendCount.SetValue(ref _packedValue, value);
    }

    public OverflowItem<TItem> VisibleHost { get; }

    #endregion

    #region  Methods

    internal void Attach()
    {
      IsAttached = true;
    }

    internal void Detach()
    {
      IsAttached = false;
    }

    public bool Resume()
    {
      if (SuspendCount > 0)
        SuspendCount--;

      if (SuspendCount > 0)
        return false;

      return UpdateHost();
    }

    public void Suspend()
    {
      SuspendCount++;
    }

    private bool UpdateHost()
    {
      if (IsAttached == false)
      {
        VisibleHost.Child = null;
        OverflowHost.Child = null;
        return false;
      }

      if (IsDirty == false || SuspendCount > 0)
        return false;

      if (IsOverflow)
      {
        VisibleHost.Child = null;
        OverflowHost.Child = Item;
      }
      else
      {
        OverflowHost.Child = null;
        VisibleHost.Child = Item;
      }

      IsDirty = false;

      return true;
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    void IDisposable.Dispose()
    {
      if (Item == null)
        return;

      VisibleHost.Child = null;
      OverflowHost.Child = null;

      Item = null;
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsAttached;
      public static readonly PackedBoolItemDefinition IsDirty;
      public static readonly PackedBoolItemDefinition IsOverflow;
      public static readonly PackedByteItemDefinition SuspendCount;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsAttached = allocator.AllocateBoolItem();
        IsDirty = allocator.AllocateBoolItem();
        IsOverflow = allocator.AllocateBoolItem();
        SuspendCount = allocator.AllocateByteItem();
      }

      #endregion
    }

    #endregion
  }
}