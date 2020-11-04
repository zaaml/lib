// <copyright file="ILayoutInfoProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore
{
  internal interface ILayoutInfoProvider
  {
    #region Properties

    Rect HostRelativeBox { get; }

    #endregion
  }
}