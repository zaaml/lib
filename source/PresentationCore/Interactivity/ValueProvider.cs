// <copyright file="ValueProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Interactivity
{
  [Flags]
  internal enum ValueProviderOptions
  {
    Default = 0x0,
    StaticValue = 0x1,
    Shared = 0x2,
    LongLife = 0x4
  }

  internal interface ISetterValueProvider
  {
    #region Properties

    ValueProviderOptions Options { get; }

    object OriginalValue { get; }

    #endregion

    #region  Methods

    void Attach(RuntimeSetter setter);
    void Detach(RuntimeSetter setter);
    object ProvideValue(RuntimeSetter setter);

    #endregion
  }

  internal static class SetterValueProviderExtensions
  {
    #region  Methods

    public static bool IsDynamic(this ISetterValueProvider provider)
    {
      return provider.IsStatic() == false;
    }

    public static bool IsLongLife(this ISetterValueProvider provider)
    {
      return (provider.Options & ValueProviderOptions.LongLife) == ValueProviderOptions.LongLife;
    }

    public static bool IsShared(this ISetterValueProvider provider)
    {
      return (provider.Options & ValueProviderOptions.Shared) == ValueProviderOptions.Shared;
    }

    public static bool IsStatic(this ISetterValueProvider provider)
    {
      return (provider.Options & ValueProviderOptions.StaticValue) == ValueProviderOptions.StaticValue;
    }

    #endregion
  }

  internal abstract class BaseValueProvider : ISetterValueProvider
  {
    #region Ctors

    protected BaseValueProvider(ValueProviderOptions options)
    {
      Options = options;
    }

    #endregion

    #region  Methods

    protected virtual void OnChanged()
    {
    }

    #endregion

    #region Interface Implementations

    #region ISetterValueProvider

    public ValueProviderOptions Options { get; }
    public abstract object OriginalValue { get; }

    public virtual void Attach(RuntimeSetter setter)
    {
    }

    public virtual void Detach(RuntimeSetter setter)
    {
    }

    public abstract object ProvideValue(RuntimeSetter setter);

    #endregion

    #endregion
  }

  internal abstract class ConstCachingValueProviderBase : BaseValueProvider
  {
    #region Fields

    private ConstValueCache _cache = new ConstValueCache();

    #endregion

    #region Ctors

    protected ConstCachingValueProviderBase(ValueProviderOptions options) : base(options)
    {
    }

    #endregion

    #region  Methods

    protected void ClearCache()
    {
      _cache.Reset();
    }

    protected sealed override void OnChanged()
    {
      ClearCache();
      OnChangedOverride();
    }

    protected virtual void OnChangedOverride()
    {
    }

    public sealed override object ProvideValue(RuntimeSetter setter)
    {
      return _cache.ProvideValue(ProvideValueCore(setter), setter.EffectiveValue.Property.GetPropertyType());
    }

    protected abstract object ProvideValueCore(RuntimeSetter setter);

    #endregion
  }

  internal struct ConstValueCache
  {
    #region Fields

    private Type _cacheTargetType;
    private object _convertedCache;

    #endregion

    #region  Methods

    public object ProvideValue(object value, Type targetPropertyType)
    {
      if (targetPropertyType == _cacheTargetType)
        return _convertedCache;

      _convertedCache = XamlStaticConverter.ConvertValue(value, targetPropertyType);
      _cacheTargetType = targetPropertyType;

      return _convertedCache;
    }

    public void Reset()
    {
      _cacheTargetType = null;
      _convertedCache = null;
    }

    #endregion
  }


  internal class ExpandoValueProvider : ConstCachingValueProviderBase, IDependencyPropertyListener
  {
    #region Fields

    private readonly DependencyProperty _property;
    private readonly DependencyObject _target;
    private RuntimeSetter _runtimeSetter;
    private object _value = DependencyProperty.UnsetValue;

    #endregion

    #region Ctors

    public ExpandoValueProvider(DependencyObject target, string propertyName) : base(ValueProviderOptions.LongLife)
    {
      _target = target;
      _property = DependencyPropertyManager.GetExpandoProperty(propertyName);
    }

    #endregion

    #region Properties

    public override object OriginalValue => throw new InvalidOperationException();

    #endregion

    #region  Methods

    public override void Attach(RuntimeSetter setter)
    {
      _target.GetDependencyPropertyService().AddExpandoPropertyListener(_property, this);
      _value = _target.GetValue(_property);

      _runtimeSetter = setter;
    }

    public override void Detach(RuntimeSetter setter)
    {
      _target.GetDependencyPropertyService().RemoveExpandoListener(_property, this);
      _runtimeSetter = null;
    }

    protected override object ProvideValueCore(RuntimeSetter setter)
    {
      return _value;
    }

    #endregion

    #region Interface Implementations

    #region IDependencyPropertyListener

    void IDependencyPropertyListener.OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
    {
      _value = newValue;
      ClearCache();
      _runtimeSetter.OnProviderValueChanged();
    }

    #endregion

    #endregion
  }
}