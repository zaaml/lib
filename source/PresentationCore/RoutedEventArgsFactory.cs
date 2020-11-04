// <copyright file="RoutedEventArgsFactory.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore
{
  internal static class RoutedEventArgsFactory
  {
    #region  Methods

    public static RoutedEventArgs Create(DependencyObject source)
    {
      return new RoutedEventArgs();
    }

    #endregion
  }
}