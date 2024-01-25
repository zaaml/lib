// <copyright file="WindowBase.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.UI.Windows
{
	public partial class WindowBase : Window
	{
		private const string AdornedContentControlTemplateString =
			@"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" TargetType=""ContentControl""><AdornerDecorator><ContentPresenter /></AdornerDecorator></ControlTemplate>";

		private static readonly Lazy<ControlTemplate> LazyAdornedContentControlTemplate;

		private ContentControl _adornedContentControl;
		private PresentationWindowService _presentationWindowService;

		static WindowBase()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<WindowBase>();
			LazyAdornedContentControlTemplate = new Lazy<ControlTemplate>(() => XamlUtils.Load<ControlTemplate>(AdornedContentControlTemplateString));
		}

		private static ControlTemplate AdornedContentControlTemplateInstance => LazyAdornedContentControlTemplate.Value;

		internal bool IsContentRendered { get; set; }

		partial void BeginDragMoveImpl(bool async)
		{
			_presentationWindowService.BeginDragMove(async);
		}

		private void EnableTheme()
		{
			ThemeManager.EnableTheme(this);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			_presentationWindowService.WindowChromeBehavior.OnMeasure();

			return base.MeasureOverride(availableSize);
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			_presentationWindowService.OnClosed();
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			if (e.Cancel == false)
				_presentationWindowService.OnClosing();
		}

		protected override void OnContentRendered(EventArgs e)
		{
			IsContentRendered = true;

			base.OnContentRendered(e);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			EnableTheme();
		}

		partial void OnPlatformAfterApplyTemplate()
		{
			_presentationWindowService.WindowPresenterControl = WindowPresenter;
		}

		partial void OnPlatformBeforeApplyTemplate()
		{
			_adornedContentControl = (ContentControl)GetTemplateChild("AdornedContentControl");

			if (_adornedContentControl != null)
				_adornedContentControl.Template = AdornedContentControlTemplateInstance;
		}

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);

			OnHeaderButtonVisibilityRelatedPropertyChanged();
		}

		partial void OnWindowPresenterTemplateContractAttachedPartial()
		{
			WindowPresenter.HeaderPresenter.AddHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnHeaderMouseLeftButtonDown, true);
		}

		partial void OnWindowPresenterTemplateContractDetachingPartial()
		{
			WindowPresenter.HeaderPresenter.RemoveHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler)OnHeaderMouseLeftButtonDown);
		}

		partial void PlatformCtor()
		{
			WindowStyle = WindowStyle.None;
			AllowsTransparency = false;
			BorderThickness = new Thickness(0);

			_presentationWindowService = new PresentationWindowService(this);
		}

		partial void UpdateDraggableBehavior()
		{
		}

		partial void UpdateResizableBehavior()
		{
		}
	}
}