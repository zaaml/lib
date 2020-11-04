// <copyright file="PresentationTreeUtils.Logical.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

#if !SILVERLIGHT
using System;
using System.Linq;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
#endif

namespace Zaaml.PresentationCore.Utils
{
  internal static partial class PresentationTreeUtils
  {
#region  Methods

    public static IEnumerable<DependencyObject> GetLogicalAncestors(DependencyObject depObj, bool self)
    {
      return GetAncestors(depObj, self, LogicalTreeEnumerationStrategy.Instance);
    }

    public static DependencyObject GetLogicalParent(DependencyObject dependencyObject)
    {
#if SILVERLIGHT
      DependencyObject logicalParent = null;
      var logicalTreeNode = dependencyObject as ILogicalTreeNode;

      if (logicalTreeNode != null)
        logicalParent = logicalTreeNode.LogicalParent as DependencyObject;

      if (logicalParent != null)
        return logicalParent;

      var fre = dependencyObject as FrameworkElement;
      return fre?.Parent ?? VisualTreeHelper.GetParent(dependencyObject);
#else
      return LogicalTreeHelper.GetParent(dependencyObject);
#endif
    }

#if !SILVERLIGHT
    private static IEnumerable<object> GetLogicalDescendants(DependencyObject parent, bool self, Func<object, bool> excludeSubtree)
    {
      throw new NotImplementedException();
    }

    public static IEnumerable<object> GetLogicalChildren(DependencyObject dependencyObject)
    {
      return LogicalTreeHelper.GetChildren(dependencyObject).OfType<object>();
    }


    public static IEnumerable<object> GetLogicalDescendants(DependencyObject dependencyObject)
    {
      return LogicalTreeEnumerator.Instance.GetEnumerator(dependencyObject).Enumerate();
    }
#endif

    #endregion

    #region  Nested Types

#if !SILVERLIGHT
    private class LogicalTreeEnumerator : ITreeEnumeratorAdvisor<object>
    {
    #region Static Fields and Constants

      private static readonly IEnumerator<object> Empty = Enumerable.Empty<object>().GetEnumerator();

      public static readonly LogicalTreeEnumerator Instance = new LogicalTreeEnumerator(null);

    #endregion

    #region Fields

      private readonly Func<object, bool> _excludeSubtree;

    #endregion

    #region Ctors

      public LogicalTreeEnumerator(Func<object, bool> excludeSubtree)
      {
        _excludeSubtree = excludeSubtree;
      }

    #endregion

    #region  Methods

      public IEnumerator<object> GetChildren(object node)
      {
        var dependencyObject = node as DependencyObject;
        return dependencyObject == null || _excludeSubtree != null && _excludeSubtree(node) ? Empty : LogicalTreeHelper.GetChildren(dependencyObject).OfType<object>().GetEnumerator();
      }

    #endregion
    }
#endif

    #endregion
  }
}