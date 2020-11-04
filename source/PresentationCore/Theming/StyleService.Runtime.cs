// <copyright file="StyleService.Runtime.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.Services;
using SetterBase = Zaaml.PresentationCore.Interactivity.SetterBase;
using TriggerBase = Zaaml.PresentationCore.Interactivity.TriggerBase;

namespace Zaaml.PresentationCore.Theming
{
  internal sealed partial class StyleService
  {
    #region  Nested Types

    private class Runtime : ServiceBase<FrameworkElement>, IRuntimeStyleService
    {
      #region Fields

      private readonly StyleService _styleService;
      private RuntimeSetterCollection _runtimeSetters;
      private RuntimeTriggerCollection _runtimeTriggers;

      #endregion

      #region Ctors

      public Runtime(StyleService styleService)
      {
        _styleService = styleService;
        _styleService._services.Add(this);

        Style.EnsureDeferredStylesLoaded();
      }

      #endregion

      #region Properties

      private StyleBase Style => _styleService.Style;

      #endregion

      #region  Methods

      private void Attach()
      {
        var target = Target;
        var interactivityService = target.GetInteractivityService();

        _styleService.EnsureNativeStyle();

        var style = Style;

        style.OnAttaching(target);

        _runtimeSetters = new RuntimeSetterCollection(_styleService.Setters, interactivityService);
        _runtimeSetters.Load();

        // Service has detached
        if (Target == null)
          return;

        _runtimeTriggers = new RuntimeTriggerCollection(_styleService.Triggers, interactivityService);
        _runtimeTriggers.Load();

        style.OnAttached(target);
      }

      private void Detach()
      {
        var target = Target;

        var style = Style;
        style.OnDetaching(target);

        _runtimeSetters?.Unload();
        _runtimeTriggers?.Unload();

        _runtimeSetters = null;
        _runtimeTriggers = null;

        style.OnDetached(target);
      }

      public void ExternalLoadSetter(SetterBase setter)
      {
        if (IsAttached == false) return;

        if (_runtimeSetters.Any(s => s.DependencyProperty == setter.DependencyProperty))
          return;

        _runtimeSetters.Add(setter.DeepClone<SetterBase>());
      }

      protected override void OnAttach()
      {
        base.OnAttach();

        Attach();
      }

      protected override void OnDetach()
      {
        Detach();
        base.OnDetach();
      }

      public void OnStyleServiceStyleChanged()
      {
        if (IsAttached)
          OnDetach();

        if (IsAttached)
          Attach();
      }

      #endregion

      #region Interface Implementations

      #region IRuntimeStyleService

      SetterCollectionBase IRuntimeStyleService.Setters => _runtimeSetters;

      TriggerCollectionBase IRuntimeStyleService.Triggers => _runtimeTriggers;

      #endregion

      #endregion

      #region  Nested Types

      private sealed class RuntimeTriggerCollection : TriggerCollectionBase
      {
        #region Ctors

        internal RuntimeTriggerCollection(StyleServiceTriggerCollection triggers, InteractivityService interactivityServicee) : base(interactivityServicee.ActualStyleRoot)
        {
          foreach (var trigger in triggers)
            Add(trigger.DeepClone<TriggerBase>());
        }

        #endregion

        #region Properties

        private StyleRoot Root => (StyleRoot) Parent;

        #endregion

        #region  Methods

        internal override InteractivityCollection<TriggerBase> CreateInstance(IInteractivityObject parent)
        {
          throw new NotSupportedException();
        }

        internal void Load()
        {
          var root = Root;

          foreach (var trigger in this)
          {
            trigger.Load(root);
            trigger.IsEnabled = true;
          }
        }

        internal void Unload()
        {
          var root = Root;

          foreach (var trigger in this)
          {
            trigger.IsEnabled = false;
            trigger.Unload(root);
          }
        }

        #endregion
      }

      private sealed class RuntimeSetterCollection : SetterCollectionBase
      {
        #region Ctors

        internal RuntimeSetterCollection(StyleServiceSetterCollection setters, InteractivityService interactivityServicee) : base(interactivityServicee.ActualStyleRoot)
        {
          foreach (var setter in setters)
            Add(setter.DeepClone<SetterBase>());
        }

        #endregion

        #region Properties

        protected override bool IsApplied => false;

        private StyleRoot Root => (StyleRoot) Parent;

        #endregion

        #region  Methods

        internal override InteractivityCollection<SetterBase> CreateInstance(IInteractivityObject parent)
        {
          throw new NotSupportedException();
        }

        internal void Load()
        {
          var root = Root;

          foreach (var setter in this)
          {
            setter.Load(root);
            setter.Apply();
          }
        }

        internal void Unload()
        {
          var root = Root;

          foreach (var setter in this)
          {
            setter.Undo();
            setter.Unload(root);
          }
        }

        #endregion
      }

      #endregion
    }

    #endregion
  }

  internal interface IRuntimeStyleService
  {
    #region Properties

    SetterCollectionBase Setters { get; }

    TriggerCollectionBase Triggers { get; }

    #endregion
  }
}