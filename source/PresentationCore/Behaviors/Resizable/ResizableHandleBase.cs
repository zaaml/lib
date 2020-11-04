// <copyright file="ResizableHandleBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal abstract class ResizableHandleBase : InheritanceContextObject, IResizableHandle
	{
		private ResizableBehavior _behavior;
		protected abstract ResizableHandleKind HandleKindCore { get; set; }

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

		protected virtual void UpdateCursor()
		{
		}

		ResizableHandleKind IResizableHandle.HandleKind => HandleKindCore;

		public abstract Point OriginLocation { get; }

		public ResizableBehavior Behavior
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

		protected virtual void Attach(ResizableBehavior behavior)
		{
		}

		protected virtual void Detach(ResizableBehavior behavior)
		{
		}

		public abstract Point CurrentLocation { get; }
	}
}