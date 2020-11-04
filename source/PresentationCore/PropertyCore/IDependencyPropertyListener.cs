// <copyright file="IDependencyPropertyListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore
{
  internal interface IDependencyPropertyListener
  {
    #region  Methods

    void OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue);

    #endregion
  }
}