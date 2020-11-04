// <copyright file="StackPanelOrderAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal class FlexPanelOrderAdvisor : StackOrderableAdvisor
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty ElementSiteProperty = DPM.RegisterAttached<FrameworkElementSite, FlexPanelOrderAdvisor>
      ("ElementSite");

    #endregion

    #region  Methods

    protected override List<ElementSite> CaptureOrderedSequence(FrameworkElement source)
    {
      var stackPanel = GetParentStackPanel(source);
      var offset = 0.0;
      var elementSites = new List<ElementSite>();
      var orientation = stackPanel.Orientation;
      var orderedChildren = stackPanel.Children.OfType<FrameworkElement>();

      foreach (var child in orderedChildren)
      {
        var ls = LayoutInformation.GetLayoutSlot(child);
        var size = orientation == Orientation.Horizontal ? ls.Width : ls.Height;
        var elementSite = new FrameworkElementSite(child, offset, size, orientation);

        SetElementSite(child, elementSite);

        elementSite.SetDefinitionOffset(offset);
        elementSites.Add(elementSite);
        offset += size;
      }

      return elementSites;
    }

    protected override ElementSite GetActualSite(DependencyObject dependencyObject)
    {
      foreach (var ancestor in dependencyObject.GetAncestorsAndSelf(VisualTreeEnumerationStrategy.Instance))
      {
        var site = GetElementSite(ancestor);

        if (site != null)
          return site;

        if (ReferenceEquals(this, OrderableBehavior.GetAdvisor(ancestor)))
          break;
      }

      return null;
    }

    private static FrameworkElementSite GetElementSite(DependencyObject element)
    {
      return (FrameworkElementSite) element.GetValue(ElementSiteProperty);
    }

    protected override Point GetMousePosition(FrameworkElement element)
    {
      return MouseInternal.GetPosition(GetParentStackPanel(element));
    }

    private static StackPanel GetParentStackPanel(FrameworkElement element)
    {
      return element.FindVisualAncestor<StackPanel>();
    }

    protected override void ReleaseOrderedSequence(FrameworkElement source, List<ElementSite> elements)
    {
      var sourceSite = (FrameworkElementSite) GetActualSite(source);
      var stackPanel = GetParentStackPanel(source);
      var orientation = stackPanel.Orientation;

      if (stackPanel.IsItemsHost)
      {

      }
      else
      {
        stackPanel.Children.Remove(sourceSite.Element);
        stackPanel.Children.Insert(elements.IndexOf(sourceSite), sourceSite.Element);
      }

      stackPanel.UpdateLayout();

      var offset = 0.0;

      foreach (var elementSite in elements.Cast<FrameworkElementSite>())
      {
        var child = elementSite.Element;
        var ls = LayoutInformation.GetLayoutSlot(child);
        var size = orientation == Orientation.Horizontal ? ls.Width : ls.Height;

        elementSite.SetDefinitionOffset(offset);

        offset += size;
      }
    }

    private static void SetElementSite(DependencyObject element, FrameworkElementSite value)
    {
      element.SetValue(ElementSiteProperty, value);
    }

    #endregion
  }
}