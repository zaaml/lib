// <copyright file="RunAnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core;
using Zaaml.PresentationCore.Animation;

namespace Zaaml.PresentationCore.Interactivity
{
	public sealed class RunAnimationCommand : TriggerActionBase
	{
		private static readonly InteractivityProperty CommandProperty = RegisterInteractivityProperty(OnCommandChanged);
		private static readonly InteractivityProperty AnimationProperty = RegisterInteractivityProperty(AnimationChanged);
		private object _animation;

		private object _command;

		public AnimationCommand Command
		{
			get => GetValue(CommandProperty, ref _command) as AnimationCommand;
			set => SetValue(CommandProperty, ref _command, value);
		}

		public AnimationTimeline Animation
		{
			get => GetValue(AnimationProperty, ref _animation) as AnimationTimeline;
			set => SetValue(AnimationProperty, ref _animation, value);
		}

		private static void AnimationChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
		}

		protected internal override void CopyMembersOverride(InteractivityObject source)
		{
			base.CopyMembersOverride(source);

			var sourceInvokeCommand = (RunAnimationCommand)source;

			InteractivityProperty.Copy(out _command, sourceInvokeCommand._command);
			InteractivityProperty.Copy(out _animation, sourceInvokeCommand._animation);
		}

		protected override InteractivityObject CreateInstance()
		{
			return new RunAnimationCommand();
		}

		protected override void InvokeCore()
		{
			var command = Command;
			var animation = Animation;

			if (command != null && animation != null)
				command.Run(animation);
		}

		internal override void LoadCore(IInteractivityRoot root)
		{
			base.LoadCore(root);

			Load(CommandProperty, ref _command);
			Load(AnimationProperty, ref _animation);
		}

		private static void OnCommandChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
		}

		[UsedImplicitly]
		internal void SetAnimationProperty(object value)
		{
			SetValue(AnimationProperty, ref _animation, value);
		}

		[UsedImplicitly]
		internal void SetCommandProperty(object value)
		{
			SetValue(CommandProperty, ref _command, value);
		}

		internal override void UnloadCore(IInteractivityRoot root)
		{
			Unload(CommandProperty, ref _command);
			Unload(AnimationProperty, ref _animation);

			base.UnloadCore(root);
		}
	}
}