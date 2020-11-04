// <copyright file="DragMoveRoutedEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal sealed class DragMoveRoutedEventArgs : DragRoutedEventArgs
	{
		internal DragMoveRoutedEventArgs(RoutedEvent routedEvent, DraggableBehavior behavior)
			: base(routedEvent, behavior)
		{
		}

		public Vector DragDelta => Behavior.DragDelta;
	}
}