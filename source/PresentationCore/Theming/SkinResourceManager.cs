// <copyright file="SkinResourceManager.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Theming
{
	internal sealed class SkinResourceManager
	{
		private readonly SkinnedTheme _theme;
		private readonly Dictionary<string, ThemeResource> _themeResources = new(StringComparer.OrdinalIgnoreCase);

		public SkinResourceManager()
		{
		}

		public SkinResourceManager(SkinnedTheme theme) : this()
		{
			_theme = theme;
		}

		private SkinDictionary Root { get; } = new();

		public HashSet<string> UnresolvedDependencies
		{
			get
			{
				var dependencies = new HashSet<string>();

				TreeEnumerator.Visit(Root, SkinDictionary.SkinDictionaryTreeAdvisor, s => dependencies.AddRange(s.BasedOn.Where(d => d.IsDeferred).Select(d => d.DeferredKey)));

				return dependencies;
			}
		}

		public void AddThemeResource(ThemeResource themeResource)
		{
			AddThemeResource(new KeyValuePair<string, object>(themeResource.ActualKey, themeResource.Value));
		}

		private void AddThemeResource(KeyValuePair<string, object> themeResourceKeyValuePair)
		{
			var key = themeResourceKeyValuePair.Key;
			var value = themeResourceKeyValuePair.Value;

			if (value is ThemeResource themeResourceValue)
				value = themeResourceValue.Value;

			if (value is ThemeKeywordGroup themeKeywordGroup)
				foreach (var themeKeyword in themeKeywordGroup)
					AddThemeResource(new KeyValuePair<string, object>(themeKeyword.ActualKey, themeKeyword.Value));

			var themeResource = EnsureThemeResource(key);

			themeResource.BindValue(value);

			_themeResources[themeResource.ActualKey] = themeResource;

			_theme?.BindThemeResourceInternal(themeResource);
		}

		private ThemeResource EnsureThemeResource(string key)
		{
			return new(this) {Key = key};
		}

		public IEnumerable<ThemeResource> EnumerateResources()
		{
			return _themeResources.Values;
		}

		private static IEnumerable<KeyValuePair<string, object>> EnumerateResources(ResourceDictionary resourceDictionary)
		{
			foreach (var rd in resourceDictionary.EnumerateDictionaries())
			{
				foreach (var keyObj in rd.Keys)
				{
					var key = keyObj as string;

					if (key == null)
						continue;

					var value = rd[keyObj];

					yield return new KeyValuePair<string, object>(key, value);
				}
			}
		}

		public ThemeResource GetResource(string key)
		{
			return _themeResources.GetValueOrDefault(key);
		}

		public void LoadResources(ThemeSkinResourceDictionary resourceDictionary)
		{
			LoadResourcesImpl(EnumerateResources(resourceDictionary));
		}

		private void LoadResourcesImpl(IEnumerable<KeyValuePair<string, object>> themeResources)
		{
			foreach (var themeResource in themeResources)
			{
				if (themeResource.Value is not SkinDictionary)
				{
					Root.Merge(themeResource, SkinDictionaryMergeFlags.Override);

					AddThemeResource(themeResource);
				}
				else
					Root.Merge(themeResource, SkinDictionaryMergeFlags.Default);
			}

			if (Root.ResolveDependencies(Root) == false)
				return;

			var frozenRoot = Root.AsFrozen();

			foreach (var themeResourceKeyValuePair in frozenRoot.Flatten().Select(UnwrapValue))
				AddThemeResource(themeResourceKeyValuePair);
		}

		private static KeyValuePair<string, object> UnwrapValue(KeyValuePair<string, object> keyValuePair)
		{
			return keyValuePair.Value is ThemeResource themeResourceValue ? keyValuePair.WithValue(themeResourceValue.Value) : keyValuePair;
		}
	}
}