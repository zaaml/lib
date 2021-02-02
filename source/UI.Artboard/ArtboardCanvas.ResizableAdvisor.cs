// <copyright file="ArtboardCanvas.ResizableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Resizable;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardCanvas
	{
		private sealed class ArtboardCanvasResizableAdvisor : ResizableAdvisorBase
		{
			private ArtboardSnapEngineContext _snapEngineContext;

			public ArtboardCanvasResizableAdvisor(ArtboardCanvas canvas)
			{
				Canvas = canvas;
			}

			private ArtboardCanvas Canvas { get; }

			public override Rect GetBoundingBox(UIElement element)
			{
				var position = GetPosition(element);
				var size = element.RenderSize;

				if (element is FrameworkElement frameworkElement)
				{
					if (double.IsNaN(frameworkElement.Width) == false)
						size.Width = frameworkElement.Width;

					if (double.IsNaN(frameworkElement.Height) == false)
						size.Height = frameworkElement.Height;
				}

				return new Rect(position, size);
			}

			protected override void OnResizeEnd(UIElement element, ResizableBehavior resizableBehavior)
			{
				base.OnResizeEnd(element, resizableBehavior);

				_snapEngineContext = _snapEngineContext.DisposeExchange();
			}

			protected override void OnResizeStart(UIElement element, ResizableBehavior resizableBehavior)
			{
				base.OnResizeStart(element, resizableBehavior);

				var snapSide = ArtboardSnapEngineUtils.GetResizeSide(resizableBehavior.ResizeInfo.HandleKind);

				_snapEngineContext = Canvas.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(element, snapSide));
			}

			public override void SetBoundingBox(UIElement element, Rect rect)
			{
				if (_snapEngineContext != null)
					rect = ArtboardSnapEngineUtils.CalcResizeRect(rect, _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(rect, _snapEngineContext)).SnapRect, _snapEngineContext.Parameters.Side);

				SetPosition(element, rect.TopLeft);

				if (element is FrameworkElement frameworkElement)
				{
					frameworkElement.Width = rect.Width;
					frameworkElement.Height = rect.Height;
				}

				Canvas.ArrangeChild(element, true);
			}
		}
	}
}