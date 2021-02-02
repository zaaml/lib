// <copyright file="ArtboardCanvas.DraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardCanvas
	{
		private sealed class ArtboardCanvasDraggableAdvisor : DraggableAdvisorBase
		{
			private ArtboardSnapEngineContext _snapEngineContext;

			public ArtboardCanvasDraggableAdvisor(ArtboardCanvas canvas)
			{
				Canvas = canvas;
			}

			private ArtboardCanvas Canvas { get; }

			public override Point GetPosition(UIElement element)
			{
				return ArtboardCanvas.GetPosition(element);
			}

			protected override void OnDragEnd(UIElement element, DraggableBehavior draggableBehavior)
			{
				base.OnDragEnd(element, draggableBehavior);

				_snapEngineContext = _snapEngineContext.DisposeExchange();
			}

			protected override void OnDragStart(UIElement element, DraggableBehavior draggableBehavior)
			{
				base.OnDragStart(element, draggableBehavior);

				_snapEngineContext = Canvas.ArtboardControl?.SnapEngine?.CreateContext(new ArtboardSnapEngineContextParameters(element, ArtboardSnapRectSide.All));
			}

			public override void SetPosition(UIElement element, Point value)
			{
				if (_snapEngineContext != null)
					value = _snapEngineContext.Engine.Snap(new ArtboardSnapParameters(new Rect(value, element.RenderSize), _snapEngineContext)).SnapRect.GetTopLeft();

				ArtboardCanvas.SetPosition(element, value);

				Canvas.ArrangeChild(element);
			}
		}
	}
}