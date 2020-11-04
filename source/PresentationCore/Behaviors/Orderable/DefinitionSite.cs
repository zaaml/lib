// <copyright file="DefinitionSite.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
#if !SILVERLIGHT
  internal class DefinitionSite : BaseSite
  {
    #region Fields

    private readonly DefinitionBase definition;
    public List<FrameworkElement> Children;

    #endregion

    #region Ctors
    
    public DefinitionSite(Grid grid, DefinitionBase definition)
      : base(GetOffset(definition), GetSize(definition), GetOrientation(definition))
    {
      this.definition = definition;

      OrderableBehavior.SetSite(definition, this);
      DragHeader = OrderableBehavior.GetDragHeader(definition);


      var isColumn = definition is ColumnDefinition;
      var defIndex = isColumn
                       ? grid.ColumnDefinitions.IndexOf((ColumnDefinition) definition)
                       : grid.RowDefinitions.IndexOf((RowDefinition) definition);

      var getDef = isColumn ? (Func<UIElement, int>) Grid.GetColumn : Grid.GetRow;

      Children = grid.Children.OfType<FrameworkElement>().Where(c => getDef(c) == defIndex).ToList();

      if (DragHeader == null)
        return;

      OrderableBehavior.SetSite(DragHeader, this);
     

      foreach (var child in Children)
      {
        AddSiteRenderTransform(child, SiteRenderTransform);

        if (child == DragHeader)
          continue;

        BindingOperations.SetBinding(child, UIElement.OpacityProperty, new Binding("Opacity") {Source = DragHeader});
        BindingOperations.SetBinding(child, Panel.ZIndexProperty, new Binding("(Panel.ZIndex)") {Source = DragHeader});
      }

      RenderOffset = Offset;
    }

    #endregion

    #region Properties

    public DefinitionBase Definition
    {
      get { return definition; }
    }

    public override double DefinitionOffset
    {
      get { return GetOffset(definition); }
    }

    #endregion

    #region Methods

    public override void Dispose()
    {
      definition.ClearValue(OrderableBehavior.SiteProperty);

      if (DragHeader == null) return;

      DragHeader.ClearValue(OrderableBehavior.SiteProperty);
      LoadProperties(DragHeader);

      foreach (var child in Children)
      {
        RemoveSiteRenderTransform(child, SiteRenderTransform);

        if (child == DragHeader) continue;

        BindingOperations.ClearBinding(child, UIElement.OpacityProperty);
        BindingOperations.ClearBinding(child, Panel.ZIndexProperty);
      }
    }

    private static double GetOffset(DefinitionBase definition)
    {
      if (definition == null)
        return 0;

      return definition is ColumnDefinition ? ((ColumnDefinition) definition).Offset
               : ((RowDefinition) definition).Offset;
    }

    private static Orientation GetOrientation(DefinitionBase definition)
    {
      return definition is ColumnDefinition ? Orientation.Horizontal : Orientation.Vertical;
    }

    private static double GetSize(DefinitionBase definition)
    {
      return definition is ColumnDefinition
               ? ((ColumnDefinition) definition).ActualWidth
               : ((RowDefinition) definition).ActualHeight;
    }

    #endregion
  }
#endif

}