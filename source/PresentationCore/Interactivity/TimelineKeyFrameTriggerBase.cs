// <copyright file="TimelineKeyFrameTriggerBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.PresentationCore.Interactivity
{
	public abstract class TimelineKeyFrameTriggerBase : TriggerBase
	{
		private TimeSpan _actualOffset;
		private TimeSpan? _offset;
		private TimelineTriggerOffsetKind _offsetKind;
		private TimelineTrigger _timelineTrigger;

		public TimeSpan ActualOffset
		{
			get => _actualOffset;
			internal set
			{
				if (_actualOffset.Equals(value))
					return;

				_actualOffset = value;

				UpdateState();
			}
		}

		public TimeSpan? Offset
		{
			get => _offset;
			set
			{
				if (Equals(_offset, value))
					return;

				_offset = value;

				TimelineTrigger?.OnTriggerOffsetChanged(this);
			}
		}

		public TimelineTriggerOffsetKind OffsetKind
		{
			get => _offsetKind;
			set
			{
				if (_offsetKind == value)
					return;

				_offsetKind = value;

				TimelineTrigger?.OnTriggerOffsetChanged(this);
			}
		}

		internal TimelineTrigger TimelineTrigger
		{
			get => _timelineTrigger;
			set
			{
				if (ReferenceEquals(_timelineTrigger, value))
					return;

				_timelineTrigger = value;

				UpdateState();
			}
		}

		internal void UpdateState()
		{
			IsEnabled = _timelineTrigger != null;

			UpdateStateCore();
		}

		protected abstract void UpdateStateCore();
	}
}