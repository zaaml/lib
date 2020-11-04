// <copyright file="GridDefinitionOrderAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Monads;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
#if !SILVERLIGHT
  internal class GridDefinitionOrderAdvisor : OrderAdvisor
  {
    #region Methods

    public override List<ElementSite> CaptureOrderedSequence(UIElement source)
    {
      var grid = GetParentGrid(source);

      List<ElementSite> elementSites = grid.ColumnDefinitions.Any(c => ReferenceEquals(source, OrderableBehavior.GetDragHeader(c))) ? GetColumnSequence(grid) : GetRowSequence(grid);

      ((DefinitionSite)OrderableBehavior.GetSite(source)).IsAnimationEnabled = false;

      return elementSites;
    }

    public override UIElement GetParentPanel(UIElement source)
    {
      return GetParentGrid(source);
    }

    public override void ReleaseOrderedSequence(UIElement source, List<ElementSite> elements)
    {
      var grid = GetParentGrid(source);

      Action<UIElement, int> setDef;
      Action<DefinitionSite> addDef;

      if (grid.ColumnDefinitions.Any(c => ReferenceEquals(source, OrderableBehavior.GetDragHeader(c))))
      {
        grid.ColumnDefinitions.Clear();
        setDef = Grid.SetColumn;
        addDef = d => grid.ColumnDefinitions.Add((ColumnDefinition)d.Definition);
      }
      else
      {
        grid.RowDefinitions.Clear();
        setDef = Grid.SetRow;
        addDef = d => grid.RowDefinitions.Add((RowDefinition)d.Definition);
      }

      for (var i = 0; i < elements.Count; i++)
      {
        var site = (DefinitionSite)elements[i];

        if (site.Children != null)
          foreach (var child in site.Children)
            setDef(child, i);

        addDef(site);
      }

      var elementSite = (DefinitionSite)OrderableBehavior.GetSite(source);

      elementSite.IsAnimationEnabled = true;
      elementSite.Offset = OrderableBehavior.CalcElementOffset(elements, elementSite);

      grid.UpdateLayout();
      foreach (var site in elements.DirectCast<DefinitionSite>())
      {
        site.UpdateRenderOffset();
        site.QueryDispose();
      }
    }

    public override void OnDettached(DependencyObject depObj)
    {
    }

    public override void OnAttached(DependencyObject depObj)
    {
    }

    private List<ElementSite> GetColumnSequence(Grid grid)
    {
      return grid.ColumnDefinitions.Select(c => GetCurrentSite(c) ?? new DefinitionSite(grid, c)).ToList();
    }

    private ElementSite GetCurrentSite(DefinitionBase definition)
    {
      var site = OrderableBehavior.GetSite(definition) as DefinitionSite;

      if (site == null)
        return null;

      site.CancelDisposing();

      if (ReferenceEquals(site.DragHeader, DraggableBehavior.DraggedElement))
      {
        site.Offset = site.RenderOffset;
        site.IsAnimationEnabled = false;
      }

      return site;
    }

    private Grid GetParentGrid(UIElement element)
    {
      return VisualTreeHelper.GetParent(element) as Grid;
    }

    private List<ElementSite> GetRowSequence(Grid grid)
    {
      return grid.RowDefinitions.Select(r => GetCurrentSite(r) ?? new DefinitionSite(grid, r)).ToList();
    }

    #endregion
  }
#endif

}