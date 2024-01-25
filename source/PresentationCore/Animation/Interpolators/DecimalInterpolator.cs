// <copyright file="DecimalInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Animation
{
	public sealed class DecimalInterpolator : PrimitiveInterpolator<decimal>
	{
		public static readonly DecimalInterpolator Instance = new();

		private DecimalInterpolator()
		{
		}

		protected internal override decimal EvaluateCore(decimal start, decimal end, double progress)
		{
			return start + (end - start) * (decimal)progress;
		}
	}
}