// <copyright file="Panel.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zaaml.UI.Panels.Core
{
  public partial class Panel
  {
    #region Properties

    protected IEnumerable<UIElement> UIChildren => Children.Cast<UIElement>();

    #endregion

    #region  Methods

    partial void PlatformCtor()
    {
    }

    #endregion
  }
}