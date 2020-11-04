// <copyright file="PropertyChangeProvider.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.PropertyCore
{
  public interface IPropertyChangeProvider : IDisposable
  {
    #region Fields

    event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

    #endregion

    #region Properties

    object Source { get; }

    #endregion
  }

  public interface IPropertyChangeProvider<in TOwner, in TPropertyValue>
  {
    #region  Methods

    void OnPropertyChanged(TOwner sender, TPropertyValue oldValue, TPropertyValue newValue);

    #endregion
  }

  public class DelegatePropertyChangeProvider : IDisposable
  {
    #region Static Fields and Constants

    private static readonly ConditionalWeakTable<object, List<IDisposable>> StrongDisposables = new ConditionalWeakTable<object, List<IDisposable>>();

    #endregion

    #region Fields

    private readonly IPropertyChangeProvider _provider;

    #endregion

    #region Ctors

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, object, object> action)
    {
      provider.PropertyChanged += (sender, args) => action(sender, args.OldValue, args.NewValue);
      _provider = provider;
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, object> action)
    {
      provider.PropertyChanged += (sender, args) => action(args.OldValue, args.NewValue);
      _provider = provider;
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, object, object> action, bool strongDisposable)
    {
      provider.PropertyChanged += (sender, args) => action(this, args.OldValue, args.NewValue);
      _provider = provider;

      if (strongDisposable)
        AddStrongDisposable(_provider.Source, this);
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<PropertyValueChangedEventArgs> action)
    {
      provider.PropertyChanged += (sender, args) => action(args);
      _provider = provider;
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, PropertyValueChangedEventArgs> action, bool strongDisposable)
    {
      provider.PropertyChanged += (sender, args) => action(this, args);
      _provider = provider;

      if (strongDisposable)
        AddStrongDisposable(_provider.Source, this);
    }

    #endregion

    #region  Methods

    internal static void AddStrongDisposable(object target, IDisposable disposable)
    {
      var disposables = StrongDisposables.GetOrCreateValue(target);
      disposables.Add(disposable);
    }

    internal static void RemoveStrongDisposable(object target, IDisposable disposable)
    {
      List<IDisposable> disposables;
      if (StrongDisposables.TryGetValue(target, out disposables))
        disposables.Remove(disposable);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _provider.Dispose();

      var source = _provider.Source;
      if (source != null)
        RemoveStrongDisposable(source, this);
    }

    #endregion

    #endregion
  }

  public class DelegatePropertyChangeProvider<TValue> : IDisposable
  {
    #region Fields

    private readonly IPropertyChangeProvider _provider;

    #endregion

    #region Ctors

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<object, TValue, TValue> action)
    {
      provider.PropertyChanged += (sender, args) => action(sender, (TValue) args.OldValue, (TValue) args.NewValue);
      _provider = provider;
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<TValue, TValue> action)
    {
      provider.PropertyChanged += (sender, args) => action((TValue) args.OldValue, (TValue) args.NewValue);
      _provider = provider;
    }

    public DelegatePropertyChangeProvider(IPropertyChangeProvider provider, Action<IDisposable, TValue, TValue> action, bool strongDisposable)
    {
      provider.PropertyChanged += (sender, args) => action(this, (TValue) args.OldValue, (TValue) args.NewValue);
      _provider = provider;

      if (strongDisposable)
        DelegatePropertyChangeProvider.AddStrongDisposable(_provider.Source, this);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _provider.Dispose();

      var source = _provider.Source;
      if (source != null)
        DelegatePropertyChangeProvider.RemoveStrongDisposable(source, this);
    }

    #endregion

    #endregion
  }

  public abstract class PropertyChangeProviderBase : IPropertyChangeProvider
  {
    #region  Methods

    public virtual void OnPropertyChanged(object sender, object oldValue, object newValue, object property)
    {
      if (Equals(oldValue, newValue) == false)
        OnPropertyChanged(new PropertyValueChangedEventArgs(sender, oldValue, newValue, property));
    }

    protected virtual void OnPropertyChanged(PropertyValueChangedEventArgs e)
    {
      PropertyChanged?.Invoke(this, e);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public virtual void Dispose()
    {
    }

    #endregion

    #region IPropertyChangeProvider

    public event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;
    public abstract object Source { get; }

    #endregion

    #endregion
  }

  internal class NotifyPropertyChangeProvider : PropertyChangeProviderBase
  {
    #region Fields

    private readonly Func<object, object> _propertyGetter;
    private readonly WeakReference _source;

    #endregion

    #region Ctors

    public NotifyPropertyChangeProvider(INotifyPropertyChanged source, string propertyName)
    {
      var propertyInfo = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

      _propertyGetter = propertyInfo.Return<PropertyInfo, Func<object, object>>(pi => s => pi.GetValue(s, null));

      if (_propertyGetter == null) return;

      Value = GetCurrentValue(source);
      PropertyName = propertyName;

      _source = new WeakReference(source);
      source.PropertyChanged += SourceOnPropertyChanged;
    }

    #endregion

    #region Properties

    public string PropertyName { get; }

    public override object Source => _source.Target;

    public object Value { get; set; }

    #endregion

    #region  Methods

    public override void Dispose()
    {
      base.Dispose();

      if (_source.IsAlive)
        _source.Target.DirectCast<INotifyPropertyChanged>().PropertyChanged -= SourceOnPropertyChanged;
    }

    private object GetCurrentValue(object source)
    {
      return _propertyGetter(source);
    }

    private void OnPropertyChanged(object sender, string propertyName)
    {
      if (propertyName != PropertyName) return;
      OnPropertyChanged(sender, Value, (Value = GetCurrentValue(sender)), propertyName);
    }

    private void SourceOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      OnPropertyChanged(sender, args.PropertyName);
    }

    #endregion
  }

  public class PropertyChangeProvider : DependencyObject, IPropertyChangeProvider
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ValueProperty = DPM.Register<object, PropertyChangeProvider>
      ("Value", d => d.OnValueChanged);

    #endregion

    #region Fields

    private readonly object _property;

    private readonly bool _senderSelf;
    private readonly WeakReference _source;
    private bool _disposing;

    #endregion

    #region Ctors

    public PropertyChangeProvider(DependencyObject source, DependencyProperty dependencyProperty, bool senderSelf = true)
    {
      _senderSelf = senderSelf;
      _source = new WeakReference(source);
      _property = dependencyProperty;

      BindingOperations.SetBinding(this, ValueProperty, new Binding {Path = new PropertyPath(dependencyProperty), Source = source});
    }

    public PropertyChangeProvider(DependencyObject source, string propertyName, bool senderSelf = true)
    {
      _senderSelf = senderSelf;
      _source = new WeakReference(source);
      _property = propertyName;

      BindingOperations.SetBinding(this, ValueProperty, new Binding {Path = new PropertyPath(propertyName), Source = source});
    }

    public PropertyChangeProvider(INotifyPropertyChanged source, string propertyName, bool senderSelf = true)
    {
      _senderSelf = senderSelf;
      _source = new WeakReference(source);
      _property = propertyName;

      BindingOperations.SetBinding(this, ValueProperty, new Binding {Path = new PropertyPath(propertyName), Source = source, Mode = BindingMode.OneWay});
    }

    #endregion

    #region Properties

    internal bool IsAnySubscriber => PropertyChanged != null;

    #endregion

    #region  Methods

    private void OnValueChanged(object oldValue, object newValue)
    {
      if (_source.IsAlive && _disposing == false)
        RaiseOnPropertyChanged(new PropertyValueChangedEventArgs(_source.Target, oldValue, newValue, _property));
    }

    protected virtual void RaiseOnPropertyChanged(PropertyValueChangedEventArgs e)
    {
      PropertyChanged?.Invoke(_senderSelf ? this : e.Source, e);
    }

    #endregion

    #region Interface Implementations

    #region IDisposable

    public void Dispose()
    {
      _disposing = true;
      ClearValue(ValueProperty);
    }

    #endregion

    #region IPropertyChangeProvider

    public event EventHandler<PropertyValueChangedEventArgs> PropertyChanged;

    public object Source => _source.Target;

    #endregion

    #endregion
  }

  public static class ProperyChangedExtensions
  {
    #region  Methods

    public static IDisposable OnPropertyChanged<TPropertyValue>(this INotifyPropertyChanged source, string name,
      Action<TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
      Action<PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
      Action<PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
      Action<PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
      Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
      Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
      Action<IDisposable, PropertyValueChangedEventArgs> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged<TPropertyValue>(this INotifyPropertyChanged source, string name,
      Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, string name,
      Action<TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, string name,
      Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, DependencyProperty dependencyProperty,
      Action<TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged<TPropertyValue>(this DependencyObject source, DependencyProperty dependencyProperty,
      Action<IDisposable, TPropertyValue, TPropertyValue> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider<TPropertyValue>(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
      Action<object, object> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this INotifyPropertyChanged source, string name,
      Action<IDisposable, object, object> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
      Action<object, object> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged);
    }

    public static IDisposable OnPropertyChanged(this DependencyObject source, string name,
      Action<IDisposable, object, object> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, name), onPropertyChanged, true);
    }

    //public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
    //  Action<object, object> onPropertyChanged)
    //{
    //  return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged);
    //}

    public static IDisposable OnPropertyChanged(this DependencyObject source, DependencyProperty dependencyProperty,
      Action<IDisposable, object, object> onPropertyChanged)
    {
      return new DelegatePropertyChangeProvider(new PropertyChangeProvider(source, dependencyProperty), onPropertyChanged, true);
    }

    #endregion
  }

  internal class PropertyChangeService : ServiceBase<DependencyObject>
  {
    #region Fields

    private readonly Dictionary<DependencyProperty, PropertyChangeProvider> _dpProviders = new Dictionary<DependencyProperty, PropertyChangeProvider>();
    private readonly Dictionary<string, PropertyChangeProvider> _strProviders = new Dictionary<string, PropertyChangeProvider>();

    #endregion

    #region  Methods

    public void AddValueChanged(DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
    {
      _dpProviders.GetValueOrCreate(depProp, () => new PropertyChangeProvider(Target, depProp, false)).PropertyChanged += handler;
    }

    public void AddValueChanged(string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
    {
      _strProviders.GetValueOrCreate(propertyName, () => new PropertyChangeProvider(Target, propertyName, false)).PropertyChanged += handler;
    }

    public void RemoveValueChanged(DependencyProperty depProp, EventHandler<PropertyValueChangedEventArgs> handler)
    {
      RemoveValueChanged(_dpProviders, depProp, handler);
    }

    public void RemoveValueChanged(string propertyName, EventHandler<PropertyValueChangedEventArgs> handler)
    {
      RemoveValueChanged(_strProviders, propertyName, handler);
    }

    private void RemoveValueChanged<TKey>(Dictionary<TKey, PropertyChangeProvider> providers, TKey key, EventHandler<PropertyValueChangedEventArgs> handler)
    {
      var provider = providers.GetValueOrDefault(key);
      if (provider == null) return;

      provider.PropertyChanged -= handler;
      if (provider.IsAnySubscriber) return;

      providers.Remove(key);

      if (_strProviders.Count == 0 && _dpProviders.Count == 0)
        Target.RemoveService<PropertyChangeService>();
    }

    #endregion
  }
}