// <copyright file="BoolInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class BoolInterpolator : PrimitiveInterpolator<bool>
	{
		public static readonly BoolInterpolator Instance = new();

		private BoolInterpolator()
		{
		}

		protected internal override bool EvaluateCore(bool start, bool end, double progress)
		{
			return progress.IsGreaterThanOrClose(1.0) ? end : start;
		}
	}
}
