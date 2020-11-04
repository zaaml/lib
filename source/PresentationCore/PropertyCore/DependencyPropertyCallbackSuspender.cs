// <copyright file="DependencyPropertyCallbackSuspender.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.PropertyCore
{
  internal static class DependencyPropertyCallbackSuspender
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty SuspendedPropertiesProperty = DependencyPropertyManager.RegisterAttached
    ("SuspendedProperties", typeof(Dictionary<DependencyProperty, ISuspendState>), typeof(DependencyPropertyCallbackSuspender),
      new PropertyMetadata(default(Dictionary<DependencyProperty, ISuspendState>)));

    #endregion

    #region  Methods

    private static ISuspendState GetSuspendState(this DependencyObject depObj, DependencyProperty dependencyProperty, bool create)
    {
      var suspendProps = create
        ? depObj.GetValueOrCreate(SuspendedPropertiesProperty, () => new Dictionary<DependencyProperty, ISuspendState>())
        : depObj.GetValue<Dictionary<DependencyProperty, ISuspendState>>(SuspendedPropertiesProperty);

      if (suspendProps == null)
        return DummySuspendState.Instance;

      return create ? suspendProps.GetValueOrCreate(dependencyProperty, () => new SuspendState()) : IDictionaryExtensions.GetValueOrDefault(suspendProps, dependencyProperty, DummySuspendState.Instance);
    }

    public static bool IsPropertyChangedCallbackSuspended(this DependencyObject depObj, DependencyProperty dependencyProperty)
    {
      return depObj.GetSuspendState(dependencyProperty, false).IsSuspended;
    }

    public static void ResumePropertyChangedCallback(this DependencyObject depObj, DependencyProperty dependencyProperty)
    {
      depObj.GetSuspendState(dependencyProperty, true).Resume();
    }

    public static void SuspendPropertyChangedCallback(this DependencyObject depObj, DependencyProperty dependencyProperty)
    {
      depObj.GetSuspendState(dependencyProperty, true).Suspend();
    }

    #endregion
  }
}