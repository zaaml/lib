// <copyright file="StyleService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using NativeStyle = System.Windows.Style;
using NativeSetter = System.Windows.Setter;
using Setter = Zaaml.PresentationCore.Interactivity.Setter;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;

namespace Zaaml.PresentationCore.Theming
{
  internal sealed partial class StyleService
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty InstanceProperty = DPM.RegisterAttached<StyleService, StyleService>
      ("Instance", OnInstanceChanged);

    private static readonly Dictionary<DependencyProperty, DependencyProperty> DynamicProperties = new Dictionary<DependencyProperty, DependencyProperty>();
    private static readonly Dictionary<string, int> DynamicPropertyNames = new Dictionary<string, int>();

    #endregion

    #region Fields

    private readonly NativeStyleSource _nativeStyleSource;
    private readonly WeakLinkedList<Runtime> _services = new WeakLinkedList<Runtime>();
    private NativeStyle _nativeStyle;

    public event EventHandler NativeStyleChanged;

    #endregion

    #region Ctors

    public StyleService(StyleBase style)
    {
      Style = style;
      Setters = new StyleServiceSetterCollection(this);
      Triggers = new StyleServiceTriggerCollection(this);

      Update();

      _nativeStyleSource = new NativeStyleSource(this);
    }

    #endregion

    #region Properties

#if INTERACTIVITY_DEBUG
    public bool Debug { get; set; }
#endif

    public NativeStyle NativeStyle => _nativeStyle ??= BuildNativeStyle();

    public Binding NativeStyleBinding => _nativeStyleSource.StyleBinding;

    public StyleBase Style { get; }

    public StyleServiceSetterCollection Setters { get; }

    public StyleServiceTriggerCollection Triggers { get; }

    #endregion

    #region  Methods

    private NativeStyle BuildNativeStyle()
    {
      var targetType = Style.TargetType ?? typeof(FrameworkElement);

      var nativeStyle = new NativeStyle
      {
        TargetType = targetType
      };

#if !SILVERLIGHT
      foreach (var baseStyle in Style.EnumerateBaseStylesAndSelf().OfType<Style>())
      {
        if (baseStyle.Resources.Count == 0)
          continue;

        foreach (var resourceKey in baseStyle.Resources.Keys)
        {
          if (nativeStyle.Resources.Contains(resourceKey) == false)
            nativeStyle.Resources.Add(resourceKey, baseStyle.Resources[resourceKey]);
        }
      }
#endif

      Triggers.AddRange(Style.ActualTriggers);

      var flatSetters = Style.ActualSetters.OfType<Setter>().Select(s => s.DeepClone<Setter>()).ToList();

      Dictionary<DependencyProperty, DependencyProperty> dynamicProperties = null;

      foreach (var setter in flatSetters)
      {
        if (string.IsNullOrEmpty(setter.ExpandoProperty) == false || string.IsNullOrEmpty(setter.VisualState))
          continue;

        var property = setter.ResolveProperty(targetType);

        if (property == null)
        {
          LogService.LogWarning($"Unable resolve property for setter: {setter}");
          continue;
        }

        dynamicProperties ??= new Dictionary<DependencyProperty, DependencyProperty>();

        dynamicProperties.GetValueOrCreate(property, GetDynamicProperty);
      }

      if (dynamicProperties != null)
        foreach (var kv in dynamicProperties)
          nativeStyle.Setters.Add(new NativeSetter {Property = kv.Key, Value = new Binding {Path = new PropertyPath(kv.Value), RelativeSource = XamlConstants.Self}});

      foreach (var setter in flatSetters)
      {
        if (string.IsNullOrEmpty(setter.ExpandoProperty) == false)
        {
          Setters.Add(setter.Optimize());
          continue;
        }

        var dependencyProperty = setter.ResolveProperty(targetType);
        var dynamicProperty = dependencyProperty != null ? dynamicProperties?.GetValueOrDefault(dependencyProperty) : null;
        if (dynamicProperty != null)
        {
          setter.Property = dynamicProperty;
          Setters.Add(setter.Optimize());
          continue;
        }

        var nativeSetter = setter.CreateNativeStyleSetter(targetType);

        if (nativeSetter != null)
          nativeStyle.Setters.Add(nativeSetter);
        else
          Setters.Add(setter.Optimize());
      }

      nativeStyle.Setters.Add(new NativeSetter(InstanceProperty, this));

      return nativeStyle;
    }

    private void EnsureNativeStyle()
    {
	    _nativeStyle ??= BuildNativeStyle();
    }

    public void ExternalLoadSetter(FrameworkElement element, SetterBase setter)
    {
      element.GetService<Runtime>()?.ExternalLoadSetter(setter);
    }

    private static DependencyProperty GetDynamicProperty(DependencyProperty property)
    {
      return DynamicProperties.GetValueOrCreate(property, RegisterDynamicProperty);
    }

    private static string GetDynamicPropertyName(DependencyProperty property)
    {
      var propertyName = property.GetName();
      var count = DynamicPropertyNames.GetValueOrDefault(propertyName);
			var dynamicPropertyName = $"{propertyName}_dyn{count}";

      DynamicPropertyNames[propertyName] = count + 1;

      return dynamicPropertyName;
    }

    public static StyleService GetFromNativeStyle(NativeStyle nativeStyle)
    {
      return nativeStyle.FindSetter<StyleService>(InstanceProperty);
    }

    public static StyleService GetInstance(DependencyObject element)
    {
      return (StyleService) element.GetValue(InstanceProperty);
    }

    public static IRuntimeStyleService GetRuntimeService(FrameworkElement target) => target.GetService<Runtime>();

    private static void OnInstanceChanged(DependencyObject fre, StyleService oldStyleService, StyleService newStyleService)
    {
      if (oldStyleService != null)
        fre.RemoveService<Runtime>();

      if (newStyleService != null)
        fre.SetService(new Runtime(newStyleService));
    }

    private void OnStyleChanged()
    {
      foreach (var service in _services)
        service.OnStyleServiceStyleChanged();
    }

    internal void OnStyleInheritanceChanged()
    {
      Update();

      _nativeStyle = null;

      NativeStyleChanged?.Invoke(this, EventArgs.Empty);

      _nativeStyleSource.RaiseStyleChanged();
    }

    private static DependencyProperty RegisterDynamicProperty(DependencyProperty property)
    {
      var propertyType = property.GetPropertyType();
      return DependencyPropertyManager.RegisterAttached(GetDynamicPropertyName(property), propertyType, typeof(StyleService), new PropertyMetadata(propertyType.CreateDefaultValue()));
    }

    public static void SetInstance(DependencyObject element, StyleService value)
    {
      element.SetValue(InstanceProperty, value);
    }

    public void Update()
    {
      Setters.Clear();
      Triggers.Clear();

      _nativeStyle = null;

      OnStyleChanged();
    }

    #endregion
  }
}