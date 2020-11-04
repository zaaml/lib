// <copyright file="IReadOnlyDependencyObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IReadOnlyDependencyObject
  {
    #region  Methods

    object GetValue(DependencyProperty property);

    #endregion
  }
}