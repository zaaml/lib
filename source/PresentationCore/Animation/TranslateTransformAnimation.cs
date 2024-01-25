// <copyright file="TranslateTransformAnimation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class TranslateTransformAnimation : AnimationBase<TranslateTransform>
	{
		public TranslateTransformAnimation() : base(TranslateTransformInterpolator.Instance)
		{
		}
	}
}
