// <copyright file="PresentationTreeUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Utils
{
  internal static partial class PresentationTreeUtils
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty DisconnectedParentProperty = DPM.RegisterAttached<DependencyObject>
      ("DisconnectedParent", typeof(PresentationTreeUtils));

		#endregion

		#region  Methods

	  internal static IEnumerable<DependencyObject> GetAncestors(DependencyObject depObj, bool self, TreeEnumerationStrategy strategy)
    {
      if (depObj == null)
        yield break;

      if (self)
        yield return depObj;

      for (var parent = strategy.GetAncestor(depObj);
        parent != null;
        parent = strategy.GetAncestor(parent))
        yield return parent;
    }

	  internal static DependencyObject GetDisconnectedParent(DependencyObject element)
    {
      return (DependencyObject)element.GetValue(DisconnectedParentProperty);
    }

	  internal static DependencyObject GetParent(DependencyObject dependencyObject, TreeEnumerationStrategy strategy)
    {
      return strategy.GetAncestor(dependencyObject);
    }

	  internal static bool IsWithinTree(DependencyObject root, DependencyObject node)
    {
      if (node == null)
        return false;

      if (ReferenceEquals(root, node))
        return true;

      foreach (var ancestor in GetAncestors(node, false, MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance))
      {
#if SILVERLIGHT
        var logicalAncestor = GetLogicalParent(ancestor);

        if (ReferenceEquals(logicalAncestor, root))
        {
          return true;
        }
#endif

        if (ReferenceEquals(root, ancestor))
        {
					return true;
				}
      }

      return false;
    }

	  internal static UIElement GetUIElementEventSource(object eventSource)
    {
      var uie = eventSource as UIElement;
#if SILVERLIGHT
		  var fce = eventSource as TextElement;
#else
      var fce = eventSource as FrameworkContentElement;
#endif

      if (uie == null && fce != null)
        uie = GetLogicalAncestors(fce, false).OfType<UIElement>().FirstOrDefault();

      return uie;
    }

	  internal static void SetDisconnectedParent(DependencyObject element, DependencyObject value)
    {
      element.SetValue(DisconnectedParentProperty, value);
    }

    #endregion
  }


  internal interface IPresentationTreeVisitor
  {
    #region  Methods

    bool Visit(DependencyObject dependencyObject);

    #endregion
  }
}