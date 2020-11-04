// <copyright file="DataCondition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class DataCondition : ConditionBase
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty TargetValueProperty = RegisterInteractivityProperty(UpdateConditionState);
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(UpdateConditionState);

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

				UpdateConditionState();
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

			var conditionSource = (DataCondition) source;

			Value = conditionSource.Value;
			Binding = conditionSource.Binding;
			Comparer = conditionSource.Comparer;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new DataCondition();
		}

		internal override void Deinitialize(IInteractivityRoot root)
		{
			Unload(TargetValueProperty, ref _targetValue);
			Unload(SourceValueProperty, ref _sourceValue);

			base.Deinitialize(root);
		}

		internal override void Initialize(IInteractivityRoot root)
		{
			base.Initialize(root);

			Load(TargetValueProperty, ref _targetValue);
			Load(SourceValueProperty, ref _sourceValue);
		}

		private static void UpdateConditionState(InteractivityObject interactivityobject, object oldvalue, object newvalue)
		{
			((DataCondition) interactivityobject).UpdateConditionState();
		}

		protected override bool UpdateConditionStateCore()
		{
			return TriggerCompareUtil.UpdateState(this, SourceValueProperty, ref _sourceValue, TargetValueProperty, ref _targetValue, Comparer) == TriggerState.Opened;
		}

		#endregion
	}
}