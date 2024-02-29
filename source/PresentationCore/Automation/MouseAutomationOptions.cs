// <copyright file="MouseAutomationOptions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Automation
{
	internal class MouseAutomationOptions : InputAutomationOptions
	{
		public TimeSpan MoveDuration { get; set; }
	}
}