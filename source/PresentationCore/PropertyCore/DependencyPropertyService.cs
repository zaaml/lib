// <copyright file="DependencyPropertyService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal class DependencyPropertyService : ServiceBase<DependencyObject>
	{
		private static readonly MultiMap<Type, DependencyProperty> ServiceProperties = [];
		private static readonly TwoWayDictionary<Type, int> TypeIdDictionary = new();
		private static int _nextTypeId;

		private readonly Dictionary<DependencyProperty, WeakLinkedList<IDependencyPropertyListener>> _expandoPropertyChangeListeners = [];
		private readonly Dictionary<DependencyProperty, WeakReference> _servicePropertyListeners = [];

		public void AddExpandoPropertyListener(DependencyProperty dependencyProperty, IDependencyPropertyListener listener)
		{
			_expandoPropertyChangeListeners.GetValueOrCreate(dependencyProperty, () => []).Add(listener);
		}

		public DependencyProperty CaptureServiceProperty(Type type, IDependencyPropertyListener listener)
		{
			var typeProperties = ServiceProperties.GetOrCreateValues(type);
			var weakListener = new WeakReference(listener ?? DummyListener.Instance);
			var property = typeProperties.FirstOrDefault(IsFreeImpl);

			if (property != null)
			{
				Target.ClearValue(property);

				_servicePropertyListeners.Add(property, weakListener);

				return property;
			}

			var typeId = TypeIdDictionary.GetValueOrCreate(type, t => _nextTypeId++);

			property = RegisterServiceProperty($"{typeId}_svc{typeProperties.Count}", type);
			_servicePropertyListeners.Add(property, weakListener);
			typeProperties.Add(property);

			return property;
		}

		private bool IsFreeImpl(DependencyProperty property)
		{
			return _servicePropertyListeners.ContainsKey(property) == false;
		}

		public void OnExpandoPropertyValueChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
		{
			var listenerList = _expandoPropertyChangeListeners.GetValueOrDefault(dependencyProperty);

			if (listenerList == null)
				return;

			foreach (var listener in listenerList)
				listener.OnPropertyChanged(depObj, dependencyProperty, oldValue, newValue);
		}

		private static void OnServicePropertyValueChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
		{
			depObj.GetDependencyPropertyService().OnServicePropertyValueChanged(depObj, args.Property, args.OldValue, args.NewValue);
		}

		private void OnServicePropertyValueChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
		{
			if (_servicePropertyListeners.TryGetValue(dependencyProperty, out var weakListener) == false) 
				return;

			var listener = weakListener.GetTarget<IDependencyPropertyListener>();

			if (listener != null)
				listener.OnPropertyChanged(depObj, dependencyProperty, oldValue, newValue);
			else
				_servicePropertyListeners.Remove(dependencyProperty);
		}

		private static DependencyProperty RegisterServiceProperty(string name, Type type)
		{
			var defaultValue = RuntimeUtils.CreateDefaultValue(type);
			var propertyMetadata = new PropertyMetadata(defaultValue, OnServicePropertyValueChanged);

			return DependencyPropertyManager.RegisterAttached(name, type, typeof(DependencyPropertyManager), propertyMetadata);
		}

		public void ReleaseServiceProperty(DependencyProperty property)
		{
			_servicePropertyListeners.Remove(property);
			Target.ClearValue(property);
		}

		public void RemoveExpandoListener(DependencyProperty dependencyProperty, IDependencyPropertyListener listener)
		{
			var listenerList = _expandoPropertyChangeListeners.GetValueOrDefault(dependencyProperty);

			if (listenerList == null) 
				return;

			listenerList.Remove(listener);

			if (listenerList.IsEmpty)
				_expandoPropertyChangeListeners.Remove(dependencyProperty);
		}

		private class DummyListener : IDependencyPropertyListener
		{
			public static readonly IDependencyPropertyListener Instance = new DummyListener();

			private DummyListener()
			{
			}

			void IDependencyPropertyListener.OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
			{
			}
		}
	}

	internal static class DependencyPropertyServiceExtensions
	{
		public static DependencyPropertyService GetDependencyPropertyService(this DependencyObject dependencyObject)
		{
			return dependencyObject.GetServiceOrCreate(() => new DependencyPropertyService());
		}
	}
}