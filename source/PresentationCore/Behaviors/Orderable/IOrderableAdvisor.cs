// <copyright file="IOrderableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Orderable
{
  internal interface IOrderableAdvisor
  {
    #region  Methods

    void OnOrderEnd(FrameworkElement element);

    void OnOrderMove(FrameworkElement element, Vector delta);

    void OnOrderStart(FrameworkElement element);

    #endregion
  }
}
