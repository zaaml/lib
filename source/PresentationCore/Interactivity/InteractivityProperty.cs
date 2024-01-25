// <copyright file="InteractivityProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Interactivity
{
	internal delegate void InteractivityPropertyChangedCallback(InteractivityObject interactivityObject, object oldValue, object newValue);

	internal class InteractivityProperty
	{
		private readonly bool _bindable;
		private readonly InteractivityPropertyChangedCallback _onPropertyChanged;

		public InteractivityProperty(InteractivityPropertyChangedCallback onPropertyChanged, bool bindable = true)
		{
			_onPropertyChanged = onPropertyChanged;
			_bindable = bindable;
		}

		public object CacheConvert(InteractivityObject interactivityObject, Type targetType, ref object valueStore)
		{
			Load(interactivityObject, ref valueStore);

			if (targetType == null)
				return GetValueInternal(valueStore);

			if (valueStore is XamlConvertCache cache)
				return cache.Convert(targetType);

			if (valueStore is InteractivityBindingProxy bindingProxy)
				return bindingProxy.GetValue(targetType);

			valueStore = new XamlConvertCache(valueStore, targetType);

			return GetValueInternal(valueStore);
		}

		public static void Copy(out object targetValue, object sourceValue)
		{
			targetValue = GetOriginalValue(sourceValue);
		}

		public static NativeBinding GetBinding(object value)
		{
			return GetBindingProxy(value)?.Binding;
		}

		private static InteractivityBindingProxy GetBindingProxy(object value)
		{
			return value as InteractivityBindingProxy;
		}

		public static object GetOriginalValue(object valueStore)
		{
			if (valueStore is XamlConvertCache cache)
				return cache.OriginalValue;

			return valueStore is InteractivityBindingProxy proxy ? proxy.Binding : valueStore;
		}

		public static object GetOriginalValue(InteractivityObject interactivityObject, object valueStore)
		{
			return GetOriginalValue(valueStore);
		}

		public object GetValue(InteractivityObject interactivityObject, ref object valueStore)
		{
			Load(interactivityObject, ref valueStore);

			return GetValueInternal(valueStore);
		}

		private static object GetValueInternal(object value)
		{
			if (value is XamlConvertCache cache)
				return cache.Value;

			return value is InteractivityBindingProxy proxy ? proxy.Value : value;
		}

		public void Load(InteractivityObject interactivityObject, ref object valueStore)
		{
			if (interactivityObject.IsLoaded == false)
				return;

			if (valueStore is XamlConvertCache || valueStore is InteractivityBindingProxy)
				return;

			if (_bindable == false)
				return;

			var binding = valueStore as NativeBinding ?? (valueStore as BindingBaseExtension)?.GetBinding(interactivityObject.InteractivityTarget, null);

			if (binding != null)
				valueStore = new InteractivityBindingProxy(interactivityObject, this, binding, interactivityObject.InteractivityTarget);
		}

		private void OnChanged(InteractivityObject interactivityObject, object oldValue, object newValue)
		{
			_onPropertyChanged?.Invoke(interactivityObject, oldValue, newValue);
		}

		public void SetValue(InteractivityObject interactivityObject, ref object valueStore, object value)
		{
			Unload(interactivityObject, ref valueStore);

			valueStore = value;

			Load(interactivityObject, ref valueStore);
		}

		public void Unload(InteractivityObject interactivityObject, ref object valueStore)
		{
			var proxy = GetBindingProxy(valueStore);

			if (proxy != null)
			{
				valueStore = proxy.Binding;
				proxy.Dispose();

				return;
			}

			if (valueStore is XamlConvertCache cache)
				valueStore = cache.OriginalValue;
		}

		private class InteractivityBindingProxy : BindingProxyBase
		{
			private readonly WeakReference _interactivityObjectWeak;
			private readonly InteractivityProperty _property;
			private ConvertCacheStore _cacheStore;

			public InteractivityBindingProxy(InteractivityObject interactivityObject, InteractivityProperty property, NativeBinding binding, DependencyObject dataObject) : base(binding, dataObject)
			{
				_interactivityObjectWeak = new WeakReference(interactivityObject);
				_property = property;
				_cacheStore.Value = Value;
			}

			public object GetValue(Type targetType)
			{
				return XamlStaticConverter.ConvertCache(ref _cacheStore, targetType);
			}

			protected override void OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
			{
				_cacheStore.Value = newValue;

				var interactivityObject = _interactivityObjectWeak.GetTarget<InteractivityObject>();

				if (interactivityObject != null)
					_property.OnChanged(interactivityObject, oldValue, newValue);
				else
					Dispose();
			}
		}
	}
}