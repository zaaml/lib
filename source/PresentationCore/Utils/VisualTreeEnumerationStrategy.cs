// <copyright file="VisualTreeEnumerationStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
  public sealed class VisualTreeEnumerationStrategy : TreeEnumerationStrategy
  {
    #region Static Fields and Constants

    private static readonly Lazy<TreeEnumerationStrategy> LazyInstance = new Lazy<TreeEnumerationStrategy>(() => new VisualTreeEnumerationStrategy());

    #endregion

    #region Ctors

    private VisualTreeEnumerationStrategy()
    {
    }

    #endregion

    #region Properties

    public static TreeEnumerationStrategy Instance => LazyInstance.Value;

    #endregion

    #region  Methods

    public override DependencyObject GetAncestor(DependencyObject dependencyObject)
    {
      return PresentationTreeUtils.GetVisualParent(dependencyObject);
    }

    #endregion
  }
}