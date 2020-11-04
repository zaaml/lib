// <copyright file="DelayStateTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
	[ContentProperty("Setters")]
	public abstract class DelayStateTriggerBase : StateTriggerBase
	{
		#region Ctors

		internal DelayStateTriggerBase()
		{
		}

		#endregion

		#region Properties

		protected TimeSpan ActualCloseDelay => DelayTrigger?.CloseDelay ?? TimeSpan.Zero;

		protected DelayStateTrigger ActualDelayTrigger => DelayTrigger ?? (DelayTrigger = new DelayStateTrigger(base.OpenTriggerCore, TimeSpan.Zero, base.CloseTriggerCore, TimeSpan.Zero));

		protected TimeSpan ActualOpenDelay => DelayTrigger?.OpenDelay ?? TimeSpan.Zero;

		public Duration CloseDelay
		{
			get => DelayTrigger?.CloseDelay ?? default(Duration);
			set
			{
				if (CloseDelay == value)
					return;

				ActualDelayTrigger.CloseDelay = value.HasTimeSpan ? value.TimeSpan : TimeSpan.Zero;
			}
		}

		private DelayStateTrigger DelayTrigger { get; set; }

		public Duration OpenDelay
		{
			get => DelayTrigger?.OpenDelay ?? default(Duration);
			set
			{
				if (OpenDelay == value)
					return;

				ActualDelayTrigger.OpenDelay = value.HasTimeSpan ? value.TimeSpan : TimeSpan.Zero;
			}
		}

		#endregion

		#region  Methods

		protected override void CloseTriggerCore()
		{
			if (DelayTrigger != null)
			{
        if (IsActuallyOpen)
			    DelayTrigger.InvokeClose();
        else
          DelayTrigger.RevokeOpen();
			}
			else
				base.CloseTriggerCore();
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceDelayTrigger = (DelayStateTriggerBase) source;

			if (sourceDelayTrigger.DelayTrigger == null) return;

			OpenDelay = sourceDelayTrigger.OpenDelay;
			CloseDelay = sourceDelayTrigger.CloseDelay;
		}

		protected override void OpenTriggerCore()
		{
			if (DelayTrigger != null)
			{
        if (IsActuallyOpen == false)
			    DelayTrigger.InvokeOpen();
        else
          DelayTrigger.RevokeClose();
			}
			else
				base.OpenTriggerCore();
		}

		#endregion
	}
}