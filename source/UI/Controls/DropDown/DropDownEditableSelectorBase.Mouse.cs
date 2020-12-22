// <copyright file="DropDownEditableSelectorBase.Mouse.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract partial class DropDownEditableSelectorBase<TItemsControl, TItem>
	{
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (e.Handled)
				return;

			BeginEdit();

			e.Handled = true;
		}

		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnPreviewMouseLeftButtonDown(e);

			var editor = EditorCore;

			if (e.OriginalSource is DependencyObject dependencyObject && editor != null && dependencyObject.IsSelfOrVisualDescendantOf(editor))
				EffectiveKeyboard = MainKeyboard;
		}
	}
}