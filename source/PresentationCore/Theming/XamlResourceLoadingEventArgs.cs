// <copyright file="XamlResourceLoadingEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
  internal class XamlResourceLoadingEventArgs : EventArgs
  {
    #region Fields

    public readonly List<XamlResourceInfo> NewXamlResources;

    #endregion

    #region Ctors

    public XamlResourceLoadingEventArgs(List<XamlResourceInfo> newXamlResources)
    {
      NewXamlResources = newXamlResources;
    }

    #endregion
  }
}