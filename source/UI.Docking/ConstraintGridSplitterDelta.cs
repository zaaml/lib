// <copyright file="ConstraintGridSplitterDelta.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Docking
{
  internal class ConstraintGridSplitterDelta
  {
    #region Fields

    private readonly Size _constraint;
#if !SILVERLIGHT
    private readonly Dictionary<DefinitionBase, double> _definitionToLength = new Dictionary<DefinitionBase, double>();
#endif
    private Grid _grid;

    #endregion

    #region Ctors

    public ConstraintGridSplitterDelta(Size constraint)
    {
      _constraint = constraint;
    }

    #endregion

    #region Properties

    public Grid Grid
    {
      get => _grid;
      set => _grid = value;
    }

    #endregion

    #region  Methods

    public void AddSplitter(GridSplitter splitter)
    {
#if !SILVERLIGHT
      splitter.DragCompleted += OnSplitterDragCompleted;
      splitter.DragStarted += OnSplitterDragStarted;
#endif
    }

    public void RemoveSplitter(GridSplitter splitter)
    {
#if !SILVERLIGHT
      splitter.DragCompleted -= OnSplitterDragCompleted;
      splitter.DragStarted -= OnSplitterDragStarted;
#endif
    }

    #endregion

#if !SILVERLIGHT
    private double GetActualLength(DefinitionBase definition)
    {
      return definition is ColumnDefinition ? ((ColumnDefinition) definition).ActualWidth : ((RowDefinition) definition).ActualHeight;
    }

    private double GetCurrentConstraint(DefinitionBase definition)
    {
      return definition is ColumnDefinition ? ((ColumnDefinition) definition).MinWidth : ((RowDefinition) definition).MinHeight;
    }

    private void SetCurrentConstraint(DefinitionBase definition, double value)
    {
      definition.SetCurrentValueInternal(definition is ColumnDefinition ? ColumnDefinition.MinWidthProperty : RowDefinition.MinHeightProperty, Math.Min(value, GetActualLength(definition)));
    }
#endif

#if !SILVERLIGHT
    private void OnSplitterDragCompleted(object sender, DragCompletedEventArgs e)
    {
      if (_grid == null)
        return;

      foreach (var columnDefinition in _grid.ColumnDefinitions.Where(c => c.Width.IsStar))
        SetCurrentConstraint(columnDefinition, _definitionToLength[columnDefinition]);

      foreach (var rowDefinition in _grid.RowDefinitions.Where(c => c.Height.IsStar))
        SetCurrentConstraint(rowDefinition, _definitionToLength[rowDefinition]);

      _definitionToLength.Clear();
    }

    private void OnSplitterDragStarted(object sender, DragStartedEventArgs e)
    {
      if (_grid == null)
        return;

      foreach (var columnDefinition in _grid.ColumnDefinitions.Where(c => c.Width.IsStar))
      {
        _definitionToLength[columnDefinition] = GetCurrentConstraint(columnDefinition);
        SetCurrentConstraint(columnDefinition, _constraint.Width);
      }

      foreach (var rowDefinition in _grid.RowDefinitions.Where(c => c.Height.IsStar))
      {
        _definitionToLength[rowDefinition] = GetCurrentConstraint(rowDefinition);
        SetCurrentConstraint(rowDefinition, _constraint.Height);
      }
    }
#endif
  }
}