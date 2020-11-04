// <copyright file="DropCompass.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

#if SILVERLIGHT
using RoutedEventArgs = System.Windows.RoutedEventArgsSL;
using RoutedEventHandler = System.Windows.RoutedEventHandlerSL;
using RoutedEvent = System.Windows.RoutedEventSL;
#else

#endif

namespace Zaaml.UI.Controls.Docking
{
  public abstract class DropCompass : Control
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty AllowedActionsProperty = DPM.Register<DropGuideAction, DropCompass>
      ("AllowedActions", c => c.AllowDropGuides);

    public static readonly DependencyProperty PlacementTargetProperty = DPM.Register<FrameworkElement, DropCompass>
      ("PlacementTarget", c => c.OnPlacementTargetChangedInt);

    public static readonly DependencyProperty IsDocumentBandEnabledProperty = DPM.Register<bool, DropCompass>
      ("IsDocumentBandEnabled", true);

    public static readonly RoutedEvent PlacementTargetChangedEvent = EventManager.RegisterRoutedEvent
      ("PlacementTargetChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DropCompass));

    private static readonly List<string> DropGuideNames = new List<string>
    {
      "AutoHideLeftGuide",
      "DockLeftGuide",
      "AutoHideTopGuide",
      "DockTopGuide",
      "AutoHideRightGuide",
      "DockRightGuide",
      "AutoHideBottomGuide",
      "DockBottomGuide",
      "SplitLeftGuide",
      "SplitDocumentLeftGuide",
      "SplitTopGuide",
      "SplitDocumentTopGuide",
      "SplitRightGuide",
      "SplitDocumentRightGuide",
      "SplitBottomGuide",
      "SplitDocumentBottomGuide",
      "TabCenterGuide",
      "TabLeftGuide",
      "TabTopGuide",
      "TabRightGuide",
      "TabBottomGuide"
    };

    #endregion

    #region Fields

    private List<DropGuide> _dropGuides;


    public event RoutedEventHandler PlacementTargetChanged
    {
      add => this.AddHandlerInt(PlacementTargetChangedEvent, value, false);
      remove => this.RemoveHandlerInt(PlacementTargetChangedEvent, value);
    }

    #endregion

    #region Ctors

    static DropCompass()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DropCompass>();
    }

    protected DropCompass()
    {
      this.OverrideStyleKey<DropCompass>();
    }

    #endregion

    #region Properties

    public DropGuideAction AllowedActions
    {
      get => (DropGuideAction) GetValue(AllowedActionsProperty);
      set => SetValue(AllowedActionsProperty, value);
    }

    public IEnumerable<DropGuide> DropGuides => _dropGuides.AsEnumerable();

    public bool IsDocumentBandEnabled
    {
      get => (bool) GetValue(IsDocumentBandEnabledProperty);
      set => SetValue(IsDocumentBandEnabledProperty, value);
    }

    public FrameworkElement PlacementTarget
    {
      get => (FrameworkElement) GetValue(PlacementTargetProperty);
      set => SetValue(PlacementTargetProperty, value);
    }

    #endregion

    #region  Methods

    private void AllowDropGuides()
    {
      if (_dropGuides == null)
        return;

      foreach (var dropGuide in _dropGuides)
        dropGuide.IsAllowed = (AllowedActions & dropGuide.Action) != 0;
    }

    private void AttachEvent(FrameworkElement newTarget)
    {
      newTarget.LayoutUpdated += OnPlacementTargetLayoutUpdated;
    }

    private void DetachEvent(FrameworkElement oldTarget)
    {
      oldTarget.LayoutUpdated -= OnPlacementTargetLayoutUpdated;
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      if (_dropGuides != null)
        foreach (var dropGuide in _dropGuides)
          dropGuide.Compass = null;

      _dropGuides = DropGuideNames.Select(GetTemplateChild).OfType<DropGuide>().SkipNull().ToList();

      foreach (var dropGuide in _dropGuides)
        dropGuide.Compass = this;

      AllowDropGuides();
    }

    protected virtual void OnPlacementTargetChanged()
    {
      this.RaiseEventInt(new RoutedEventArgs(PlacementTargetChangedEvent, this));

      IsDocumentBandEnabled = PlacementTarget.Return(p => p.GetVisualAncestors().Any(t => t is DocumentLayout));

      UpdatePlacement();
    }

    private void OnPlacementTargetChangedInt(FrameworkElement oldTarget, FrameworkElement newTarget)
    {
      if (oldTarget != null)
        DetachEvent(oldTarget);

      if (newTarget != null)
        AttachEvent(newTarget);

      OnPlacementTargetChanged();
    }

    private void OnPlacementTargetLayoutUpdated(object sender, EventArgs e)
    {
      UpdatePlacement();
    }

    public void UpdatePlacement()
    {
    }

    #endregion
  }
}