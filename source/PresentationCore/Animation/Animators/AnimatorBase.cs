// <copyright file="AnimatorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation.Animators
{
	public abstract class AnimatorBase<T> : IAnimator<T>
	{
		private T _current;
		private IEasingFunction _easingFunction;
		private T _end;
		private bool _invert;
		private bool _isCurrentDirty = true;
		private T _start;
		private double _time;

		public bool Invert
		{
			get => _invert;
			set
			{
				_invert = value;
				_isCurrentDirty = true;
			}
		}

		protected abstract T EvaluateCurrent();

		public T Current
		{
			get
			{
				if (_isCurrentDirty == false)
					return _current;

				_current = RelativeTime.IsCloseTo(1.0) ? ActualEnd : EvaluateCurrent();
				_isCurrentDirty = false;

				return _current;
			}
		}

		protected T ActualEnd => Invert ? Start : End;

		protected T ActualStart => Invert ? End : Start;

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
			get => _time;
			set
			{
				_time = value;
				_isCurrentDirty = true;
			}
		}
	}

	public class Animation<TValue, TAnimator> : AnimationBase<TValue> where TAnimator : IAnimator<TValue>, new()
	{
		internal override IAnimator<TValue> CreateAnimator()
		{
			return new TAnimator
			{
				Start = From,
				End = To,
				EasingFunction = ActualEasingFunction
			};
		}
	}
}