// <copyright file="StaticTheme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
#if !SILVERLIGHT
using System.Reflection;

#endif

namespace Zaaml.PresentationCore.Theming
{
	public sealed class StaticTheme : Theme
	{
		#region Ctors

		internal StaticTheme(SkinnedTheme skinnedTheme)
		{
			SkinnedTheme = skinnedTheme;
		}

		#endregion

		#region Properties

		public override Theme MasterTheme => SkinnedTheme.MasterTheme;

		public override string Name => $"{SkinnedTheme.Name}Static";

		private SkinnedTheme SkinnedTheme { get; }

		protected internal override ThemeResourceDictionary ThemeResourceDictionary => SkinnedTheme.ThemeResourceDictionary;

		#endregion

		#region  Methods

		protected internal override void BindThemeResource(ThemeResourceReference themeResourceReference)
		{
			SkinnedTheme.BindThemeResource(themeResourceReference);
		}

		protected internal override IEnumerable<ThemeResource> EnumerateResources()
		{
			return SkinnedTheme.EnumerateResources();
		}

		protected internal override ThemeResource GetResource(string key)
		{
			return SkinnedTheme.GetResource(key);
		}

#if !SILVERLIGHT
		internal override Assembly GetThemeKeyAssembly(ThemeKey themeKey)
		{
			return SkinnedTheme.GetThemeKeyAssembly(themeKey);
		}
#endif

		protected override void OnApplied()
		{
			SkinnedTheme.IsApplied = true;
		}

		protected override void OnUnapplied()
		{
			SkinnedTheme.IsApplied = false;
		}

		internal override void ProcessXamlResource(XamlResourceInfo resource)
		{
			SkinnedTheme.ProcessXamlResource(resource);
		}

		#endregion
	}
}