// <copyright file="FocusPopupTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class FocusPopupTrigger : SourcePopupTrigger
	{
		protected override void AttachTarget(FrameworkElement source)
		{
			source.GotKeyboardFocus += OnSourceGotFocus;
			source.LostKeyboardFocus += OnSourceLostFocus;
		}

		protected override void DetachTarget(FrameworkElement source)
		{
			source.GotKeyboardFocus -= OnSourceGotFocus;
			source.LostKeyboardFocus -= OnSourceLostFocus;
		}

		private void OnSourceGotFocus(object sender, RoutedEventArgs e)
		{
			Open();
		}

		private void OnSourceLostFocus(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}