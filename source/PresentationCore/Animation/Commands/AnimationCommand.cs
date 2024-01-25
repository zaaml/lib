// <copyright file="AnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;
using Zaaml.PresentationCore.CommandCore;

namespace Zaaml.PresentationCore.Animation
{
	public abstract class AnimationCommand : InheritanceContextObject
	{
		public static readonly StopAnimationCommand Stop = new();
		public static readonly BeginAnimationCommand Begin = new();
		public static readonly PauseAnimationCommand Pause = new();
		public static readonly ResumeAnimationCommand Resume = new();

		protected AnimationCommand()
		{
			RunCommand = new RelayCommand(OnCommandExecuted, OnCanExecuteCommand);
		}

		public ICommand RunCommand { get; }

		private bool OnCanExecuteCommand(object commandParameter)
		{
			return commandParameter is AnimationTimeline;
		}

		private void OnCommandExecuted(object commandParameter)
		{
			if (commandParameter is AnimationTimeline animationTimeline)
				RunCore(animationTimeline);
		}

		public void Run(AnimationTimeline timeline)
		{
			RunCore(timeline);
		}

		protected abstract void RunCore(AnimationTimeline timeline);
	}
}