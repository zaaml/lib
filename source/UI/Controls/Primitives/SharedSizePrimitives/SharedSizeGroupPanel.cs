// <copyright file="SharedSizeGroupPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Primitives.SharedSizePrimitives
{
  public sealed class SharedSizeGroupPanel : Panel
  {
    #region Fields

    private FrameworkElement _child;
    private SharedSizeGroupControl _sharedSizeGroupControl;

    #endregion

    #region Properties

    private FrameworkElement Child
    {
      set
      {
        if (ReferenceEquals(_child, value))
          return;

        if (_child != null)
        {
          if (value == null)
            Children.Clear();
        }

        _child = value;

        if (_child != null)
        {
          if (Children.Count == 0)
            Children.Add(_child);
          else
            Children[0] = _child;
        }

        InvalidateMeasure();
      }
    }

    internal bool IsInMeasure { get; private set; }

    internal SharedSizeGroupControl SharedSizeGroupControl
    {
      get => _sharedSizeGroupControl;
      set
      {
        if (ReferenceEquals(_sharedSizeGroupControl, value))
          return;

        _sharedSizeGroupControl = value;

        UpdateChild();
        InvalidateMeasure();
      }
    }

    private Dictionary<string, SharedSizeEntry> SharedSizes => SharedSizeGroupControl?.SharedSizes;

    #endregion

    #region  Methods

    protected override Size ArrangeOverride(Size finalSize)
    {
      _child?.Arrange(finalSize.Rect());
      return finalSize;
    }

    private Size MeasureChild(Size availableSize)
    {
      if (_child == null)
        return XamlConstants.ZeroSize;

      _child.Measure(availableSize);

      return _child.DesiredSize;
    }

    internal void InvalidateSharedSizePanel(SharedSizeContentPanel panel)
    {
#if SILVERLIGHT
      panel.InvalidateMeasure();
#else
      for (FrameworkElement current = panel; current != null; current = current.GetVisualParent<FrameworkElement>())
      {
        if (ReferenceEquals(current, _child))
          break;

        current.InvalidateMeasure();
      }
#endif
    }

#if NEVER
		private List<UIElement> FindInvalid()
		{
			return this.GetVisualDescendants(f => IsVisible(f) == false).OfType<UIElement>().Where(u => IsVisible(u) && u.IsMeasureValid == false).ToList();
		}

		private static bool IsVisible(DependencyObject depObj)
		{
			var uie = depObj as UIElement;
			if (uie == null)
				return true;

			return uie.IsVisible;
		}
#endif

    internal void InvalidateInternal()
    {
      _child?.InvalidateMeasure();
      InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size constraint)
    {
      IsInMeasure = true;

      var generation = SharedSizes?.Values.Select(s => s.Generation).Max() ?? 0;

      if (SharedSizes != null)
        foreach (var sharedSize in SharedSizes.Values)
          sharedSize.BeginMeasurePass(generation);

      var measureResult = new Size();

      // Limit measure pass count to 8
      for (var iMeasurePass = 0; iMeasurePass < 8; iMeasurePass++)
      {
        _child.InvalidateMeasure();
        measureResult = MeasureChild(constraint);

        if (SharedSizes == null)
          break;

        var prevGeneration = generation;

        foreach (var sharedSize in SharedSizes.Values)
        {
          if (sharedSize.Generation != prevGeneration)
            foreach (var panel in sharedSize.EnumerateSharedPanels())
              InvalidateSharedSizePanel(panel);

          generation = Math.Max(sharedSize.Generation, generation);
        }

        if (generation == prevGeneration)
          break;

        foreach (var sharedSize in SharedSizes.Values)
          sharedSize.NextMeasurePass(generation);
      }

      if (SharedSizes != null)
        foreach (var sharedSize in SharedSizes.Values)
          sharedSize.EndMeasurePass();

      IsInMeasure = false;

      return measureResult;
    }

    internal void OnChildChanged()
    {
      UpdateChild();
    }

    private void UpdateChild()
    {
      Child = _sharedSizeGroupControl?.Child;
    }

    #endregion
  }
}