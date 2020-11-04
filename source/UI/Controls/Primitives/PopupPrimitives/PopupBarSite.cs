// <copyright file="PopupBarSite.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using NativeContentControl = System.Windows.Controls.ContentControl;
using NativeContentPresenter = System.Windows.Controls.ContentPresenter;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
  [ContentProperty("Bar")]
  public class PopupBarSite : FixedTemplateControl
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty BarProperty = DPM.Register<PopupBar, PopupBarSite>
      ("Bar", s => s.OnBarChanged);

    public static readonly DependencyProperty HostPopupContentProperty = DPM.Register<bool, PopupBarSite>
      ("HostPopupContent", s => s.OnHostPopupContentChanged);

    #endregion

    #region Fields

    private readonly NativeContentPresenter _hostContentPresenter;
    private Popup _popup;

    #endregion

    #region Ctors

    public PopupBarSite()
      : base(true)
    {
      _hostContentPresenter = new NativeContentPresenter();
    }

    #endregion

    #region Properties

    public PopupBar Bar
    {
      get => (PopupBar) GetValue(BarProperty);
      set => SetValue(BarProperty, value);
    }

    public bool HostPopupContent
    {
      get => (bool) GetValue(HostPopupContentProperty);
      set => SetValue(HostPopupContentProperty, value);
    }

    private Popup Popup
    {
      set
      {
        if (ReferenceEquals(_popup, value))
          return;

        _popup = value;
      }
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverride(Size finalSize)
    {
      base.ArrangeOverride(XamlConstants.ZeroSize);
      return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
      base.MeasureOverride(HostPopupContent ? Screen.FromElement(this).Bounds.Size() : XamlConstants.ZeroSize);
      return XamlConstants.ZeroSize;
    }

    private void OnBarChanged(PopupBar oldBar, PopupBar newBar)
    {
      if (oldBar != null)
      {
        oldBar.IsOpenChanged -= OnBarIsOpenChanged;
        oldBar.TemplateContractAttached -= OnBarTemplateContractChange;
        oldBar.TemplateContractDetaching -= OnBarTemplateContractChange;
      }

      Popup = Bar?.PopupController?.Popup;

      if (newBar != null)
      {
        newBar.IsOpenChanged += OnBarIsOpenChanged;
        newBar.TemplateContractAttached += OnBarTemplateContractChange;
        newBar.TemplateContractDetaching += OnBarTemplateContractChange;
      }

      UpdateChild();
    }

    private void OnBarIsOpenChanged(object sender, EventArgs eventArgs)
    {
      UpdateChild();
    }

    private void OnBarTemplateContractChange(object sender, EventArgs eventArgs)
    {
      UpdateChild();
    }

    private void OnHostPopupContentChanged()
    {
      UpdateChild();
    }

    private void UpdateChild()
    {
      if (Bar == null)
      {
        ChildInternal = null;
        return;
      }

      if (Bar.IsOpen)
      {
        _hostContentPresenter.ClearValue(NativeContentPresenter.ContentProperty);
        Bar.MountContent();
      }
      else
      {
        Bar.ReleaseContent();
        _hostContentPresenter.SetBinding(NativeContentPresenter.ContentProperty, new Binding
        {
          Path = new PropertyPath(NativeContentControl.ContentProperty),
          Source = Bar
        });
      }
      ChildInternal = Bar.IsOpen ? (UIElement) _popup : _hostContentPresenter;
    }

    #endregion
  }
}