// <copyright file="ResourceDictionaryUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using Zaaml.Core;
using Zaaml.Core.Extensions;

#if !SILVERLIGHT
using System.Windows.Baml2006;
using System.Xaml;
#endif


namespace Zaaml.PresentationCore.Utils
{
  internal static class ResourceDictionaryUtils
  {
    #region Static Fields and Constants

    public static readonly Uri PackUri;

    #endregion

    #region Ctors

    static ResourceDictionaryUtils()
    {
      if (PresentationCoreUtils.IsInDesignMode)
        PackUri = new Uri("pack://application:,,,/", UriKind.Absolute);
      else
      {
#if SILVERLIGHT
        PackUri = new Uri("pack://", UriKind.Absolute);
#else
        PackUri = new Uri("pack://application:,,,/", UriKind.Absolute);
#endif
      }
    }

    #endregion

    #region  Methods

    public static ResourceDictionary CloneResources(ResourceDictionary resources)
    {
      var copy = new ResourceDictionary();

      foreach (var key in resources.Keys.OfType<object>().ToList())
        copy.Add(key, resources[key]);

      return copy;
    }

    public static IEnumerable<ResourceDictionary> EnumerateDictionaries(ResourceDictionary resourceDictionary)
    {
      if (resourceDictionary == null)
        yield break;

      var sourceHash = new HashSet<string>();
      var queue = new Queue<ResourceDictionary>();

      queue.Enqueue(resourceDictionary);

      do
      {
        var current = queue.Dequeue();

        yield return current;

        foreach (var child in current.MergedDictionaries.Where(r => ShouldEnumerate(sourceHash, r)))
        {
          queue.Enqueue(child);
          sourceHash.Add(child.Source?.ToString());
        }
      } while (queue.Count > 0);
    }

    internal static IEnumerable<Uri> EnumerateReferencedDictionaries(ResourceDictionary resourceDictionary, Uri baseUri)
    {
      if (resourceDictionary == null)
        yield break;

      var absoluteUri = new Uri("pack://application/", UriKind.Absolute);

      var sourceHash = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var queue = new Queue<Tuple<ResourceDictionary, Uri>>();
      queue.Enqueue(Tuple.Create(resourceDictionary, baseUri));
      do
      {
        var current = queue.Dequeue();
        var currentDictionary = current.Item1;
        var currentBase = current.Item2;

        if (ReferenceEquals(currentDictionary, resourceDictionary) == false && sourceHash.Contains(currentBase.OriginalString) == false)
        {
          sourceHash.Add(currentBase.OriginalString);
          yield return currentBase;
        }

        foreach (var child in currentDictionary.MergedDictionaries.Where(r => r.Source != null))
        {
          var childUri = child.Source;
          if (childUri.IsAbsoluteUri == false)
          {
            var currentAbsolute = new Uri(absoluteUri, currentBase);
            var childAbsolute = new Uri(currentAbsolute, childUri);

            childUri = absoluteUri.MakeRelativeUri(childAbsolute);
          }

          queue.Enqueue(Tuple.Create(child, childUri));
        }
      } while (queue.Count > 0);
    }

    private static bool ShouldEnumerate(HashSet<string> sourceHash, ResourceDictionary resourceDictionary)
    {
      var uriString = resourceDictionary.Source?.ToString();
      return string.IsNullOrEmpty(uriString) || sourceHash.Contains(uriString) == false;
    }

    public static Uri CreateAbsoluteUri(Uri resourceDictionarySource)
    {
      if (resourceDictionarySource.IsAbsoluteUri)
        throw new InvalidOperationException();

      return new Uri(PackUri, resourceDictionarySource);
    }

    public static Uri CreateRelativeUri(Uri resourceDictionarySource, ResourceDictionary mergedResourceDictionary)
    {
      var currentAbsolute = new Uri(PackUri, resourceDictionarySource);
      return PackUri.MakeRelativeUri(new Uri(currentAbsolute, mergedResourceDictionary.Source.OriginalString));
    }

    public static Uri CreateRelativeUri(ResourceDictionary resourceDictionary, ResourceDictionary mergedResourceDictionary)
    {
      return CreateRelativeUri(resourceDictionary.Source, mergedResourceDictionary);
    }

    public static ResourceDictionary LoadResourceDictionary(Uri uri)
    {
      try
      {
        return LoadResourceDictionaryUnsafe(uri);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }

      return null;
    }

    private static readonly Dictionary<string, ResourceDictionary> EmptyResourceDictionaryCache = new Dictionary<string, ResourceDictionary>(StringComparer.OrdinalIgnoreCase);

#if SILVERLIGHT
    private static ResourceDictionary LoadResourceDictionaryUnsafe(Uri uri)
    {
      uri = RebuildBamlUri(uri);

      var streamInfo = Application.GetResourceStream(uri);

      if (streamInfo == null)
        return null;

      var resourceDictionary = GetEmptyResourceDictionaryInstance(uri);

      if (resourceDictionary != null)
        Application.LoadComponent(resourceDictionary, uri);

      return resourceDictionary;
    }
#else
    private static ResourceDictionary LoadResourceDictionaryUnsafe(Uri uri)
    {
      return Application.LoadComponent(RebuildBamlUri(uri)) as ResourceDictionary;
    }
#endif

    private static readonly Dictionary<XName, Func<ResourceDictionary>> ResourceDictionaryFactoryMap = new Dictionary<XName, Func<ResourceDictionary>>();

    private static Func<ResourceDictionary> CreateResourceDictionaryFactory(XName name)
    {
      try
      {
        var rootElement = new XElement(name);
        var resourceDictionary = XamlUtils.Load(rootElement.ToString()) as ResourceDictionary;
        if (resourceDictionary == null)
          return () => null;

        return () => (ResourceDictionary) Activator.CreateInstance(resourceDictionary.GetType());
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }

      return () => null;
    }

    public static bool IsXamlOrBamlResource(Uri uri)
    {
      return IsXamlResource(uri) || IsBamlResource(uri);
    }

    public static bool IsXamlResource(Uri uri)
    {
      return uri.OriginalString.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsBamlResource(Uri uri)
    {
      return uri.OriginalString.EndsWith(".baml", StringComparison.OrdinalIgnoreCase);
    }

    public static Uri RebuildBamlUri(Uri uri)
    {
      var path = uri.OriginalString;
      return path.EndsWith(".baml", StringComparison.OrdinalIgnoreCase) ? new Uri(path.Substring(0, path.Length - 5) + ".xaml", uri.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative) : uri;
    }

    private static ResourceDictionary GetResourceDictionary(XName name)
    {
      return ResourceDictionaryFactoryMap.GetValueOrCreate(name, CreateResourceDictionaryFactory)();
    }

    public static ResourceDictionary GetEmptyResourceDictionaryInstance(Uri uri)
    {
      return EmptyResourceDictionaryCache.GetValueOrCreate(uri.OriginalString, () => UpdateActualUri(CreateEmptyResourceDictionaryInstance(uri), uri));
    }

    public static Uri GetActualUri(ResourceDictionary resourceDictionary)
    {
      var actualUriSource = resourceDictionary as IActualUriSource;
      return actualUriSource?.ActualSource ?? resourceDictionary.Source;
    }

    private static ResourceDictionary UpdateActualUri(ResourceDictionary resourceDictionary, Uri uri)
    {
      var uriSource = resourceDictionary as IActualUriSource;
      if (uriSource != null)
        uriSource.ActualSource = uri;

      return resourceDictionary;
    }

    public static object TryGetValue(ResourceDictionary resourceDictionary, object key)
    {
#if SILVERLIGHT
      return resourceDictionary.Contains(key) ? resourceDictionary[key] : null;
#else
      return resourceDictionary.GetSetValueOrDefault(key);
#endif
    }

    public static T TryGetValue<T>(ResourceDictionary resourceDictionary, object key)
    {
      var value = TryGetValue(resourceDictionary, key);

      if (value is T)
        return (T) value;

      return default(T);
    }

    private static ResourceDictionary CreateEmptyResourceDictionaryInstance(Uri uri)
    {
      try
      {
        var streamInfo = Application.GetResourceStream(RebuildBamlUri(uri));

        if (streamInfo == null)
          return null;

#if !SILVERLIGHT
        if (IsBamlResource(uri))
        {
          using (var bamlReader = new Baml2006Reader(streamInfo.Stream))
          {
            while (bamlReader.Read())
            {
              if (bamlReader.NodeType != XamlNodeType.StartObject)
                continue;

              if (bamlReader.Type == null)
                return null;

              var objectType = bamlReader.Type.UnderlyingType;
              if (objectType == typeof(ResourceDictionary) || objectType.IsSubclassOf(typeof(ResourceDictionary)))
                return (ResourceDictionary) Activator.CreateInstance(objectType);
            }
          }
          return null;
        }
#endif

        using (var streamReader = new StreamReader(streamInfo.Stream, Encoding.UTF8))
          return GetResourceDictionary(XElement.Parse(streamReader.ReadToEnd()).Name);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }

      return null;
    }
#endregion
  }

  internal interface IActualUriSource
  {
#region Properties

    Uri ActualSource { get; set; }

#endregion
  }
}