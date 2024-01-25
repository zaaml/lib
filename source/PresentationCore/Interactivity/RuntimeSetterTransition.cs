// <copyright file="RuntimeSetterTransition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Zaaml.Core.Pools;
using Zaaml.PresentationCore.Animation;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using DoubleAnimation = System.Windows.Media.Animation.DoubleAnimation;

#if NETCOREAPP
#else
using Zaaml.Core.Extensions;
#endif

namespace Zaaml.PresentationCore.Interactivity
{
	internal class RuntimeSetterTransition : DependencyObject
	{
		public static readonly DependencyProperty ProgressProperty = DPM.Register<double, RuntimeSetterTransition>
			("Progress", e => e.OnProgressChanged);

		private static readonly Dictionary<Type, RuntimeSetterTransitionPool> TransitionPools = new();

		private readonly IAnimationValue _animationValue;
		private readonly RuntimeSetterTransitionPool _pool;
		private readonly Action _releaseAction;
		private readonly Storyboard _storyboard;
		private object _fromValue;
		private RuntimeSetter _setter;
		private object _toValue;

		private RuntimeSetterTransition(IAnimationValue animationValue, RuntimeSetterTransitionPool pool)
		{
			_releaseAction = Release;
			_animationValue = animationValue;
			_pool = pool;

			var doubleAnimation = new DoubleAnimation
			{
				From = 0.0,
				To = 1.0
			};

			Storyboard.SetTarget(doubleAnimation, this);
			Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(ProgressProperty));

			_storyboard = new Storyboard
			{
				Children = { doubleAnimation }
			};

			_storyboard.Begin();
			_storyboard.Stop();

			void OnStoryboardCompleted(object sender, EventArgs args)
			{
				_storyboard.Completed -= OnStoryboardCompleted;

				OnCompleted(true);
			}

			_storyboard.Completed += OnStoryboardCompleted;
		}

		public object CurrentValue => _animationValue.Current;

		public bool IsAnimating => _storyboard.GetCurrentState() != ClockState.Stopped && _setter != null;

		public object ValueStore { get; private set; }

		private static object GetActualValue(RuntimeSetter setter, object value, ref object valueStore)
		{
			if (value is Binding binding)
			{
				var target = setter.EffectiveValue.Target;
				var dependencyPropertyService = target.GetDependencyPropertyService();
				var dependencyProperty = dependencyPropertyService.CaptureServiceProperty(setter.TargetPropertyType, null);

				target.SetBinding(dependencyProperty, binding);

				valueStore = target.ReadLocalBindingExpression(dependencyProperty);

				if (valueStore == null)
				{
					target.ClearValue(dependencyProperty);

					dependencyPropertyService.ReleaseServiceProperty(dependencyProperty);

					return null;
				}

				return target.GetValue(dependencyProperty);
			}

			if (value is BindingExpression)
			{
				var target = setter.EffectiveValue.Target;
				var dependencyPropertyService = target.GetDependencyPropertyService();
				var dependencyProperty = dependencyPropertyService.CaptureServiceProperty(setter.TargetPropertyType, null);

				BindingUtil.RestoreBindingExpressionValue(target, dependencyProperty, value);

				valueStore = target.ReadLocalBindingExpression(dependencyProperty);

				if (valueStore == null)
				{
					target.ClearValue(dependencyProperty);

					dependencyPropertyService.ReleaseServiceProperty(dependencyProperty);

					return null;
				}

				return target.GetValue(dependencyProperty);
			}

			if (value is TemplateBindingExpression)
				throw new InvalidOperationException("Transition can not operate on TemplateBindingExpression. Use Binding with TemplatedParent value as RelativeSource instead of TemplateBinding.");

			return value;
		}

		private void OnCompleted(bool timeLineFinished)
		{
			if (_setter == null)
				return;

			if (timeLineFinished)
				_animationValue.RelativeTime = 1.0;

			var setter = _setter;

			ReleaseValue(ref _fromValue);
			ReleaseValue(ref _toValue);

			_setter = null;

			setter.OnTransitionCompleted();

			setter.EffectiveValue.Target.Dispatcher.BeginInvoke(_releaseAction, DispatcherPriority.Background);
		}

		private void OnProgressChanged(double oldValue, double newValue)
		{
			if (IsAnimating == false)
				return;

			_animationValue.RelativeTime = newValue;

			if (_setter.SetAnimatedValue(_animationValue.Current) == false)
				StopImpl();
		}

		private void Release()
		{
			_pool.Release(this);
		}

		private static void ReleaseValue(ref object valueStore)
		{
			if (valueStore is BindingExpression bindingExpression)
			{
				var target = bindingExpression.Target;
				var dependencyPropertyService = target.GetDependencyPropertyService();
				var dependencyProperty = bindingExpression.TargetProperty;

				target.ClearValue(dependencyProperty);

				dependencyPropertyService.ReleaseServiceProperty(dependencyProperty);
			}

			valueStore = null;
		}

		public static RuntimeSetterTransition RunTransition(RuntimeSetter setter, object from, object to)
		{
			var type = setter.TargetPropertyType;

			if (type == null)
				return null;

			var pool = TransitionPools.GetValueOrDefault(type);

			if (pool == null)
			{
				var interpolator = Interpolator.GetInterpolator(type);

				if (interpolator == null)
					return null;

				TransitionPools[type] = pool = new RuntimeSetterTransitionPool(interpolator);
			}

			var runtimeTransition = pool.GeTransition();

			runtimeTransition.ValueStore = setter.ActualValueStore;
			runtimeTransition.Start(setter, from, to);

			return runtimeTransition;
		}

		public void Start(RuntimeSetter setter, object from, object to)
		{
			var transition = setter.Transition;

			_animationValue.Start = GetActualValue(setter, from, ref _fromValue);
			_animationValue.End = GetActualValue(setter, to, ref _toValue);
			_animationValue.EasingFunction = transition.EasingFunction;
			_animationValue.RelativeTime = 0;

			var doubleAnimation = (DoubleAnimation)_storyboard.Children[0];

			doubleAnimation.BeginTime = transition.BeginTime;
			doubleAnimation.Duration = transition.Duration;

			_storyboard.Begin();

			_setter = setter;
		}

		public void Stop()
		{
			StopImpl();
		}

		private void StopImpl()
		{
			OnCompleted(false);

			_storyboard.Stop();
		}

		private sealed class RuntimeSetterTransitionPool
		{
			private readonly LightObjectPool<RuntimeSetterTransition> _transitionsPool;

			public RuntimeSetterTransitionPool(IInterpolator interpolator)
			{
				_transitionsPool = new LightObjectPool<RuntimeSetterTransition>(() => new RuntimeSetterTransition(interpolator.CreateAnimationValue(), this));
			}

			public RuntimeSetterTransition GeTransition()
			{
				return _transitionsPool.GetObject();
			}

			public void Release(RuntimeSetterTransition runtimeTransition)
			{
				_transitionsPool.Release(runtimeTransition);
			}
		}
	}
}