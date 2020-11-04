// <copyright file="SingleChildPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;

namespace Zaaml.UI.Panels.Core
{
  [ContentProperty(nameof(Child))]
  public class SingleChildPanel : Panel
  {
    #region Fields

    private UIElement _child;

    #endregion

    #region Properties

    public UIElement Child
    {
      get => _child;
      set
      {
        if (ReferenceEquals(_child, value))
          return;

        var prevChild = _child;

        if (_child != null)
          Children.Remove(_child);

        _child = value;

        if (_child != null)
          Children.Add(_child);

        OnChildChanged(prevChild, value);
      }
    }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      EnsureChild();
      var arrangeOverride = base.ArrangeOverrideCore(finalSize);
      return arrangeOverride;
    }

    private void EnsureChild()
    {
      if (_child == null)
      {
        if (Children.Count > 0)
          Children.Clear();
      }
      else
      {
        if (Children.Count == 0)
          Children.Add(_child);
        else if (Children.Count == 1 && ReferenceEquals(Children[0], _child))
        {
        }
        else
        {
          Children.Clear();
          Children.Add(_child);
        }
      }
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      EnsureChild();
      var measureOverride = base.MeasureOverrideCore(availableSize);
      return measureOverride;
    }

    protected virtual void OnChildChanged(UIElement oldChild, UIElement newChild)
    {
    }

    #endregion
  }

  //public sealed class MouseEventsBarrier : SingleChildPanel
  //{
  //  #region Static Fields and Constants

  //  private static readonly Brush TransparentBrush = Colors.Transparent.ToSolidColorBrush().AsFrozen();

  //  #endregion

  //  #region Ctors

  //  public MouseEventsBarrier()
  //  {
  //    Background = TransparentBrush;
  //  }

  //  #endregion

  //  #region Properties

  //  internal bool HandleMouseEvents => true;

  //  #endregion

  //  #region  Methods

  //  protected override void OnMouseEnter(MouseEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseLeave(MouseEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseMove(MouseEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  protected override void OnMouseWheel(MouseWheelEventArgs e)
  //  {
  //    if (HandleMouseEvents)
  //      e.Handled = true;
  //  }

  //  #endregion
  //}
}