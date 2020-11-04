// <copyright file="PresentationTreeUtils.Visual.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Utils
{
  internal static partial class PresentationTreeUtils
  {
    #region  Methods

    internal static bool IsVisual(DependencyObject dependencyObject)
    {
#if SILVERLIGHT
      return dependencyObject is UIElement;
#else
      return dependencyObject is Visual || dependencyObject is Visual3D;
#endif
    }

    public static DependencyObject GetVisualParent(DependencyObject dependencyObject)
    {
      return IsVisual(dependencyObject) ? VisualTreeHelper.GetParent(dependencyObject) : null;
    }

    public static IEnumerable<DependencyObject> GetVisualAncestors(DependencyObject depObj, bool self)
    {
      return GetAncestors(depObj, self, VisualTreeEnumerationStrategy.Instance);
    }

    public static HwndSource GetPopupHwndSource(Popup popup)
    {
	    foreach (var source in PresentationSource.CurrentSources.OfType<HwndSource>())
	    {
		    if (source.RootVisual is FrameworkElement popupRoot && ReferenceEquals(popupRoot.GetParent(MixedTreeEnumerationStrategy.VisualThenLogicalInstance), popup))
			    return source;
	    }

	    return default;
    }

#if !SILVERLIGHT
    internal static IEnumerable<Popup> EnumeratePopups()
    {
      return PresentationSource.CurrentSources.OfType<HwndSource>()
        .Select(h => h.RootVisual)
        .OfType<FrameworkElement>()
        .Select(f => f.Parent)
        .OfType<Popup>();
    }
#endif

    internal static IEnumerable<DependencyObject> EnumerateVisualRoots()
    {
      return EnumerateVisualRootsImpl().SkipNull();
    }

    private static IEnumerable<DependencyObject> EnumerateVisualRootsImpl()
    {
#if SILVERLIGHT
      yield return Application.Current.RootVisual;

      foreach (var popupChild in VisualTreeHelper.GetOpenPopups().Select(p => p.Child).SkipNull())
        yield return popupChild;
#else
      return PresentationSource.CurrentSources.OfType<HwndSource>().Select(s => s.RootVisual);
#endif
    }

    public static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject parent, bool self)
    {
      if (parent == null)
        yield break;

      if (self)
        yield return parent;
			
      if (parent is Popup popup)
        yield return popup.Child;
      else
      {
        var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (var iChild = 0; iChild < childrenCount; iChild++)
          yield return VisualTreeHelper.GetChild(parent, iChild);
      }
    }

    public static IEnumerable<DependencyObject> GetVisualDescendants(DependencyObject parent, bool self, Func<DependencyObject, bool> excludeSubtree)
    {
      if (parent == null)
        yield break;

      if (self)
        yield return parent;

      var queue = new Queue<DependencyObject>();

      queue.Enqueue(parent);

      do
      {
        var current = queue.Dequeue();

        if (ReferenceEquals(current, parent) == false)
          yield return current;

        if (excludeSubtree != null && excludeSubtree(current))
          continue;

        foreach (var child in GetVisualChildren(current, false))
          queue.Enqueue(child);

      } while (queue.Count > 0);
    }

    #endregion

    #region  Nested Types

    private class VisualTreeEnumerator : ITreeEnumeratorAdvisor<DependencyObject>
    {
      #region Static Fields and Constants

      private static readonly IEnumerator<DependencyObject> Empty = Enumerable.Empty<DependencyObject>().GetEnumerator();

      public static readonly VisualTreeEnumerator Instance = new VisualTreeEnumerator(null);

      #endregion

      #region Fields

      private readonly Func<DependencyObject, bool> _excludeSubtree;

      #endregion

      #region Ctors

      public VisualTreeEnumerator(Func<DependencyObject, bool> excludeSubtree)
      {
        _excludeSubtree = excludeSubtree;
      }

      #endregion

      #region  Methods

      public IEnumerator<DependencyObject> GetChildren(DependencyObject node)
      {
        return _excludeSubtree != null && _excludeSubtree(node) ? Empty : GetVisualChildren(node, false).GetEnumerator();
      }

      #endregion
    }

    #endregion
  }
}