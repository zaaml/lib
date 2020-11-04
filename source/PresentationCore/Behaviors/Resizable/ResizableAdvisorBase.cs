// <copyright file="ResizableAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal abstract class ResizableAdvisorBase : IResizableAdvisor
	{
		protected virtual void OnResize(UIElement element, ResizableBehavior resizableBehavior)
		{
		}

		protected virtual void OnResizeEnd(UIElement element, ResizableBehavior resizableBehavior)
		{
		}

		protected virtual void OnResizeStart(UIElement element, ResizableBehavior resizableBehavior)
		{
		}

		public abstract Rect GetBoundingBox(UIElement element);

		public abstract void SetBoundingBox(UIElement element, Rect rect);

		void IResizableAdvisor.OnResize(UIElement element, ResizableBehavior resizableBehavior)
		{
			OnResize(element, resizableBehavior);
		}

		void IResizableAdvisor.OnResizeEnd(UIElement element, ResizableBehavior resizableBehavior)
		{
			OnResizeEnd(element, resizableBehavior);
		}

		void IResizableAdvisor.OnResizeStart(UIElement element, ResizableBehavior resizableBehavior)
		{
			OnResizeStart(element, resizableBehavior);
		}
	}
}