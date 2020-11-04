// <copyright file="DragEndedRoutedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal sealed class DragEndedRoutedEventArgs : DragRoutedEventArgs
	{
		internal DragEndedRoutedEventArgs(RoutedEvent routedEvent, DraggableBehavior behavior) : base(routedEvent, behavior)
		{
		}

		public bool Cancel { get; set; }
	}
}