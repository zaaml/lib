// <copyright file="InputAutomation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Threading.Tasks;

namespace Zaaml.PresentationCore.Automation
{
	internal abstract class InputAutomation
	{
		protected abstract InputAutomationOptions OptionsCore { get; }

		protected async Task PostEventDelay()
		{
			await Task.Delay(OptionsCore.PostEventDelay);
		}

		protected async Task PreEventDelay()
		{
			await Task.Delay(OptionsCore.PreEventDelay);
		}
	}
}