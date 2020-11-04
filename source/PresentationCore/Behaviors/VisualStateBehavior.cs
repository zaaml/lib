// <copyright file="VisualStateBehavior.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Behaviors
{
	public sealed class VisualStateBehavior : BehaviorBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty VisualStateProperty = DPM.Register<string, VisualStateBehavior>
			("VisualState", b => b.OnVisualStateChanged);

		public static readonly DependencyProperty TargetProperty = DPM.Register<FrameworkElement, VisualStateBehavior>
			("Target", b => b.OnTargetChanged);

		#endregion

		#region Properties

		public FrameworkElement Target
		{
			get => (FrameworkElement) GetValue(TargetProperty);
			set => SetValue(TargetProperty, value);
		}

		public bool UseTransitions { get; set; }

		public string VisualState
		{
			get => (string) GetValue(VisualStateProperty);
			set => SetValue(VisualStateProperty, value);
		}

		#endregion

		#region  Methods

		protected override void OnAttached()
		{
			base.OnAttached();

			UpdateVisualState(false);
		}

		private void OnTargetChanged()
		{
			UpdateVisualState(UseTransitions);
		}

		private void OnVisualStateChanged()
		{
			UpdateVisualState(UseTransitions);
		}

		private void UpdateVisualState(bool useTransitions)
		{
#if SILVERLIGHT
      var target = (Target ?? FrameworkElement) as Control;
#else
			var target = Target ?? FrameworkElement;
#endif
			var visualState = VisualState;

			if (target == null || visualState.IsNullOrEmpty())
				return;

			VisualStateManager.GoToState(target, visualState, useTransitions);
		}

		#endregion
	}
}