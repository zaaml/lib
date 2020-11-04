// <copyright file="IDraggableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal interface IDraggableHandle
	{
		DraggableBehavior Behavior { get; set; }
		
		Point CurrentLocation { get; }

		Point OriginLocation { get; }

		void BeginDrag();

		void StopDrag();
	}
}