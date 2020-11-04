// <copyright file="SetCurrentValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class SetCurrentValue : PropertyValueActionBase
	{
		#region  Methods

		protected override InteractivityObject CreateInstance()
		{
			return new SetCurrentValue();
		}

		protected override void InvokeOverride()
		{
			var actualTarget = ActualTarget;
			var actualProperty = ActualProperty;
			var actualValue = ActualValue;

			if (actualValue.IsDependencyPropertyUnsetValue())
				actualValue = DependencyPropertyUtils.GetDefaultValue(actualTarget, actualProperty);

			actualTarget.SetCurrentValueInternal(actualProperty, actualValue);
		}

		#endregion
	}
}