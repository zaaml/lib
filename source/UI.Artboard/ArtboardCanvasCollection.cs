// <copyright file="ArtboardCanvasCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.Core;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Artboard
{
	public sealed class ArtboardCanvasCollection : DependencyObjectCollectionBase<ArtboardCanvas>
	{
		internal event EventHandler<EventArgs<ArtboardCanvas>> CanvasAdded;
		internal event EventHandler<EventArgs<ArtboardCanvas>> CanvasRemoved;

		internal ArtboardCanvasCollection(ArtboardControl artboard)
		{
			Artboard = artboard;
		}

		internal ArtboardControl Artboard { get; }

		protected override void OnItemAdded(ArtboardCanvas canvas)
		{
			base.OnItemAdded(canvas);

			canvas.Artboard = Artboard;

			CanvasAdded?.Invoke(this, new EventArgs<ArtboardCanvas>(canvas));
		}

		protected override void OnItemRemoved(ArtboardCanvas canvas)
		{
			base.OnItemRemoved(canvas);

			CanvasRemoved?.Invoke(this, new EventArgs<ArtboardCanvas>(canvas));

			canvas.Artboard = null;
		}
	}
}