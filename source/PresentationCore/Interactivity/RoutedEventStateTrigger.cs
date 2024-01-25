// <copyright file="RoutedEventStateTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class RoutedEventStateTrigger : RoutedEventStateTriggerBase
	{
		private RoutedEvent _closeEvent;
		private RoutedEvent _openEvent;

		public RoutedEvent CloseEvent
		{
			get => _closeEvent;
			set
			{
				if (ReferenceEquals(CloseEvent, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_closeEvent = value;

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		protected override RoutedEvent CloseEventCore => CloseEvent;

		public RoutedEvent OpenEvent
		{
			get => _openEvent;
			set
			{
				if (ReferenceEquals(OpenEvent, value))
					return;

				if (IsLoaded)
					DeinitializeRuntime();

				_openEvent = value;

				if (IsLoaded)
					InitializeRuntime();
			}
		}

		protected override RoutedEvent OpenEventCore => OpenEvent;

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var triggerSource = (RoutedEventStateTrigger)source;

			CloseEvent = triggerSource.CloseEvent;
			OpenEvent = triggerSource.OpenEvent;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new RoutedEventStateTrigger();
		}
	}
}