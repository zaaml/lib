// <copyright file="IconContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Runtime;
using Zaaml.UI.Panels;

//TODO Review ActualShow and Visibility updates logic

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	[ContentProperty(nameof(Content))]
	public class IconContentPresenter : IconPresenterBase, IDockPanel
	{
		private static readonly DependencyPropertyKey ActualHasContentPropertyKey = DPM.RegisterReadOnly<bool, IconContentPresenter>
			("ActualHasContent", false, c => c.OnActualHasContentPropertyChanged);

		public static readonly DependencyProperty ActualHasContentProperty = ActualHasContentPropertyKey.DependencyProperty;

		private static readonly DependencyPropertyKey ActualShowPropertyKey = DPM.RegisterReadOnly<bool, IconContentPresenter>
			("ActualShow", false, c => c.OnActualShowPropertyChanged);

		public static readonly DependencyProperty ActualShowProperty = ActualShowPropertyKey.DependencyProperty;

		public static readonly DependencyProperty ContentProperty = DPM.Register<object, IconContentPresenter>
			("Content", p => p.OnContentChanged);

		public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, IconContentPresenter>
			("ContentTemplate", p => p.OnContentTemplateChanged);

		public static readonly DependencyProperty ContentStringFormatProperty = DPM.Register<string, IconContentPresenter>
			("ContentStringFormat", p => p.OnContentStringFormatChanged);

		public static readonly DependencyProperty ContentTemplateSelectorProperty = DPM.Register<DataTemplateSelector, IconContentPresenter>
			("ContentTemplateSelector", p => p.ContentTemplateSelectorChanged);

		public static readonly DependencyProperty HorizontalContentAlignmentProperty = DPM.Register<HorizontalAlignment, IconContentPresenter>
			("HorizontalContentAlignment", HorizontalAlignment.Center, p => p.OnHorizontalContentAlignmentChanged);

		public static readonly DependencyProperty IconDistanceProperty = DPM.Register<double, IconContentPresenter>
			("IconDistance", 4, p => p.OnIconDistanceChanged);

		public static readonly DependencyProperty IconDockProperty = DPM.Register<Dock, IconContentPresenter>
			("IconDock", Dock.Left, p => p.OnIconDockChanged);

		public static readonly DependencyProperty ShowContentProperty = DPM.Register<bool, IconContentPresenter>
			("ShowContent", true, p => p.UpdateActualShow);

		public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, IconContentPresenter>
			("ShowIcon", true, p => p.UpdateActualShow);

		public static readonly DependencyProperty VerticalContentAlignmentProperty = DPM.Register<VerticalAlignment, IconContentPresenter>
			("VerticalContentAlignment", VerticalAlignment.Center, p => p.OnVerticalContentAlignmentChanged);

		private readonly ContentPresenter _contentPresenter = new()
		{
			Visibility = Visibility.Collapsed,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center
		};

		private FrameworkElement _actualVisualChild;

		public IconContentPresenter()
		{
			Children.Add(_contentPresenter);
		}

		public bool ActualHasContent
		{
			get => (bool)GetValue(ActualHasContentProperty);
			private set => this.SetReadOnlyValue(ActualHasContentPropertyKey, value.Box());
		}

		public bool ActualShow
		{
			get => (bool)GetValue(ActualShowProperty);
			private set => this.SetReadOnlyValue(ActualShowPropertyKey, value.Box());
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public string ContentStringFormat
		{
			get => (string)GetValue(ContentStringFormatProperty);
			set => SetValue(ContentStringFormatProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate)GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		public DataTemplateSelector ContentTemplateSelector
		{
			get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
			set => SetValue(ContentTemplateSelectorProperty, value);
		}

		public HorizontalAlignment HorizontalContentAlignment
		{
			get => (HorizontalAlignment)GetValue(HorizontalContentAlignmentProperty);
			set => SetValue(HorizontalContentAlignmentProperty, value.Box());
		}

		public double IconDistance
		{
			get => (double)GetValue(IconDistanceProperty);
			set => SetValue(IconDistanceProperty, value);
		}

		public Dock IconDock
		{
			get => (Dock)GetValue(IconDockProperty);
			set => SetValue(IconDockProperty, value);
		}

		public bool ShowContent
		{
			get => (bool)GetValue(ShowContentProperty);
			set => SetValue(ShowContentProperty, value.Box());
		}

		public bool ShowIcon
		{
			get => (bool)GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value.Box());
		}

		public VerticalAlignment VerticalContentAlignment
		{
			get => (VerticalAlignment)GetValue(VerticalContentAlignmentProperty);
			set => SetValue(VerticalContentAlignmentProperty, value.Box());
		}

		private FrameworkElement VisualChild
		{
			get => _actualVisualChild;
			set
			{
				if (ReferenceEquals(_actualVisualChild, value))
					return;

				_actualVisualChild = value;
			}
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return DockPanelLayout.Arrange(this, finalSize);
		}

		private void ContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			_contentPresenter.ContentTemplateSelector = newContentTemplateSelector;

			UpdateActualHasContent();
		}

		private protected virtual Dock? GetDockCore(UIElement element)
		{
			return ReferenceEquals(element, _contentPresenter) ? null : (ShowContent ? IconDock : null);
		}

		private protected virtual double GetDockDistanceCore(UIElement element)
		{
			return 0;
		}

		private void InvalidateContentMargin()
		{
			UpdateContentMargin();
			InvalidateMeasure();
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			UpdateItem(ActualIcon, ShowIcon);
			UpdateItem(_contentPresenter, ShowContent);
			UpdateVisibility();

			return DockPanelLayout.Measure(this, availableSize);
		}

		private void OnActualHasContentPropertyChanged()
		{
			_contentPresenter.Visibility = ActualHasContent ? Visibility.Visible : Visibility.Collapsed;

			UpdateActualShow();
		}

		private protected override void OnActualHasIconChanged()
		{
			base.OnActualHasIconChanged();

			UpdateActualShow();
		}

		protected override void OnActualIconChanged()
		{
			base.OnActualIconChanged();

			UpdateActualShow();
			UpdateContentMargin();
		}

		private void OnActualShowPropertyChanged()
		{
			UpdateVisibility();
		}

		private void OnContentChanged(object oldContent, object newContent)
		{
			_contentPresenter.Content = newContent;

			UpdateActualHasContent();
		}

		private void OnContentStringFormatChanged(string oldStringFormat, string newStringFormat)
		{
			_contentPresenter.ContentStringFormat = newStringFormat;

			UpdateActualHasContent();
		}

		private void OnContentTemplateChanged(DataTemplate oldDataTemplate, DataTemplate newDataTemplate)
		{
			_contentPresenter.ContentTemplate = newDataTemplate;

			UpdateActualHasContent();
		}

		private void OnHorizontalContentAlignmentChanged(HorizontalAlignment oldAlignment, HorizontalAlignment newAlignment)
		{
			_contentPresenter.HorizontalAlignment = newAlignment;
		}

		private void OnIconDistanceChanged()
		{
			InvalidateContentMargin();
		}

		private void OnIconDockChanged()
		{
			InvalidateContentMargin();
		}

		private void OnVerticalContentAlignmentChanged(VerticalAlignment oldAlignment, VerticalAlignment newAlignment)
		{
			_contentPresenter.VerticalAlignment = newAlignment;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);

			//if (ReferenceEquals(VisualChild, visualRemoved))
			//	VisualChild = null;

			//if (visualAdded != null)
			//	VisualChild = visualAdded as FrameworkElement;

			UpdateActualHasContent();
		}

		private void UpdateActualHasContent()
		{
			var actualHasContent = Content != null || ContentTemplate != null || ContentTemplateSelector != null || ContentStringFormat != null || VisualChild != null;

			if (ActualHasContent != actualHasContent)
				ActualHasContent = actualHasContent;

			UpdateContentMargin();
			UpdateActualShow();
		}

		private void UpdateActualShow()
		{
			InvalidateContentMargin();

			ActualShow = (ActualHasIcon && ShowIcon) | (ActualHasContent && ShowContent);
		}

		private void UpdateContentMargin()
		{
			var d = IconDistance;

			var contentPresenterMargin = new Thickness(0, 0, 0, 0);

			if (ActualIcon != null && ShowIcon && ShowContent && _contentPresenter.Visibility == Visibility.Visible)
			{
				switch (IconDock)
				{
					case Dock.Left:
						contentPresenterMargin.Left = d;
						break;
					case Dock.Top:
						contentPresenterMargin.Top = d;
						break;
					case Dock.Right:
						contentPresenterMargin.Right = d;
						break;
					case Dock.Bottom:
						contentPresenterMargin.Bottom = d;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			if (_contentPresenter.Margin != contentPresenterMargin)
				_contentPresenter.Margin = contentPresenterMargin;
		}

		private void UpdateItem(FrameworkElement item, bool show)
		{
			if (show)
			{
				if (item != null && Children.Contains(item) == false)
					Children.Add(item);
			}
			else
			{
				if (item != null && Children.Contains(item))
					Children.Remove(item);
			}
		}

		private void UpdateVisibility()
		{
			if (this.GetDependencyPropertyValueInfo(VisibilityProperty).ValueSource != PropertyValueSource.Default)
				return;

			this.SetCurrentValueInternal(VisibilityProperty, ActualShow ? VisibilityBoxes.Visible : VisibilityBoxes.Collapsed);
		}

		Dock? IDockPanel.GetDock(UIElement element) => GetDockCore(element);

		double IDockPanel.GetDockDistance(UIElement element) => GetDockDistanceCore(element);
	}
}