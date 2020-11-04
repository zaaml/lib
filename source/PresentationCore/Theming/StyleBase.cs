// <copyright file="StyleBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.Core.Interfaces;
using Zaaml.Core.Packed;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore.Interactivity;
using Setter = Zaaml.PresentationCore.Interactivity.Setter;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;
using TriggerBase = Zaaml.PresentationCore.Interactivity.TriggerBase;

namespace Zaaml.PresentationCore.Theming
{
  [TypeConverter(typeof(StyleTypeConverter))]
  public abstract class StyleBase : ISealable
  {
    #region Static Fields and Constants

    private static readonly ITreeEnumeratorAdvisor<SetterBase> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<SetterBase>(GetChildrenEnumerator);

    #endregion

    #region Fields

    private StyleBase _basedOnCore;

    private uint _packedValue;
    private Type _targetType;


    internal event EventHandler StyleInheritanceChanged;

    #endregion

    #region Ctors

    internal StyleBase()
    {
      StyleService = new StyleService(this);
    }

    #endregion

    #region Properties

    internal IEnumerable<SetterBase> ActualSetters => MergeSetters(ReversedBaseStylesAndSelf);

    internal IEnumerable<TriggerBase> ActualTriggers => MergeTriggers(ReversedBaseStylesAndSelf);

    protected virtual StyleBase BasedOnCore
    {
      get => _basedOnCore;
      set
      { 
        if (ReferenceEquals(_basedOnCore, value))
          return;

				if (value?.BaseStylesAndSelf.Any(styleBase => ReferenceEquals(styleBase, this)) == true)
					throw new InvalidOperationException("Style deadlock detected");

				if (_basedOnCore != null)
          _basedOnCore.StyleInheritanceChanged -= OnBaseStyleInheritanceChanged;

        _basedOnCore = value;

        ValidateBaseStyles(this);

        if (_basedOnCore != null)
          _basedOnCore.StyleInheritanceChanged += OnBaseStyleInheritanceChanged;

        if (value != null && value.IsInitialized == false)
          value.Initialize();

        OnStyleInheritanceChanged();
      }
    }

    internal string BaseResourceKey => BaseStylesAndSelf.Select(s => s.ResourceKey).FirstOrDefault(r => r != null);

    internal Uri BaseSource => BaseStylesAndSelf.Select(s => s.Source).FirstOrDefault(u => u != null);

    protected IEnumerable<StyleBase> BaseStylesAndSelf => EnumerateBaseStylesAndSelf(this);

#if INTERACTIVITY_DEBUG
    public bool Debug
    {
      get => StyleService.Debug;
      set => StyleService.Debug = value;
    }
#endif

    protected bool IsInitialized
    {
      get => PackedDefinition.IsInitialized.GetValue(_packedValue);
      private set => PackedDefinition.IsInitialized.SetValue(ref _packedValue, value);
    }

    private bool IsSealed
    {
      get => PackedDefinition.IsSealed.GetValue(_packedValue);
      set => PackedDefinition.IsSealed.SetValue(ref _packedValue, value);
    }

    internal bool IsThemeBased => EnumerateBaseStylesAndSelf(this).Any(s => s.IsThemeBasedCore);

    protected virtual bool IsThemeBasedCore => false;

    internal string ResourceKey { get; set; }

    protected IEnumerable<StyleBase> ReversedBaseStylesAndSelf => BaseStylesAndSelf.Reverse();

    protected abstract IEnumerable<SetterBase> SettersCore { get; }

    internal Uri Source { get; set; }

    internal virtual StyleService StyleService { get; }

    public virtual Type TargetType
    {
      get => _targetType;
      set
      {
        _targetType = value;

        if (_targetType != null)
          BasedOnCore?.ApplyTargetType(TargetType);

        if (IsInitialized == false)
          Initialize();

        ValidateBaseStyles(this);
      }
    }

    protected abstract IEnumerable<TriggerBase> TriggersCore { get; }

    #endregion

    #region  Methods

    protected void ApplyTargetType(Type targetType)
    {
      if (TargetType == null)
        TargetType = targetType;
    }

    internal void EnsureDeferredStylesLoaded()
    {
      var current = this;
      bool nextLoop;

      do
      {
        nextLoop = false;

        foreach (var baseStyle in EnumerateBaseStylesAndSelf(current).OfType<ThemeStyle>())
        {
          if (baseStyle.IsDeferred == false || baseStyle.Owner == null)
          {
            current = baseStyle;

            continue;
          }

          baseStyle.Owner.EnsureDeferredStylesLoaded(baseStyle);
          nextLoop = true;

          break;
        }
      } while (nextLoop);
    }

    internal static IEnumerable<StyleBase> EnumerateBaseStyles(StyleBase style)
    {
      while ((style = style.BasedOnCore) != null)
        yield return style;
    }

    internal static IEnumerable<StyleBase> EnumerateBaseStylesAndSelf(StyleBase style)
    {
      yield return style;

      foreach (var baseStyle in EnumerateBaseStyles(style))
        yield return baseStyle;
    }

    internal static IEnumerable<Setter> EnumerateSetters(IEnumerable<SetterBase> setters)
    {
      return TreeEnumerator.GetEnumerator(setters, TreeEnumeratorAdvisor).Enumerate().OfType<Setter>();
    }

    internal static IEnumerable<Setter> FlattenSetters(IEnumerable<SetterBase> setters)
    {
      return TreeEnumerator.GetEnumerator(setters, TreeEnumeratorAdvisor).Enumerate().OfType<Setter>().Select(s => s.Flatten());
    }

    private static IEnumerator<SetterBase> GetChildrenEnumerator(SetterBase setter)
    {
      var groupSetter = setter as SetterGroup;

      return groupSetter?.ActualSetters.GetEnumerator() ?? Enumerable.Empty<SetterBase>().GetEnumerator();
    }

    protected virtual void Initialize()
    {
      IsInitialized = true;
    }

    protected static IEnumerable<SetterBase> MergeSetters(IEnumerable<StyleBase> styles)
    {
      return MergeSetters(styles.SelectMany(s => s.SettersCore));
    }

    internal static IEnumerable<SetterBase> MergeSetters(IEnumerable<SetterBase> setters)
    {
      var concurrentSettersDict = new Dictionary<DependencyProperty, Setter>();

      foreach (var setter in FlattenSetters(setters))
      {
        var dependencyProperty = setter.MergeDependencyProperty;

        if (dependencyProperty == null)
          continue;

        concurrentSettersDict[dependencyProperty] = setter;
      }

      return concurrentSettersDict.Values.Select(s => s.Optimize()).ToList();
    }

    protected static IEnumerable<TriggerBase> MergeTriggers(IEnumerable<StyleBase> styles)
    {
      return MergeTriggers(styles.SelectMany(s => s.TriggersCore));
    }

    internal static IEnumerable<TriggerBase> MergeTriggers(IEnumerable<TriggerBase> triggers)
    {
      return triggers.Select(t => t.DeepClone<TriggerBase>());
    }

    internal virtual void OnAttached(FrameworkElement target)
    {
    }

    internal virtual void OnAttaching(FrameworkElement target)
    {
    }

    private void OnBaseStyleInheritanceChanged(object sender, EventArgs eventArgs)
    {
      OnStyleInheritanceChanged();
    }

    internal virtual void OnDetached(FrameworkElement target)
    {
    }

    internal virtual void OnDetaching(FrameworkElement frameworkElement)
    {
    }
    
    protected virtual void OnStyleInheritanceChanged()
    {
      StyleService.OnStyleInheritanceChanged();
      StyleInheritanceChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Seal()
    {
      IsSealed = true;
    }

    public override string ToString()
    {
      var baseResourceKey = BaseResourceKey;
      var baseSource = BaseSource;

      var resKey = baseResourceKey != null ? $" {{{baseResourceKey}}}" : "";
      var source = baseSource != null ? $" {{{baseSource}}}" : "";

      if (baseResourceKey != null && baseSource != null)
      {
        resKey = resKey.TrimEnd('}', ' ') + " : " + source.TrimStart('{', ' ');
        source = "";
      }

      return $"{GetType().Name} - {TargetType?.Name ?? "Undefined"}" + resKey + source;
    }

    private static bool ValidateBasedOn(Type targetType, Type basedOnTargetType)
    {
      return basedOnTargetType.IsAssignableFrom(targetType);
    }

    protected static void ValidateBaseStyles(StyleBase style)
    {
      var currentTargetType = style.TargetType ?? typeof(object);

      foreach (var baseStyle in style.BaseStylesAndSelf)
      {
        if (currentTargetType == null)
        {
          currentTargetType = baseStyle.TargetType;
          style = baseStyle;

          continue;
        }

        var baseTargetType = baseStyle.TargetType ?? typeof(object);

        if (ValidateBasedOn(currentTargetType, baseTargetType))
        {
          currentTargetType = baseStyle.TargetType;
          style = baseStyle;

          continue;
        }

        throw new InvalidOperationException($"Can only base on a Style with target type that is base type of this style's target type. This Style '{style}', BasedOn Style '{style.BasedOnCore}'");
      }
    }

    #endregion

    #region Interface Implementations

    #region ISealable

    bool ISealable.IsSealed => IsSealed;

    void ISealable.Seal()
    {
      Seal();
    }

    #endregion

    #endregion

    #region  Nested Types

    private static class PackedDefinition
    {
      #region Static Fields and Constants

      public static readonly PackedBoolItemDefinition IsSealed;
      public static readonly PackedBoolItemDefinition IsInitialized;

      #endregion

      #region Ctors

      static PackedDefinition()
      {
        var allocator = new PackedValueAllocator();

        IsSealed = allocator.AllocateBoolItem();
        IsInitialized = allocator.AllocateBoolItem();
      }

      #endregion
    }

    #endregion
  }
}