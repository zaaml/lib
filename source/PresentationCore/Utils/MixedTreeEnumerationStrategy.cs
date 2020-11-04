// <copyright file="MixedTreeEnumerationStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
  public sealed class MixedTreeEnumerationStrategy : TreeEnumerationStrategy
  {
    #region Static Fields and Constants

    private static readonly Lazy<MixedTreeEnumerationStrategy> VisualThenLogicalLazyInstance = new Lazy<MixedTreeEnumerationStrategy>(() => new MixedTreeEnumerationStrategy(VisualTreeEnumerationStrategy.Instance, LogicalTreeEnumerationStrategy.Instance, false));

    private static readonly Lazy<MixedTreeEnumerationStrategy> LogicalThenVisualLazyInstance = new Lazy<MixedTreeEnumerationStrategy>(() => new MixedTreeEnumerationStrategy(LogicalTreeEnumerationStrategy.Instance, VisualTreeEnumerationStrategy.Instance, false));

    private static readonly Lazy<MixedTreeEnumerationStrategy> DisconnectedThenVisualThenLogicalLazyInstance = new Lazy<MixedTreeEnumerationStrategy>(() => new MixedTreeEnumerationStrategy(VisualTreeEnumerationStrategy.Instance, LogicalTreeEnumerationStrategy.Instance, true));

    private static readonly Lazy<MixedTreeEnumerationStrategy> DisconnectedThenLogicalThenVisualLazyInstance = new Lazy<MixedTreeEnumerationStrategy>(() => new MixedTreeEnumerationStrategy(LogicalTreeEnumerationStrategy.Instance, VisualTreeEnumerationStrategy.Instance, true));

    #endregion

    #region Fields

    private readonly TreeEnumerationStrategy _firstStrategy;
    private readonly TreeEnumerationStrategy _secondStrategy;
    private readonly bool _disconnectedTree;

    #endregion

    #region Ctors

    private MixedTreeEnumerationStrategy(TreeEnumerationStrategy firstStrategy, TreeEnumerationStrategy secondStrategy, bool disconnectedTree)
    {
      _firstStrategy = firstStrategy;
      _secondStrategy = secondStrategy;
      _disconnectedTree = disconnectedTree;
    }

    #endregion

    #region Properties

    public static TreeEnumerationStrategy LogicalThenVisualInstance => LogicalThenVisualLazyInstance.Value;

    public static TreeEnumerationStrategy VisualThenLogicalInstance => VisualThenLogicalLazyInstance.Value;

    internal static TreeEnumerationStrategy DisconnectedThenLogicalThenVisualInstance => DisconnectedThenLogicalThenVisualLazyInstance.Value;

    internal static TreeEnumerationStrategy DisconnectedThenVisualThenLogicalInstance => DisconnectedThenVisualThenLogicalLazyInstance.Value;

    #endregion

    #region  Methods

    public override DependencyObject GetAncestor(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        return null;

      if (_disconnectedTree)
      {
        var disconnectedParent = PresentationTreeUtils.GetDisconnectedParent(dependencyObject);
        if (disconnectedParent != null)
          return disconnectedParent;
      }

      return _firstStrategy.GetAncestor(dependencyObject) ?? _secondStrategy.GetAncestor(dependencyObject);
    }

    #endregion
  }
}