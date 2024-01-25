// <copyright file="CompositeAnimationCommand.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	[ContentProperty("Commands")]
	public sealed class CompositeAnimationCommand : AnimationCommand
	{
		private static readonly DependencyPropertyKey CommandsPrivatePropertyKey = DPM.RegisterReadOnly<AnimationCommandCollection, CompositeAnimationCommand>
			("CommandsPrivate");

		public static readonly DependencyProperty CommandsPrivateProperty = CommandsPrivatePropertyKey.DependencyProperty;

		public AnimationCommandCollection Commands
		{
			get { return this.GetValueOrCreate(CommandsPrivatePropertyKey, () => new AnimationCommandCollection()); }
		}

		protected override void RunCore(AnimationTimeline timeline)
		{
			foreach (var command in Commands) command.Run(timeline);
		}
	}
}