// <copyright file="ModifierKeysExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
	internal static class ModifierKeysExtensions
  {
    #region  Methods

    public static bool IsAltPressed(this ModifierKeys modifierKeys)
    {
      return (modifierKeys & ModifierKeys.Alt) != ModifierKeys.None;
    }

    public static bool IsControlPressed(this ModifierKeys modifierKeys)
    {
      return (modifierKeys & (ModifierKeys.Control | ModifierKeys.Windows)) != ModifierKeys.None;
    }

    public static bool IsShiftPressed(this ModifierKeys modifierKeys)
    {
      return (modifierKeys & ModifierKeys.Shift) != ModifierKeys.None;
    }

    #endregion
  }
}