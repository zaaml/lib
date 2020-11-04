// <copyright file="DragRoutedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal abstract class DragRoutedEventArgs : RoutedEventArgs
	{
		private protected DragRoutedEventArgs(RoutedEvent routedEvent, DraggableBehavior behavior)
			: base(routedEvent)
		{
			Behavior = behavior;
		}

		public DraggableBehavior Behavior { get; }
	}
}