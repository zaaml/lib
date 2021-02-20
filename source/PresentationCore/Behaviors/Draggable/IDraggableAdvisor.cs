// <copyright file="IDraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal interface IDraggableAdvisor
	{
		Point GetPosition(UIElement element, DraggableBehavior draggableBehavior);

		void OnDragEnd(UIElement element, DraggableBehavior draggableBehavior);

		void OnDragMove(UIElement element, DraggableBehavior draggableBehavior);

		void OnDragStart(UIElement element, DraggableBehavior draggableBehavior);

		void SetPosition(UIElement element, Point value, DraggableBehavior draggableBehavior);
	}

	internal sealed class DummyDraggableAdvisor : IDraggableAdvisor
	{
		public static readonly IDraggableAdvisor Instance = new DummyDraggableAdvisor();

		private DummyDraggableAdvisor()
		{
		}

		public Point GetPosition(UIElement element, DraggableBehavior draggableBehavior)
		{
			return XamlConstants.ZeroPoint;
		}

		public void SetPosition(UIElement element, Point value, DraggableBehavior draggableBehavior)
		{
		}

		public void OnDragEnd(UIElement element, DraggableBehavior draggableBehavior)
		{
		}

		public void OnDragMove(UIElement element, DraggableBehavior draggableBehavior)
		{
		}

		public void OnDragStart(UIElement element, DraggableBehavior draggableBehavior)
		{
		}
	}

	internal interface IDraggableAdvisorProvider
	{
		IDraggableAdvisor GetAdvisor(UIElement element);
	}

	internal abstract class DraggableAdvisorBase : IDraggableAdvisor
	{
		protected virtual void OnDragEnd(UIElement element, DraggableBehavior draggableBehavior)
		{
		}

		protected virtual void OnDragMove(UIElement element, DraggableBehavior draggableBehavior)
		{
		}

		protected virtual void OnDragStart(UIElement element, DraggableBehavior draggableBehavior)
		{
		}

		public abstract Point GetPosition(UIElement element, DraggableBehavior draggableBehavior);

		public abstract void SetPosition(UIElement element, Point value, DraggableBehavior draggableBehavior);

		void IDraggableAdvisor.OnDragEnd(UIElement element, DraggableBehavior draggableBehavior)
		{
			OnDragEnd(element, draggableBehavior);
		}

		void IDraggableAdvisor.OnDragMove(UIElement element, DraggableBehavior draggableBehavior)
		{
			OnDragMove(element, draggableBehavior);
		}

		void IDraggableAdvisor.OnDragStart(UIElement element, DraggableBehavior draggableBehavior)
		{
			OnDragStart(element, draggableBehavior);
		}
	}

	internal class DelegateDraggableAdvisor<T> : DraggableAdvisorBase where T : UIElement
	{
		private readonly Func<T, Point> _getPosition;
		private readonly Action<T, Point> _setPosition;

		public DelegateDraggableAdvisor(Func<T, Point> getPosition, Action<T, Point> setPosition)
		{
			_getPosition = getPosition ?? (u => new Point());
			_setPosition = setPosition ?? ((u, p) => { });
		}

		public override Point GetPosition(UIElement element, DraggableBehavior draggableBehavior)
		{
			return _getPosition((T) element);
		}

		public override void SetPosition(UIElement element, Point value, DraggableBehavior draggableBehavior)
		{
			_setPosition((T) element, value);
		}
	}
}