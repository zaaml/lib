// <copyright file="IconContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
	[ContentProperty(nameof(Content))]
	public class IconContentPresenter : IconPresenterBase, IDockPanel
	{
		private static readonly DependencyPropertyKey ActualShowPropertyKey = DPM.RegisterReadOnly<bool, IconContentPresenter>
			("ActualShow", true);

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
			("ShowContent", true, p => p.ShowPartChanged);

		public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, IconContentPresenter>
			("ShowIcon", true, p => p.ShowPartChanged);

		public static readonly DependencyProperty VerticalContentAlignmentProperty = DPM.Register<VerticalAlignment, IconContentPresenter>
			("VerticalContentAlignment", VerticalAlignment.Center, p => p.OnVerticalContentAlignmentChanged);

		private readonly ContentPresenter _contentPresenter = new ContentPresenter
		{
			Visibility = Visibility.Collapsed,
			VerticalAlignment = VerticalAlignment.Center,
			HorizontalAlignment = HorizontalAlignment.Center
		};

		public IconContentPresenter()
		{
			Children.Add(_contentPresenter);
		}

		public bool ActualShow
		{
			get => (bool) GetValue(ActualShowProperty);
			private set => this.SetReadOnlyValue(ActualShowPropertyKey, value);
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		public string ContentStringFormat
		{
			get => (string) GetValue(ContentStringFormatProperty);
			set => SetValue(ContentStringFormatProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate) GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		public DataTemplateSelector ContentTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(ContentTemplateSelectorProperty);
			set => SetValue(ContentTemplateSelectorProperty, value);
		}

		public HorizontalAlignment HorizontalContentAlignment
		{
			get => (HorizontalAlignment) GetValue(HorizontalContentAlignmentProperty);
			set => SetValue(HorizontalContentAlignmentProperty, value);
		}

		public double IconDistance
		{
			get => (double) GetValue(IconDistanceProperty);
			set => SetValue(IconDistanceProperty, value);
		}

		public Dock IconDock
		{
			get => (Dock) GetValue(IconDockProperty);
			set => SetValue(IconDockProperty, value);
		}

		public bool ShowContent
		{
			get => (bool) GetValue(ShowContentProperty);
			set => SetValue(ShowContentProperty, value);
		}

		public bool ShowIcon
		{
			get => (bool) GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value);
		}

		public VerticalAlignment VerticalContentAlignment
		{
			get => (VerticalAlignment) GetValue(VerticalContentAlignmentProperty);
			set => SetValue(VerticalContentAlignmentProperty, value);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return DockPanelLayout.Arrange(this, finalSize);
		}

		private void ContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
		{
			_contentPresenter.ContentTemplateSelector = newContentTemplateSelector;

			UpdateContentPresenterVisibility();
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

			return DockPanelLayout.Measure(this, availableSize);
		}

		protected override void OnActualIconChanged()
		{
			base.OnActualIconChanged();

			UpdateContentMargin();
		}

		private void OnContentChanged(object oldContent, object newContent)
		{
			_contentPresenter.Content = newContent;

			UpdateContentPresenterVisibility();
		}

		private void OnContentStringFormatChanged(string oldStringFormat, string newStringFormat)
		{
			_contentPresenter.ContentStringFormat = newStringFormat;

			UpdateContentPresenterVisibility();
		}

		private void OnContentTemplateChanged(DataTemplate oldDataTemplate, DataTemplate newDataTemplate)
		{
			_contentPresenter.ContentTemplate = newDataTemplate;

			UpdateContentPresenterVisibility();
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

		private void ShowPartChanged()
		{
			InvalidateContentMargin();

			ActualShow = ShowIcon | ShowContent;
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

			_contentPresenter.Margin = contentPresenterMargin;
		}

		private void UpdateContentPresenterVisibility()
		{
			var visibility = Content == null && ContentTemplate == null && ContentTemplateSelector == null ? Visibility.Collapsed : Visibility.Visible;

			if (_contentPresenter.Visibility == visibility)
				return;

			_contentPresenter.Visibility = visibility;
			
			UpdateContentMargin();
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

		private protected virtual Dock? GetDockCore(UIElement element)
		{
			return ReferenceEquals(element, _contentPresenter) ? null : (ShowContent ? IconDock : (Dock?)null);
		}

		private protected virtual double GetDockDistanceCore(UIElement element)
		{
			return 0;
		}

		Dock? IDockPanel.GetDock(UIElement element) => GetDockCore(element);

		double IDockPanel.GetDockDistance(UIElement element) => GetDockDistanceCore(element);
	}
}