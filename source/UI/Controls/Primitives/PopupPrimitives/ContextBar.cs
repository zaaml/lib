// <copyright file="ContextBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  public partial class ContextBar : PopupBarBase, IContextPopupControlInternal
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey TargetPropertyKey = DPM.RegisterReadOnly<DependencyObject, ContextBar>
      ("Target");

    public static readonly DependencyProperty TargetProperty = TargetPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private bool _isShared;

    #endregion

    #region Ctors

    static ContextBar()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<ContextBar>();
    }

    public ContextBar()
    {
      Owners = new SharedItemOwnerCollection(this);
      this.OverrideStyleKey<ContextBar>();
    }

    #endregion

    #region Properties

    private bool IsShared
    {
      get => _isShared;
      set
      {
        if (_isShared == value)
          return;

        _isShared = value;

        OnIsSharedChanged();
      }
    }

    private SharedItemOwnerCollection Owners { get; }

    public DependencyObject Target
    {
      get => (DependencyObject) GetValue(TargetProperty);
      private set => this.SetReadOnlyValue(TargetPropertyKey, value);
    }

    internal virtual bool OwnerAttachSelector => true;

		#endregion

		#region  Methods

		private void OnIsSharedChanged()
    {
      PlatformOnIsSharedChanged();
    }

    partial void PlatformOnIsSharedChanged();

    #endregion

    #region Interface Implementations

    #region IContextPopupControlInternal

    FrameworkElement IContextPopupControlInternal.Owner
    {
      get => Owner;
      set => Owner = value;
    }

    DependencyObject IContextPopupControlInternal.Target
    {
      get => Target;
      set => Target = value;
    }

    bool IContextPopupControlInternal.OwnerAttachSelector => OwnerAttachSelector;

		#endregion

		#region ISharedItem

		bool ISharedItem.IsShared
    {
      get => IsShared;
      set => IsShared = value;
    }

    SharedItemOwnerCollection ISharedItem.Owners => Owners;

    #endregion

    #endregion
  }
}