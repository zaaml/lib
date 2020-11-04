// <copyright file="LogicalTreeEnumerationStrategy.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Utils
{
  public sealed class LogicalTreeEnumerationStrategy : TreeEnumerationStrategy
  {
    #region Static Fields and Constants

    private static readonly Lazy<LogicalTreeEnumerationStrategy> LazyInstance = new Lazy<LogicalTreeEnumerationStrategy>(() => new LogicalTreeEnumerationStrategy());

    #endregion

    #region Ctors

    private LogicalTreeEnumerationStrategy()
    {
    }

    #endregion

    #region Properties

    public static TreeEnumerationStrategy Instance => LazyInstance.Value;

    #endregion

    #region  Methods

    public override DependencyObject GetAncestor(DependencyObject dependencyObject)
    {
      return PresentationTreeUtils.GetLogicalParent(dependencyObject);
    }

    #endregion
  }
}