// <copyright file="IconBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Packed;
using Zaaml.Core.Extensions;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  [TypeConverter(typeof(IconTypeConverter))]
  public abstract partial class IconBase
  {
    #region Static Fields and Constants

    private static readonly List<DependencyProperty> BaseProperties = new List<DependencyProperty>
    {
      VerticalAlignmentProperty,
      HorizontalAlignmentProperty,
      WidthProperty,
      HeightProperty,
      MinWidthProperty,
      MinHeightProperty,
      MaxWidthProperty,
      MaxHeightProperty,
      MarginProperty,
      StyleProperty
    };

    private static readonly DependencyPropertyKey PresenterPropertyKey = DPM.RegisterReadOnly<IconPresenterBase, IconBase>
      ("Presenter", i => i.OnPresenterChanged);

    public static readonly DependencyProperty SharedResourceProperty = DPM.Register<bool, IconBase>
      ("SharedResource", false);

    public static readonly DependencyProperty PresenterProperty = PresenterPropertyKey.DependencyProperty;

    protected static readonly Dictionary<DependencyProperty, Func<IconBase>> Factories = new Dictionary<DependencyProperty, Func<IconBase>>();

    #endregion

    #region Fields

    private WeakLinkedList<IconBase> _clones;
    private byte _packedValue;
#if SILVERLIGHT
    private IconBase _treeDetachedIcon;
#endif

    #endregion

    #region Ctors

    protected IconBase()
    {
      AttachedIcon = new AttachedIconObject(this);
    }

    #endregion

    #region Properties

    private AttachedIconObject AttachedIcon { get; }

    private WeakLinkedList<IconBase> Clones => _clones ??= new WeakLinkedList<IconBase>();

    protected internal abstract FrameworkElement IconElement { get; }

    private bool IsBusy
    {
      get => PackedDefinition.IsBusy.GetValue(_packedValue);
      set => PackedDefinition.IsBusy.SetValue(ref _packedValue, value);
    }

    public IconPresenterBase Presenter
    {
      get => (IconPresenterBase) GetValue(PresenterProperty);
      internal set => this.SetReadOnlyValue(PresenterPropertyKey, value);
    }

    protected abstract IEnumerable<DependencyProperty> PropertiesCore { get; }

    public bool SharedResource
    {
      get => (bool) GetValue(SharedResourceProperty);
      set => SetValue(SharedResourceProperty, value);
    }

#if SILVERLIGHT
    internal IconBase TreeDetachedIcon => SharedResource == false ? this : _treeDetachedIcon ?? (_treeDetachedIcon = CloneImpl(true));
#endif

    #endregion

    #region  Methods

    internal IconBase Clone(bool asFrozen)
    {
      var clone = CloneImpl(SharedResource);

      if (asFrozen == false)
        Clones.Add(clone);

      return clone;
    }

    private IconBase CloneImpl(bool cloneBindings)
    {
      var clone = CreateInstanceCore();

#if SILVERLIGHT
      clone._treeDetachedIcon = _treeDetachedIcon ?? clone;
#endif

      CopyCoreProperties(clone, cloneBindings);

      return clone;
    }

    protected virtual void CopyAttachedProperties(DependencyObject dependencyObject)
    {
      foreach (var property in BaseProperties)
        SetValue(property, CopyValue(dependencyObject.GetValue(property)));
    }

    private void CopyCoreProperties(IconBase clone, bool cloneBindings)
    {
      CopyProperties(clone, BaseProperties, cloneBindings);
      CopyProperties(clone, PropertiesCore, cloneBindings);
    }

    protected IconBase CopyProperties(IconBase clone, IEnumerable<DependencyProperty> properties, bool cloneBindings)
    {
      foreach (var property in properties)
        CopyProperty(clone, property, cloneBindings);

      return clone;
    }

    private void CopyProperty(DependencyObject target, DependencyProperty property, bool cloneBindings)
    {
      if (this.IsDefaultValue(property))
	      return;

      var localValue = ReadLocalValue(property);

      if (cloneBindings && localValue is BindingExpression localBindingExpression)
        target.SetBinding(property, CopyBinding(localBindingExpression.ParentBinding));
      else
        target.SetValue(property, CopyValue(GetValue(property)));
    }

    protected virtual object CopyValue(object value)
    {
      return value;
    }

    protected virtual Binding CopyBinding(Binding binding)
    {
      return binding;
    }

    protected abstract IconBase CreateInstanceCore();

    private AttachedIconObject EnsureAttachedIcon(DependencyObject owner)
    {
      AttachedIcon.Owner = owner;

      return AttachedIcon;
    }

    protected T GetActualValue<T>(DependencyProperty dependencyProperty)
    {
      if (this.IsDefaultValue(dependencyProperty) == false)
        return (T) GetValue(dependencyProperty);

      return (T) AttachedIcon.GetActualValue(dependencyProperty);
    }

    protected virtual void OnAttachedPropertyChanged(DependencyProperty dependencyProperty)
    {
    }

    protected virtual void OnIconPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    protected static void OnIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
	    if (d is AttachedIconObject attachedIcon)
      {
        attachedIcon.Icon.OnIconPropertyChanged(e);

        return;
      }

	    if (d is IconBase icon)
        icon.OnIconPropertyChangedCore(e, false);
      else
      {
	      if (d is IconPresenterBase iconPresenter)
        {
          iconPresenter.ActualIconInternal?.AttachedIcon.UpdateActualValue(e.Property, e.NewValue);

          return;
        }

	      if (d is IIconPresenter iiconPresenter)
	      {
		      iiconPresenter.Icon?.AttachedIcon.UpdateActualValue(e.Property, e.NewValue);

		      return;
	      }

	      if (!(d is IIconOwner iconOwner))
          return;

        var ownerIcon = iconOwner.Icon;

        if (ownerIcon == null)
        {
          var factory = Factories.GetValueOrDefault(e.Property);

          if (factory == null)
            return;

          iconOwner.Icon = ownerIcon = factory();

#if SILVERLIGHT
          ownerIcon._treeDetachedIcon = ownerIcon;
#endif

          ownerIcon.EnsureAttachedIcon(d).UpdateActualValue(e.Property, e.NewValue);
        }
        else
          ownerIcon.EnsureAttachedIcon(d).UpdateActualValue(e.Property, e.NewValue);
      }
    }

    private void OnIconPropertyChangedCore(DependencyPropertyChangedEventArgs e, bool isAttached)
    {
      OnIconPropertyChanged(e);

      if (_clones == null || isAttached)
        return;

#if SILVERLIGHT
      _treeDetachedIcon?.OnIconPropertyChanged(e);
#endif

      foreach (var iconBase in _clones)
        iconBase.OnIconPropertyChanged(e);
    }

    private void OnPresenterChanged(IconPresenterBase oldPresenter, IconPresenterBase newPresenter)
    {
      AttachedIcon.Presenter = newPresenter;
    }

    public static implicit operator IconBase(ImageSource x)
    {
      return new BitmapIcon {Source = x};
    }

    public static implicit operator IconBase(PathGeometry x)
    {
      return new PathIcon {Data = x};
    }

    public static implicit operator IconBase(DataTemplate x)
    {
      return new TemplateIcon {Template = x};
    }

    internal static void UseIcon(ref IconBase iconStore, IconBase icon, Panel panel)
    {
      if (ReferenceEquals(iconStore, icon))
        return;

      if (iconStore != null)
      {
        iconStore.IsBusy = false;
      
        panel.Children.Remove(iconStore);

				LogicalChildMentor.AttachLogical(iconStore);
      }

#if SILVERLIGHT
      var treeDetached = icon?.TreeDetachedIcon;
#else
      var treeDetached = icon;
#endif
      if (treeDetached == null)
        return;

      iconStore = treeDetached.IsBusy ? icon.Clone(false) : treeDetached;

      iconStore.IsBusy = true;

      LogicalChildMentor.DetachLogical(iconStore);

			panel.Children.Add(iconStore);
    }

    #endregion

    #region  Nested Types

    private class AttachedIconObject : DependencyObject
    {
      #region Static Fields and Constants

      private static readonly AttachedIconObject Default = new AttachedIconObject(null);

      #endregion

      #region Fields

      private DependencyObject _owner;
      private IconPresenterBase _presenter;

      #endregion

      #region Ctors

      public AttachedIconObject(IconBase icon)
      {
        Icon = icon;
      }

      #endregion

      #region Properties

      public IconBase Icon { get; }

      public DependencyObject Owner
      {
        set
        {
          if (ReferenceEquals(_owner, value))
            return;

          _owner = value;

          UpdateAttachedProperties();
        }
      }

      public IconPresenterBase Presenter
      {
        set
        {
          if (ReferenceEquals(_presenter, value))
            return;

          _presenter = value;

          UpdateAttachedProperties();
        }
      }

      #endregion

      #region  Methods

      public object GetActualValue(DependencyProperty property)
      {
        return GetActualValueInt(property, null);
      }

      private object GetActualValueInt(DependencyProperty property, object value)
      {
        if (_presenter != null && _presenter.IsDefaultValue(property) == false)
          return _presenter.GetValue(property);

        if (_owner != null && _owner.IsDefaultValue(property) == false)
          return _owner.GetValue(property);

        return value ?? GetValue(property);// Default.GetValue(property);
      }

      public void UpdateActualValue(DependencyProperty property, object value)
      {
        SetValue(property, GetActualValueInt(property, value));
      }

      private void UpdateAttachedProperties()
      {
        foreach (var property in Icon.PropertiesCore)
          SetValue(property, GetActualValue(property));
      }

      #endregion
    }

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsBusy;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsBusy = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }

  internal interface IIconOwner
  {
    #region Properties

    IconBase Icon { get; set; }

    #endregion
  }

  public abstract class IconSelector
  {
    #region  Methods

    protected abstract IconBase Select(object itemSource);

    #endregion
  }
}