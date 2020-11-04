// <copyright file="BindingMarkupExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.Converters;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.Utils;
using NativeBinding = System.Windows.Data.Binding;
using NativeSetter = System.Windows.SetterBase;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;

namespace Zaaml.PresentationCore.Data.MarkupExtensions
{
  public abstract class BindingMarkupExtension : MarkupExtensionBase
  {
    #region Static Fields and Constants

    protected static readonly NativeBinding UnallowedBinding = new NativeBinding {Source = Unset.Value, BindsDirectlyToSource = true, Converter = TargetNullValueConverter.Instance, Mode = BindingMode.OneTime};

    #endregion

    #region Ctors

    internal BindingMarkupExtension()
    {
    }

    #endregion

    #region Properties

#if INTERACTIVITY_DEBUG
    public bool Debug { get; set; }
#endif

    protected virtual bool SupportNativeSetter => true;

    #endregion

    #region  Methods

    internal void FinalizeXamlInitialization(IServiceProvider serviceProvider)
    {
      FinalizeXamlInitializationCore(serviceProvider);
    }

    protected virtual void FinalizeXamlInitializationCore(IServiceProvider serviceProvider)
    {
    }

    protected internal abstract NativeBinding GetBinding(IServiceProvider serviceProvider);

    internal NativeBinding GetBinding(object target, object targetProperty)
    {
      using (var serviceProvider = TargetServiceProvider.GetServiceProvider(target, targetProperty))
        return GetBinding(serviceProvider);
    }

    private object GetDefaultValue(object targetProperty)
    {
      var propertyType = GetPropertyType(targetProperty);
      return propertyType == null ? null : RuntimeUtils.CreateDefaultValue(propertyType);
    }

    protected object GetSafeTarget(IServiceProvider serviceProvider)
    {
      object target;
      object targetProperty;
      bool reflected;

      return GetTarget(serviceProvider, out target, out targetProperty, out reflected) == false ? null : target;
    }

    private object ProvideSetterValue(IServiceProvider serviceProvider)
    {
      FinalizeXamlInitializationCore(serviceProvider);
      return this;
    }

    public sealed override object ProvideValue(IServiceProvider serviceProvider)
    {
      object target;
      object targetProperty;
      bool reflected;

      if (GetTarget(serviceProvider, out target, out targetProperty, out reflected) == false)
      {
        FinalizeXamlInitializationCore(serviceProvider);
        return this;
      }

      var dependencyObjectTarget = target as DependencyObject;
      var dependencyProperty = targetProperty as DependencyProperty;

      if (reflected && dependencyObjectTarget != null)
      {
        dependencyObjectTarget.SetBinding(dependencyProperty, GetBinding(serviceProvider));
        throw new XamlMarkupInstalException();
      }

      var nativeSetter = target as NativeSetter;
      if (nativeSetter != null)
      {
        if (SupportNativeSetter)
          return ProvideSetterValue(serviceProvider);

        throw new InvalidOperationException($"{GetType().Name} markup extension does not support native setters");
      }

      var setter = target as SetterBase;
      return setter != null ? ProvideSetterValue(serviceProvider) : ProvideValueCore(target, targetProperty, serviceProvider);
    }

    protected virtual object ProvideValueCore(object target, object targetProperty, IServiceProvider serviceProvider)
    {
      var dependencyObjectTarget = target as DependencyObject;
      var interactivityTarget = target as InteractivityObject;
      var propertyInfo = targetProperty as PropertyInfo;

      if (dependencyObjectTarget == null)
      {
        if (target?.GetType().Name == "SharedDp")
        {
          FinalizeXamlInitializationCore(serviceProvider);
          return this;
        }
      }

      var binding = GetBinding(serviceProvider);
      if (serviceProvider is InteractivityObject)
        return binding;

      if (ReferenceEquals(binding, UnallowedBinding))
        return GetDefaultValue(targetProperty);

      if (interactivityTarget != null && propertyInfo != null)
      {
        if (interactivityTarget.SetMarkupExtension(propertyInfo, this, serviceProvider, true))
        {
          if (PresentationCoreUtils.IsInDesignMode)
            throw new XamlMarkupInstalException();

          return GetDefaultValue(targetProperty);
        }
      }

      return binding.ProvideValue(serviceProvider);
    }

    #endregion

    #region  Nested Types

    internal class XamlMarkupInstalException : InvalidOperationException
    {
    }

    #endregion
  }
}