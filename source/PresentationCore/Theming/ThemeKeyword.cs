// <copyright file="ThemeKeyword.cs" author="Dmitry Kravchenin" email="d.kravchenin@xmetropol.com">
//   Copyright (c) xmetropol. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Theming
{
  public enum ThemeKeyword
  {
    Undefined,

    #region Application

    ApplicationBackgroundColor,
    ApplicationBackgroundBrush,

    ApplicationForegroundColor,
    ApplicationForegroundBrush,

    ApplicationBorderBrush,
    ApplicationBorderColor,

    ApplicationFontFamily,
    ApplicationFontSize,
    ApplicationFontWeight,
    ApplicationFontStyle,
    ApplicationFontStretch,

    #endregion

    #region Control

    ControlBackgroundColor,
    ControlBackgroundBrush,

    ControlForegroundColor,
    ControlForegroundBrush,

    ControlBorderBrush,
    ControlBorderColor,

    ControlFontFamily,
    ControlFontSize,
    ControlFontWeight,
    ControlFontStyle,
    ControlFontStretch,

		#endregion

		#region Selection

		// Active (Focused)
		SelectionBorderColor,
		SelectionBorderBrush,
		SelectionBackgroundColor,
    SelectionBackgroundBrush,
    SelectionForegroundColor,
    SelectionForegroundBrush,


		// Inactive (Unfocused)
		InactiveSelectionBorderColor,
		InactiveSelectionBorderBrush,
		InactiveSelectionBackgroundColor,
    InactiveSelectionBackgroundBrush,
    InactiveSelectionForegroundColor,
    InactiveSelectionForegroundBrush,

    #endregion

    #region Bar

    BarBackgroundColor,
    BarBackgroundBrush,
    BarForegroundColor,
    BarForegroundBrush,
    BarBorderBrush,
    BarBorderColor,

    #endregion

    #region BarItem

    BarItemBackgroundColor,
    BarItemBackgroundBrush,
    BarItemForegroundColor,
    BarItemForegroundBrush,
    BarItemBorderBrush,
    BarItemBorderColor,

    #endregion

    #region Menu

    MenuBackgroundColor,
    MenuBackgroundBrush,
    MenuForegroundColor,
    MenuForegroundBrush,
    MenuBorderBrush,
    MenuBorderColor,

    #endregion

    #region MenuItem

    MenuItemBackgroundColor,
    MenuItemBackgroundBrush,
    MenuItemForegroundColor,
    MenuItemForegroundBrush,
    MenuItemBorderBrush,
    MenuItemBorderColor,

    #endregion

    MessageWindowErrorImage,
    MessageWindowWarningImage,
    MessageWindowQuestionImage
  }
}