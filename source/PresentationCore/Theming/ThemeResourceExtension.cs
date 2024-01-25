// <copyright file="ThemeResourceExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Binding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Theming
{
	public sealed class ThemeResourceExtension : BindingMarkupExtension, IValueConverter, IThemeResourceChangeListener, INotifyPropertyChanged, ISetterValueProvider
	{
		private StrongReferenceBinding _binding;
		private ConstValueCache _cache;

		private object _keyStore;
		//private DependencyObject _targetObject;
		//private DependencyProperty _targetProperty;

		private event PropertyChangedEventHandler PropertyChanged;

		private StrongReferenceBinding ActualBinding => _binding ??= new StrongReferenceBinding("Value")
		{
			Converter = this,
			Reference = this,
			Mode = BindingMode.OneWay
		};

		internal string ActualKey => ThemeManager.GetKeyFromKeyword(Keyword) ?? Key;

		public IValueConverter Converter { get; set; }

		public object FallbackValue
		{
			get => ActualBinding.FallbackValue;
			set => ActualBinding.FallbackValue = value;
		}

		public string Key
		{
			get => _keyStore as string;
			set => _keyStore = value;
		}

		public ThemeKeyword Keyword
		{
			get => _keyStore as ThemeKeyword? ?? ThemeKeyword.Undefined;
			set => _keyStore = value;
		}

		private static PropertyChangedEventArgs PropertyChangedEventArgs { get; } = new PropertyChangedEventArgs("Value");

		protected override bool SupportNativeSetter => false;

		public object TargetNullValue
		{
			get => ActualBinding.TargetNullValue;
			set => ActualBinding.TargetNullValue = value;
		}

		private ThemeResourceReference ThemeResourceReference => ThemeManager.GetThemeResourceReference(ActualKey, true);

		public object Value => ThemeResourceReference.Value;

		private object CoerceValueProvider(object value, Type targetType)
		{
			return _cache.ProvideValue(value.IsUnset() || value.IsDependencyPropertyUnsetValue() ? null : value, targetType);
		}

		protected internal override Binding GetBinding(IServiceProvider serviceProvider)
		{
			if (ActualBinding.Source != null)
				return ActualBinding;

			//if (targetObject == null || targetProperty == null)
			//  throw new InvalidOperationException("ThemeResourceExtension require TargetObject and TargetProperty to be accessible through ServiceProvider in the call to ProvideValue");

			//_targetObject = targetObject;
			//_targetProperty = GetDependencyProperty(targetProperty);

			var themeResourceReference = ThemeManager.GetThemeResourceReference(ActualKey, true);

			if (ThemeManager.IsStatic == false)
				themeResourceReference.AddListener(this);

			ActualBinding.Source = this;

			return ActualBinding;
		}

		private void LogError(object target, object targetProperty, Type targetPropertyType, Exception exception)
		{
			LogService.LogWarning($"Error occurred while setting '{targetProperty}' property of type '{targetPropertyType}' to theme resource value with key '{Key}' on target '{target}'.\n\t{exception}");
		}

		protected override object ProvideValueCore(object target, object targetProperty, IServiceProvider serviceProvider)
		{
			//if (target is SetterBase)
			//  return this;

			var actualKey = ActualKey;
			var themeResourceReference = ThemeManager.GetThemeResourceReference(actualKey, true);

			if (themeResourceReference.IsBound == false || ThemeManager.IsStatic == false)
			{
				if (ThemeManager.IsStatic)
					LogService.LogWarning($"Theme resource with key '{actualKey}' can not be resolved statically in a static theme. Dynamic link to theme resource is created.");

				return base.ProvideValueCore(target, targetProperty, serviceProvider);
			}

			var targetPropertyType = GetPropertyType(targetProperty) ?? typeof(object);

			if (_binding == null)
			{
				var convertResult = themeResourceReference.XamlConvertValue(targetPropertyType);

				if (convertResult.IsFailed)
				{
					LogError(target, targetProperty, targetPropertyType, convertResult.Exception);

					return targetPropertyType.CreateDefaultValue();
				}

				return DefaultInnerConverter.Convert(themeResourceReference.CachedValue, targetPropertyType);
			}

			_binding.Mode = BindingMode.OneTime;

			try
			{
				var evaluatedValue = BindingEvaluator.EvaluateBinding(_binding, targetPropertyType);
				var convertResult = XamlStaticConverter.TryConvertValue(evaluatedValue, targetPropertyType);

				if (convertResult.IsFailed)
				{
					LogError(target, targetProperty, targetPropertyType, convertResult.Exception);

					return targetPropertyType.CreateDefaultValue();
				}

				return DefaultInnerConverter.Convert(convertResult.Result, targetPropertyType);
			}
			catch (Exception e)
			{
				LogError(target, targetProperty, targetPropertyType, e);
			}

			return targetPropertyType.CreateDefaultValue();
		}

		internal void UpdateThemeResource()
		{
			_cache.Reset();
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add => PropertyChanged += value;
			remove => PropertyChanged -= value;
		}

		ValueProviderOptions ISetterValueProvider.Options => ValueProviderOptions.StaticValue | ValueProviderOptions.Shared | ValueProviderOptions.LongLife;

		object ISetterValueProvider.OriginalValue => this;

		void ISetterValueProvider.Attach(RuntimeSetter setter)
		{
		}

		void ISetterValueProvider.Detach(RuntimeSetter setter)
		{
		}

		object ISetterValueProvider.ProvideValue(RuntimeSetter setter)
		{
			var targetType = setter.EffectiveValue.Property.GetPropertyType();

			var value = ThemeResourceReference.Value;

			if (value == null)
				return CoerceValueProvider(TargetNullValue, targetType);

			if (value.IsUnset() || value.IsDependencyPropertyUnsetValue())
				return CoerceValueProvider(FallbackValue, targetType);

			return CoerceValueProvider(Converter?.Convert(value, typeof(object), null, CultureInfo.CurrentCulture) ?? value, targetType);
		}

		void IThemeResourceChangeListener.OnThemeResourceChanged()
		{
			_cache.Reset();
			PropertyChanged?.Invoke(this, PropertyChangedEventArgs);
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value.IsDependencyPropertyUnsetValue() || value.IsUnset())
				value = FallbackValue;

			return (Converter ?? DefaultInnerConverter.Instance).Convert(value, targetType, parameter, culture);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		private class DefaultInnerConverter : IValueConverter
		{
			public static readonly IValueConverter Instance = new DefaultInnerConverter();

			public static object Convert(object value, Type targetType)
			{
				if (value == null || value.IsDependencyPropertyUnsetValue() || value.IsUnset())
					return RuntimeUtils.CreateDefaultValue(targetType);

				return value;
			}

			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				return Convert(value, targetType);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotSupportedException();
			}
		}
	}

	internal sealed class IsStyleExtensionConverter : IValueConverter
	{
		public static readonly IValueConverter Instance = new IsStyleExtensionConverter();

		private IsStyleExtensionConverter()
		{
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}