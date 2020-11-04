// <copyright file="IResizableHandle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Behaviors.Resizable
{
	internal interface IResizableHandle
	{
		Point CurrentLocation { get; }

		ResizableHandleKind HandleKind { get; }

		Point OriginLocation { get; }

		ResizableBehavior Behavior { get; set; }
	}
}