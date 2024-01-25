// <copyright file="DataChangeTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class DataChangeTrigger : ActionTriggerBase
	{
		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);
		
		private object _sourceValue;

		public Binding Binding
		{
			get => (Binding)GetOriginalValue(SourceValueProperty, _sourceValue);
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		private void OnDataChanged()
		{
			Invoke();
		}

		protected override InteractivityObject CreateInstance()
		{
			return new DataChangeTrigger();
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (DataChangeTrigger)source;

			Binding = triggerSource.Binding;
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Load(SourceValueProperty, ref _sourceValue);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(SourceValueProperty, ref _sourceValue);

			base.UnloadCore(root);
		}

		private static void OnValueChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((DataChangeTrigger)interactivityObject).OnDataChanged();
		}
	}
}