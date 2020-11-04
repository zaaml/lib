// <copyright file="BoolInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation.Interpolators
{
	public sealed class BoolInterpolator : InterpolatorBase<bool>
	{
		#region Static Fields and Constants

		public static BoolInterpolator Instance = new BoolInterpolator();

		#endregion

		#region Ctors

		private BoolInterpolator()
		{
		}

		#endregion

		#region  Methods

		protected internal override bool EvaluateCore(bool start, bool end, double progress)
		{
			return progress.IsGreaterThanOrClose(1.0) ? end : start;
		}

		#endregion
	}
}
