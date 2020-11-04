// <copyright file="TrackBarControl.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Primitives.TrackBar
{
  public partial class TrackBarControl
  {
    #region  Methods

    public override void BeginInit()
    {
      base.BeginInit();

      BeginInitImpl();
    }

    public override void EndInit()
    {
      EndInitImpl();

      base.EndInit();
    }

    #endregion
  }
}