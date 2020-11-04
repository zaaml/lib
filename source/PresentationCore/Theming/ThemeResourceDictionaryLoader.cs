// <copyright file="ThemeResourceDictionaryLoader.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.XPath;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.Core.Reflection;
using Zaaml.PresentationCore.Utils;
#if !SILVERLIGHT
using System.IO.Compression;

#endif

#if SILVERLIGHT
using System.Xml.Linq;
#endif

namespace Zaaml.PresentationCore.Theming
{
	internal class ThemeResourceDictionaryLoader
	{
		private static readonly Lazy<ThemeResourceDictionaryLoader> LazyInstance = new Lazy<ThemeResourceDictionaryLoader>(() => new ThemeResourceDictionaryLoader());

		private readonly AppDomainObserver _appDomainObserver;
		private readonly Dictionary<string, ResourceDictionary> _dictionaryCache = new Dictionary<string, ResourceDictionary>(StringComparer.OrdinalIgnoreCase);
		private readonly Dictionary<ResourceDictionary, Uri> _dictionaryUriCache = new Dictionary<ResourceDictionary, Uri>();
		private readonly List<XamlResourceInfo> _xamlResources = new List<XamlResourceInfo>();
		internal event EventHandler<XamlResourceLoadingEventArgs> XamlResourceLoading;

		private ThemeResourceDictionaryLoader()
		{
			_appDomainObserver = new AppDomainObserver(LoadThemeAssemblies);
			CompositionTarget.Rendering += (sender, args) => _appDomainObserver.Update();
		}

		public static ThemeResourceDictionaryLoader Instance
		{
			get
			{
				var instance = LazyInstance.Value;

				instance.Update();

				return instance;
			}
		}

		private MultiMap<string, ThemePart> ThemeParts { get; } = new MultiMap<string, ThemePart>();

		internal List<XamlResourceInfo> XamlResources => _xamlResources;

		private ResourceDictionary CacheLoadResourceDictionary(Uri uri)
		{
			var resourceDictionary = _dictionaryCache.GetValueOrDefault(uri.OriginalString);

			if (resourceDictionary != null)
				return resourceDictionary;

			resourceDictionary = ResourceDictionaryUtils.LoadResourceDictionary(uri);

			if (resourceDictionary == null)
				return null;

			if (resourceDictionary is ResourceDictionaryBase resourceDictionaryBase)
				resourceDictionaryBase.LoadUri = uri;

			try
			{
				ResourceDictionaryTreeAdvisor.Instance.Visit(resourceDictionary, (dictionary, enumerator) =>
				{
					var parent = enumerator.Enumerate().FirstOrDefault();
					var currentUri = uri;

					if (parent != null && dictionary.Source != null)
						currentUri = ResourceDictionaryUtils.CreateRelativeUri(_dictionaryUriCache[parent], dictionary);

					CacheResourceDictionary(dictionary, currentUri);
				});
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			return resourceDictionary;
		}

		private void CacheResourceDictionary(ResourceDictionary resourceDictionary, Uri uri)
		{
			_dictionaryCache[uri.OriginalString] = resourceDictionary;
			_dictionaryUriCache[resourceDictionary] = uri;
		}

		private static bool ExcludeResource(Uri uri)
		{
			return ThemeManager.ActualBehavior.ExcludeResource(uri);
		}

		private static Uri GetNonDeferredUri(Uri uri)
		{
			if (ResourceDictionaryUtils.IsXamlResource(uri))
				return new Uri(uri.OriginalString.RemoveFromEnd(".deferred.xaml") + ".xaml", UriKind.RelativeOrAbsolute);

			if (ResourceDictionaryUtils.IsBamlResource(uri))
				return new Uri(uri.OriginalString.RemoveFromEnd(".deferred.baml") + ".baml", UriKind.RelativeOrAbsolute);

			throw new InvalidOperationException("Non Xaml or Baml resource");
		}

		public Uri GetSource(ResourceDictionary resourceDictionary)
		{
			var uri = _dictionaryUriCache.GetValueOrDefault(resourceDictionary);

			return uri != null ? ResourceDictionaryUtils.RebuildBamlUri(uri) : null;
		}

		private static bool IsDeferredResourceDictionary(Uri uri)
		{
			var path = uri.OriginalString;

			return path.EndsWith(".deferred.xaml", StringComparison.OrdinalIgnoreCase) || path.EndsWith(".deferred.baml");
		}

		internal void LoadThemeAssemblies(IEnumerable<Assembly> assemblies)
		{
			var newXamlResources = new List<XamlResourceInfo>();

			foreach (var assembly in assemblies)
			{
				try
				{
					foreach (var themePart in LoadThemeParts(assembly))
					{
						if (themePart.HasDependencies)
						{
							foreach (var themePartAssemblyDependency in themePart.Dependencies)
								ThemeParts.AddValue(themePartAssemblyDependency, themePart);
						}
						else
						{
							var themePartAssembly = themePart.EnsureThemePartAssembly();

							if (themePartAssembly != null)
								LoadThemeAssembly(themePartAssembly, newXamlResources);
						}
					}

					LoadThemeAssembly(assembly, newXamlResources);
				}
				catch (Exception e)
				{
					LogService.LogError(e);
				}
			}

			try
			{
				foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					var name = ThemePart.GetName(assembly);

					if (ThemeParts.TryGetValue(name, out var themeParts))
					{
						foreach (var themePart in themeParts.ToList())
						{
							themePart.ResolveDependency(assembly);

							if (themePart.HasDependencies == false)
							{
								var themePartAssembly = themePart.EnsureThemePartAssembly();

								if (themePartAssembly != null)
									LoadThemeAssembly(themePartAssembly, newXamlResources);

								ThemeParts.RemoveValue(name, themePart);
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LogService.LogError(e);
			}

			if (newXamlResources.Count > 0)
				OnXamlResourceLoading(new XamlResourceLoadingEventArgs(newXamlResources));
		}

		private void LoadThemeAssembly(Assembly assembly, List<XamlResourceInfo> newXamlResources)
		{
			if (assembly.HasAttribute<ThemeAssemblyAttribute>(false) == false)
				return;

			var resourceGroups = new List<ResourceGroup>();

			foreach (var themeAssemblyAttribute in assembly.GetAttributes<ThemeAssemblyAttribute>())
			{
				var resourcePath = themeAssemblyAttribute.Source;

				if (resourcePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
				{
#if !SILVERLIGHT
					var stream = assembly.GetResourceStream(resourcePath);

					using var archive = new ZipArchive(stream);

					foreach (var entry in archive.Entries)
					{
						if (!entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
							continue;

						using var entryStream = entry.Open();
						using var ms = new MemoryStream();

						entryStream.CopyTo(ms);

						var themeAssembly = Assembly.Load(ms.ToArray());

						LoadThemeAssemblies(new[] { themeAssembly });
					}
#endif
				}
				else
				{
					var themeUri = ResourceDictionaryUtils.CreateAbsoluteUri(assembly.GetResourcePathUri(resourcePath));

					resourceGroups.Add(new ResourceGroup(themeAssemblyAttribute.ThemeType, themeUri));
				}
			}

			foreach (var resourceUri in assembly.EnumerateEmbeddedResources().Select(assembly.GetResourceUri).Where(ResourceDictionaryUtils.IsXamlOrBamlResource))
			{
				var absoluteResourceUri = ResourceDictionaryUtils.CreateAbsoluteUri(resourceUri);

				foreach (var resourceGroup in resourceGroups)
				{
					if (resourceGroup.BaseUri.IsBaseOf(absoluteResourceUri) == false)
						continue;

					resourceGroup.Resources.Add(resourceUri);

					break;
				}
			}

			foreach (var resourceGroup in resourceGroups)
			{
				var deferredDictionary = new Dictionary<string, Uri>(StringComparer.OrdinalIgnoreCase);

				foreach (var uri in resourceGroup.Resources.WhereNot(ExcludeResource))
				{
					if (IsDeferredResourceDictionary(uri))
						deferredDictionary[GetNonDeferredUri(uri).OriginalString] = uri;
				}

				foreach (var uri in resourceGroup.Resources.WhereNot(ExcludeResource))
				{
					if (IsDeferredResourceDictionary(uri))
						continue;

					var deferredUri = deferredDictionary.GetValueOrDefault(uri.OriginalString);

					var deferredResourceDictionary = deferredUri != null ? UpdateResourceDictionaryMetadata(ResourceDictionaryUtils.LoadResourceDictionary(deferredUri), deferredUri, true) : null;
					var xamlResourceInfo = new XamlResourceInfo(assembly, resourceGroup.ThemeType, uri, CacheLoadResourceDictionary, deferredResourceDictionary);

					newXamlResources.Add(xamlResourceInfo);
					_xamlResources.Add(xamlResourceInfo);
				}
			}
		}

		private static IEnumerable<ThemePart> LoadThemeParts(Assembly assembly)
		{
			if (assembly.HasAttribute<ThemeAssemblyAttribute>(false) == false)
				yield break;

			var themeParts = new Dictionary<string, Tuple<string, string>>();

			foreach (var resourceUri in assembly.EnumerateEmbeddedResources())
			{
				if (resourceUri.EndsWith(".themepart.xml", StringComparison.OrdinalIgnoreCase))
				{
					var themePartName = resourceUri.LeftOf(".themepart", StringComparison.OrdinalIgnoreCase);
					var themePartTuple = themeParts.GetValueOrCreate(themePartName, n => new Tuple<string, string>(resourceUri, null));

					themeParts[themePartName] = new Tuple<string, string>(resourceUri, themePartTuple.Item2);
				}
				else if (resourceUri.EndsWith(".themepart.zip", StringComparison.OrdinalIgnoreCase))
				{
					var themePartName = resourceUri.LeftOf(".themepart", StringComparison.OrdinalIgnoreCase);
					var themePartTuple = themeParts.GetValueOrCreate(themePartName, n => new Tuple<string, string>(null, resourceUri));

					themeParts[themePartName] = new Tuple<string, string>(themePartTuple.Item1, resourceUri);
				}
			}

			foreach (var kv in themeParts)
			{
				var themePart = new ThemePart(kv.Key, assembly, kv.Value.Item1, kv.Value.Item2);

				yield return themePart;
			}
		}

		private void OnXamlResourceLoading(XamlResourceLoadingEventArgs e)
		{
			XamlResourceLoading?.Invoke(this, e);
		}

		private void Update()
		{
			_appDomainObserver.Update();
		}

		internal ResourceDictionary UpdateResourceDictionaryMetadata(ResourceDictionary resourceDictionary, Uri source, bool isDeferred)
		{
			CacheResourceDictionary(resourceDictionary, source);

			if (!(resourceDictionary is ThemeResourceDictionary themeResourceDictionary))
				return resourceDictionary;

			themeResourceDictionary.IsDeferred = isDeferred;

			return resourceDictionary;
		}

		private sealed class ThemePart
		{
			private bool _loaded;

			private Assembly _themePartAssembly;

			public ThemePart(string name, Assembly sourceAssembly, string infoUri, string zipUri)
			{
				Name = name;
				SourceAssembly = sourceAssembly;
				InfoUri = infoUri;
				ZipUri = zipUri;

				var infoElement = XDocument.Load(sourceAssembly.GetResourceStream(infoUri));

				Dependencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

				foreach (var assemblyDependencyElement in infoElement.XPathSelectElements("/ThemePart/AssemblyDependency"))
				{
					var assemblyName = assemblyDependencyElement.Attribute("Name")?.Value;

					if (assemblyName != null)
						Dependencies.Add(assemblyName);
				}
			}

			public static string GetName(Assembly assembly)
			{
				return assembly.GetName().Name;
			}

			public HashSet<string> Dependencies { get; }

			public bool HasDependencies => Dependencies.Count > 0;

			public string InfoUri { get; }

			public string Name { get; }

			public Assembly SourceAssembly { get; }

			public string ZipUri { get; }

			public Assembly EnsureThemePartAssembly()
			{
				if (_loaded == false)
				{
					_loaded = true;
					_themePartAssembly = LoadThemePartAssembly();
				}

				return _themePartAssembly;
			}

			private Assembly LoadThemePartAssembly()
			{
				try
				{
					var stream = SourceAssembly.GetResourceStream(ZipUri);
					using var archive = new ZipArchive(stream);

					foreach (var entry in archive.Entries)
					{
						if (!entry.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
							continue;

						using var entryStream = entry.Open();
						using var ms = new MemoryStream();

						entryStream.CopyTo(ms);

						return Assembly.Load(ms.ToArray());
					}

					return null;
				}
				catch (Exception e)
				{
					LogService.LogError(e);

					return null;
				}
			}

			public void ResolveDependency(Assembly assembly)
			{
				Dependencies.Remove(GetName(assembly));
			}
		}

		private readonly struct ResourceGroup
		{
			public ResourceGroup(Type themeType, Uri baseUri) : this()
			{
				BaseUri = baseUri;
				ThemeType = themeType;
				Resources = new List<Uri>();
			}

			public Uri BaseUri { get; }
			public Type ThemeType { get; }
			public List<Uri> Resources { get; }
		}

		public void EnsureThemePartLoaded(Assembly assembly)
		{
			var name = ThemePart.GetName(assembly);

			if (ThemeParts.TryGetValue(name, out var themeParts) == false) 
				return;

			var newXamlResources = new List<XamlResourceInfo>();

			foreach (var themePart in themeParts.ToList())
			{
				themePart.ResolveDependency(assembly);

				if (themePart.HasDependencies == false)
				{
					var themePartAssembly = themePart.EnsureThemePartAssembly();

					if (themePartAssembly != null)
						LoadThemeAssembly(themePartAssembly, newXamlResources);

					ThemeParts.RemoveValue(name, themePart);
				}
			}

			if (newXamlResources.Count > 0)
				OnXamlResourceLoading(new XamlResourceLoadingEventArgs(newXamlResources));
		}
	}
}