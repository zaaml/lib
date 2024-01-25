// <copyright file="LongInterpolator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class LongInterpolator : PrimitiveInterpolator<long>
	{
		public static readonly LongInterpolator Instance = new();

		private LongInterpolator()
		{
		}

		[SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
		protected internal override long EvaluateCore(long start, long end, double progress)
		{
			if (progress == 0.0)
				return start;

			if (progress == 1.0)
				return end;

			var addend = (double)(end - start);

			addend *= progress;
			addend += addend > 0.0 ? 0.5 : -0.5;

			return start + (long)addend;
		}
	}
}