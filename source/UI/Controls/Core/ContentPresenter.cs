// <copyright file="ContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interactivity;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using TriggerCollection = Zaaml.PresentationCore.Interactivity.TriggerCollection;

namespace Zaaml.UI.Controls.Core
{
#if SILVERLIGHT
  public partial class ContentPresenter : System.Windows.Controls.ContentPresenter
#else
	public class ContentPresenter : System.Windows.Controls.ContentPresenter
#endif
	{
		private static readonly DependencyPropertyKey ActualHasContentPropertyKey = DPM.RegisterReadOnly<bool, ContentPresenter>
			("ActualHasContent", false, c => c.OnActualHasContentPropertyChanged);

		public static readonly DependencyProperty EmptyVisibilityProperty = DPM.Register<Visibility, ContentPresenter>
			("EmptyVisibility", Visibility.Visible, c => c.OnEmptyVisibilityChanged);

		public static readonly DependencyProperty ActualHasContentProperty = ActualHasContentPropertyKey.DependencyProperty;

		private SetterCollection _actualSetters;

		private TriggerCollection _actualTriggers;
		private FrameworkElement _actualVisualChild;
		private BehaviorCollection _contentBehaviorCollection;
		private SetterCollection _contentSetterCollection;
		private TriggerCollection _contentTriggerCollection;

		public ContentPresenter()
		{
			UpdateActualHasContent();
			UpdateVisibility();
		}

		public bool ActualHasContent
		{
			get => (bool) GetValue(ActualHasContentProperty);
			private set => this.SetReadOnlyValue(ActualHasContentPropertyKey, value);
		}

		internal BehaviorCollection ContentBehaviorCollection
		{
			get => _contentBehaviorCollection;
			set
			{
				if (ReferenceEquals(_contentBehaviorCollection, value))
					return;

				DetachBehaviors(VisualChild, _contentBehaviorCollection);

				_contentBehaviorCollection = value;

				AttachBehaviors(VisualChild, _contentBehaviorCollection);
			}
		}

		internal SetterCollection ContentSetterCollection
		{
			get => _contentSetterCollection;
			set
			{
				if (ReferenceEquals(_contentSetterCollection, value))
					return;

				DetachSetters(VisualChild, _contentSetterCollection);

				_contentSetterCollection = value;

				AttachSetters(VisualChild, _contentSetterCollection);
			}
		}

		internal TriggerCollection ContentTriggerCollection
		{
			get => _contentTriggerCollection;
			set
			{
				if (ReferenceEquals(_contentTriggerCollection, value))
					return;

				DetachTriggers(VisualChild, _contentTriggerCollection);

				_contentTriggerCollection = value;

				AttachTriggers(VisualChild, _contentTriggerCollection);
			}
		}

		public Visibility EmptyVisibility
		{
			get => (Visibility) GetValue(EmptyVisibilityProperty);
			set => SetValue(EmptyVisibilityProperty, value);
		}

		private FrameworkElement VisualChild
		{
			get => _actualVisualChild;
			set
			{
				if (ReferenceEquals(_actualVisualChild, value))
					return;

				DetachSetters(_actualVisualChild, ContentSetterCollection);
				DetachTriggers(_actualVisualChild, ContentTriggerCollection);
				DetachBehaviors(_actualVisualChild, ContentBehaviorCollection);

				_actualVisualChild = value;

				if (ContentBehaviorCollection != null)
					ContentBehaviorCollection.FrameworkElement = _actualVisualChild;

				AttachBehaviors(_actualVisualChild, ContentBehaviorCollection);
				AttachTriggers(_actualVisualChild, ContentTriggerCollection);
				AttachSetters(_actualVisualChild, ContentSetterCollection);
			}
		}

		private void AttachBehaviors(FrameworkElement element, BehaviorCollection behaviors)
		{
			if (element == null || behaviors == null)
				return;

			behaviors.FrameworkElement = element;
		}

		private void AttachSetters(FrameworkElement element, SetterCollection setters)
		{
			if (element == null || setters == null)
				return;

			_actualSetters = setters.DeepClone(element);
			_actualSetters.Load();
		}

		private void AttachTriggers(FrameworkElement element, TriggerCollection triggers)
		{
			if (element == null || triggers == null)
				return;

			_actualTriggers = triggers.DeepClone(element);
			_actualTriggers.Load();
		}

		private void DetachBehaviors(FrameworkElement element, BehaviorCollection behaviors)
		{
			if (element == null || behaviors == null)
				return;

			behaviors.FrameworkElement = null;
		}

		private void DetachSetters(FrameworkElement element, SetterCollection setters)
		{
			if (element == null || setters == null)
				return;

			_actualSetters.Unload();
			_actualSetters = null;
		}

		private void DetachTriggers(FrameworkElement element, TriggerCollection triggers)
		{
			if (element == null || triggers == null)
				return;

			_actualTriggers.Unload();
			_actualTriggers = null;
		}

		private void OnActualHasContentPropertyChanged()
		{
			UpdateVisibility();
		}

		protected override void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat)
		{
			base.OnContentStringFormatChanged(oldContentStringFormat, newContentStringFormat);

			UpdateActualHasContent();
		}

		protected override void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
		{
			base.OnContentTemplateChanged(oldContentTemplate, newContentTemplate);

			UpdateActualHasContent();
		}

		protected override void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			base.OnContentTemplateSelectorChanged(oldContentTemplateSelector, newContentTemplateSelector);

			UpdateActualHasContent();
		}

		private void OnEmptyVisibilityChanged()
		{
			UpdateVisibility();
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);

			if (ReferenceEquals(VisualChild, visualRemoved))
				VisualChild = null;

			if (visualAdded != null)
				VisualChild = visualAdded as FrameworkElement;

			UpdateActualHasContent();
		}

		private void UpdateActualHasContent()
		{
			var actualHasContent = Content != null || ContentTemplate != null || ContentTemplateSelector != null || ContentStringFormat != null || VisualChild != null;

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
	}
}