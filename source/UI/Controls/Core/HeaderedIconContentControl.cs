// <copyright file="HeaderedIconContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Runtime;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Runtime;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Interfaces;

namespace Zaaml.UI.Controls.Core
{
	public class HeaderedIconContentControl : IconContentControl, IHeaderedIconContentControl
	{
		public static readonly DependencyProperty HeaderProperty = DPM.Register<object, HeaderedIconContentControl>
			("Header", h => h.OnHeaderChangedPrivate);

		private static readonly DependencyPropertyKey HasHeaderPropertyKey = DPM.RegisterReadOnly<bool, HeaderedIconContentControl>
			("HasHeader");

		public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;

		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<DataTemplate, HeaderedIconContentControl>
			("HeaderTemplate", h => h.OnHeaderTemplateChangedPrivate);

		public static readonly DependencyProperty HeaderTemplateSelectorProperty = DPM.Register<DataTemplateSelector, HeaderedIconContentControl>
			("HeaderTemplateSelector", h => h.OnHeaderTemplateSelectorChangedPrivate);

		public static readonly DependencyProperty HeaderStringFormatProperty = DPM.Register<string, HeaderedIconContentControl>
			("HeaderStringFormat", h => h.OnHeaderStringFormatChangedPrivate);

		public static readonly DependencyProperty HeaderDockProperty = DPM.Register<Dock, HeaderedIconContentControl>
			("HeaderDock", Dock.Top);

		public static readonly DependencyProperty HeaderDistanceProperty = DPM.Register<double, HeaderedIconContentControl>
			("HeaderDistance");

		public static readonly DependencyProperty VerticalHeaderAlignmentProperty = DPM.Register<VerticalAlignment, HeaderedIconContentControl>
			("VerticalHeaderAlignment", VerticalAlignment.Top);

		public static readonly DependencyProperty HorizontalHeaderAlignmentProperty = DPM.Register<HorizontalAlignment, HeaderedIconContentControl>
			("HorizontalHeaderAlignment", HorizontalAlignment.Left);

		public static readonly DependencyProperty ShowHeaderProperty = DPM.Register<bool, HeaderedIconContentControl>
			("ShowHeader", true);

		static HeaderedIconContentControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<HeaderedIconContentControl>();
		}

		public HeaderedIconContentControl()
		{
			this.OverrideStyleKey<HeaderedIconContentControl>();
		}

		public bool HasHeader
		{
			get => (bool) GetValue(HasHeaderProperty);
			set => this.SetReadOnlyValue(HasHeaderPropertyKey, value);
		}


		public double HeaderDistance
		{
			get => (double) GetValue(HeaderDistanceProperty);
			set => SetValue(HeaderDistanceProperty, value);
		}


		public Dock HeaderDock
		{
			get => (Dock) GetValue(HeaderDockProperty);
			set => SetValue(HeaderDockProperty, value);
		}


		public HorizontalAlignment HorizontalHeaderAlignment
		{
			get => (HorizontalAlignment) GetValue(HorizontalHeaderAlignmentProperty);
			set => SetValue(HorizontalHeaderAlignmentProperty, value.Box());
		}


		public bool ShowHeader
		{
			get => (bool) GetValue(ShowHeaderProperty);
			set => SetValue(ShowHeaderProperty, value.Box());
		}


		public VerticalAlignment VerticalHeaderAlignment
		{
			get => (VerticalAlignment) GetValue(VerticalHeaderAlignmentProperty);
			set => SetValue(VerticalHeaderAlignmentProperty, value.Box());
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return this.OnMeasureOverride(base.MeasureOverride, availableSize);
		}

		protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
		{
		}

		internal virtual void OnHeaderChangedInternal(object oldHeader, object newHeader)
		{
			OnHeaderChanged(oldHeader, newHeader);
		}

		private void OnHeaderChangedPrivate(object oldHeader, object newHeader)
		{
			OnHeaderChangedInternal(oldHeader, newHeader);
		}

		protected virtual void OnHeaderStringFormatChanged(string oldStringFormat, string newStringFormat)
		{
		}

		internal virtual void OnHeaderStringFormatChangedInternal(string oldStringFormat, string newStringFormat)
		{
			OnHeaderStringFormatChanged(oldStringFormat, newStringFormat);
		}

		private void OnHeaderStringFormatChangedPrivate(string oldStringFormat, string newStringFormat)
		{
			OnHeaderStringFormatChangedInternal(oldStringFormat, newStringFormat);
		}

		protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
		{
		}

		internal virtual void OnHeaderTemplateChangedInternal(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
		{
			OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
		}

		private void OnHeaderTemplateChangedPrivate(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
		{
			OnHeaderTemplateChangedInternal(oldHeaderTemplate, newHeaderTemplate);
		}

		protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
		{
		}

		internal virtual void OnHeaderTemplateSelectorChangedInternal(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
		{
			OnHeaderTemplateSelectorChanged(oldHeaderTemplateSelector, newHeaderTemplateSelector);
		}

		private void OnHeaderTemplateSelectorChangedPrivate(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
		{
			OnHeaderTemplateSelectorChangedInternal(oldHeaderTemplateSelector, newHeaderTemplateSelector);
		}

		public object Header
		{
			get => GetValue(HeaderProperty);
			set => SetValue(HeaderProperty, value);
		}

		DependencyProperty IHeaderedIconContentControl.HeaderProperty => HeaderProperty;

		public string HeaderStringFormat
		{
			get => (string) GetValue(HeaderStringFormatProperty);
			set => SetValue(HeaderStringFormatProperty, value);
		}

		DependencyProperty IHeaderedIconContentControl.HeaderStringFormatProperty => HeaderStringFormatProperty;

		public DataTemplate HeaderTemplate
		{
			get => (DataTemplate) GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		DependencyProperty IHeaderedIconContentControl.HeaderTemplateProperty => HeaderTemplateProperty;

		public DataTemplateSelector HeaderTemplateSelector
		{
			get => (DataTemplateSelector) GetValue(HeaderTemplateSelectorProperty);
			set => SetValue(HeaderTemplateSelectorProperty, value);
		}

		DependencyProperty IHeaderedIconContentControl.HeaderTemplateSelectorProperty => HeaderTemplateSelectorProperty;

		DependencyProperty IIconContentControl.IconProperty => IconProperty;
	}
}