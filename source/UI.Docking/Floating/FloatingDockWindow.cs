// <copyright file="FloatingDockWindow.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Windows;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Controls.Docking
{
  public class FloatingDockWindow : WindowBase
  {
    #region Static Fields and Constants

    private static readonly DependencyPropertyKey DockItemPropertyKey = DPM.RegisterReadOnly<DockItem, FloatingDockWindow>
      ("DockItem", w => w.OnDockItemChanged);

    public static readonly DependencyProperty DockItemProperty = DockItemPropertyKey.DependencyProperty;

    #endregion

    #region Fields

    private readonly ContentPresenter _presenter;
    private readonly ContentPresenter _previewPresenter;
    private DockItemHeaderPresenter _dockItemHeaderPresenter;
    private DockItem _previewDockItem;
    private bool _suspendLocationSizeHandler;
    internal event EventHandler DockItemChanged;

    #endregion

    #region Ctors

    static FloatingDockWindow()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<FloatingDockWindow>();
    }

    internal FloatingDockWindow(FloatingDockWindowController controller)
    {
      this.OverrideStyleKey<FloatingDockWindow>();

      Controller = controller;

      ShowInTaskbar = false;
      ShowActivated = false;

      _presenter = new ContentPresenter();
      _previewPresenter = new ContentPresenter();

      var host = new Panel
      {
        Children =
        {
          _previewPresenter,
          _presenter
        }
      };

      Content = host;

      SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Properties

    internal FloatingDockWindowController Controller { get; }

    public DockItem DockItem
    {
      get => (DockItem) GetValue(DockItemProperty);
      private set => this.SetReadOnlyValue(DockItemPropertyKey, value);
    }

    public DockItemHeaderPresenter DockItemHeaderPresenter
    {
      get => _dockItemHeaderPresenter;
      set
      {
        if (ReferenceEquals(_dockItemHeaderPresenter, value))
          return;

        if (_dockItemHeaderPresenter != null)
          OnDockItemHeaderPresenterDetaching(_dockItemHeaderPresenter);

        _dockItemHeaderPresenter = value;

        if (_dockItemHeaderPresenter != null)
          OnDockItemHeaderPresenterAttached(_dockItemHeaderPresenter);
      }
    }

    internal Point DragOutLocation { get; set; }

    internal DockItem PreviewDockItem
    {
      get => _previewDockItem;
      private set
      {
        if (ReferenceEquals(_previewDockItem, value))
          return;

        _previewDockItem = value;
        _previewPresenter.Content = value;
      }
    }

    private Rect Rect { get; set; }

    #endregion

    #region  Methods

    internal void AttachContent()
    {
      _presenter.Content = DockItem;
      _previewPresenter.Content = PreviewDockItem;
    }

    internal void AttachItem(DockItem dockItem)
    {
      if (dockItem.IsPreview)
      {
        if (dockItem.PreviewFloatingWindow != null || PreviewDockItem != null)
          throw new InvalidOperationException();

        PreviewDockItem = dockItem;

        dockItem.PreviewFloatingWindow = this;
      }
      else
      {
        if (dockItem.FloatingWindow != null || DockItem != null)
          throw new InvalidOperationException();

        DockItem = dockItem;

        dockItem.FloatingWindow = this;

        UpdateLocationAndSize(dockItem);

        _presenter.Content = DockItem;
      }
    }

    internal void DetachContent()
    {
      _presenter.Content = null;
      _previewPresenter.Content = null;
    }

    internal void DetachItem(DockItem dockItem)
    {
      if (dockItem.IsPreview)
      {
        if (ReferenceEquals(dockItem.PreviewFloatingWindow, this) == false || ReferenceEquals(PreviewDockItem, dockItem) == false)
          throw new InvalidOperationException();

        PreviewDockItem = null;
        dockItem.PreviewFloatingWindow = null;
      }
      else
      {
        if (ReferenceEquals(dockItem.FloatingWindow, this) == false || ReferenceEquals(DockItem, dockItem) == false)
          throw new InvalidOperationException();

        DockItem = null;
        dockItem.FloatingWindow = null;

        _presenter.Content = null;
      }
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);

      DockItem?.Select();
    }

    internal override void OnBeginDragMove()
    {
      SyncPosition();

      DockItem.OnBeginDragMoveInternal();
    }

    protected override void OnContentRendered(EventArgs e)
    {
      base.OnContentRendered(e);

      if (DockItem?.EnqueueSyncDragPosition != true)
        return;

      Activate();
      SyncPosition();
      BeginDragMove(true);
    }

    private void OnDockItemChanged()
    {
      DockItemChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnDockItemHeaderPresenterAttached(DockItemHeaderPresenter dockItemHeaderPresenter)
    {
      dockItemHeaderPresenter.FloatingWindow = this;
    }

    private void OnDockItemHeaderPresenterDetaching(DockItemHeaderPresenter dockItemHeaderPresenter)
    {
      dockItemHeaderPresenter.FloatingWindow = null;
    }

    internal override void OnDragMove()
    {
      DockItem.OnDragMoveInternal();
    }

    internal override void OnEndDragMove()
    {
      DockItem.OnEndDragMoveInternal();
    }

    internal override void OnHeaderPresenterAttachedInternal(WindowHeaderPresenter headerPresenter)
    {
      base.OnHeaderPresenterAttachedInternal(headerPresenter);

      headerPresenter.TemplateContractAttached += OnHeaderPresenterTemplateContractAttached;
      headerPresenter.TemplateContractDetaching += OnHeaderPresenterTemplateContractDetaching;

      DockItemHeaderPresenter = (DockItemHeaderPresenter) HeaderPresenter.FindName("DockItemHeaderPresenter");
    }

    internal override void OnHeaderPresenterDetachingInternal(WindowHeaderPresenter headerPresenter)
    {
      headerPresenter.TemplateContractAttached -= OnHeaderPresenterTemplateContractAttached;
      headerPresenter.TemplateContractDetaching -= OnHeaderPresenterTemplateContractDetaching;

      DockItemHeaderPresenter = null;

      base.OnHeaderPresenterDetachingInternal(headerPresenter);
    }

    private void OnHeaderPresenterTemplateContractAttached(object sender, EventArgs e)
    {
      DockItemHeaderPresenter = (DockItemHeaderPresenter) HeaderPresenter.GetTemplateChildInternal("DockItemHeaderPresenter");
    }

    private void OnHeaderPresenterTemplateContractDetaching(object sender, EventArgs e)
    {
      DockItemHeaderPresenter = null;
    }

    protected override void OnLocationChanged(EventArgs e)
    {
      base.OnLocationChanged(e);

      var dockItem = DockItem;

      if (_suspendLocationSizeHandler || dockItem == null || dockItem.EnqueueSyncDragPosition)
        return;

      _suspendLocationSizeHandler = true;

      FloatLayout.SetFloatLeft(dockItem, Left);
      FloatLayout.SetFloatTop(dockItem, Top);

      _suspendLocationSizeHandler = false;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
    {
      var dockItem = DockItem;

      if (_suspendLocationSizeHandler || dockItem == null)
        return;

      _suspendLocationSizeHandler = true;

      FloatLayout.SetFloatWidth(dockItem, Width);
      FloatLayout.SetFloatHeight(dockItem, Height);

      _suspendLocationSizeHandler = false;
    }

    private void SyncPosition()
    {
      var mousePosition = MouseInternal.ScreenPosition;
      var rect = new Rect(new Point(Left, Top), new Size(Width, Height));

      if (DockItem != null)
      {
        if (DockItem.EnqueueSyncDragPosition)
        {
          DockItem.EnqueueSyncDragPosition = false;

          rect.Size = Rect.Size;
        }
        else
          rect.Size = FloatLayout.GetFloatRect(DockItem).Size;

        var dragOrigin = DockItem.HeaderMousePosition;

        if (dragOrigin.HasValue)
        {
          rect.X = mousePosition.X - dragOrigin.Value.X;
          rect.Y = mousePosition.Y - dragOrigin.Value.Y;
        }
      }
      else
      {
        rect.X = mousePosition.X;
        rect.Y = mousePosition.Y;
      }

      UpdateLocationAndSize(rect);
    }

    private void UpdateLocationAndSize(Rect rect)
    {
      if (Left.IsCloseTo(rect.Left) == false)
        Left = rect.Left;

      if (Top.IsCloseTo(rect.Top) == false)
        Top = rect.Top;

      if (Width.IsCloseTo(rect.Width) == false)
        Width = rect.Width;

      if (Height.IsCloseTo(rect.Height) == false)
        Height = rect.Height;
    }

    internal void UpdateLocationAndSize(DockItem dockItem)
    {
      if (ReferenceEquals(DockItem, dockItem) == false)
        return;

      if (dockItem.IsItemLayoutValid == false)
        return;

      if (dockItem.Controller?.IsLayoutSuspended == true)
        return;

      if (_suspendLocationSizeHandler)
        return;

      _suspendLocationSizeHandler = true;

      Rect = FloatLayout.GetFloatRect(dockItem);

      UpdateLocationAndSize(Rect);

      _suspendLocationSizeHandler = false;
    }

    #endregion
  }
}