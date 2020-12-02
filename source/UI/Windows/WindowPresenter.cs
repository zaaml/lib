// <copyright file="WindowPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using NativeStyle = System.Windows.Style;

namespace Zaaml.UI.Windows
{
	[ContentProperty(nameof(Content))]
	[TemplateContractType(typeof(WindowContentPresenterTemplateContract))]
	public sealed class WindowPresenter : TemplateContractControl, IWindowElement
	{
		public static readonly DependencyProperty ContentProperty = DPM.Register<object, WindowPresenter>
			("Content");

		public static readonly DependencyProperty ContentTemplateProperty = DPM.Register<DataTemplate, WindowPresenter>
			("ContentTemplate");

		public static readonly DependencyProperty ShowHeaderProperty = DPM.Register<bool, WindowPresenter>
			("ShowHeader", true);

		public static readonly DependencyProperty ShowFooterProperty = DPM.Register<bool, WindowPresenter>
			("ShowFooter", true);

		public static readonly DependencyProperty ShowFrameProperty = DPM.Register<bool, WindowPresenter>
			("ShowFrame", true);

		private static readonly DependencyPropertyKey ActualWindowPropertyKey = DPM.RegisterReadOnly<IWindow, WindowPresenter>
			("ActualWindow");

		public static readonly DependencyProperty ActualWindowProperty = ActualWindowPropertyKey.DependencyProperty;

		public static readonly DependencyProperty DropShadowProperty = DPM.Register<bool, WindowPresenter>
			("DropShadow", false);

		public static readonly DependencyProperty HeaderPresenterStyleProperty = DPM.Register<NativeStyle, WindowPresenter>
			("HeaderPresenterStyle");

		public static readonly DependencyProperty FooterPresenterStyleProperty = DPM.Register<NativeStyle, WindowPresenter>
			("FooterPresenterStyle");

		public static readonly DependencyProperty ContentPresenterStyleProperty = DPM.Register<NativeStyle, WindowPresenter>
			("ContentPresenterStyle");

		static WindowPresenter()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<WindowPresenter>();
		}

		public WindowPresenter()
		{
			this.OverrideStyleKey<WindowPresenter>();

#if !SILVERLIGHT
			Focusable = false;
#endif
			IsTabStop = false;
		}

		public IWindow ActualWindow
		{
			get => (IWindow) GetValue(ActualWindowProperty);
			internal set => this.SetReadOnlyValue(ActualWindowPropertyKey, value);
		}

		public object Content
		{
			get => GetValue(ContentProperty);
			set => SetValue(ContentProperty, value);
		}

		internal WindowContentPresenter ContentPresenter => TemplateContract.ContentPresenter;

		public NativeStyle ContentPresenterStyle
		{
			get => (NativeStyle) GetValue(ContentPresenterStyleProperty);
			set => SetValue(ContentPresenterStyleProperty, value);
		}

		public DataTemplate ContentTemplate
		{
			get => (DataTemplate) GetValue(ContentTemplateProperty);
			set => SetValue(ContentTemplateProperty, value);
		}

		public bool DropShadow
		{
			get => (bool) GetValue(DropShadowProperty);
			set => SetValue(DropShadowProperty, value);
		}

		internal WindowFooterPresenter FooterPresenter => TemplateContract.FooterPresenter;

		public NativeStyle FooterPresenterStyle
		{
			get => (NativeStyle) GetValue(FooterPresenterStyleProperty);
			set => SetValue(FooterPresenterStyleProperty, value);
		}

		internal WindowFramePresenter FramePresenter => TemplateContract.FramePresenter;

		internal WindowHeaderPresenter HeaderPresenter => TemplateContract.HeaderPresenter;

		public NativeStyle HeaderPresenterStyle
		{
			get => (NativeStyle) GetValue(HeaderPresenterStyleProperty);
			set => SetValue(HeaderPresenterStyleProperty, value);
		}

		public bool ShowFooter
		{
			get => (bool) GetValue(ShowFooterProperty);
			set => SetValue(ShowFooterProperty, value);
		}

		public bool ShowFrame
		{
			get => (bool) GetValue(ShowFrameProperty);
			set => SetValue(ShowFrameProperty, value);
		}

		public bool ShowHeader
		{
			get => (bool) GetValue(ShowHeaderProperty);
			set => SetValue(ShowHeaderProperty, value);
		}

		private WindowContentPresenterTemplateContract TemplateContract => (WindowContentPresenterTemplateContract) TemplateContractInternal;

		private IEnumerable<IWindowElement> EnumerateWindowElements()
		{
			if (FramePresenter != null)
				yield return FramePresenter;

			if (FooterPresenter != null)
				yield return FooterPresenter;

			if (HeaderPresenter != null)
				yield return HeaderPresenter;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			foreach (var windowElement in EnumerateWindowElements())
				windowElement.Window = ActualWindow;
		}

		protected override void OnTemplateContractDetaching()
		{
			foreach (var windowElement in EnumerateWindowElements())
				windowElement.Window = null;

			base.OnTemplateContractDetaching();
		}

		IWindow IWindowElement.Window
		{
			get => ActualWindow;
			set
			{
				if (ReferenceEquals(ActualWindow, value))
					return;

				ActualWindow = value;

				foreach (var windowElement in EnumerateWindowElements())
					windowElement.Window = value;
			}
		}

		IEnumerable<IWindowElement> IWindowElement.EnumerateWindowElements()
		{
			return EnumerateWindowElements();
		}
	}

	public sealed class WindowContentPresenterTemplateContract : TemplateContract
	{
		[TemplateContractPart] public WindowContentPresenter ContentPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart] public WindowFooterPresenter FooterPresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart] public WindowFramePresenter FramePresenter { get; [UsedImplicitly] private set; }

		[TemplateContractPart] public WindowHeaderPresenter HeaderPresenter { get; [UsedImplicitly] private set; }
	}
}