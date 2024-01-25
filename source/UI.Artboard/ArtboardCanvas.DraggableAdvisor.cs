// <copyright file="ArtboardCanvas.DraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Behaviors.Draggable;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardCanvas
	{
		private sealed class ArtboardCanvasDraggableAdvisor : DraggableAdvisorBase
		{
			public ArtboardCanvasDraggableAdvisor(ArtboardCanvas canvas)
			{
				Canvas = canvas;
			}

			private ArtboardCanvas Canvas { get; }

			public override Point GetPosition(UIElement element)
			{
				return ArtboardCanvas.GetPosition(element);
			}

			public override void SetPosition(UIElement element, Point value)
			{
				ArtboardCanvas.SetPosition(element, value);

				Canvas.ArrangeChild(element);
			}
		}
	}
}