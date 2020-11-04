// <copyright file="IFrameworkElement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IFrameworkElement : IDependencyObject
  {
    #region Properties

    double Height { get; }

    HorizontalAlignment HorizontalAlignment { get; }

    double MaxHeight { get; }

    double MaxWidth { get; }

    double MinHeight { get; }

    double MinWidth { get; }

    VerticalAlignment VerticalAlignment { get; }

    double Width { get; }

    #endregion

    #region  Methods

    void InvalidateArrange();

    void InvalidateMeasure();

    #endregion
  }
}