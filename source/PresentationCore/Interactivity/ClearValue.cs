// <copyright file="ClearValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class ClearValue : PropertyActionBase
	{
		protected override InteractivityObject CreateInstance()
		{
			return new ClearValue();
		}

		protected override void InvokeCore()
		{
			var actualProperty = ActualProperty;
			var actualTarget = ActualTarget;

			if (actualTarget == null || actualProperty == null)
				return;

			actualTarget.ClearValue(actualProperty);
		}
	}
}