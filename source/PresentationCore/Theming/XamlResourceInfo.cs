// <copyright file="XamlResourceInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Theming
{
	internal sealed class XamlResourceInfo
	{
		#region Static Fields and Constants

		private static readonly ResourceDictionary UnsetEmptyResourceDictionary = new ResourceDictionary();

		#endregion

		#region Fields

		public ResourceDictionary DeferredResourceDictionary;
		public ResourceDictionary ResourceDictionary;

		#endregion

		#region Ctors

		public XamlResourceInfo(Assembly assembly, Type themeType, Uri uri, Func<Uri, ResourceDictionary> resourceLoader, ResourceDictionary deferredDictionary)
		{
			ThemeType = themeType;
			Assembly = assembly;
			Uri = uri;
			ResourceLoader = resourceLoader;

			ResourceDictionary = null;
			DeferredResourceDictionary = deferredDictionary;

			LoadError = false;
		}

		#endregion

		#region Properties

		public Assembly Assembly { get; }

		private ResourceDictionary EmptyResourceDictionaryInstance
		{
			get
			{
				if (ReferenceEquals(UnsetEmptyResourceDictionary, ResourceDictionaryEmptyInstance))
					ResourceDictionaryEmptyInstance = ResourceDictionaryUtils.GetEmptyResourceDictionaryInstance(Uri);

				return ResourceDictionaryEmptyInstance;
			}
		}

		public bool IsDeferred => DeferredResourceDictionary != null;

		public bool IsResourceDictionaryLoaded => ResourceDictionary != null;

		private bool LoadError { get; set; }

		public int Priority => GetResourceDictionaryPriority(EmptyResourceDictionaryInstance);

		private ResourceDictionary ResourceDictionaryEmptyInstance { get; set; } = UnsetEmptyResourceDictionary;

		private Func<Uri, ResourceDictionary> ResourceLoader { get; }

		public Type ThemeType { get; }

		public Uri Uri { get; }

		#endregion

		#region  Methods

		public ResourceDictionary EnsureResourceDictionary()
		{
			if (IsResourceDictionaryLoaded || LoadError)
				return ResourceDictionary;

			try
			{
				ResourceDictionary = ResourceLoader(Uri);
			}
			catch (Exception)
			{
				LoadError = true;
			}

			return ResourceDictionary;
		}

		private static int GetResourceDictionaryPriority(ResourceDictionary resourceDictionary)
		{
			if (resourceDictionary is ThemeSkinResourceDictionary)
				return 0;

			if (resourceDictionary is ThemeResourceDictionary)
				return 1;

			return 2;
		}

		public override string ToString()
		{
			return Uri.ToString();
		}

		#endregion
	}
}