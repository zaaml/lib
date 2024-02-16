// <copyright file="PropertyChangedExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using Zaaml.Core;

namespace Zaaml.PresentationCore.PropertyCore.Extensions
{
	public static class PropertyChangedExtensions
	{
		public static IDisposable OnPropertyChanged<TPropertyValue>(this INotifyPropertyChanged source, string name,
			Action<TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new NotifyPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
			Action<PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new NotifyPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
			Action<PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
			Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new NotifyPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
			Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged<TPropertyValue>(this INotifyPropertyChanged source, string name,
			Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new NotifyPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, string name,
			Action<TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, string name,
			Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider<TPropertyValue>(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
			Action<object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new NotifyPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
			Action<IDisposable, object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new NotifyPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
			Action<object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
			Action<IDisposable, object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, name), onPropertyChanged, true);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
		}

		public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
			Action<IDisposable, object, object> onPropertyChanged)
		{
			return new DelegatePropertyChangeProvider(new DependencyObjectPropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
		}
	}
}