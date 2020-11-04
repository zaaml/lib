// <copyright file="FloatingDockWindowController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.PresentationCore.Extensions;
using Zaaml.UI.Windows;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class FloatingDockWindowController
  {
    #region Fields

    private readonly HashSet<FloatingDockWindow> _hideQueue = new HashSet<FloatingDockWindow>();
    private readonly FloatLayoutView _layoutView;
    private readonly HashSet<FloatingDockWindow> _showQueue = new HashSet<FloatingDockWindow>();

    #endregion

    #region Ctors

    public FloatingDockWindowController(FloatLayoutView layoutView)
    {
      _layoutView = layoutView;
    }

    #endregion

    #region  Methods

    public void ArrangeWindows()
    {
      foreach (var floatingDockWindow in _showQueue)
        floatingDockWindow.Show();

      foreach (var floatingDockWindow in _hideQueue)
        floatingDockWindow.Close();

      _showQueue.Clear();
      _hideQueue.Clear();
    }

    public void AttachWindow(DockItem item)
    {
      if (item.IsPreview)
        GetPreviewTarget(item)?.AttachItem(item);
      else
        GetFloatingDockWindow(item).AttachItem(item);
    }

    private FloatingDockWindow CreateFloatingDockWindow()
    {
      return new FloatingDockWindow(this)
      {
        Owner = WindowBase.GetWindowInternal(_layoutView),
      };
    }

    public void DetachWindow(DockItem item)
    {
      if (item.IsPreview)
        item.PreviewFloatingWindow?.DetachItem(item);
      else
      {
        var floatingDockWindow = item.FloatingWindow;

        if (floatingDockWindow == null)
          return;

        floatingDockWindow.DetachItem(item);

        if (floatingDockWindow.PreviewDockItem != null)
          floatingDockWindow.DetachItem(floatingDockWindow.PreviewDockItem);

        ReleaseFloatingDockWindow(floatingDockWindow);
      }
    }

    private FloatingDockWindow GetFloatingDockWindow(DockItem item)
    {
      if (_hideQueue.Any() == false)
      {
        var dockWindow = CreateFloatingDockWindow();

        _showQueue.Add(dockWindow);

        return dockWindow;
      }

      var targetRect = FloatLayout.GetFloatRect(item);

      FloatingDockWindow floatingWindow = null;

      foreach (var current in _hideQueue)
      {
        floatingWindow = current;

        if (current.GetLayoutBox().IsCloseTo(targetRect))
          break;
      }

      _hideQueue.Remove(floatingWindow);

      return floatingWindow;
    }

    private FloatingDockWindow GetPreviewTarget(DockItem item)
    {
      var dropTargetWindow = item.DockControl?.CurrentDropGuide?.Compass.PlacementTarget as DockItem;

      return dropTargetWindow?.DockState != DockItemState.Float ? null : dropTargetWindow.Root.FloatingWindow;
    }

    private void ReleaseFloatingDockWindow(FloatingDockWindow window)
    {
      _hideQueue.Add(window);
    }

    public void UpdateWindow(DockItem item)
    {
      if (item.IsPreview == false)
        return;

      var target = GetPreviewTarget(item);
      var current = item.PreviewFloatingWindow;

      if (ReferenceEquals(current, target))
        return;

      current?.DetachItem(item);
      target?.AttachItem(item);
    }

    #endregion
  }
}