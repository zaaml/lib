// <copyright file="DataTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class DataTrigger : DelayStateTriggerBase
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(OnValueChanged);
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);

		#endregion

		#region Fields

		private ITriggerValueComparer _comparer;
		private object _sourceValue;
		private object _targetValue;

		#endregion

		#region Properties

		public Binding Binding
		{
			get => (Binding) GetOriginalValue(SourceValueProperty, _sourceValue);
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		public ITriggerValueComparer Comparer
		{
			get => _comparer;
			set
			{
				if (ReferenceEquals(_comparer, value))
					return;

				_comparer = value;

				UpdateTriggerState();
			}
		}

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

			var triggerSource = (DataTrigger) source;

			Value = triggerSource.Value;
			Binding = triggerSource.Binding;
			Comparer = triggerSource.Comparer;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new DataTrigger();
		}

		internal override void DeinitializeTrigger(IInteractivityRoot root)
		{
			Unload(TargetValueProperty, ref _targetValue);
			Unload(SourceValueProperty, ref _sourceValue);

			base.DeinitializeTrigger(root);
		}

		internal override void InitializeTrigger(IInteractivityRoot root)
		{
			Load(TargetValueProperty, ref _targetValue);
			Load(SourceValueProperty, ref _sourceValue);

			base.InitializeTrigger(root);
		}

		private void OnValueChanged()
		{
			UpdateTriggerState();
		}

		private static void OnValueChanged(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
			((DataTrigger) interactivityobject).OnValueChanged();
		}

		protected override TriggerState UpdateTriggerStateCore()
		{
			return TriggerCompareUtil.UpdateState(this, SourceValueProperty, ref _sourceValue, TargetValueProperty, ref _targetValue, Comparer);
		}

		#endregion
	}
}