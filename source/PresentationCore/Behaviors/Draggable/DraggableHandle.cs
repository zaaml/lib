// <copyright file="DraggableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal abstract class DraggableHandle : InheritanceContextObject, IDraggableHandle
	{
		private DraggableBehavior _behavior;

		protected virtual void Attach(DraggableBehavior behavior)
		{
		}

		protected virtual void Detach(DraggableBehavior behavior)
		{
		}

		private protected void OnDragEnded()
		{
			Behavior?.OnHandleDragEnded();
		}

		private protected void OnDragMove()
		{
			Behavior?.OnHandleDragMove();
		}

		private protected void OnDragStarted()
		{
			Behavior?.OnHandleDragStarted();
		}

		public DraggableBehavior Behavior
		{
			get => _behavior;
			set
			{
				if (ReferenceEquals(_behavior, value))
					return;

				if (_behavior != null)
					Detach(_behavior);

				_behavior = value;

				if (_behavior != null)
					Attach(_behavior);
			}
		}

		public abstract Point OriginLocation { get; }

		public abstract Point CurrentLocation { get; }

		public abstract void BeginDrag();

		public abstract void StopDrag();
	}
}