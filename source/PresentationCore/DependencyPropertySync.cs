// <copyright file="DependencyPropertySync.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;

namespace Zaaml.PresentationCore
{
	internal sealed class DependencyPropertySync<TSource, TTarget> where TSource : DependencyObject where TTarget : DependencyObject
	{
		private readonly Dictionary<DependencyProperty, DependencyProperty> _dependencyProperties = new();

		public DependencyPropertySync<TSource, TTarget> AddProperty(DependencyProperty source, DependencyProperty target)
		{
			_dependencyProperties.Add(source, target);

			return this;
		}

		public DependencyPropertySync<TSource, TTarget> AddProperty(DependencyProperty property)
		{
			_dependencyProperties.Add(property, property);

			return this;
		}

		public void Sync(DependencyProperty sourceProperty, TSource source, TTarget target)
		{
			if (_dependencyProperties.TryGetValue(sourceProperty, out var targetProperty))
				target.SetValue(targetProperty, source.GetValue(sourceProperty));
		}

		public void Sync(TSource source, TTarget target)
		{
			foreach (var kv in _dependencyProperties)
				target.SetValue(kv.Value, source.GetValue(kv.Key));
		}
	}
}