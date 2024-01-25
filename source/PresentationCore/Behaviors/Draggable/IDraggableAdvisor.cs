// <copyright file="IDraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal interface IDraggableAdvisor
	{
		Point GetPosition(UIElement element);

		void SetPosition(UIElement element, Point value);
	}

	internal sealed class DummyDraggableAdvisor : IDraggableAdvisor
	{
		public static readonly IDraggableAdvisor Instance = new DummyDraggableAdvisor();

		private DummyDraggableAdvisor()
		{
		}

		public Point GetPosition(UIElement element)
		{
			return XamlConstants.ZeroPoint;
		}

		public void SetPosition(UIElement element, Point value)
		{
		}
	}

	internal interface IDraggableAdvisorProvider
	{
		IDraggableAdvisor GetAdvisor(UIElement element);
	}

	internal abstract class DraggableAdvisorBase : IDraggableAdvisor
	{
		public abstract Point GetPosition(UIElement element);

		public abstract void SetPosition(UIElement element, Point value);
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

		public override Point GetPosition(UIElement element)
		{
			return _getPosition((T) element);
		}

		public override void SetPosition(UIElement element, Point value)
		{
			_setPosition((T) element, value);
		}
	}
}