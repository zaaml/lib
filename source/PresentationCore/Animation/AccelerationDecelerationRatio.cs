// <copyright file="AccelerationDecelerationRatio.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public struct AccelerationDecelerationRatio
	{
		public double AccelerationRatio { get; set; }

		public double DecelerationRatio { get; set; }

		public double CalcProgress(double progress)
		{
			if (progress.IsLessThanOrClose(0.0))
				return 0.0;

			if (progress.IsGreaterThanOrClose(1.0))
				return 1.0;

			var isAccelerationZero = AccelerationRatio.IsZero();
			var isDecelerationZero = DecelerationRatio.IsZero();

			if (isAccelerationZero && isDecelerationZero)
				return progress;

			if (isAccelerationZero)
				return CalcDeceleration(progress);

			if (isDecelerationZero)
				return CalcAcceleration(progress);

			return CalcAccelerationDeceleration(progress);
		}

		private double CalcDeceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}

		private double CalcAcceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}

		private double CalcAccelerationDeceleration(double progress)
		{
			var speed = 1 / (1 / (2 * DecelerationRatio) + 1 - AccelerationRatio - 1 / (2 * AccelerationRatio));

			var acceleration = speed / AccelerationRatio;

			if (progress.IsLessThan(AccelerationRatio))
				return acceleration * progress * progress / 2;

			if (progress.IsLessThan(1 - DecelerationRatio))
				return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (progress - AccelerationRatio);

			var t = DecelerationRatio - (1 - progress);
			var deceleration = -speed / DecelerationRatio;

			return acceleration * AccelerationRatio * AccelerationRatio / 2 + speed * (1 - DecelerationRatio - AccelerationRatio) + speed * t + deceleration * t * t / 2;
		}
	}
}
