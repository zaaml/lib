// <copyright file="Theme.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Collections;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Theming
{
  public abstract partial class Theme
  {
    #region Static Fields and Constants

    private static readonly Dictionary<Type, Theme> Themes = new Dictionary<Type, Theme>();

    private static readonly MultiMap<Type, GenericResourceDictionary> ThemeGenericDictionaries = new MultiMap<Type, GenericResourceDictionary>();

    #endregion

    #region Fields

    private readonly Dictionary<string, XamlResourceInfo> _deferredResources = new Dictionary<string, XamlResourceInfo>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<ThemeStyle, XamlResourceInfo> _deferredStylesDictionary = new Dictionary<ThemeStyle, XamlResourceInfo>();
    private readonly List<GenericResourceDictionary> _genericDictionaries = new List<GenericResourceDictionary>();
    private readonly Dictionary<Type, ThemeStyle> _themeStyles = new Dictionary<Type, ThemeStyle>();
    private readonly List<XamlResourceInfo> _unprocessedResources = new List<XamlResourceInfo>();
    protected readonly HashSet<ResourceDictionary> ProcessedResourceDictionaries = new HashSet<ResourceDictionary>();
    internal readonly WeakLinkedList<ThemeResourceDictionary> ThemeResourceDictionaries = new WeakLinkedList<ThemeResourceDictionary>();

    private bool _isApplied;
    private bool _suspendThemeBinding;

    internal bool UseDeferredProcessing = false;

    #endregion

    #region Ctors

    protected Theme()
    {
      var themeType = GetType();

      if (Themes.ContainsKey(themeType))
        throw new Exception("Theme must be a Singleton");

      var genericDictionaries = ThemeGenericDictionaries.GetValueOrDefault(themeType);

      if (genericDictionaries != null)
        _genericDictionaries.AddRange(genericDictionaries);

      Themes[themeType] = this;

      var themeResourceDictionaryLoader = ThemeResourceDictionaryLoader.Instance;

      themeResourceDictionaryLoader.XamlResourceLoading += OnXamlResourceLoading;
    }

    #endregion

    #region Properties

    protected internal bool IsApplied
    {
      get => _isApplied;
      set
      {
        if (_isApplied == value)
          return;

        _isApplied = value;

        if (_isApplied)
          OnApplied();
        else
          OnUnapplied();
      }
    }

    public bool IsStatic => this is StaticTheme;

    public virtual Theme MasterTheme => this;

    public abstract string Name { get; }

    protected internal virtual ThemeResourceDictionary ThemeResourceDictionary { get; } = new ThemeResourceDictionary();

    #endregion

    #region  Methods

    protected internal virtual void BindThemeResource(ThemeResourceReference themeResourceReference)
    {
    }

    protected internal ThemeResourceDictionary CreateThemeResourceDictionary()
    {
      var resourceDictionary = new ThemeResourceDictionary();

      ThemeStyleBinder.Instance.AttachResourceDictionary(resourceDictionary);

      return resourceDictionary;
    }

    internal void EnsureDeferredStylesLoaded(ThemeStyle themeStyle)
    {
      if (themeStyle.IsDeferred == false)
        return;

      try
      {
        var xamlResourceInfo = _deferredStylesDictionary[themeStyle];

        _deferredStylesDictionary.Remove(themeStyle);

        if (_deferredResources.Remove(xamlResourceInfo.Uri.OriginalString) == false)
          return;

        ProcessXamlResourceInt(xamlResourceInfo, (ThemeResourceDictionary) xamlResourceInfo.DeferredResourceDictionary);
      }
      finally
      {
        themeStyle.IsDeferred = false;
      }
    }

    protected internal virtual IEnumerable<ThemeResource> EnumerateResources()
    {
      return Enumerable.Empty<ThemeResource>();
    }

    private static void FreezeResourceDictionary(ResourceDictionary themeResourceDictionary)
    {
#if !SILVERLIGHT
      foreach (var resourceDictionary in themeResourceDictionary.EnumerateDictionaries())
      {
        foreach (var freezable in resourceDictionary.Values.OfType<Freezable>())
        {
          if (freezable.IsFrozen)
            continue;

          if (freezable.CanFreeze)
            freezable.Freeze();
        }
      }
#endif
    }

    protected internal virtual ThemeResource GetResource(string key)
    {
      return null;
    }

    internal virtual bool IsThemeResource(XamlResourceInfo resource)
    {
      return resource.ThemeType == GetType();
    }

    internal void LoadXamlResources(IEnumerable<XamlResourceInfo> resources, bool isApplied)
    {
      var filteredResource = resources.Where(ShouldProcessXamlResource);

      if (isApplied == false)
        _unprocessedResources.AddRange(filteredResource);
      else
        ProcessXamlResources(filteredResource);
    }

    protected virtual void OnApplied()
    {
      _suspendThemeBinding = true;

      LoadXamlResources(ThemeResourceDictionaryLoader.Instance.XamlResources, false);

      ProcessPendingResources();

      var themeStyleBinder = ThemeStyleBinder.Instance;

      themeStyleBinder.BindThemeStyles(_themeStyles.Values);

      themeStyleBinder.AttachResourceDictionary(ThemeResourceDictionary);

      foreach (var themeResourceDictionary in ThemeResourceDictionaries)
        themeStyleBinder.AttachResourceDictionary(themeResourceDictionary);

      _suspendThemeBinding = false;
    }

    private void OnGenericDictionaryRegistered(GenericResourceDictionary genericDictionary)
    {
      _genericDictionaries.Add(genericDictionary);
      PlatformOnGenericDictionaryRegistered(genericDictionary);
      RegisterGenericDictionaryCore(genericDictionary);
    }


    protected virtual void OnUnapplied()
    {
      var themeStyleBinder = ThemeStyleBinder.Instance;

      themeStyleBinder.DetachResourceDictionary(ThemeResourceDictionary);

      foreach (var themeResourceDictionary in ThemeResourceDictionaries)
        themeStyleBinder.DetachResourceDictionary(themeResourceDictionary);

      themeStyleBinder.UnbindThemeStyles(_themeStyles.Values);
    }

    private void OnXamlResourceLoading(object sender, XamlResourceLoadingEventArgs xamlResourceLoadingEventArgs)
    {
      LoadXamlResources(xamlResourceLoadingEventArgs.NewXamlResources, IsApplied);
    }

    partial void PlatformOnGenericDictionaryRegistered(GenericResourceDictionary genericDictionary);

    protected void ProcessPendingResources()
    {
      ProcessXamlResources(_unprocessedResources);

      _unprocessedResources.Clear();
    }

    private void ProcessThemeResourceDictionaryInt(XamlResourceInfo xamlResourceInfo, ResourceDictionary themeResourceDictionary, ThemeResourceDictionary deferredDictionary)
    {
      FreezeResourceDictionary(themeResourceDictionary);

      if (deferredDictionary != null)
      {
        var binder = ThemeStyleBinder.Instance;
				var list = new List<ThemeStyle>();

        foreach (var kv in themeResourceDictionary.Cast<DictionaryEntry>().Where(kv => kv.Value is ThemeStyle))
        {
          var deferredStyle = (ThemeStyle) deferredDictionary[kv.Key];
					var actualStyle = (ThemeStyle) kv.Value;

          actualStyle.Owner = this;
          actualStyle.Source = xamlResourceInfo.Uri;
          actualStyle.Assembly = xamlResourceInfo.Assembly;

          if (deferredStyle != null)
            _themeStyles.Remove(deferredStyle.TargetType);

          _themeStyles.Add(actualStyle.TargetType, actualStyle);

          list.Add(actualStyle);
        }

        binder.RebindThemeStyles(list);

        UpdateStyleKeys(themeResourceDictionary);
      }
      else
      {
        var themeStyleTypes = new Dictionary<Type, ThemeStyle>();
				var resourceDictionary = themeResourceDictionary;

        try
        {
          foreach (var kv in resourceDictionary.Cast<DictionaryEntry>().Where(kv => kv.Value is ThemeStyle))
          {
            var themeStyle = (ThemeStyle) kv.Value;

            themeStyle.Owner = this;
            themeStyle.Source = xamlResourceInfo.Uri;
            themeStyle.Assembly = xamlResourceInfo.Assembly;

            if (themeStyle.BasedOn == null && themeStyle.IsDeferred == false)
            {
              LogService.LogWarning($"ThemeStyle for target type '{themeStyle.TargetType?.Name}' without base style is detected in ResourceDictionary '{xamlResourceInfo.Uri}'");

              continue;
            }

            _themeStyles[themeStyle.TargetType] = themeStyle;

            themeStyleTypes[themeStyle.TargetType] = themeStyle;
          }

          UpdateStyleKeys(resourceDictionary);
        }
        catch (Exception ex)
        {
          LogService.LogError(ex);
        }

        if (IsApplied == false || _suspendThemeBinding) return;

        ThemeStyleBinder.Instance.BindThemeStyles(themeStyleTypes.Values);
      }
    }

    internal virtual void ProcessXamlResource(XamlResourceInfo resource)
    {
      if (UseDeferredProcessing)
        ProcessXamlResourceDeferred(resource);
      else
        ProcessXamlResourceInt(resource, null);
    }

    private void ProcessXamlResourceDeferred(XamlResourceInfo xamlResourceInfo)
    {
      if (xamlResourceInfo.IsDeferred && xamlResourceInfo.IsResourceDictionaryLoaded == false)
      {
        try
        {
	        if (xamlResourceInfo.DeferredResourceDictionary is ThemeResourceDictionary deferredDictionary)
          {
            var styles = deferredDictionary.Values.OfType<ThemeStyle>().ToList();

            foreach (var themeStyle in styles)
            {
              themeStyle.Owner = this;
              themeStyle.IsDeferred = true;

              _deferredStylesDictionary[themeStyle] = xamlResourceInfo;
              _themeStyles[themeStyle.TargetType] = themeStyle;
            }

            _deferredResources.Add(xamlResourceInfo.Uri.OriginalString, xamlResourceInfo);

            ProcessThemeResourceDictionaryInt(xamlResourceInfo, deferredDictionary, null);

            return;
          }
        }
        catch (Exception ex)
        {
          LogService.LogError(ex);
        }
      }

      //ProcessXamlResourceInt(xamlResourceInfo, null);
    }

    private void ProcessXamlResourceInt(XamlResourceInfo xamlResourceInfo, ThemeResourceDictionary deferredDictionary)
    {
      var resourceDictionary = xamlResourceInfo.EnsureResourceDictionary();

      if (resourceDictionary == null)
        return;

#if DEBUG
      // XamlResource contains references to dictionaries which are pending for deferred load
      // Review Theme to exclude references to Deferred ThemeResourceDictionaries
      var mergedDictionaries = resourceDictionary.EnumerateReferencedDictionaries(xamlResourceInfo.Uri).Select(s => _deferredResources.GetValueOrDefault(s.OriginalString)).SkipNull().ToList();

      foreach (var mergedDictionary in mergedDictionaries)
        LogService.LogInfo($"{xamlResourceInfo.Uri} contains reference to {mergedDictionary} which supposed to be loaded deferred");
#endif

      ProcessThemeResourceDictionaryInt(xamlResourceInfo, resourceDictionary, deferredDictionary);
    }

    private void ProcessXamlResources(IEnumerable<XamlResourceInfo> resources)
    {
      foreach (var xamlResourceInfo in resources.OrderBy(r => r.Priority))
      {
        ProcessXamlResource(xamlResourceInfo);

        if (xamlResourceInfo.IsResourceDictionaryLoaded && xamlResourceInfo.ResourceDictionary != null)
          ProcessedResourceDictionaries.Add(xamlResourceInfo.ResourceDictionary);
      }
    }

    internal static void RegisterGenericDictionary(Type themeType, GenericResourceDictionary genericDictionary)
    {
      ThemeGenericDictionaries.AddValue(themeType, genericDictionary);

      Themes.GetValueOrDefault(themeType)?.OnGenericDictionaryRegistered(genericDictionary);
    }

    protected virtual void RegisterGenericDictionaryCore(GenericResourceDictionary genericDictionary)
    {
    }

    protected internal void ReleaseDesignTimeThemeResourceDictionary(ThemeResourceDictionary themeResourceDictionary)
    {
      ThemeStyleBinder.Instance.DetachResourceDictionary(themeResourceDictionary);
    }

    internal virtual bool ShouldProcessXamlResource(XamlResourceInfo resource)
    {
      return GetType().IsAssignableFrom(resource.ThemeType);
    }

    private static void UpdateStyleKeys(ResourceDictionary themeResourceDictionary)
    {
      foreach (var resourceDictionary in themeResourceDictionary.EnumerateDictionaries())
      {
        foreach (var kv in resourceDictionary.Cast<DictionaryEntry>().Where(kv => kv.Value is StyleBase))
        {
          var style = (StyleBase) kv.Value;

          style.ResourceKey = kv.Key?.ToString();
        }
      }
    }

    #endregion
  }
}