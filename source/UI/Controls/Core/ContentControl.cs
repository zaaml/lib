// <copyright file="ContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;
using Zaaml.UI.Extensions;
using NativeContentControl = System.Windows.Controls.ContentControl;
using NativeVisualStateManager = System.Windows.VisualStateManager;
using TriggerCollection = Zaaml.PresentationCore.Interactivity.TriggerCollection;

namespace Zaaml.UI.Controls.Core
{
	public partial class ContentControl : NativeContentControl, IContentControl, ILogicalOwner
	{
		private static readonly DependencyPropertyKey ActualHasContentPropertyKey = DPM.RegisterReadOnly<bool, ContentControl>
			("ActualHasContent", false);

		public static readonly DependencyProperty EmptyVisibilityProperty = DPM.Register<Visibility, ContentControl>
			("EmptyVisibility", Visibility.Visible, c => c.OnEmptyVisibilityChanged);

		private static readonly DependencyPropertyKey ContentBehaviorsPropertyKey = DPM.RegisterReadOnly<BehaviorCollection, ContentControl>
			("ContentBehaviorsPrivate");

		private static readonly DependencyPropertyKey ContentSettersPropertyKey = DPM.RegisterReadOnly<SetterCollection, ContentControl>
			("ContentSettersPrivate");

		private static readonly DependencyPropertyKey ContentTriggersPropertyKey = DPM.RegisterReadOnly<TriggerCollection, ContentControl>
			("ContentTriggersPrivate");

		public static readonly DependencyProperty ContentTriggersProperty = ContentTriggersPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ContentBehaviorsProperty = ContentBehaviorsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ContentSettersProperty = ContentSettersPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ActualHasContentProperty = ActualHasContentPropertyKey.DependencyProperty;

		private ContentPresenter _contentPresenter;
		private LogicalChildMentor<ContentControl> _logicalChildMentor;

		static ContentControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ContentControl>();
		}

		public ContentControl()
		{
			this.OverrideStyleKey<ContentControl>();

			PlatformCtor();

			Update();

			IsEnabledChanged += (sender, args) => OnIsEnabledChanged();
		}

		public bool ActualHasContent
		{
			get => (bool) GetValue(ActualHasContentProperty);
			private set => this.SetReadOnlyValue(ActualHasContentPropertyKey, value);
		}

		public BehaviorCollection ContentBehaviors => this.GetValueOrCreate(ContentBehaviorsPropertyKey, CreateBehaviorCollection);

		private BehaviorCollection ContentBehaviorsPrivate => (BehaviorCollection) GetValue(ContentBehaviorsProperty);

		private ContentPresenter ContentPresenter
		{
			get => _contentPresenter;
			set
			{
				if (ReferenceEquals(_contentPresenter, value))
					return;

				if (_contentPresenter != null)
				{
					_contentPresenter.ContentBehaviorCollection = null;
					_contentPresenter.ContentSetterCollection = null;
				}

				_contentPresenter = value;

				if (_contentPresenter != null)
				{
					_contentPresenter.ContentBehaviorCollection = ContentBehaviorsPrivate;
					_contentPresenter.ContentSetterCollection = ContentSettersPrivate;
				}
			}
		}

		public SetterCollection ContentSetters => this.GetValueOrCreate(ContentSettersPropertyKey, CreateSetterCollection);

		private SetterCollection ContentSettersPrivate => (SetterCollection) GetValue(ContentSettersProperty);

		public TriggerCollection ContentTriggers => this.GetValueOrCreate(ContentTriggersPropertyKey, CreateTriggerCollection);

		public Visibility EmptyVisibility
		{
			get => (Visibility) GetValue(EmptyVisibilityProperty);
			set => SetValue(EmptyVisibilityProperty, value);
		}

		private protected LogicalChildMentor LogicalChildMentor => _logicalChildMentor ??= LogicalChildMentor.Create(this);

		protected override IEnumerator LogicalChildren => _logicalChildMentor?.GetLogicalChildren(base.LogicalChildren) ?? base.LogicalChildren;

		private BehaviorCollection CreateBehaviorCollection()
		{
			var behaviorCollection = new BehaviorCollection(null);

			if (ContentPresenter != null)
				ContentPresenter.ContentBehaviorCollection = behaviorCollection;

			return behaviorCollection;
		}

		private SetterCollection CreateSetterCollection()
		{
			var setterCollection = new SetterCollection();

			if (ContentPresenter != null)
				ContentPresenter.ContentSetterCollection = setterCollection;

			return setterCollection;
		}

		private TriggerCollection CreateTriggerCollection()
		{
			var triggerCollection = new TriggerCollection();

			if (ContentPresenter != null)
				ContentPresenter.ContentTriggerCollection = triggerCollection;

			return triggerCollection;
		}

		internal DependencyObject GetTemplateChildInternal(string name)
		{
			return GetTemplateChild(name);
		}

		protected virtual bool GotoVisualState(string stateName, bool useTransitions)
		{
			return NativeVisualStateManager.GoToState(this, stateName, useTransitions);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return this.OnMeasureOverride(base.MeasureOverride, availableSize);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			ContentPresenter = GetTemplateChild("ContentPresenter") as ContentPresenter;

			UpdateVisualState(false);
		}

		protected override void OnContentChanged(object oldContent, object newContent)
		{
			base.OnContentChanged(oldContent, newContent);

			Update();
		}

		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);

			Update();
		}

		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);

			Update();
		}

		private void OnEmptyVisibilityChanged()
		{
			UpdateVisibility();
		}

		protected virtual void OnIsEnabledChanged()
		{
			UpdateVisualState(true);
		}

		protected virtual void OnLoaded()
		{
		}

		protected virtual void OnUnloaded()
		{
		}

		partial void PlatformCtor();

		private void Update()
		{
			UpdateActualHasContent();
			UpdateVisibility();
		}

		private void UpdateActualHasContent()
		{
			var actualHasContent = Content != null || ContentTemplate != null || ContentTemplateSelector != null;

			if (ActualHasContent != actualHasContent)
				ActualHasContent = actualHasContent;
		}

		private void UpdateVisibility()
		{
			if (this.GetValueSource(VisibilityProperty) != PropertyValueSource.Default)
				return;

			var visibility = ActualHasContent ? Visibility.Visible : EmptyVisibility;

			if (Visibility != visibility)
				this.SetCurrentValueInternal(VisibilityProperty, visibility);
		}

		protected virtual void UpdateVisualState(bool useTransitions)
		{
			this.UpdateVisualGroups(useTransitions);
		}

		DependencyProperty IContentControl.ContentProperty => ContentProperty;

		DependencyProperty IContentControl.ContentTemplateProperty => ContentTemplateProperty;

		DependencyProperty IContentControl.ContentStringFormatProperty => ContentStringFormatProperty;

		DependencyProperty IContentControl.ContentTemplateSelectorProperty => ContentTemplateSelectorProperty;

		void ILogicalOwner.AddLogicalChild(object child)
		{
			AddLogicalChild(child);
		}

		void ILogicalOwner.RemoveLogicalChild(object child)
		{
			RemoveLogicalChild(child);
		}

		IEnumerator ILogicalOwner.BaseLogicalChildren => base.LogicalChildren;
	}
}