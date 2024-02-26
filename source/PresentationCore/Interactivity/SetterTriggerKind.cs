// <copyright file="SetterTriggerKind.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	[Flags]
	internal enum SetterTriggerKind
	{
		Unspecified = 0,
		VisualState = 1,
		Class = 2
	}
}