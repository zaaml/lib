// <copyright file="DragStartedRoutedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal sealed class DragStartedRoutedEventArgs : DragRoutedEventArgs
	{
		internal DragStartedRoutedEventArgs(RoutedEvent routedEvent, DraggableBehavior behavior) : base(routedEvent, behavior)
		{
		}

		public bool Cancel { get; set; }
	}
}