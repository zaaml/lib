// <copyright file="ArtboardCanvas.DraggableAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardCanvas
	{
		private sealed class ArtboardCanvasDraggableAdvisor : ArtboardDraggableAdvisorBase
		{
			public ArtboardCanvasDraggableAdvisor(ArtboardCanvas canvas)
			{
				Canvas = canvas;
			}

			private ArtboardCanvas Canvas { get; }

			protected override Point GetPositionCore(UIElement element)
			{
				return ArtboardCanvas.GetPosition(element);
			}

			protected override void SetPositionCore(UIElement element, Point value)
			{
				ArtboardCanvas.SetPosition(element, value);

				Canvas.ArrangeChild(element);
			}
		}
	}
}