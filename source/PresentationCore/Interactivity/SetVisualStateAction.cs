// <copyright file="SetVisualStateAction.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class SetVisualStateAction : TargetTriggerActionBase
	{
		public bool UseTransitions { get; set; }

		public string VisualState { get; set; }

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var setVisualStateAction = (SetVisualStateAction)source;

			VisualState = setVisualStateAction.VisualState;
			UseTransitions = setVisualStateAction.UseTransitions;
		}

		protected override InteractivityObject CreateInstance()
		{
			return new SetVisualStateAction();
		}

		protected override void InvokeCore()
		{
			if (VisualState.IsNullOrEmpty())
				return;

			if (ActualTarget is not FrameworkElement target)
				return;

			System.Windows.VisualStateManager.GoToState(target, VisualState, UseTransitions);
		}
	}
}