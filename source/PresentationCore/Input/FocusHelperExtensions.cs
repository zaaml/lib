// <copyright file="FocusHelperExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;

namespace Zaaml.PresentationCore.Input
{
  internal static class FocusHelperExtensions
  {
    #region  Methods

    public static bool HasFocus(this Control control)
    {
      return FocusHelper.HasFocus(control);
    }

	  public static bool HasKeyboardFocus(this Control control)
	  {
		  return FocusHelper.HasKeyboardFocus(control);
	  }

		#endregion
	}
}