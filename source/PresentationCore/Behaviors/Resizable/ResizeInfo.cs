// <copyright file="ResizeInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal readonly struct ResizeInfo
	{
		public ResizeInfo(Point originLocation, Point currentLocation, ResizableHandleKind handleKind)
		{
			OriginLocation = originLocation;
			CurrentLocation = currentLocation;
			HandleKind = handleKind;
		}

		public Point OriginLocation { get; }

		public Point CurrentLocation { get; }

		public ResizableHandleKind HandleKind { get; }
	}
}