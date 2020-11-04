// <copyright file="IInteractivityService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IInteractivityService
	{
		#region Properties

		ElementRoot ActualElementRoot { get; }

		StyleRoot ActualStyleRoot { get; }

		FrameworkElement Target { get; }

		bool UseTransitions { get; set; }

		#endregion
	}
}