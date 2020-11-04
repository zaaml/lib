// <copyright file="HiddenLayout.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;

namespace Zaaml.UI.Controls.Docking
{
  internal sealed class HiddenLayout : BaseLayout
  {
    #region Ctors

    static HiddenLayout()
    {
      RegisterLayoutProperties<HiddenLayout>(Enumerable.Empty<DependencyProperty>());
    }

    #endregion

    #region Properties

    public override LayoutKind LayoutKind => LayoutKind.Hidden;

    #endregion
  }
}