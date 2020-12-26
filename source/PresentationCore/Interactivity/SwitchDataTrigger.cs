// <copyright file="SwitchDataTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class SwitchDataTrigger : SwitchTriggerBase
	{
		#region Static Fields and Constants

		private static readonly InteractivityProperty SourceValueProperty = RegisterInteractivityProperty(OnValueChanged);

		#endregion

		#region Fields

		private object _sourceValue;

		#endregion

		#region Properties

		protected override object ActualSourceValue => GetValue(SourceValueProperty, ref _sourceValue);

		public Binding Binding
		{
			get => (Binding) GetOriginalValue(SourceValueProperty, _sourceValue);
			set => SetValue(SourceValueProperty, ref _sourceValue, value);
		}

		#endregion

		#region  Methods

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (SwitchDataTrigger) source;
			
			Binding = triggerSource.Binding;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SwitchDataTrigger();
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			Load(SourceValueProperty, ref _sourceValue);

			base.LoadCore(root);

			UpdateTrigger();
		}

		private void OnValueChanged()
		{
			UpdateTrigger();
		}

		private static void OnValueChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			((SwitchDataTrigger) interactivityObject).OnValueChanged();
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(SourceValueProperty, ref _sourceValue);
			base.UnloadCore(root);
		}

		#endregion
	}
}