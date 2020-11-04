// <copyright file="IconBase.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  public abstract partial class IconBase : Panel
  {
    #region  Methods

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var icon = IconElement;

      if ((Children.Count == 1 && ReferenceEquals(Children[0], icon)) == false)
      {
        Children.Clear();
        Children.Add(icon);
      }

      return base.MeasureOverrideCore(availableSize);
    }

    #endregion
  }
}
