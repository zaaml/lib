// <copyright file="IDependencyObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IDependencyObject : IReadOnlyDependencyObject
  {
    #region  Methods

    void ClearValue(DependencyProperty property);

    void SetValue(DependencyProperty property, object value);

    #endregion
  }
}