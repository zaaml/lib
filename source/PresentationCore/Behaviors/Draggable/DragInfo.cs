// <copyright file="DragInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Draggable
{
	internal readonly struct DragInfo
	{
		public DragInfo(Point originLocation, Point currentLocation)
		{
			OriginLocation = originLocation;
			CurrentLocation = currentLocation;
		}

		public Point OriginLocation { get; }

		public Point CurrentLocation { get; }
	}
}