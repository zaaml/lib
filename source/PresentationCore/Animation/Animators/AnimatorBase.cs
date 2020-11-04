// <copyright file="AnimatorBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Media.Animation;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Animation.Animators
{
	public abstract class AnimatorBase<T> : IAnimator<T>
  {
    #region Fields

    private T _current;
    private IEasingFunction _easingFunction;
    private T _end;
    private bool _isCurrentDirty = true;
    private T _start;
    private double _time;

    #endregion

    #region  Methods

    protected abstract T EvaluateCurrent();

    #endregion

    #region Interface Implementations

    #region IAnimator<T>

    public T Current
    {
      get
      {
        if (_isCurrentDirty == false)
          return _current;

        _current = Time.IsCloseTo(1.0) ? End : EvaluateCurrent();
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

    public double Time
    {
      get => _time;
      set
      {
        _time = value;
        _isCurrentDirty = true;
      }
    }

    #endregion

    #endregion
  }

	public class Animation<TValue, TAnimator> : AnimationBase<TValue> where TAnimator : IAnimator<TValue>, new()
	{
		#region  Methods

		internal override IAnimator<TValue> CreateAnimator()
		{
			return new TAnimator
			{
				Start = From,
				End = To,
				EasingFunction = EasingFunction
			};
		}

		#endregion
	}
}