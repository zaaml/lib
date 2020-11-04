// <copyright file="PulseTriggerBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	[Flags]
	public enum PulseTriggerBehavior
	{
		Default = 0,
		KeepClosed = 1,
		KeepOpened = 2
	}
}