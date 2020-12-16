// <copyright file="SwitchConverter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Windows.Markup;
using Zaaml.Core.Collections;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Converters
{
	[ContentProperty("Value")]
	public abstract class SwitchOption<TKey, TValue>
	{
		private XamlConvertCacheStruct<TValue> _valueCache;

		public TValue Value
		{
			get => _valueCache.Value;
			set => _valueCache.Value = value;
		}

		internal abstract TKey XamlConvertKey(Type targetType);

		internal TValue XamlConvertValue(Type targetType)
		{
			return _valueCache.XamlConvert(targetType);
		}
	}

	public class Case<TKey, TValue> : SwitchOption<TKey, TValue>
	{
		private XamlConvertCacheStruct<TKey> _keyCache;

		public TKey Key
		{
			get => _keyCache.Value;
			set => _keyCache.Value = value;
		}

		internal override TKey XamlConvertKey(Type targetType)
		{
			return _keyCache.XamlConvert(targetType);
		}
	}

	public class Default<TKey, TValue> : SwitchOption<TKey, TValue>
	{
		private XamlConvertCacheStruct<TKey> _keyCache = new XamlConvertCacheStruct<TKey> {Value = default};

		internal override TKey XamlConvertKey(Type targetType)
		{
			return _keyCache.XamlConvert(targetType);
		}
	}

	public sealed class OptionCollection<TKey, TValue> : CollectionBase<SwitchOption<TKey, TValue>>
	{
	}

	[ContentProperty("Options")]
	public class SwitchConverter<TKey, TValue> : BaseValueConverter
	{
		private static readonly SwitchOption<TKey, TValue> FallBackCase = new Case<TKey, TValue>();

		public OptionCollection<TKey, TValue> Options { get; } = new OptionCollection<TKey, TValue>();

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertImpl(value, targetType, SwitchConvertDirection.Reverse);
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertImpl(value, targetType, SwitchConvertDirection.Direct);
		}

		private object ConvertImpl(object value, Type targetType, SwitchConvertDirection direction)
		{
			var valueType = value?.GetType() ?? typeof(object);

			Default<TKey, TValue> defaultCase = null;

			foreach (var option in Options)
			{
				if (option is Case<TKey, TValue> caseOption)
				{
					var caseKey = GetKey(caseOption, direction, valueType);

					if (Equals(value, caseKey))
						return GetValue(caseOption, direction, targetType);

					continue;
				}

				var defaultOption = option as Default<TKey, TValue>;

				if (defaultOption != null && defaultCase != null)
					throw new Exception("SwitchConverter can have only 1 default case");

				defaultCase = defaultOption;
			}

			return defaultCase != null ? GetValue(defaultCase, direction, targetType) : FallBackCase.Value.XamlConvert(targetType);
		}

		private static object GetKey(SwitchOption<TKey, TValue> switchOption, SwitchConvertDirection direction, Type targetType)
		{
			return direction == SwitchConvertDirection.Direct ? (object) switchOption.XamlConvertKey(targetType) : switchOption.XamlConvertValue(targetType);
		}

		private static object GetValue(SwitchOption<TKey, TValue> switchOption, SwitchConvertDirection direction, Type targetType)
		{
			return direction == SwitchConvertDirection.Direct ? (object) switchOption.XamlConvertValue(targetType) : switchOption.XamlConvertKey(targetType);
		}

		private enum SwitchConvertDirection
		{
			Direct,
			Reverse
		}
	}

	[ContentProperty("Value")]
	public abstract class SwitchOption
	{
		private XamlConvertCacheStruct _valueCache;

		public object Value
		{
			get => _valueCache.Value;
			set => _valueCache.Value = value;
		}

		internal abstract object XamlConvertKey(Type targetType);

		internal object XamlConvertValue(Type targetType)
		{
			return _valueCache.XamlConvert(targetType);
		}
	}

	public class Case : SwitchOption
	{
		private XamlConvertCacheStruct _keyCache;

		public object Key
		{
			get => _keyCache.Value;
			set => _keyCache.Value = value;
		}

		internal override object XamlConvertKey(Type targetType)
		{
			return _keyCache.XamlConvert(targetType);
		}
	}

	public class Default : SwitchOption
	{
		private XamlConvertCacheStruct _keyCache = new XamlConvertCacheStruct {Value = default};

		internal override object XamlConvertKey(Type targetType)
		{
			return _keyCache.XamlConvert(targetType);
		}
	}

	public sealed class OptionCollection : CollectionBase<SwitchOption>
	{
	}

	[ContentProperty("Options")]
	public class SwitchConverter : BaseValueConverter
	{
		private static readonly SwitchOption FallBackCase = new Case();

		public OptionCollection Options { get; } = new OptionCollection();

		protected override object ConvertBackCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertImpl(value, targetType, SwitchConvertDirection.Reverse);
		}

		protected override object ConvertCore(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertImpl(value, targetType, SwitchConvertDirection.Direct);
		}

		private object ConvertImpl(object value, Type targetType, SwitchConvertDirection direction)
		{
			var valueType = value?.GetType() ?? typeof(object);

			Default defaultCase = null;

			foreach (var option in Options)
			{
				if (option is Case caseOption)
				{
					var caseKey = GetKey(caseOption, direction, valueType);

					if (Equals(value, caseKey))
						return GetValue(caseOption, direction, targetType);

					continue;
				}

				var defaultOption = option as Default;

				if (defaultOption != null && defaultCase != null)
					throw new Exception("SwitchConverter can have only 1 default case");

				defaultCase = defaultOption;
			}

			return defaultCase != null ? GetValue(defaultCase, direction, targetType) : FallBackCase.Value.XamlConvert(targetType);
		}

		private static object GetKey(SwitchOption switchOption, SwitchConvertDirection direction, Type targetType)
		{
			return direction == SwitchConvertDirection.Direct ? switchOption.XamlConvertKey(targetType) : switchOption.XamlConvertValue(targetType);
		}

		private static object GetValue(SwitchOption switchOption, SwitchConvertDirection direction, Type targetType)
		{
			return direction == SwitchConvertDirection.Direct ? switchOption.XamlConvertValue(targetType) : switchOption.XamlConvertKey(targetType);
		}

		private enum SwitchConvertDirection
		{
			Direct,
			Reverse
		}
	}
}