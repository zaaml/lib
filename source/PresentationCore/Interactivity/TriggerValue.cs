// <copyright file="TriggerValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class TriggerValue : PropertyValueActionBase
	{
		private object _localValue;

		public Duration SustainDelay { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceTriggerPropertyValue = (TriggerValue)source;

			SustainDelay = sourceTriggerPropertyValue.SustainDelay;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new TriggerValue();
		}

		protected override void InvokeOverride()
		{
			var actualTarget = ActualTarget;
			var actualProperty = ActualProperty;
			var actualValue = ActualValue;

			_localValue = actualTarget.ReadLocalValue(actualProperty);

			if (actualValue.IsDependencyPropertyUnsetValue())
				actualTarget.ClearValue(actualProperty);
			else
				actualTarget.SetValue(actualProperty, ActualValue);

			if (SustainDelay.HasTimeSpan == false)
				RestoreValue();
			else
				DelayAction.StaticInvoke(RestoreValue, SustainDelay.TimeSpan);
		}

		private void RestoreValue()
		{
			if (_localValue.IsDependencyPropertyUnsetValue())
				ActualTarget.ClearValue(ActualProperty);
			else
				ActualTarget.SetValue(ActualProperty, _localValue);

			_localValue = null;
		}
	}
}