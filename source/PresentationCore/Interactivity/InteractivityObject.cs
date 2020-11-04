// <copyright file="InteractivityObject.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Pools;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.MarkupExtensions;
using Zaaml.PresentationCore.Utils;

#if SILVERLIGHT
using System.Collections;
#else

#endif

#pragma warning disable 108

namespace Zaaml.PresentationCore.Interactivity
{
#if !SILVERLIGHT
  [XamlSetMarkupExtension("SetMarkupExtensionHandler")]
#endif
  //public abstract class InteractivityObject : MarkupExtensionBase, IInteractivityObject, ISupportInitialize
  public abstract class InteractivityObject : MarkupExtensionBase, IInteractivityObject, IXamlRootOwner
  {
    #region Static Fields and Constants

#if INTERACTIVITY_DEBUG
    private static int _nextId;
#endif

    private static readonly PackedHierarchicalAllocator<InteractivityObject> TypeAllocator = new PackedHierarchicalAllocator<InteractivityObject>();
    private static readonly ITreeEnumeratorAdvisor<InteractivityObject> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<InteractivityObject>(GetChildrenEnumerator);
    private static readonly LightObjectPool<Stack<InteractivityObject>> ApiChangePropagationStackPool = new LightObjectPool<Stack<InteractivityObject>>(() => new Stack<InteractivityObject>(), DummyAction<Stack<InteractivityObject>>.Instance, s => s.Clear());

    #endregion

    #region Fields

    private object _mutableField = Unset.Value;
    private object _parentOrXamlRootStore;
    protected uint PackedValue;

    #endregion

    #region Ctors

    static InteractivityObject()
    {
      RuntimeHelpers.RunClassConstructor(typeof(PackedDefinition).TypeHandle);
    }

    protected InteractivityObject()
    {
#if INTERACTIVITY_DEBUG
      Id = _nextId++;
#endif
    }

    #endregion

    #region Properties

    private object ActualXamlRoot
    {
      get
      {
        var current = (IInteractivityObject) this;

        while (current != null)
        {
          var xamlRootOwner = (IXamlRootOwner) current;
          var xamlRoot = xamlRootOwner.XamlRoot;

          if (xamlRoot != null)
            return xamlRoot;
          current = current.Parent;
        }

        return null;
      }
    }

    internal virtual IEnumerable<InteractivityObject> Children => Enumerable.Empty<InteractivityObject>();

    private bool IgnorePropertyChange
    {
      get => PackedDefinition.IgnorePropertyChange.GetValue(PackedValue);
      set => PackedDefinition.IgnorePropertyChange.SetValue(ref PackedValue, value);
    }

    internal FrameworkElement InteractivityTarget => Root?.InteractivityTarget;

    protected bool IsInStyle => Root is StyleRoot;

    protected internal bool IsLoaded
    {
      get => PackedDefinition.IsLoaded.GetValue(PackedValue);
      set => PackedDefinition.IsLoaded.SetValue(ref PackedValue, value);
    }

    // ReSharper disable once ConvertToAutoProperty
    internal object MutableData
    {
      get => _mutableField;
      set => _mutableField = value;
    }

    internal IInteractivityObject Parent
    {
      get => _parentOrXamlRootStore as IInteractivityObject;
      set
      {
        var xamlRoot = XamlRoot;

        if (xamlRoot != null)
        {
          if (value == null) return;

          var current = value;

          while (current.Parent != null)
            current = current.Parent;

          ((IXamlRootOwner) current).XamlRoot = xamlRoot;

          _parentOrXamlRootStore = value;
        }
        else
          _parentOrXamlRootStore = value ?? ((IXamlRootOwner) Parent)?.ActualXamlRoot;
      }
    }

    internal IInteractivityRoot Root => GetRoot();

    private object XamlRoot
    {
      get => _parentOrXamlRootStore is IInteractivityObject ? null : _parentOrXamlRootStore;
      set => _parentOrXamlRootStore = value;
    }

    #endregion

    #region  Methods

    internal object CacheConvert(InteractivityProperty property, Type targetType, ref object valueStore)
    {
      return property.CacheConvert(this, targetType, ref valueStore);
    }

    private void CopyMembers(InteractivityObject source)
    {
#if INTERACTIVITY_DEBUG
      Debug = source.Debug;
#endif
      CopyMembersOverride(source);
    }

    protected internal virtual void CopyMembersOverride(InteractivityObject source)
    {
    }

    protected abstract InteractivityObject CreateInstance();

    protected internal virtual InteractivityObject DeepClone()
    {
      var instance = CreateInstance();

      instance.CopyMembers(this);

      return instance;
    }

    internal T DeepClone<T>() where T : InteractivityObject
    {
      return (T) DeepClone();
    }

    internal static PackedValueAllocator GetAllocator<T>() where T : InteractivityObject
    {
      return TypeAllocator.GetAllocator<T>();
    }

    private static IEnumerator<InteractivityObject> GetChildrenEnumerator(InteractivityObject interactivityObject)
    {
      return interactivityObject.Children.GetEnumerator();
    }

    internal object GetOriginalValue(InteractivityProperty property, object valueStore)
    {
      return property.GetOriginalValue(this, valueStore);
    }

    private IInteractivityRoot GetRoot()
    {
      var current = Parent;

      while (current != null)
      {
	      if (current is IInteractivityRoot root)
          return root;

        current = current.Parent;
      }

      return null;
    }

    protected T GetService<T>() where T : class
    {
      return (T) Root?.GetService(typeof(T));
    }

    internal object GetValue(InteractivityProperty property, ref object valueStore)
    {
      return property.GetValue(this, ref valueStore);
    }

    protected virtual void InitFromServiceProvider(IServiceProvider serviceProvider)
    {
    }

    internal void Load()
    {
      Load(Root);
    }

    internal void Load(IInteractivityRoot root)
    {
      IsLoaded = true;

      LoadCore(root);
    }

    internal void Load(InteractivityProperty property, ref object valueStore)
    {
      property.Load(this, ref valueStore);
    }

    internal virtual void LoadCore(IInteractivityRoot root)
    {
    }

    protected virtual void OnApiPropertyChanged(string propertyName)
    {
      var stack = ApiChangePropagationStackPool.GetObject();

      stack.Push(this);

      Parent?.OnDescendantApiPropertyChanged(stack, propertyName);

      stack.Pop();

      ApiChangePropagationStackPool.Release(stack);
    }

    protected virtual void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      descendants.Push(this);

      Parent?.OnDescendantApiPropertyChanged(descendants, propertyName);

      descendants.Pop();
    }

    public sealed override object ProvideValue(IServiceProvider serviceProvider)
    {
#if SILVERLIGHT
      object target;
      object targetProperty;
      bool reflected;

      GetTarget(serviceProvider, out target, out targetProperty, out reflected);

      object currentValue;

      if (GetCurrentValue(target, targetProperty, out currentValue))
      {
        var interactivityCollection = currentValue as IList;
        if (interactivityCollection != null)
        {
          interactivityCollection.Add(this);
          return interactivityCollection;
        }
      }
#else

	    GetTarget(serviceProvider, out var target, out _, out _);

      //if (ReferenceEquals(typeof(FrameworkElement).GetProperty("Resources"), targetProperty))
      //{

      //}

      if (target is FrameworkElement frameworkElement)
      {
	      if (this is SetterBase setter)
        {
          var setters = Extension.GetSetters(frameworkElement);

          setters.Add(setter);

          return setters;
        }

	      if (this is TriggerBase trigger)
          return new TriggerCollection(frameworkElement) {trigger};
      }
#endif

      InitFromServiceProvider(serviceProvider);

      _parentOrXamlRootStore = serviceProvider.GetRootObject();

      return this;
    }

    internal static InteractivityProperty RegisterInteractivityProperty(InteractivityPropertyChangedCallback onPropertyChanged, bool isBindable = true)
    {
      return new InteractivityProperty(onPropertyChanged, isBindable);
    }

    internal bool SetMarkupExtension(MemberInfo memberInfo, MarkupExtension extension, IServiceProvider serviceProvider, bool fromBinding)
    {
      var binding = extension as BindingMarkupExtension;

      binding?.FinalizeXamlInitialization(serviceProvider);

      if ((extension is BindingBase || extension is BindingBaseExtension) == false)
        return false;

      var propertyInfo = memberInfo as PropertyInfo;

      if (propertyInfo != null)
      {
        var type = GetType();
        var setMethod = type.GetMethod($"Set{propertyInfo.Name}Property", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

        if (setMethod != null)
        {
          setMethod.Invoke(this, new object[] {extension});

          IgnorePropertyChange = fromBinding;

          return true;
        }
      }

      if (propertyInfo == null || !propertyInfo.PropertyType.IsAssignableFrom(typeof(MarkupExtension)))
        return false;

      propertyInfo.SetValue(this, extension, null);

      return true;
    }

#if !SILVERLIGHT
    [UsedImplicitly]
    private static void SetMarkupExtensionHandler(object sender, XamlSetMarkupExtensionEventArgs args)
    {
      if ((args.MarkupExtension is BindingBase || args.MarkupExtension is BindingBaseExtension) == false)
        return;

      var interactivityObject = sender as InteractivityObject;
      var memberInfo = args.Member?.UnderlyingMember;
      var markupExtension = args.MarkupExtension;

      if (interactivityObject != null && memberInfo != null && markupExtension != null)
        args.Handled = interactivityObject.SetMarkupExtension(memberInfo, markupExtension, args.ServiceProvider, false);
    }
#endif

    internal void SetValue(InteractivityProperty property, ref object valueStore, object value)
    {
      // Value is already assigned at SetMarkupExtension. Skip
      if (IgnorePropertyChange)
      {
        IgnorePropertyChange = false;
        return;
      }

      property.SetValue(this, ref valueStore, value);
    }

    internal void Unload()
    {
      Unload(Root);
    }

    internal void Unload(IInteractivityRoot root)
    {
      UnloadCore(root);

      IsLoaded = false;
    }

    internal void Unload(InteractivityProperty property, ref object value)
    {
      property.Unload(this, ref value);
    }

    internal virtual void UnloadCore(IInteractivityRoot root)
    {
    }

    internal void WalkTree(IInteractivityVisitor visitor)
    {
      foreach (var interactivityObject in TreeEnumeratorAdvisor.GetEnumerator(this).Enumerate())
        visitor.Visit(interactivityObject);
    }

    internal ResourceDictionary ResourceDictionary => XamlRoot as ResourceDictionary;

    internal Uri Uri => ResourceDictionary != null ? ResourceDictionaryUtils.GetActualUri(ResourceDictionary) : null;

    #endregion

    #region Interface Implementations

    #region IInteractivityObject

    void IInteractivityObject.OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
    {
      OnDescendantApiPropertyChanged(descendants, propertyName);
    }

    IInteractivityObject IInteractivityObject.Parent => Parent;

    #endregion

    #region IXamlRootOwner

    object IXamlRootOwner.XamlRoot
    {
      get => XamlRoot;
      set => XamlRoot = value;
    }

    object IXamlRootOwner.ActualXamlRoot => ActualXamlRoot;

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

#if INTERACTIVITY_DEBUG
      public static readonly PackedBoolItemDefinition Debug;
#endif
      public static readonly PackedBoolItemDefinition IsLoaded;
      public static readonly PackedBoolItemDefinition IgnorePropertyChange;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = GetAllocator<InteractivityObject>();

#if INTERACTIVITY_DEBUG
        Debug = allocator.AllocateBoolItem();
#endif
        IsLoaded = allocator.AllocateBoolItem();
        IgnorePropertyChange = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion

#if INTERACTIVITY_DEBUG
    public bool Debug
    {
      get => PackedDefinition.Debug.GetValue(PackedValue);
      set => PackedDefinition.Debug.SetValue(ref PackedValue, value);
    }

    public int Id { get; }
#endif
  }

  internal interface IInteractivityVisitor
  {
    #region  Methods

    void Visit(InteractivityObject interactivityObject);

    #endregion
  }

  [AttributeUsage(AttributeTargets.Property)]
  internal class ApiPropertyAttribute : Attribute
  {
  }
}