// <copyright file="IPanelLayoutListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interfaces
{
  internal interface IPanelLayoutListener
  {
    #region  Methods

    void Arranged(Size finalSize, Size arrangeSize);

    void Arranging(Size finalSize);

    void Measured(Size availableSize, Size measureSize);

    void Measuring(Size availableSize);

    #endregion
  }
}