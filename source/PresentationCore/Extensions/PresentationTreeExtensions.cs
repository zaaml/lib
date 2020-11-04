// <copyright file="PresentationTreeExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  public static class PresentationTreeExtensions
  {
    #region  Methods

    public static T FindVisualAncestor<T>(this DependencyObject child)
      where T : DependencyObject
    {
      return GetVisualAncestors(child).OfType<T>().FirstOrDefault();
    }

    public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject depObj, TreeEnumerationStrategy strategy)
    {
      return PresentationTreeUtils.GetAncestors(depObj, false, strategy);
    }

		public static IEnumerable<T> GetAncestors<T>(this DependencyObject depObj, TreeEnumerationStrategy strategy) where T : DependencyObject
    {
      return PresentationTreeUtils.GetAncestors(depObj, false, strategy).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetAncestorsAndSelf(this DependencyObject depObj, TreeEnumerationStrategy strategy)
    {
      return PresentationTreeUtils.GetAncestors(depObj, true, strategy);
    }

    public static IEnumerable<T> GetAncestorsAndSelf<T>(this DependencyObject depObj, TreeEnumerationStrategy strategy) where T : DependencyObject
    {
      return PresentationTreeUtils.GetAncestors(depObj, true, strategy).OfType<T>();
    }


    public static IEnumerable<T> GetLogicalAncestors<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetLogicalAncestors(depObj, false).OfType<T>();
    }


    public static IEnumerable<DependencyObject> GetLogicalAncestors(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetLogicalAncestors(depObj, false);
    }

    public static IEnumerable<T> GetLogicalAncestorsAndSelf<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetLogicalAncestors(depObj, true).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetLogicalAncestorsAndSelf(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetLogicalAncestors(depObj, true);
    }

    public static DependencyObject GetLogicalParent(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetLogicalParent(depObj);
    }

    public static T GetLogicalParent<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetLogicalParent(depObj) as T;
    }

    public static DependencyObject GetRoot(this DependencyObject depObj, TreeEnumerationStrategy strategy)
    {
      return depObj.GetAncestorsAndSelf(strategy).Last();
    }

    public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetVisualAncestors(depObj, false);
    }

    public static IEnumerable<T> GetVisualAncestors<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualAncestors(depObj, false).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualAncestorsAndSelf(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetVisualAncestors(depObj, true);
    }

    public static IEnumerable<T> GetVisualAncestorsAndSelf<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualAncestors(depObj, true).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject parent)
    {
      return PresentationTreeUtils.GetVisualChildren(parent, false);
    }

    public static IEnumerable<T> GetVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualChildren(parent, false).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualChildrenAndSelf(this DependencyObject parent)
    {
      return PresentationTreeUtils.GetVisualChildren(parent, true);
    }

    public static IEnumerable<T> GetVisualChildrenAndSelf<T>(this DependencyObject parent) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualChildren(parent, true).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject parent)
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, false, null);
    }

    public static IEnumerable<T> GetVisualDescendants<T>(this DependencyObject parent) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, false, null).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject parent, Func<DependencyObject, bool> excludeSubtree)
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, false, excludeSubtree);
    }

    public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject parent)
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, true, null);
    }

    public static IEnumerable<T> GetVisualDescendantsAndSelf<T>(this DependencyObject parent) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, true, null).OfType<T>();
    }

    public static IEnumerable<DependencyObject> GetVisualDescendantsAndSelf(this DependencyObject parent, Func<DependencyObject, bool> excludeSubtree)
    {
      return PresentationTreeUtils.GetVisualDescendants(parent, true, excludeSubtree);
    }

    public static DependencyObject GetVisualParent(this DependencyObject depObj)
    {
      return PresentationTreeUtils.GetVisualParent(depObj);
    }

    public static DependencyObject GetParent(this DependencyObject depObj, TreeEnumerationStrategy strategy)
    {
      return PresentationTreeUtils.GetParent(depObj, strategy);
    }

    public static T GetParent<T>(this DependencyObject depObj, TreeEnumerationStrategy strategy) where T : DependencyObject
    {
      return PresentationTreeUtils.GetParent(depObj, strategy) as T;
    }

    public static T GetVisualParent<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return PresentationTreeUtils.GetVisualParent(depObj) as T;
    }

    public static DependencyObject GetVisualRoot(this DependencyObject depObj)
    {
      return depObj.GetVisualAncestorsAndSelf().Last();
    }

    public static T GetVisualRoot<T>(this DependencyObject depObj) where T : DependencyObject
    {
      return depObj.GetVisualAncestorsAndSelf().Last() as T;
    }

    public static bool IsAncestorOf(this DependencyObject depObj, DependencyObject descendant, TreeEnumerationStrategy strategy)
    {
      return GetAncestors(descendant, strategy).Any(a => ReferenceEquals(a, depObj));
    }

    public static bool IsDescendantOf(this DependencyObject depObj, DependencyObject ancestor, TreeEnumerationStrategy strategy)
    {
      return GetAncestors(depObj, strategy).Any(a => ReferenceEquals(a, ancestor));
    }

    public static bool IsLogicalAncestorOf(this DependencyObject depObj, DependencyObject descendant)
    {
      return GetLogicalAncestors(descendant).Any(a => ReferenceEquals(a, depObj));
    }

    public static bool IsLogicalDescendantOf(this DependencyObject depObj, DependencyObject ancestor)
    {
      return GetLogicalAncestors(depObj).Any(a => ReferenceEquals(a, ancestor));
    }

    public static bool IsSelfOrAncestorOf(this DependencyObject depObj, DependencyObject descendant, TreeEnumerationStrategy strategy)
    {
      return ReferenceEquals(depObj, descendant) || IsAncestorOf(depObj, descendant, strategy);
    }

    public static bool IsSelfOrDescendantOf(this DependencyObject depObj, DependencyObject ancestor, TreeEnumerationStrategy strategy)
    {
      return ReferenceEquals(depObj, ancestor) || IsDescendantOf(depObj, ancestor, strategy);
    }

    public static bool IsSelfOrLogicalAncestorOf(this DependencyObject depObj, DependencyObject descendant)
    {
      return ReferenceEquals(depObj, descendant) || IsLogicalAncestorOf(depObj, descendant);
    }

    public static bool IsSelfOrLogicalDescendantOf(this DependencyObject depObj, DependencyObject ancestor)
    {
      return ReferenceEquals(depObj, ancestor) || IsLogicalDescendantOf(depObj, ancestor);
    }

    public static bool IsSelfOrVisualAncestorOf(this DependencyObject depObj, DependencyObject descendant)
    {
      return ReferenceEquals(depObj, descendant) || IsVisualAncestorOf(depObj, descendant);
    }

    public static bool IsSelfOrVisualDescendantOf(this DependencyObject depObj, DependencyObject ancestor)
    {
      return ReferenceEquals(depObj, ancestor) || IsVisualDescendantOf(depObj, ancestor);
    }

    public static bool IsVisualAncestorOf(this DependencyObject depObj, DependencyObject descendant)
    {
      return GetVisualAncestors(descendant).Any(a => ReferenceEquals(a, depObj));
    }

    public static bool IsVisualDescendantOf(this DependencyObject depObj, DependencyObject ancestor)
    {
      return GetVisualAncestors(depObj).Any(a => ReferenceEquals(a, ancestor));
    }

    #endregion
  }
}