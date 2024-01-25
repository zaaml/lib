// <copyright file="Theme.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	public partial class Theme
	{
		private readonly Dictionary<Type, ThemeKey> _themeKeys = new();

		private ThemeKey CreateThemeKey(Type elementType)
		{
			var masterTheme = MasterTheme;

			if (ReferenceEquals(this, masterTheme) == false)
				return masterTheme.CreateThemeKey(elementType);

			var themeStyle = _themeStyles.GetValueOrDefault(elementType);

			if (themeStyle == null)
			{
				ThemeResourceDictionaryLoader.Instance.EnsureThemePartLoaded(elementType.Assembly);

				themeStyle = _themeStyles.GetValueOrDefault(elementType);
			}

			if (themeStyle == null)
				return null;

			if (themeStyle.IsDeferred)
			{
				themeStyle.EnsureDeferredStylesLoaded();

				themeStyle = _themeStyles.GetValueOrDefault(elementType);

				if (themeStyle == null)
					return null;
			}

			if (ThemeManager.EnableThemeStyle(elementType, themeStyle.TargetType) == false)
				return null;

			var themeKey = new ThemeKey(elementType, this, themeStyle);

			foreach (var genericDictionary in _genericDictionaries)
				RegisterGenericThemeKey(genericDictionary, themeKey);

			return themeKey;
		}

		public ThemeKey GetThemeKey(Type elementType)
		{
			var masterTheme = MasterTheme;

			if (ReferenceEquals(this, masterTheme) == false)
				return masterTheme.GetThemeKey(elementType);

			return _themeKeys.GetValueOrCreate(elementType, CreateThemeKey);
		}

		internal virtual Assembly GetThemeKeyAssembly(ThemeKey themeKey)
		{
			return themeKey.Theme.GetType().Assembly;
		}

		partial void PlatformOnGenericDictionaryRegistered(GenericResourceDictionary genericDictionary)
		{
			foreach (var themeKey in _themeKeys.Values.SkipNull())
				RegisterGenericThemeKey(genericDictionary, themeKey);
		}

		private static void RegisterGenericThemeKey(GenericResourceDictionary genericDictionary, ThemeKey themeKey)
		{
			if (themeKey.ThemeStyle != null)
				genericDictionary[themeKey] = themeKey.ThemeStyle.NativeStyle;
		}
	}
}