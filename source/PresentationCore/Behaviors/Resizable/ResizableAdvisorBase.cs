// <copyright file="ResizableAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal abstract class ResizableAdvisorBase : IResizableAdvisor
	{
		public abstract Rect GetBoundingBox(UIElement element);

		public abstract void SetBoundingBox(UIElement element, Rect rect);
	}
}