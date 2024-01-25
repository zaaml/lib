// <copyright file="FocusTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class FocusTrigger : RoutedEventStateTriggerBase
	{
		protected override RoutedEvent CloseEventCore => UIElement.LostFocusEvent;

		protected override RoutedEvent OpenEventCore => UIElement.GotFocusEvent;

		protected override InteractivityObject CreateInstance()
		{
			return new FocusTrigger();
		}
	}
}