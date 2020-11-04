// <copyright file="LayoutTransformer.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Primitives
{
  public sealed class LayoutTransformer : ContentControl
  {
    #region Ctors

    static LayoutTransformer()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<LayoutTransformer>();
    }

    public LayoutTransformer()
    {
      this.OverrideStyleKey<LayoutTransformer>();

      IsTabStop = false;
#if !SILVERLIGHT
      Focusable = false;
#endif
    }

    #endregion
  }
}
