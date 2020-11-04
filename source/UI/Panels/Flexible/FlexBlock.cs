// <copyright file="FlexBlock.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Interfaces;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels.Flexible
{
  public sealed class FlexBlock : Panel, IFlexPanel, IParentLayoutListener
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty DefinitionProperty = DPM.RegisterAttached<FlexBlockDefinition, FlexBlock>
      ("Definition", OnDefinitionPropertyChanged);

    private static readonly FlexBlockDefinition DefaultDefinition = new FlexBlockDefinition();

    #endregion

    #region Fields

    private FlexPanelLayout _layout;
    private bool _observeLayout;

    #endregion

    #region Properties

    private FlexBlockDefinition ActualDefinition => ParentDefinition ?? DefaultDefinition;

    internal FlexPanelLayout Layout => _layout ?? (_layout = CreateLayout());

    public bool ObserveLayoutUpdate
    {
      get => _observeLayout;
      set
      {
        if (_observeLayout == value)
          return;

        _observeLayout = value;

        if (_observeLayout)
          LayoutUpdated += OnLayoutUpdated;
        else
          LayoutUpdated -= OnLayoutUpdated;

        Layout.OnLayoutUpdated();
      }
    }

    private FlexBlockDefinition ParentDefinition => this.GetVisualParent<FrameworkElement>()?.GetValue<FlexBlockDefinition>(DefinitionProperty);

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      return Layout.Arrange(finalSize);
    }

    private FlexPanelLayout CreateLayout()
    {
      return new FlexPanelLayout(this);
    }

    public static FlexBlockDefinition GetDefinition(DependencyObject element)
    {
      return (FlexBlockDefinition) element.GetValue(DefinitionProperty);
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      return Layout.Measure(availableSize);
    }

    private static void OnDefinitionPropertyChanged(DependencyObject arg1, FlexBlockDefinition oldDefinition, FlexBlockDefinition newDefinition)
    {
    }

    private void OnLayoutUpdated(object sender, EventArgs eventArgs)
    {
      if (IsLoaded == false)
        return;

      Layout.OnLayoutUpdated();
    }

    public static void SetDefinition(DependencyObject element, FlexBlockDefinition value)
    {
      element.SetValue(DefinitionProperty, value);
    }

    #endregion

    #region Interface Implementations

    #region IFlexPanel

    IFlexDistributor IFlexPanel.Distributor => ActualDefinition.Distributor;

    bool IFlexPanel.HasHiddenChildren { get; set; }

    double IFlexPanel.Spacing => ActualDefinition.Spacing;

    FlexStretch IFlexPanel.Stretch => ActualDefinition.Stretch;

    FlexElement IFlexPanel.GetFlexElement(UIElement child)
    {
      return GetFlexElement(child);
    }

    private FlexElement GetFlexElement(UIElement child)
    {
      return child.GetFlexElement(this, ActualDefinition.Definitions.ElementAtOrDefault(Children.IndexOf(child)));
    }

    bool IFlexPanel.GetIsHidden(UIElement child)
    {
      return FlexPanel.GetIsHidden(child);
    }

    void IFlexPanel.SetIsHidden(UIElement child, bool value)
    {
      FlexPanel.SetIsHidden(child, value);
    }

    #endregion

    #region IOrientedPanel

    Orientation IOrientedPanel.Orientation => (this.GetVisualParent() as IFlexPanel)?.Orientation.Rotate() ?? Orientation.Horizontal;

    #endregion

    #region IParentLayoutListener

    private int _measureLayoutVersion;
    private int _arrangeLayoutVersion;

    void IParentLayoutListener.EnterParentMeasurePass(ParentLayoutPass layoutPass)
    {
      if (_measureLayoutVersion == layoutPass.LayoutPassVersion)
        return;

      //var parentDefinition = ParentDefinition;
      //if (parentDefinition != null)
      //{
      //  for(var iChild = 0; iChild < Children.Count; iChild++)
      //  {
      //    var child = Children[iChild];

      //    parentDefinition.SharedFlexElements.EnsureCount(iChild);
      //    parentDefinition.SharedFlexElements[iChild] = GetFlexElement(child);
      //  }
      //}

      _measureLayoutVersion = layoutPass.LayoutPassVersion;
    }

    void IParentLayoutListener.LeaveParentMeasurePass(ref ParentLayoutPass layoutPass)
    {
      var parentDefinition = ParentDefinition;
      if (parentDefinition != null)
      {

      }
    }

    void IParentLayoutListener.EnterParentArrangePass(ParentLayoutPass layoutPass)
    {
      if (_arrangeLayoutVersion == layoutPass.LayoutPassVersion)
        return;

      _arrangeLayoutVersion = layoutPass.LayoutPassVersion;
    }

    void IParentLayoutListener.LeaveParentArrangePass(ref ParentLayoutPass layoutPass)
    {
    }

    #endregion

    #endregion
  }
}