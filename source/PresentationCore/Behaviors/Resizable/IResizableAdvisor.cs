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
	}

	internal interface IResizableAdvisorProvider
	{
		IResizableAdvisor GetAdvisor(UIElement element);
	}
}