// <copyright file="ResourceDictionaryExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Extensions
{
  internal static class ResourceDictionaryExtensions
  {
    #region  Methods

    public static ThemeResource CreateThemeResource(this ResourceDictionary resourceDictionary, string resourceKey)
    {
      var resource = resourceDictionary[resourceKey];
      var themeResource = new ThemeResource
      {
        Key = resourceKey,
        Value = resource
      };

      return themeResource;
    }

    public static void SetResourceValue(this ResourceDictionary resourceDictionary, object key, object value)
    {
#if SILVERLIGHT
      if (resourceDictionary.Contains(key))
      {
        resourceDictionary.Remove(key);
        resourceDictionary.Add(key, value);
      }
      else
      {
        resourceDictionary.Add(key, value);
      }
#else
      resourceDictionary[key] = value;
#endif
    }

    public static IEnumerable<ResourceDictionary> EnumerateDictionaries(this ResourceDictionary resourceDictionary)
    {
      return ResourceDictionaryUtils.EnumerateDictionaries(resourceDictionary);
    }

    public static IEnumerable<Uri> EnumerateReferencedDictionaries(this ResourceDictionary resourceDictionary, Uri baseUri)
    {
      return ResourceDictionaryUtils.EnumerateReferencedDictionaries(resourceDictionary, baseUri);
    }

#endregion
  }
}