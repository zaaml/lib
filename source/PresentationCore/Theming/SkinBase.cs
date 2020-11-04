// <copyright file="SkinBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Setter = Zaaml.PresentationCore.Interactivity.Setter;

namespace Zaaml.PresentationCore.Theming
{
  [TypeConverter(typeof(SkinTypeConverter))]
  public abstract class SkinBase : IInteractivityVisitor
  {
    #region Ctors

    internal SkinBase()
    {
    }

    #endregion

    #region Properties

    internal abstract IEnumerable<KeyValuePair<string, object>> Resources { get; }

    #endregion

    #region  Methods

    protected abstract object GetValue(string key);

    internal object GetValueInternal(string key)
    {
      return GetValue(key);
    }

    internal object GetValueInternal(ThemeResourceKey key)
    {
      return key.IsEmpty ? null : GetValueInternal(key.Key);
    }

    protected virtual void OnAttached(FrameworkElement frameworkElement)
    {
    }

    internal void OnAttachedInternal(FrameworkElement frameworkElement)
    {
      OnAttached(frameworkElement);
    }

    protected virtual void OnDetached(FrameworkElement frameworkElement)
    {
    }

    internal void OnDetachedInternal(FrameworkElement frameworkElement)
    {
      OnDetached(frameworkElement);
    }

    #endregion

    #region Interface Implementations

    #region IInteractivityVisitor

    void IInteractivityVisitor.Visit(InteractivityObject interactivityObject)
    {
      var setter = interactivityObject as Setter;

      setter?.UpdateSkin(this);
    }

    #endregion

    #endregion
  }
}