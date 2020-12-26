// <copyright file="RibbonDropDownButton.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.UI.Controls.Ribbon
{
  public class RibbonDropDownButton : RibbonDropDownButtonBase
  {
    #region Ctors

    static RibbonDropDownButton()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<RibbonDropDownButton>();
    }

    public RibbonDropDownButton()
    {
      this.OverrideStyleKey<RibbonDropDownButton>();
    }

    #endregion

    #region  Methods

    protected override void OnClick()
    {
      base.OnClick();

			if (IsDropDownOpen)
				CloseDropDown();
			else
				OpenDropDown();
		}

    #endregion
  }
}