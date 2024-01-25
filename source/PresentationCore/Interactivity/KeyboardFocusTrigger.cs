// <copyright file="KeyboardFocusTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class KeyboardFocusTrigger : RoutedEventStateTriggerBase
	{
		protected override RoutedEvent CloseEventCore => UIElement.LostKeyboardFocusEvent;

		protected override RoutedEvent OpenEventCore => UIElement.GotKeyboardFocusEvent;

		protected override InteractivityObject CreateInstance()
		{
			return new KeyboardFocusTrigger();
		}
	}
}