// <copyright file="TemplateContract.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;

namespace Zaaml.PresentationCore.TemplateCore
{
  public class TemplateContract
  {
    #region Fields

    private readonly Action _onAttached;
    private readonly Action _onDetaching;

    private TemplateContractBinder _templateContractBinder;
    public event EventHandler Attached;
    public event EventHandler Detaching;

    #endregion

    #region Ctors

    public TemplateContract(FrameworkElement frameworkElement)
      : this(new TemplateContractBinder(frameworkElement), null, null)
    {
    }

    public TemplateContract(GetTemplateChild templateDiscovery)
      : this(new TemplateContractBinder(templateDiscovery), null, null)
    {
    }

    public TemplateContract(FrameworkElement frameworkElement, Action onAttached, Action onDetaching)
      : this(new TemplateContractBinder(frameworkElement), onAttached, onDetaching)
    {
    }

    public TemplateContract(GetTemplateChild templateDiscovery, Action onAttached, Action onDetaching)
      : this(new TemplateContractBinder(templateDiscovery), onAttached, onDetaching)
    {
    }

    public TemplateContract()
    {
    }

    internal TemplateContract(TemplateContractBinder templateContractBinder, Action onAttached, Action onDetaching)
    {
      _onAttached = onAttached;
      _onDetaching = onDetaching;
      _templateContractBinder = templateContractBinder;
    }

    #endregion

    #region Properties

    public bool IsAttached { get; private set; }

    internal TemplateContractBinder TemplateContractBinder
    {
      get => _templateContractBinder;
      set
      {
        if (ReferenceEquals(_templateContractBinder, value))
          return;

        if (IsAttached)
        {
          _templateContractBinder?.Detach(this);

          _templateContractBinder = value;

          _templateContractBinder?.Attach(this);
        }
        else
          _templateContractBinder = value;
      }
    }

    #endregion

    #region  Methods

    public void Attach()
    {
      Detach();

      _templateContractBinder.Attach(this);
      IsAttached = true;
      OnAttached();
    }

    public void Detach()
    {
      if (IsAttached == false)
        return;

      OnDetaching();
      IsAttached = false;
      _templateContractBinder.Detach(this);
    }

    protected virtual void OnAttached()
    {
      _onAttached?.Invoke();

      Attached?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnDetaching()
    {
      _onDetaching?.Invoke();

      Detaching?.Invoke(this, EventArgs.Empty);
    }

    #endregion
  }

  //[AttributeUsage(AttributeTargets.Class)]
  public class TemplateContractTypeAttribute : Attribute
  {
    #region Ctors

    public TemplateContractTypeAttribute(Type templateContractType)
    {
      TemplateContractType = templateContractType;
    }

    #endregion

    #region Properties

    public Type TemplateContractType { get; }

    #endregion

    #region  Methods

    internal TemplateContract CreateTemplateContractInternal()
    {
      return (TemplateContract) Activator.CreateInstance(TemplateContractType);
    }

    #endregion
  }
}