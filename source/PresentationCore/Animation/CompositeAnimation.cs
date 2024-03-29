﻿// <copyright file="CompositeAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Animation
{
	[ContentProperty("AnimationCollection")]
	public class CompositeAnimation : AnimationBase
	{
		private static readonly DependencyPropertyKey AnimationCollectionPropertyKey = DPM.RegisterReadOnly<AnimationCollection, CompositeAnimation>
			("AnimationCollectionPrivate");

		public static readonly DependencyProperty AnimationCollectionProperty = AnimationCollectionPropertyKey.DependencyProperty;

		public AnimationCollection AnimationCollection
		{
			get { return this.GetValueOrCreate(AnimationCollectionPropertyKey, () => new AnimationCollectionInt(this)); }
		}

		internal override void AttachContext(IInheritanceContext inheritanceContext)
		{
			base.AttachContext(inheritanceContext);

			foreach (var animation in AnimationCollection)
				inheritanceContext.Attach(animation);
		}

		internal override void DetachContext(IInheritanceContext inheritanceContext)
		{
			foreach (var animation in AnimationCollection)
				inheritanceContext.Detach(animation);

			base.DetachContext(inheritanceContext);
		}

		private void OnAnimationAdded(AnimationBase animation)
		{
			animation.RelativeTime = RelativeTime;
		}

		private void OnAnimationRemoved(AnimationBase animation)
		{
			animation.RelativeTime = 0.0;
		}

		internal override void OnRelativeTimeChanged()
		{
			foreach (var animation in AnimationCollection)
				animation.RelativeTime = RelativeTime;
		}

		private class AnimationCollectionInt : AnimationCollection
		{
			private readonly CompositeAnimation _compositeAnimation;

			public AnimationCollectionInt(CompositeAnimation compositeAnimation)
			{
				_compositeAnimation = compositeAnimation;
			}

			protected override void OnItemAdded(AnimationBase animation)
			{
				base.OnItemAdded(animation);

				InheritanceContext?.Attach(animation);

				_compositeAnimation.OnAnimationAdded(animation);
			}

			protected override void OnItemRemoved(AnimationBase animation)
			{
				InheritanceContext?.Detach(animation);

				animation.RelativeTime = 0.0;

				_compositeAnimation.OnAnimationRemoved(animation);

				base.OnItemRemoved(animation);
			}
		}
	}
}