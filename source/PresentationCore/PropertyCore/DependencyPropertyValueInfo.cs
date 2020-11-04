// <copyright file="DependencyPropertyValueInfo.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore.Extensions;

namespace Zaaml.PresentationCore.PropertyCore
{
  internal struct DependencyPropertyValueInfo
  {
    private readonly DependencyObject _dependencyObject;
    private readonly DependencyProperty _dependencyProperty;
    private object _localValue;
    private object _value;
    private object _defaultValue;
    private PropertyValueSource? _valueSource;
    private Type _propertyType;
    private PropertyMetadata _propertyMetadata;

    public DependencyPropertyValueInfo(DependencyObject dependencyObject, DependencyProperty dependencyProperty) : this()
    {
      _dependencyObject = dependencyObject;
      _dependencyProperty = dependencyProperty;

      _localValue = Unset.Value;
      _value = Unset.Value;
      _defaultValue = Unset.Value;
    }

    public object LocalValue
    {
      get
      {
        if (_localValue.IsUnset())
          _localValue = _dependencyObject.ReadLocalValue(_dependencyProperty);

        return _localValue;
      }
    }

    public object Value
    {
      get
      {
        if (_value.IsUnset())
          _value = _dependencyObject.GetValue(_dependencyProperty);

        return _value;
      }
    }

    public object DefaultValue
    {
      get
      {
        if (_defaultValue.IsSet())
          return _defaultValue;

        _defaultValue = PropertyMetadata.DefaultValue;

        if (_defaultValue != DependencyProperty.UnsetValue)
          return _defaultValue;

        var propertyType = PropertyType;

        _defaultValue = propertyType != null ? RuntimeUtils.CreateDefaultValue(propertyType) : null;

        return _defaultValue;
      }
    }

    public PropertyMetadata PropertyMetadata
    {
      get
      {
        if (_propertyMetadata != null)
          return _propertyMetadata;

        _propertyMetadata = _dependencyProperty.GetMetadata(_dependencyObject.GetType());

        return _propertyMetadata;
      }
    }

    public Type PropertyType
    {
      get
      {
        if (_propertyType != null)
          return _propertyType;

        _propertyType = _dependencyProperty.GetPropertyType();

        return _propertyType;
      }
    }

    private PropertyValueSource EvaluateValueSource()
    {
#if !SILVERLIGHT
      var valueSource = DependencyPropertyHelper.GetValueSource(_dependencyObject, _dependencyProperty);

      switch (valueSource.BaseValueSource)
      {
        case BaseValueSource.Unknown:
        case BaseValueSource.Default:
          return PropertyValueSource.Default;
        case BaseValueSource.Inherited:
        case BaseValueSource.Style:
        case BaseValueSource.StyleTrigger:
        case BaseValueSource.DefaultStyle:
        case BaseValueSource.DefaultStyleTrigger:
        case BaseValueSource.ImplicitStyleReference:
          return PropertyValueSource.Inherited;
        case BaseValueSource.TemplateTrigger:
        case BaseValueSource.ParentTemplate:
        case BaseValueSource.ParentTemplateTrigger:
          return PropertyValueSource.TemplatedParent;
        case BaseValueSource.Local:
          return LocalValue is BindingExpression ? PropertyValueSource.LocalBinding : PropertyValueSource.Local;
      }
#endif
      var localValue = LocalValue;
      var isUnset = ReferenceEquals(DependencyProperty.UnsetValue, localValue);

      if (localValue is BindingExpression)
        return PropertyValueSource.LocalBinding;

      if (isUnset == false)
        return PropertyValueSource.Local;

      return Equals(Value, DefaultValue) == false ? PropertyValueSource.Inherited : PropertyValueSource.Default;
    }

    public PropertyValueSource ValueSource
    {
      get
      {
        if (_valueSource.HasValue == false)
          _valueSource = EvaluateValueSource();

        return _valueSource.Value;
      }
    }

    public bool HasLocalValue => ReferenceEquals(LocalValue, DependencyProperty.UnsetValue) == false;

    public bool IsDefaultValue => HasLocalValue == false && ValueSource == PropertyValueSource.Default;
  }
}