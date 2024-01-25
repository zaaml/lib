// <copyright file="MouseOverTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class MouseOverTrigger : RoutedEventStateTriggerBase
	{
		protected override RoutedEvent CloseEventCore => UIElement.MouseLeaveEvent;

		protected override RoutedEvent OpenEventCore => UIElement.MouseEnterEvent;

		protected override InteractivityObject CreateInstance()
		{
			return new MouseOverTrigger();
		}
	}
}