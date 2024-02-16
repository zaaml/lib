// <copyright file="NotifyPropertyChangeProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Reflection;
using Zaaml.Core.Monads;
using Zaaml.Core.Weak;

namespace Zaaml.PresentationCore.PropertyCore
{
	internal class NotifyPropertyChangeProvider : PropertyChangeProviderBase
	{
		private readonly Func<object, object> _propertyGetter;
		private readonly WeakReference _source;
		private readonly IDisposable _weakEventListener;

		public NotifyPropertyChangeProvider(INotifyPropertyChanged source, string propertyName)
		{
			var propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

			_propertyGetter = propertyInfo.Return<PropertyInfo, Func<object, object>>(pi => s => pi.GetValue(s, null));

			if (_propertyGetter == null)
				return;

			Value = GetCurrentValue(source);
			PropertyName = propertyName;

			_source = new WeakReference(source);
			_weakEventListener = new WeakEventHandler<NotifyPropertyChangeProvider, PropertyChangedEventHandler, PropertyChangedEventArgs>(
				this,
				(t, o, e) => t.SourceOnPropertyChanged(o, e),
				a => source.PropertyChanged += a,
				a => source.PropertyChanged -= a);
		}

		public string PropertyName { get; }

		public override object Source => _source.Target;

		public object Value { get; set; }

		public override void Dispose()
		{
			base.Dispose();

			_weakEventListener.Dispose();
		}

		private object GetCurrentValue(object source)
		{
			return _propertyGetter(source);
		}

		private void OnPropertyChanged(object sender, string propertyName)
		{
			if (propertyName != PropertyName)
				return;

			OnPropertyChanged(sender, Value, (Value = GetCurrentValue(sender)), propertyName);
		}

		private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			OnPropertyChanged(sender, args.PropertyName);
		}
	}
}