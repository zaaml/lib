// <copyright file="ArtboardDraggableAdvisorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	internal abstract class ArtboardDraggableAdvisorBase : DraggableAdvisorBase
	{
		private ArtboardSnapEngineContext _snapEngineContext;

		public sealed override Point GetPosition(UIElement element, DraggableBehavior draggableBehavior)
		{
			return GetPositionCore(element);
		}

		protected abstract Point GetPositionCore(UIElement element);

		protected override void OnDragEnd(UIElement element, DraggableBehavior draggableBehavior)
		{
			base.OnDragEnd(element, draggableBehavior);

			_snapEngineContext = _snapEngineContext.DisposeExchange();
		}

		protected override void OnDragStart(UIElement element, DraggableBehavior draggableBehavior)
		{
			base.OnDragStart(element, draggableBehavior);

			var artboardDraggableBehavior = (ArtboardDraggableBehavior) draggableBehavior;
			var canvas = artboardDraggableBehavior.Adorner.ArtboardCanvas;

			_snapEngineContext = canvas?.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(element, ArtboardSnapRectSide.All));
		}

		public sealed override void SetPosition(UIElement element, Point value, DraggableBehavior draggableBehavior)
		{
			if (_snapEngineContext != null)
			{
				var elementRect = new Rect(value, element.RenderSize);

				value = _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(elementRect, _snapEngineContext)).SnapRect.GetTopLeft();
			}

			SetPositionCore(element, value);
		}

		protected abstract void SetPositionCore(UIElement element, Point value);
	}
}