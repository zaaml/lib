// <copyright file="NameScope.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
  public class NameScope : InheritanceContextObject, INameScope
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty LinkProperty = DPM.Register<NameScopeLink, NameScope>
      ("Link");

#if SILVERLIGHT
    public static readonly DependencyProperty NameScopeProperty = DPM.RegisterAttached<INameScope, NameScope>
      ("NameScope");
#else
    public static readonly DependencyProperty NameScopeProperty;
#endif

    #endregion

    #region Fields

    private readonly INameScope _innerNameScope = new System.Windows.NameScope();

    #endregion

    #region Ctors

#if !SILVERLIGHT
    static NameScope()
    {
      NameScopeProperty = System.Windows.NameScope.NameScopeProperty.AddOwner(typeof(NameScope));
    }
#endif

    #endregion

    #region Properties

    public NameScopeLink Link
    {
      get => (NameScopeLink) GetValue(LinkProperty);
      set => SetValue(LinkProperty, value);
    }

    #endregion

    #region  Methods

    internal static object FindName(DependencyObject dependencyObject, string name)
    {
      var nameScope = GetNameScope(dependencyObject) as NameScope;
      return nameScope?.FindName(name);
    }

    public static INameScope GetNameScope(DependencyObject dependencyObject)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof(dependencyObject));

      return (INameScope) dependencyObject.GetValue(NameScopeProperty);
    }

    public static void SetNameScope(DependencyObject dependencyObject, INameScope value)
    {
      if (dependencyObject == null)
        throw new ArgumentNullException(nameof(dependencyObject));

      dependencyObject.SetValue(NameScopeProperty, value);
    }

    public virtual object FindName(string name)
    {
      return _innerNameScope.FindName(name) ?? Link?.Reference?.FindName(name);
    }

    public virtual void RegisterName(string name, object scopedElement)
    {
      _innerNameScope.RegisterName(name, scopedElement);
    }

    public virtual void UnregisterName(string name)
    {
      _innerNameScope.UnregisterName(name);
    }

    #endregion

    #region Interface Implementations

    #region INameScope

    void INameScope.RegisterName(string name, object scopedElement)
    {
      RegisterName(name, scopedElement);
    }

    void INameScope.UnregisterName(string name)
    {
      UnregisterName(name);
    }

    object INameScope.FindName(string name)
    {
      return FindName(name);
    }

    #endregion

    #endregion
  }
}
