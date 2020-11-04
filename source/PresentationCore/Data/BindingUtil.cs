// <copyright file="BindingUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
  internal static class BindingUtil
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty TargetPropertyProperty = DPM.RegisterAttached<object>
      ("TargetProperty", typeof(BindingUtil));

    #endregion

    #region Properties

    private static DependencyObject TargetObject { get; } = new DepObj();

    #endregion

    #region  Methods

    internal static BindingExpression BuildExpression(this Binding binding)
    {
      return BuildExpression(binding, TargetObject, TargetPropertyProperty);
    }

    internal static BindingExpression BuildExpression(this Binding binding, DependencyObject target, DependencyProperty targetProperty)
    {
      target.SetBinding(targetProperty, binding);

      var be = (BindingExpression) target.ReadLocalValue(targetProperty);

      target.ClearValue(targetProperty);

      return be;
    }

    internal static IEnumerable<BindingExpression> EnumerateInstalledBindings(DependencyObject dependencyObject)
    {
	    var lve = dependencyObject.GetLocalValueEnumerator();

	    while (lve.MoveNext())
	    {
		    var entry = lve.Current;

		    if (!BindingOperations.IsDataBound(dependencyObject, entry.Property)) 
			    continue;

				if (entry.Value is BindingExpression bindingExpression)
					yield return bindingExpression;
	    }
		}

    public static void EnsureBindingAttached(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
    {
	    if (!(dependencyObject.ReadLocalValue(dependencyProperty) is BindingExpression bindingExpression))
        return;

      // Rebind to actual DataContext
      if (bindingExpression.Status == BindingStatus.Unattached)
      {
        dependencyObject.ClearValue(dependencyProperty);
        dependencyObject.SetBinding(dependencyProperty, bindingExpression.ParentBinding);
      }
    }

    public static Binding CloneBinding(this Binding binding)
    {
#if SILVERLIGHT
      return new Binding(binding);
#else
      return CloneBindingImpl(binding);
#endif
    }

#if !SILVERLIGHT
    public static Binding CloneBindingImpl(Binding bindingBase)
    {
      var binding = bindingBase;

      if (binding == null)
	      throw new NotSupportedException("Failed to clone binding");

      var result = new Binding
      {
	      AsyncState = binding.AsyncState,
	      BindingGroupName = binding.BindingGroupName,
	      BindsDirectlyToSource = binding.BindsDirectlyToSource,
	      Converter = binding.Converter,
	      ConverterCulture = binding.ConverterCulture,
	      ConverterParameter = binding.ConverterParameter,
	      FallbackValue = binding.FallbackValue,
	      IsAsync = binding.IsAsync,
	      Mode = binding.Mode,
	      NotifyOnSourceUpdated = binding.NotifyOnSourceUpdated,
	      NotifyOnTargetUpdated = binding.NotifyOnTargetUpdated,
	      NotifyOnValidationError = binding.NotifyOnValidationError,
	      Path = binding.Path,
	      StringFormat = binding.StringFormat,
	      TargetNullValue = binding.TargetNullValue,
	      UpdateSourceExceptionFilter = binding.UpdateSourceExceptionFilter,
	      UpdateSourceTrigger = binding.UpdateSourceTrigger,
	      ValidatesOnDataErrors = binding.ValidatesOnDataErrors,
	      ValidatesOnExceptions = binding.ValidatesOnExceptions,
	      XPath = binding.XPath,
      };

      if (bindingBase.RelativeSource != null)
	      result.RelativeSource = bindingBase.RelativeSource;
      else if (bindingBase.ElementName != null)
	      result.ElementName = bindingBase.ElementName;
      else if (bindingBase.Source != null)
	      result.Source = bindingBase.Source;

      foreach (var validationRule in binding.ValidationRules)
	      result.ValidationRules.Add(validationRule);

      return result;

    }
#endif

    #endregion

    #region  Nested Types

    private class DepObj : DependencyObject
    {
    }

    #endregion
  }
}