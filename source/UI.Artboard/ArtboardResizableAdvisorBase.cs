// <copyright file="ArtboardResizableAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Resizable;

namespace Zaaml.UI.Controls.Artboard
{
	internal abstract class ArtboardResizableAdvisorBase : ResizableAdvisorBase
	{
		private ArtboardSnapEngineContext _snapEngineContext;

		public sealed override Rect GetBoundingBox(UIElement element)
		{
			return GetBoundingBoxCore(element);
		}

		protected abstract Rect GetBoundingBoxCore(UIElement element);

		protected override void OnResizeEnd(UIElement element, ResizableBehavior resizableBehavior)
		{
			base.OnResizeEnd(element, resizableBehavior);

			_snapEngineContext = _snapEngineContext.DisposeExchange();
		}

		protected override void OnResizeStart(UIElement element, ResizableBehavior resizableBehavior)
		{
			base.OnResizeStart(element, resizableBehavior);

			var snapSide = ArtboardSnapEngineUtils.GetResizeSide(resizableBehavior.ResizeInfo.HandleKind);
			var artboardResizableBehavior = (ArtboardResizableBehavior) resizableBehavior;
			var canvas = artboardResizableBehavior.Adorner.ArtboardCanvas;

			_snapEngineContext = canvas?.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(element, snapSide));
		}

		public sealed override void SetBoundingBox(UIElement element, Rect rect)
		{
			if (_snapEngineContext != null)
				rect = ArtboardSnapEngineUtils.CalcResizeRect(rect, _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(rect, _snapEngineContext)).SnapRect, _snapEngineContext.Parameters.Side);

			SetBoundingBoxCore(element, rect);
		}

		protected abstract void SetBoundingBoxCore(UIElement element, Rect rect);
	}
}