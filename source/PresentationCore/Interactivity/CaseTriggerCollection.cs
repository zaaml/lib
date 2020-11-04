// <copyright file="CaseTriggerCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class CaseTriggerCollection : InteractivityCollection<CaseTriggerBase>
	{
		#region Fields

		private readonly SwitchTriggerBase _switchDataTrigger;
		private DefaultCaseTrigger _defaultCaseTrigger;

		#endregion

		#region Ctors

		public CaseTriggerCollection(SwitchTriggerBase switchDataTrigger) : base(switchDataTrigger)
		{
			_switchDataTrigger = switchDataTrigger;
		}

		#endregion

		#region Properties

		internal DefaultCaseTrigger DefaultCaseTrigger
		{
			get => _defaultCaseTrigger;
			private set
			{
				if (_defaultCaseTrigger != null && value != null)
					throw new InvalidOperationException("Default case trigger can be added one time");

				_defaultCaseTrigger = value;
			}
		}

		#endregion

		#region  Methods

		internal override InteractivityCollection<CaseTriggerBase> CreateInstance(IInteractivityObject parent)
		{
			return new CaseTriggerCollection((SwitchTriggerBase) parent);
		}

		protected override void OnItemAddedCore(CaseTriggerBase item)
		{
			base.OnItemAddedCore(item);

			var defaultCase = item as DefaultCaseTrigger;
			if (defaultCase != null)
				DefaultCaseTrigger = defaultCase;
		}

		protected override void OnItemRemovedCore(CaseTriggerBase item)
		{
			if (ReferenceEquals(DefaultCaseTrigger, item))
				DefaultCaseTrigger = null;

			base.OnItemRemovedCore(item);
		}

		#endregion
	}
}