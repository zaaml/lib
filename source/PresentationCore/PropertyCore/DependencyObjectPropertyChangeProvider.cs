// <copyright file="DependencyObjectPropertyChangeProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;

namespace Zaaml.PresentationCore.PropertyCore
{
	public class DependencyObjectPropertyChangeProvider : DependencyObject, IPropertyChangeProvider
	{
		public static readonly DependencyProperty ValueProperty = DPM.Register<object, DependencyObjectPropertyChangeProvider>
			("Value", d => d.OnValueChanged);

		private readonly object _property;
		private readonly bool _senderSelf;
		private readonly WeakReference _source;
		private bool _disposing;

		public DependencyObjectPropertyChangeProvider(DependencyObject source, DependencyProperty dependencyProperty, bool senderSelf = true)
		{
			_senderSelf = senderSelf;
			_source = new WeakReference(source);
			_property = dependencyProperty;

			BindingOperations.SetBinding(this, ValueProperty, new Binding { Path = new PropertyPath(dependencyProperty), Source = source });
		}

		public DependencyObjectPropertyChangeProvider(DependencyObject source, string propertyName, bool senderSelf = true)
		{
			_senderSelf = senderSelf;
			_source = new WeakReference(source);
			_property = propertyName;

			BindingOperations.SetBinding(this, ValueProperty, new Binding { Path = new PropertyPath(propertyName), Source = source });
		}

		internal bool IsAnySubscriber => PropertyChanged != null;

		private void OnValueChanged(object oldValue, object newValue)
		{
			if (_source.IsAlive && _disposing == false)
				RaiseOnPropertyChanged(new PropertyValueChangedEventArgs(_source.Target, oldValue, newValue, _property));
		}

		protected virtual void RaiseOnPropertyChanged(PropertyValueChangedEventArgs e)
		{
			PropertyChanged?.Invoke(_senderSelf ? this : e.Source, e);
		}

		public void Dispose()
		{
			_disposing = true;

			ClearValue(ValueProperty);
		}

		public event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

		public object Source => _source.Target;
	}
}