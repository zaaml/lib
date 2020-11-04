// <copyright file="IDraggableMouseInteraction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.Input
{
	internal interface IDraggableMouseInteraction : IDisposable
	{
		event EventHandler DragEnded;
		event EventHandler DragMove;
		event EventHandler DragStarted;

		Point ScreenOrigin { get; }

		Point ScreenPoint { get; }

		bool IsDragging { get; }

		bool MarkHandledEvents { get; set; }

		bool ProcessHandledEvents { get; set; }

		double Threshold { get; set; }

		void BeginDrag();

		void StopDrag();
	}
}