// <copyright file="DockTabViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.TabView;

namespace Zaaml.UI.Controls.Docking
{
  public class DockTabViewControl : TabViewControl
  {
    #region Ctors

    static DockTabViewControl()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DockTabViewControl>();
    }

    private protected override bool DefaultAllowNullSelection => true;

    private protected override bool DefaultPreferSelection => true;

		public DockTabViewControl()
    {
      this.OverrideStyleKey<DockTabViewControl>();
    }

    #endregion
  }

  public class DocumentTabViewControl : DockTabViewControl
  {
  }
}