// <copyright file="ThemeManagerStyle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>


namespace Zaaml.PresentationCore.Theming
{
  internal sealed class ThemeManagerStyle : ThemeStyleBase
  {
		#region  Methods

    internal void BindThemeManagerStyle(ThemeStyleBase themeStyle)
    {
      BasedOnCore = GetActualThemeStyle(themeStyle);
    }

    private ThemeStyleBase GetActualThemeStyle(ThemeStyleBase themeStyleBase)
    {
	    if (themeStyleBase is ThemeStyle themeStyle)
		    return themeStyle.IsDeferred ? null : themeStyle;

	    return themeStyleBase;
    }

    #endregion
  }
}