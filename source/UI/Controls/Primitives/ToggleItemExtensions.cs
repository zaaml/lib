// <copyright file="ToggleItemExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Primitives
{
	internal static class ToggleItemExtensions
	{
		#region  Methods

		public static void OnToggle<T>(this T item) where T : Control, IToggleButton
		{
		  object isChecked;

			if (item.IsChecked == true)
			  isChecked = item.IsThreeState ? null : BooleanBoxes.False;
			else
			  isChecked = item.IsChecked.HasValue ? BooleanBoxes.True : BooleanBoxes.False;

      item.SetCurrentValueInternal(item.IsCheckedPropertyInt, isChecked);
    }

		#endregion
	}
}