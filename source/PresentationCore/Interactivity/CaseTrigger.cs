// <copyright file="CaseTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class CaseTrigger : CaseTriggerBase
	{
		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(OnValueChanged);

		private object _targetValue;

		public object Value
		{
			get => GetOriginalValue(TargetValueProperty, _targetValue);
			set => SetValue(TargetValueProperty, ref _targetValue, value);
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (CaseTrigger) source;

			Value = triggerSource.Value;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new CaseTrigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			Unload(TargetValueProperty, ref _targetValue);

			base.DeinitializeTrigger(root);
		}

		internal object GetActualValue(Type targetType)
		{
			return CacheConvert(TargetValueProperty, targetType, ref _targetValue);
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			Load(TargetValueProperty, ref _targetValue);

			base.InitializeTrigger(root);
		}

		private void OnValueChanged()
		{
			UpdateSwitchTrigger();
		}

		private static void OnValueChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((CaseTrigger) interactivityObject).OnValueChanged();
		}

		internal void UpdateSwitchTrigger()
		{
			SwitchDataTrigger?.UpdateTrigger();
		}
	}
}