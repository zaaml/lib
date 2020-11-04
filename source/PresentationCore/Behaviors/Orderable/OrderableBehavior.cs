// <copyright file="OrderableBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
#else
#endif

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal class OrderableBehavior : BehaviorBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty AdvisorProperty = DPM.RegisterAttached<IOrderableAdvisor, OrderableBehavior>
      ("Advisor");

    #endregion

    #region Fields

    private IOrderableAdvisor _advisor;
    private IDraggableMouseInteraction _draggableMouse;

    #endregion

    #region Properties

    public bool IsOrdering { get; private set; }

    #endregion

    #region  Methods

    private static IOrderableAdvisor FindAdvisor(DependencyObject dependencyObject)
    {
      return dependencyObject.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.VisualThenLogicalInstance).Select(GetAdvisor).FirstOrDefault(a => a != null);
    }

    public static IOrderableAdvisor GetAdvisor(DependencyObject depObj)
    {
      return (IOrderableAdvisor) depObj.GetValue(AdvisorProperty);
    }

    protected override void OnAttached()
    {
      base.OnAttached();

      _draggableMouse = DraggableMouseInteractionService.Create(FrameworkElement, 1000);
      _draggableMouse.Threshold = 0;

      _draggableMouse.DragStarted += OnDragStartedInternal;
      _draggableMouse.DragEnded += OnDragEndedPrivate;
      _draggableMouse.DragMove += OnDragMoveInternal;
    }

    protected override void OnDetaching()
    {
      _draggableMouse.DragStarted -= OnDragStartedInternal;
      _draggableMouse.DragEnded -= OnDragEndedPrivate;
      _draggableMouse.DragMove -= OnDragMoveInternal;

      _draggableMouse.Dispose();
      _draggableMouse = null;

      base.OnDetaching();
    }

    private void OnDragEndedPrivate(object sender, EventArgs e)
    {
      IsOrdering = false;

      if (_advisor == null)
        return;

      _advisor.OnOrderEnd(FrameworkElement);

      _advisor = null;
    }

    private void OnDragMoveInternal(object sender, EventArgs args)
    {
      _advisor?.OnOrderMove(FrameworkElement, PointUtils.SubtractPoints(_draggableMouse.ScreenPoint, _draggableMouse.ScreenOrigin));
    }

    private void OnDragStartedInternal(object sender, EventArgs e)
    {
      var element = FrameworkElement;

      _advisor = FindAdvisor(element);

      if (_advisor == null)
        return;

      _advisor.OnOrderStart(FrameworkElement);

      IsOrdering = true;
    }

    public static void SetAdvisor(DependencyObject depObj, IOrderableAdvisor value)
    {
      depObj.SetValue(AdvisorProperty, value);
    }

    #endregion
  }
}