// <copyright file="SkinnedTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace Zaaml.PresentationCore.Theming
{
	public abstract class SkinnedTheme : Theme
	{
		#region Ctors

		protected SkinnedTheme()
		{
			LazyStaticTheme = new Lazy<StaticTheme>(() => new StaticTheme(this));
			SkinResourceManager = new SkinResourceManager(this);
		}

		#endregion

		#region Properties

		internal Theme BaseTheme => BaseThemeCore;

		protected abstract Theme BaseThemeCore { get; }

		private bool IsChangingSkin { get; set; }

		private Lazy<StaticTheme> LazyStaticTheme { get; }

		public override Theme MasterTheme => BaseTheme.MasterTheme;

		protected abstract string SkinName { get; }

		private SkinResourceManager SkinResourceManager { get; }

		public StaticTheme Static => LazyStaticTheme.Value;

		protected internal sealed override ThemeResourceDictionary ThemeResourceDictionary => BaseThemeCore.ThemeResourceDictionary;

		#endregion

		#region  Methods

		internal void AddThemeResourceInternal(ThemeResource themeResource)
		{
			SkinResourceManager.AddThemeResource(themeResource);
		}

		private static void BindThemeResource(ThemeResourceReference themeResourceReference, ThemeResource themeResource)
		{
			if (themeResource != null)
				themeResourceReference?.Bind(themeResource.Value);
		}

		private static void BindThemeResource(ThemeResource themeResource)
		{
			BindThemeResource(ThemeManager.GetThemeReference(themeResource.ActualKey, false), themeResource);
		}

		protected internal override void BindThemeResource(ThemeResourceReference thereResourceReference)
		{
			BindThemeResource(thereResourceReference, GetResource(thereResourceReference.Key));
		}

		internal void BindThemeResourceInternal(ThemeResource themeResource)
		{
			if (IsApplied)
				BindThemeResource(themeResource);
		}

		internal static void ChangeSkin(SkinnedTheme from, SkinnedTheme to)
		{
			from.IsChangingSkin = true;
			to.IsChangingSkin = true;

			from.IsApplied = false;
			to.IsApplied = true;

			from.IsChangingSkin = false;
			to.IsChangingSkin = false;
		}

		protected internal override IEnumerable<ThemeResource> EnumerateResources()
		{
			return SkinResourceManager.EnumerateResources();
		}

		protected internal override ThemeResource GetResource(string key)
		{
			return SkinResourceManager.GetResource(key);
		}

		internal override bool IsThemeResource(XamlResourceInfo resource)
		{
			return resource.ThemeType == GetType() || resource.ThemeType == BaseThemeCore?.GetType();
		}

		protected sealed override void OnApplied()
		{
			LoadXamlResources(ThemeResourceDictionaryLoader.Instance.XamlResources, false);
			ProcessPendingResources();

			if (IsChangingSkin == false)
				BaseThemeCore.IsApplied = true;
		}

		protected sealed override void OnUnapplied()
		{
			if (IsChangingSkin == false)
				BaseThemeCore.IsApplied = false;
		}

		private void ProcessSkinResourceDictionary(ThemeSkinResourceDictionary resourceDictionary)
		{
			if (resourceDictionary.Name != SkinName)
				return;

			SkinResourceManager.LoadResources(resourceDictionary);
		}

		internal override void ProcessXamlResource(XamlResourceInfo resource)
		{
			if (resource.IsDeferred)
				return;

			var resourceDictionary = resource.EnsureResourceDictionary();

			if (resourceDictionary == null)
				return;

			if (ProcessedResourceDictionaries.Contains(resourceDictionary))
				return;

			var skinResourceDictionary = resource.ResourceDictionary as ThemeSkinResourceDictionary;

			if (skinResourceDictionary != null)
				ProcessSkinResourceDictionary(skinResourceDictionary);
		}

		internal override bool ShouldProcessXamlResource(XamlResourceInfo resource)
		{
			return base.ShouldProcessXamlResource(resource) || MasterTheme.ShouldProcessXamlResource(resource);
		}

		#endregion
	}
}