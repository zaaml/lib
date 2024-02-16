// <copyright file="LocalValueStore.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Extensions
{
	internal sealed class LocalValueStore : IDisposable
	{
		private readonly object _localValue;
		private readonly DependencyProperty _property;
		private readonly WeakReference _target;
		private bool _isDisposed;

		public LocalValueStore(DependencyObject target, DependencyProperty property) :
			this(target, property, target.ReadLocalValue(property))
		{
		}

		private LocalValueStore(DependencyObject target, DependencyProperty property, object localValue)
		{
			_property = property;
			_target = new WeakReference(target);
			_localValue = localValue;
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			_target.GetTarget<DependencyObject>()?.RestoreLocalValue(_property, _localValue);
		}
	}
}