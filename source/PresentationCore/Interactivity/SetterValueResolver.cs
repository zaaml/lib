// <copyright file="ValueResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class SetterValueResolver
	{
		public static void CopyFrom(IValueSetter target, IValueSetter source)
		{
			target.ValueKind = source.ValueKind;
			target.ValueStore = source.ValueStore;
		}

		private static object GetOriginalValue(IValueSetter setter)
		{
			var valueStore = setter.ValueStore;

			if (IsResolvedValueProvider(setter.ValueKind) == false)
				return valueStore;

			return valueStore is ISetterValueProvider valueProvider ? valueProvider.OriginalValue : valueStore;
		}

		public static object GetValue(IValueSetter setter)
		{
			var valueKind = setter.ValueKind & SetterValueKind.Unspecified;

			return valueKind == SetterValueKind.Explicit ? GetOriginalValue(setter) : null;
		}

		public static string GetValuePath(IValueSetter setter)
		{
			var valueKind = setter.ValueKind & SetterValueKind.Unspecified;

			if (valueKind != SetterValueKind.Explicit)
				return (string)GetOriginalValue(setter);

			return null;
		}

		public static ValuePathSource GetValuePathSource(IValueSetter setter)
		{
			var valueKind = setter.ValueKind & SetterValueKind.Unspecified;

			switch (valueKind)
			{
				case SetterValueKind.TemplateExpandoPath:
					return ValuePathSource.TemplateExpando;
				case SetterValueKind.SelfExpandoPath:
					return ValuePathSource.Expando;
				case SetterValueKind.SkinPath:
					return ValuePathSource.Skin;
				case SetterValueKind.TemplateSkinPath:
					return ValuePathSource.TemplateSkin;
				case SetterValueKind.ThemeResourcePath:
					return ValuePathSource.ThemeResource;
				default:
					return ValuePathSource.ThemeResource;
			}
		}

		private static bool IsResolvedValueProvider(SetterValueKind valueKind)
		{
			return (valueKind & SetterValueKind.Resolved) != 0;
		}

		public static bool IsSpecified(IValueSetter setter)
		{
			return (setter.ValueKind & SetterValueKind.Unspecified) != SetterValueKind.Unspecified;
		}

		public static bool IsSpecifiedValuePath(IValueSetter setter)
		{
			var valueKind = setter.ValueKind & SetterValueKind.Unspecified;

			return valueKind != SetterValueKind.Unspecified && valueKind != SetterValueKind.ValuePath;
		}

		public static ISetterValueProvider ResolveValueProvider(IValueSetter setter)
		{
			var valueProvider = setter.ValueStore as ISetterValueProvider;

			if (valueProvider != null)
				return valueProvider;

			var valueKind = setter.ValueKind;

			if (IsResolvedValueProvider(valueKind))
				return null;

			try
			{
				if (valueKind == SetterValueKind.ThemeResourcePath)
				{
					var actualValuePath = GetValuePath(setter);

					if (string.IsNullOrEmpty(actualValuePath) == false)
					{
						valueProvider = ThemeManager.GetThemeResourceReference(actualValuePath);
						setter.ValueStore = valueProvider;

						return valueProvider;
					}
				}

				if (valueKind == SetterValueKind.Explicit)
				{
					var actualValue = GetValue(setter);
					var value = actualValue;

					if (value is ThemeResourceExtension themeResourceExtension)
						valueProvider = themeResourceExtension;
				}
			}
			finally
			{
				if (valueProvider != null)
					setter.ValueStore = valueProvider;

				setter.ValueKind |= SetterValueKind.Resolved;
			}

			return valueProvider;
		}

		public static void SetValue(IValueSetter setter, object value)
		{
			UnresolveValueProvider(setter);

			setter.ValueStore = value;
			setter.ValueKind = SetterValueKind.Explicit;
		}

		public static void SetValuePath(IValueSetter setter, string value)
		{
			UnresolveValueProvider(setter);

			var valueKind = setter.ValueKind & SetterValueKind.Unspecified;

			if (valueKind != SetterValueKind.SelfExpandoPath
			    && valueKind != SetterValueKind.TemplateExpandoPath
			    && valueKind != SetterValueKind.ThemeResourcePath
			    && valueKind != SetterValueKind.SkinPath
			    && valueKind != SetterValueKind.TemplateSkinPath)
				setter.ValueKind = SetterValueKind.ValuePath;

			setter.ValueStore = value;
		}

		public static void SetValuePathSource(IValueSetter setter, ValuePathSource value)
		{
			UnresolveValueProvider(setter);

			if (value == ValuePathSource.ThemeResource)
				setter.ValueKind = SetterValueKind.ThemeResourcePath;
			else if (value == ValuePathSource.Expando)
				setter.ValueKind = SetterValueKind.SelfExpandoPath;
			else if (value == ValuePathSource.TemplateExpando)
				setter.ValueKind = SetterValueKind.TemplateExpandoPath;
			else if (value == ValuePathSource.Skin)
				setter.ValueKind = SetterValueKind.SkinPath;
			else if (value == ValuePathSource.TemplateSkin)
				setter.ValueKind = SetterValueKind.TemplateSkinPath;
			else
				setter.ValueKind = SetterValueKind.ThemeResourcePath;
		}

		public static void UnresolveValueProvider(IValueSetter setter)
		{
			var valueKind = setter.ValueKind;

			if (IsResolvedValueProvider(valueKind) == false)
				return;

			setter.ValueKind = valueKind & SetterValueKind.Unspecified;

			var valueStore = setter.ValueStore;

			if (valueStore is ISetterValueProvider valueProvider)
				setter.ValueStore = valueProvider.OriginalValue;
		}
	}
}