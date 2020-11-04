// <copyright file="ThemeResourceReference.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using Zaaml.Core;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;

namespace Zaaml.PresentationCore.Theming
{
  public class ThemeResourceReference : INotifyPropertyChanged, ISetterValueProvider
  {
    #region Fields

    private ConvertCacheStore _cacheStore = new ConvertCacheStore(Unset.Value);
    private WeakLinkedList<IThemeResourceChangeListener> _listeners;
    private event PropertyChangedEventHandler PropertyChanged;

    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
    {
      add => PropertyChanged += value;
      remove => PropertyChanged -= value;
    }

    #endregion

    #region Ctors

    public ThemeResourceReference(string key)
    {
      Key = key;
    }

    #endregion

    #region Properties

    internal object CachedValue => _cacheStore.CachedValue;

    internal bool IsBound => _cacheStore.Value.IsSet();

    public string Key { get; }

    public object Value
    {
      get => _cacheStore.Value.GetSetValueOrDefault();
      private set
      {
        if (Equals(_cacheStore.Value, value))
          return;

        _cacheStore.Value = value.GetAsFrozen();

        OnValueChanged();
        OnPropertyChanged(nameof(Value));
      }
    }

    ValueProviderOptions ISetterValueProvider.Options => ValueProviderOptions.StaticValue | ValueProviderOptions.Shared | ValueProviderOptions.LongLife;
    object ISetterValueProvider.OriginalValue => Key;

    #endregion

    #region  Methods

    internal void AddListener(IThemeResourceChangeListener listener)
    {
	    _listeners ??= new WeakLinkedList<IThemeResourceChangeListener>();

	    _listeners.Add(listener);
    }

    internal void Bind(object value)
    {
      Value = value;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnValueChanged()
    {
      if (_listeners == null)
        return;

      foreach (var listener in _listeners)
        listener.OnThemeResourceChanged();
    }

    internal void RemoveListener(IThemeResourceChangeListener listener)
    {
      _listeners?.Remove(listener);
    }

    internal void Unbind()
    {
      Value = Unset.Value;
    }

    internal XamlConvertResult XamlConvertValue(Type targetType)
    {
      return XamlStaticConverter.TryConvertCache(ref _cacheStore, targetType);
    }

    void ISetterValueProvider.Attach(RuntimeSetter setter)
    {
    }

    void ISetterValueProvider.Detach(RuntimeSetter setter)
    {
    }

    object ISetterValueProvider.ProvideValue(RuntimeSetter setter)
    {
      if (IsBound == false)
        return null;

      var result = XamlStaticConverter.TryConvertCache(ref _cacheStore, setter.TargetPropertyType);
      return result.IsValid ? result.Result.GetAsFrozen() : result.Exception;
    }

    #endregion
  }

  internal interface IThemeResourceChangeListener
  {
    #region  Methods

    void OnThemeResourceChanged();

    #endregion
  }
}