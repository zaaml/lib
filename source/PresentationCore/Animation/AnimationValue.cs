// <copyright file="AnimationValue.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation
{
	public sealed class AnimationValue<T> : IAnimationValue<T>, IAnimationValue
	{
		private readonly Interpolator<T> _interpolator;

		private T _current;
		private IEasingFunction _easingFunction;
		private T _end;
		private bool _invert;
		private bool _isCurrentDirty = true;
		private double _relativeTime;
		private T _start;

		public AnimationValue(Interpolator<T> interpolator)
		{
			_interpolator = interpolator;
		}

		private T ActualEnd => Invert ? Start : End;

		private T ActualStart => Invert ? End : Start;

		object IAnimationValue.Current => Current;

		object IAnimationValue.End
		{
			get => End;
			set => End = (T)value;
		}

		object IAnimationValue.Start
		{
			get => Start;
			set => Start = (T)value;
		}

		public bool Invert
		{
			get => _invert;
			set
			{
				_invert = value;
				_isCurrentDirty = true;
			}
		}

		public T Current
		{
			get
			{
				if (_isCurrentDirty == false)
					return _current;

				if (RelativeTime.IsCloseTo(1.0))
					_current = ActualEnd;
				else
					_interpolator.Evaluate(ref _current, ActualStart, ActualEnd, RelativeTime, EasingFunction);

				_isCurrentDirty = false;

				return _current;
			}
		}

		public IEasingFunction EasingFunction
		{
			get => _easingFunction;
			set
			{
				_easingFunction = value;
				_isCurrentDirty = true;
			}
		}

		public T End
		{
			get => _end;
			set
			{
				_end = value;
				_isCurrentDirty = true;
			}
		}

		public T Start
		{
			get => _start;
			set
			{
				_start = value;
				_isCurrentDirty = true;
			}
		}

		public double RelativeTime
		{
			get => _relativeTime;
			set
			{
				_relativeTime = value;
				_isCurrentDirty = true;
			}
		}
	}
}