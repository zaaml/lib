// <copyright file="BindingHelper.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using WeakReferenceDepObj = Zaaml.Core.Weak.WeakReference<System.Windows.DependencyObject>;

namespace Zaaml.PresentationCore.Data
{
	internal static class BindingHelper
	{
		public static void BindProperties(this DependencyObject target, DependencyProperty targetProperty, DependencyObject source,
			DependencyProperty sourceProperty, BindingMode mode = BindingMode.OneWay, IValueConverter converter = null, object targetNullValue = null)
		{
			var binding = new Binding
			{
				Path = new PropertyPath(sourceProperty),
				Source = source,
				Mode = mode,
				Converter = converter
			};

			if (targetNullValue != null)
				binding.TargetNullValue = targetNullValue;

			target.SetBinding(targetProperty, binding);
		}

		public static IDisposable BindPropertiesDisposable(this DependencyObject target, DependencyProperty targetProperty, DependencyObject source,
			DependencyProperty sourceProperty, BindingMode mode = BindingMode.OneWay, bool restoreValue = false)
		{
			return target.SetDisposableBinding(targetProperty, new Binding { Path = new PropertyPath(sourceProperty), Source = source, Mode = mode }, restoreValue);
		}

		public static IDisposable BindPropertiesDisposable(this DependencyObject target, DependencyPropertyKey targetProperty, DependencyObject source,
			DependencyPropertyKey sourceProperty, BindingMode mode = BindingMode.OneWay)
		{
			return new ReadOnlyPropertyBindingHelper(target, targetProperty, source, sourceProperty, mode);
		}

		public static IDisposable BindPropertiesDisposable(this DependencyObject target, DependencyPropertyKey targetProperty, DependencyObject source,
			DependencyProperty sourceProperty)
		{
			return new ReadOnlyPropertyBindingHelper(target, targetProperty, source, sourceProperty);
		}

		public static bool CopyBinding(this DependencyObject target, DependencyObject source, DependencyProperty property)
		{
			return source.ReadLocalBinding(property)
				.Do(b => BindingOperations.SetBinding(target, property, b))
				.Return(b => b != null);
		}

		public static IDisposable SetDisposableBinding(this DependencyObject target, DependencyProperty dependencyProperty, Binding binding,
			bool restoreValue = false)
		{
			var localValue = target.ReadLocalValue(dependencyProperty);
			var weakRef = new WeakReference(target);

			BindingOperations.SetBinding(target, dependencyProperty, binding);

			return DelegateDisposable.Create(delegate
			{
				var reference = weakRef.GetTarget<DependencyObject>();

				if (reference == null)
					return;

				if (reference.ReadLocalBinding(dependencyProperty) != binding) return;

				if (restoreValue == false)
					reference.ClearValue(dependencyProperty);
				else
					reference.RestoreLocalValue(dependencyProperty, localValue);
			});
		}

		private class ReadOnlyPropertyBindingHelper : DependencyObject, IDisposable
		{
			public static readonly DependencyProperty SourceValueProperty = DPM.Register<object, ReadOnlyPropertyBindingHelper>
				("SourceValue", h => h.OnSourceValueChanged);

			public static readonly DependencyProperty TargetValueProperty = DPM.Register<object, ReadOnlyPropertyBindingHelper>
				("TargetValue", h => h.OnTargetValueChanged);

			private static readonly DependencyProperty RefListProperty = DependencyProperty.RegisterAttached
				("RefList", typeof(List<ReadOnlyPropertyBindingHelper>), typeof(ReadOnlyPropertyBindingHelper), new PropertyMetadata(null));

			private readonly BindingMode _mode;
			private readonly WeakReferenceDepObj _source;
			private readonly DependencyProperty _sourceProperty;
			private readonly DependencyPropertyKey _sourcePropertyKey;
			private readonly WeakReferenceDepObj _target;
			private readonly DependencyPropertyKey _targetPropertyKey;
			private IDisposable _disposer;
			private bool _suspendHandlers = true;

			public ReadOnlyPropertyBindingHelper(DependencyObject target, DependencyPropertyKey targetPropertyKey, DependencyObject source,
				DependencyPropertyKey sourcePropertyKey, BindingMode mode)
			{
				_target = new WeakReferenceDepObj(target);
				_targetPropertyKey = targetPropertyKey;
				_source = new WeakReferenceDepObj(source);
				_sourcePropertyKey = sourcePropertyKey;
				_sourceProperty = sourcePropertyKey.DependencyProperty;
				_mode = mode;

				_disposer = new DisposableList
				(
					this.BindPropertiesDisposable(SourceValueProperty, source, sourcePropertyKey.DependencyProperty),
					this.BindPropertiesDisposable(TargetValueProperty, target, targetPropertyKey.DependencyProperty)
				);

				_suspendHandlers = false;

				SendValueToTarget();

				target.GetValueOrCreate(RefListProperty, () => new List<ReadOnlyPropertyBindingHelper>()).Add(this);
			}

			public ReadOnlyPropertyBindingHelper(DependencyObject target, DependencyPropertyKey targetPropertyKey, DependencyObject source,
				DependencyProperty sourceProperty)
			{
				_target = new WeakReferenceDepObj(target);
				_targetPropertyKey = targetPropertyKey;
				_source = new WeakReferenceDepObj(source);
				_sourceProperty = sourceProperty;

				_disposer = new DisposableList
				(
					this.BindPropertiesDisposable(SourceValueProperty, source, sourceProperty)
				);

				_suspendHandlers = false;

				SendValueToTarget();

				target.GetValueOrCreate(RefListProperty, () => new List<ReadOnlyPropertyBindingHelper>()).Add(this);
			}

			private bool EnsureSourceTarget(out DependencyObject source, out DependencyObject target)
			{
				source = _source.IsAlive ? _source.Target : null;
				target = _target.IsAlive ? _target.Target : null;

				if (source == null || target == null)
				{
					Dispose();
					return false;
				}

				return true;
			}

			private void OnSourceValueChanged()
			{
				if (_suspendHandlers)
					return;

				SendValueToTarget();
			}

			private void OnTargetValueChanged()
			{
				if (_suspendHandlers)
					return;

				if (_mode == BindingMode.TwoWay)
					SendValueToSource();
			}

			private void SendValueToSource()
			{
				try
				{
					_suspendHandlers = true;

					DependencyObject source;
					DependencyObject target;

					if (EnsureSourceTarget(out source, out target))
						source.SetValue(_sourcePropertyKey, target.GetValue(_targetPropertyKey.DependencyProperty));
				}
				finally
				{
					_suspendHandlers = false;
				}
			}

			private void SendValueToTarget()
			{
				try
				{
					_suspendHandlers = true;

					DependencyObject source;
					DependencyObject target;

					if (EnsureSourceTarget(out source, out target))
						target.SetValue(_targetPropertyKey, source.GetValue(_sourceProperty));
				}
				finally
				{
					_suspendHandlers = false;
				}
			}

			public void Dispose()
			{
				_disposer = _disposer.DisposeExchange();

				var target = _target.IsAlive ? _target.Target : null;
				if (target == null) return;

				var list = target.GetValue<List<ReadOnlyPropertyBindingHelper>>(RefListProperty);
				list.Remove(this);
				if (list.Count == 0)
					target.ClearValue(RefListProperty);
			}
		}
	}
}