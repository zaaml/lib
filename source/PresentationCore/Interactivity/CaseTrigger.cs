// <copyright file="CaseTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class CaseTrigger : CaseTriggerBase
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(OnValueChanged);

		#endregion

		#region Fields

		private object _targetValue;

		#endregion

		#region Properties

		public object Value
		{
			get => GetOriginalValue(TargetValueProperty, _targetValue);
			set => SetValue(TargetValueProperty, ref _targetValue, value);
		}

		#endregion

		#region  Methods

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

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			Load(TargetValueProperty, ref _targetValue);
			base.InitializeTrigger(root);
		}

		private void OnValueChanged()
		{
			UpdateSwitchTrigger();
		}

		private static void OnValueChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
			((CaseTrigger) interactivityobject).OnValueChanged();
		}

		internal void UpdateSwitchTrigger()
		{
			SwitchDataTrigger?.UpdateTrigger();
		}

		#endregion
	}
}