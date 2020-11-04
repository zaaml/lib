// <copyright file="IResizableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal interface IResizableAdvisor
	{
		Rect GetBoundingBox(UIElement element);

		void SetBoundingBox(UIElement element, Rect rect);

		void OnResize(UIElement element, ResizableBehavior resizableBehavior);

		void OnResizeEnd(UIElement element, ResizableBehavior resizableBehavior);

		void OnResizeStart(UIElement element, ResizableBehavior resizableBehavior);
	}

	internal interface IResizableAdvisorProvider
	{
		IResizableAdvisor GetAdvisor(UIElement element);
	}
}