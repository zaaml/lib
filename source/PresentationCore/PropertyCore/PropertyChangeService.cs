// <copyright file="PropertyChangeService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal class PropertyChangeService : ServiceBase<DependencyObject>
	{
		private readonly Dictionary<DependencyProperty, DependencyObjectPropertyChangeProvider> _dpProviders = [];
		private readonly Dictionary<string, DependencyObjectPropertyChangeProvider> _strProviders = [];

		public void AddValueChanged(DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			_dpProviders.GetValueOrCreate(depProp, () => new DependencyObjectPropertyChangeProvider(Target, depProp, false)).PropertyChanged += handler;
		}

		public void AddValueChanged(string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			_strProviders.GetValueOrCreate(propertyName, () => new DependencyObjectPropertyChangeProvider(Target, propertyName, false)).PropertyChanged += handler;
		}

		public void RemoveValueChanged(DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			RemoveValueChanged(_dpProviders, depProp, handler);
		}

		public void RemoveValueChanged(string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			RemoveValueChanged(_strProviders, propertyName, handler);
		}

		private void RemoveValueChanged<TKey>(Dictionary<TKey, DependencyObjectPropertyChangeProvider> providers, TKey key, EventHandler<PropertyValueChangedEventArgs> handler)
		{
			var provider = providers.GetValueOrDefault(key);

			if (provider == null)
				return;

			provider.PropertyChanged -= handler;

			if (provider.IsAnySubscriber)
				return;

			providers.Remove(key);

			if (_strProviders.Count == 0 && _dpProviders.Count == 0)
				Target.RemoveService<PropertyChangeService>();
		}
	}
}