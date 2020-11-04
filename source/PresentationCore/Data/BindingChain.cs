// <copyright file="BindingChain.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

#pragma warning disable 414

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
  public class BindingChainElement : Asset
  {
    #region Fields

    private Binding _binding;
    private CommonBindingProperties _commonBindingProperties;
    private object _source;

    #endregion

    #region Properties

    public object ActualValue => GetValue(ValueProperty);

    public Binding Binding
    {
      get => _binding;
      set
      {
        if (value == null)
          throw new InvalidOperationException("Binding can not be null");

        _binding = value;

        if (_binding.RelativeSource != null)
          throw new Exception();
        if (_binding.Source != null)
          throw new Exception();
        if (_binding.Source != null)
          throw new Exception();

        _commonBindingProperties = new CommonBindingProperties();
        _commonBindingProperties.CopyFromBinding(_binding);
      }
    }

    internal object Source
    {
      get => _source;
      set
      {
        if (ReferenceEquals(_source, value))
          return;

        _source = value;

        if (_binding == null)
          throw new InvalidOperationException("Binding can not be null");

        if (_source != null)
        {
          var binding = new Binding
          {
            Path = _binding.Path,
            Source = _source
          };
          _commonBindingProperties.InitBinding(binding);

          this.SetBinding(ValueProperty, binding);
        }
        else
          ClearValue(ValueProperty);
      }
    }

    #endregion
  }

  [ContentProperty("ChainElements")]
  public class BindingChain : AssetBase, ISupportInitialize
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SourceProperty = DPM.Register<object, BindingChain>
      ("Source", c => c.OnSourceChanged);

    private static readonly DependencyProperty ValueProperty = DPM.Register<object, BindingChain>
      ("Value", c => c.OnValueChanged);

    #endregion

    #region Fields

    private readonly DependencyObjectCollectionBase<BindingChainElement> _bindingChainElements = new DependencyObjectCollectionBase<BindingChainElement>();

    private bool _isInitialized;
    private bool _isInitializing;

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Properties

    public object ActualValue => GetValue(ValueProperty);

    public DependencyObjectCollectionBase<BindingChainElement> ChainElements => _bindingChainElements;

    public object Source
    {
      get => GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    #endregion

    #region  Methods

    private void InitChain()
    {
      var currentSource = Source;
      foreach (var element in _bindingChainElements)
      {
        element.Source = currentSource;
        currentSource = element;
      }

      var lastChainElement = _bindingChainElements.LastOrDefault();
      if (lastChainElement != null)
        this.SetBinding(ValueProperty, new Binding {Path = new PropertyPath(Asset.ValueProperty), Source = lastChainElement});
    }

    private void OnSourceChanged()
    {
      if (_isInitialized)
        InitChain();
    }

    private void OnValueChanged()
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ActualValue"));
    }

    public void BeginInit()
    {
      _isInitializing = true;
    }

    public void EndInit()
    {
      var src = ReadLocalValue(SourceProperty);
      _isInitializing = false;
      _isInitialized = true;

      if (Source != null)
        InitChain();
    }

    #endregion
  }

  internal interface IInheritanceContext
  {
    #region Properties

    DependencyObject Owner { get; set; }

    #endregion

    #region  Methods

    void Attach(DependencyObject depObj);
    void Detach(DependencyObject depObj);

    #endregion
  }

  internal class InheritanceContext : IInheritanceContext
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty ContextProperty = DPM.RegisterAttached<DependencyObject, InheritanceContext>
      ("Context");

    #endregion

    #region Fields

    private readonly DependencyObjectCollectionBase<DependencyObject> _contextObjects = new DependencyObjectCollectionBase<DependencyObject>();
    private DependencyObject _owner;

    #endregion

    #region Properties

    public DependencyObject Owner
    {
      get => _owner;
      set
      {
        if (ReferenceEquals(_owner, value))
          return;

        _owner?.ClearValue(ContextProperty);

        _owner = value;

        _owner?.SetValue(ContextProperty, _contextObjects);
      }
    }

    #endregion

    #region  Methods

    public void Attach(DependencyObject depObj)
    {
      _contextObjects.Add(depObj);
    }

    public void Detach(DependencyObject depObj)
    {
      _contextObjects.Remove(depObj);
    }

    #endregion
  }

  internal interface IInheritanceContextOwner
  {
    #region Properties

    IInheritanceContext InheritanceContext { get; }

    #endregion
  }


  public class AssetCollection : InheritanceContextDependencyObjectCollection<InheritanceContextObject>
  {
    #region Ctors

    internal AssetCollection(FrameworkElement frameworkElement)
    {
      Owner = frameworkElement;
    }

    #endregion
  }

  public class AssetBase : InheritanceContextObject, ISupportInitialize
  {
    #region Properties

#if INTERACTIVITY_DEBUG
    public bool Debug { get; set; }
#endif

		protected bool Initializing { get; private set; }

    #endregion

    protected virtual void BeginInitCore()
    {
    }

    protected virtual void EndInitCore()
    {
    }

    void ISupportInitialize.BeginInit()
    {
	    Initializing = true;

	    BeginInitCore();
    }

    void ISupportInitialize.EndInit()
    {
	    Initializing = false;

	    EndInitCore();
    }
  }

  [ContentProperty("Value")]
  public class Asset : AssetBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ValueProperty = DPM.Register<object, Asset>
      ("Value", t => t.OnValueChanged);

    #endregion

    #region Properties

    public object Value
    {
      get => GetValue(ValueProperty);
      set => SetValue(ValueProperty, value);
    }

    #endregion

    #region  Methods

    private void OnValueChanged(object oldValue, object newValue)
    {
    }

    #endregion
  }
}