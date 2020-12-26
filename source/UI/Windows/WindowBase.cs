// <copyright file="WindowBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.Core.Trees;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using NativeStyle = System.Windows.Style;

namespace Zaaml.UI.Windows
{
	public partial class WindowBase : IWindow, INotifyPropertyChanging, INotifyPropertyChanged, IImplementationRootProvider
	{
		internal static readonly RoutedCommand CloseCommand = new RoutedCommand();
		internal static readonly RoutedCommand MinimizeCommand = new RoutedCommand();
		internal static readonly RoutedCommand ToggleMaximizeNormalStateCommand = new RoutedCommand();
		internal static readonly RoutedCommand CloseWithDialogResultCommand = new RoutedCommand();

		private static readonly ITreeEnumeratorAdvisor<IWindowElement> TreeEnumeratorAdvisor = new DelegateTreeEnumeratorAdvisor<IWindowElement>(e => e.EnumerateWindowElements().GetEnumerator());

		public static readonly DependencyProperty DropShadowProperty = DPM.Register<bool, WindowBase>
			("DropShadow", false);

		public static readonly DependencyProperty FooterPresenterStyleProperty = DPM.Register<NativeStyle, WindowBase>
			("FooterPresenterStyle");

		public static readonly DependencyProperty HeaderPresenterStyleProperty = DPM.Register<NativeStyle, WindowBase>
			("HeaderPresenterStyle");

		public static readonly DependencyProperty ContentPresenterStyleProperty = DPM.Register<NativeStyle, WindowBase>
			("ContentPresenterStyle");

		public static readonly DependencyProperty IsDraggableProperty = DPM.Register<bool, WindowBase>
			("IsDraggable", true, w => w.OnIsDraggableChanged);

		public static readonly DependencyProperty IsResizableProperty = DPM.Register<bool, WindowBase>
			("IsResizable", true, w => w.OnIsResizableChanged);

		public static readonly DependencyProperty ShowCloseButtonProperty = DPM.Register<bool, WindowBase>
			("ShowCloseButton", true, w => w.OnHeaderButtonVisibilityRelatedPropertyChanged);

		public static readonly DependencyProperty ShowMaximizeButtonProperty = DPM.Register<bool, WindowBase>
			("ShowMaximizeButton", true, w => w.OnHeaderButtonVisibilityRelatedPropertyChanged);

		public static readonly DependencyProperty ShowMinimizeButtonProperty = DPM.Register<bool, WindowBase>
			("ShowMinimizeButton", true, w => w.OnHeaderButtonVisibilityRelatedPropertyChanged);

		public static readonly DependencyProperty ShowTitleProperty = DPM.Register<bool, WindowBase>
			("ShowTitle", true);

		public static readonly DependencyProperty ShowIconProperty = DPM.Register<bool, WindowBase>
			("ShowIcon", true);

		public static readonly DependencyProperty TitleBarHeadContentProperty = DPM.Register<object, WindowBase>
			("TitleBarHeadContent");

		public static readonly DependencyProperty TitleBarTailContentProperty = DPM.Register<object, WindowBase>
			("TitleBarTailContent");

		private WindowFooterPresenter _footerPresenter;
		private WindowHeaderPresenter _headerPresenter;
		private byte _packedValue;
		public event EventHandler IsResizableChanged;
		public event EventHandler IsDraggableChanged;
		internal event EventHandler HeaderButtonVisibilityRelatedPropertyChanged;
		public event EventHandler IsActiveChanged;

		public WindowBase()
		{
			this.OverrideStyleKey<WindowBase>();

			var commandBindings = CommandBindings;

			commandBindings.Add(new CommandBinding(CloseCommand, ExecutedCloseCommand, CanExecuteCloseCommand));
			commandBindings.Add(new CommandBinding(MinimizeCommand, ExecuteMinimizeCommand));
			commandBindings.Add(new CommandBinding(ToggleMaximizeNormalStateCommand, ExecuteToggleMaximizeNormalStateCommand, CanExecuteToggleMaximizeNormalStateCommand));
			commandBindings.Add(new CommandBinding(CloseWithDialogResultCommand, ExecutedCloseWithDialogResultCommand));

			PlatformCtor();
		}

		protected virtual bool ActualShowCloseButton => ShowCloseButton;

		internal bool ActualShowCloseButtonInt => ActualShowCloseButton;

		protected virtual bool ActualShowMaximizeButton => IsResizable && ShowMaximizeButton && WindowState != WindowState.Maximized;

		internal bool ActualShowMaximizeButtonInt => ActualShowMaximizeButton;

		protected virtual bool ActualShowMinimizeButton => ShowMinimizeButton;

		internal bool ActualShowMinimizeButtonInt => ActualShowMinimizeButton;

		protected bool ActualShowRestoreButton => IsResizable && ShowMaximizeButton && WindowState == WindowState.Maximized;

		internal bool ActualShowRestoreButtonInt => ActualShowRestoreButton;

		public NativeStyle ContentPresenterStyle
		{
			get => (NativeStyle) GetValue(ContentPresenterStyleProperty);
			set => SetValue(ContentPresenterStyleProperty, value);
		}

		public bool DropShadow
		{
			get => (bool) GetValue(DropShadowProperty);
			set => SetValue(DropShadowProperty, value);
		}

		internal WindowFooterPresenter FooterPresenter
		{
			get => _footerPresenter;
			set
			{
				if (ReferenceEquals(_footerPresenter, value))
					return;

				if (_footerPresenter != null)
					OnFooterPresenterDetachingPrivate(_footerPresenter);

				_footerPresenter = value;

				if (_footerPresenter != null)
					OnFooterPresenterAttachedPrivate(_footerPresenter);
			}
		}

		public NativeStyle FooterPresenterStyle
		{
			get => (NativeStyle) GetValue(FooterPresenterStyleProperty);
			set => SetValue(FooterPresenterStyleProperty, value);
		}

		internal WindowHeaderPresenter HeaderPresenter
		{
			get => _headerPresenter;
			set
			{
				if (ReferenceEquals(_headerPresenter, value))
					return;

				if (_headerPresenter != null)
					OnHeaderPresenterDetachingPrivate(_headerPresenter);

				_headerPresenter = value;

				if (_headerPresenter != null)
					OnHeaderPresenterAttachedPrivate(_headerPresenter);
			}
		}

		public NativeStyle HeaderPresenterStyle
		{
			get => (NativeStyle) GetValue(HeaderPresenterStyleProperty);
			set => SetValue(HeaderPresenterStyleProperty, value);
		}

		public bool IsDraggable
		{
			get => (bool) GetValue(IsDraggableProperty);
			set => SetValue(IsDraggableProperty, value);
		}

		protected internal bool IsManualLocation
		{
			get => PackedDefinition.IsManualLocation.GetValue(_packedValue);
			set => PackedDefinition.IsManualLocation.SetValue(ref _packedValue, value);
		}

		protected internal bool IsManualSize
		{
			get => PackedDefinition.IsManualSize.GetValue(_packedValue);
			set => PackedDefinition.IsManualSize.SetValue(ref _packedValue, value);
		}

		public bool IsResizable
		{
			get => (bool) GetValue(IsResizableProperty);
			set => SetValue(IsResizableProperty, value);
		}

		private bool QueryWindowToCenter
		{
			get => PackedDefinition.QueryWindowToCenter.GetValue(_packedValue);
			set => PackedDefinition.QueryWindowToCenter.SetValue(ref _packedValue, value);
		}

		public bool ShowCloseButton
		{
			get => (bool) GetValue(ShowCloseButtonProperty);
			set => SetValue(ShowCloseButtonProperty, value);
		}

		public bool ShowIcon
		{
			get => (bool) GetValue(ShowIconProperty);
			set => SetValue(ShowIconProperty, value);
		}

		public bool ShowMaximizeButton
		{
			get => (bool) GetValue(ShowMaximizeButtonProperty);
			set => SetValue(ShowMaximizeButtonProperty, value);
		}

		public bool ShowMinimizeButton
		{
			get => (bool) GetValue(ShowMinimizeButtonProperty);
			set => SetValue(ShowMinimizeButtonProperty, value);
		}

		public bool ShowTitle
		{
			get => (bool) GetValue(ShowTitleProperty);
			set => SetValue(ShowTitleProperty, value);
		}

		public WindowStatus Status
		{
			get => PackedDefinition.Status.GetValue(_packedValue);
			internal set
			{
				if (Status == value)
					return;

				PackedDefinition.Status.SetValue(ref _packedValue, value);

				Status = value;
				OnPropertyChanged(nameof(Status));
			}
		}

		public object TitleBarHeadContent
		{
			get => GetValue(TitleBarHeadContentProperty);
			set => SetValue(TitleBarHeadContentProperty, value);
		}

		public object TitleBarTailContent
		{
			get => GetValue(TitleBarTailContentProperty);
			set => SetValue(TitleBarTailContentProperty, value);
		}

		protected WindowPresenter WindowPresenter { get; private set; }

		protected override Size ArrangeOverride(Size finalSize)
		{
			// This code aligns window content with device pixels for non-standard Dpi

			var shouldRound = DpiUtils.DpiX != 96 || DpiUtils.DpiY != 96;

			if (shouldRound)
			{
				finalSize.Width += 0.001;
				finalSize.Height += 0.001;
			}

			var finalArrange = base.ArrangeOverride(finalSize);

			if (shouldRound)
				finalArrange = finalArrange.Round(2);

			if (QueryWindowToCenter)
			{
				QueryWindowToCenter = false;
				MoveWindowToCenter(finalArrange);
			}

			return finalArrange;
		}

		protected virtual void AttachWindowPresenter()
		{
			WindowPresenter = (WindowPresenter) GetTemplateChild("WindowPresenter");

			if (WindowPresenter == null)
				return;

			WindowPresenter.TemplateContractAttached += OnWindowPresenterTemplateContractAttached;
			WindowPresenter.TemplateContractDetaching += OnWindowPresenterTemplateContractDetaching;
		}

		internal void BeginDragMove(bool async)
		{
			BeginDragMoveImpl(async);
		}

		partial void BeginDragMoveImpl(bool async);

		protected virtual Rect CalcClientBox(Size finalArrange)
		{
			return finalArrange.Rect();
		}

		private void CanExecuteCloseCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
			e.Handled = true;
			e.ContinueRouting = false;
		}

		private void CanExecuteToggleMaximizeNormalStateCommand(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = IsResizable;
			e.Handled = true;
		}

		protected virtual void CloseByEnter()
		{
			SetDialogResultAndClose(true);
		}

		protected virtual void CloseByEscape()
		{
			SetDialogResultAndClose(false);
		}

		protected virtual void DetachWindowPresenter()
		{
			if (WindowPresenter == null)
				return;

			WindowPresenter.TemplateContractAttached -= OnWindowPresenterTemplateContractAttached;
			WindowPresenter.TemplateContractDetaching -= OnWindowPresenterTemplateContractDetaching;
		}

		private IEnumerable<IWindowElement> EnumerateDescendantWindowElements()
		{
			return EnumerateWindowElements().SelectMany(w => TreeEnumerator.GetEnumerator(w, TreeEnumeratorAdvisor).Enumerate());
		}

		private IEnumerable<IWindowElement> EnumerateWindowElements()
		{
			if (WindowPresenter != null)
				yield return WindowPresenter;
		}

		private void ExecutedCloseCommand(object sender, ExecutedRoutedEventArgs e)
		{
			OnExecutedCloseCommand(e.Parameter);

			e.Handled = true;
		}

		private void ExecutedCloseWithDialogResultCommand(object sender, ExecutedRoutedEventArgs e)
		{
			SetDialogResultAndClose((bool?) e.Parameter);

			e.Handled = true;
		}

		private void ExecuteMinimizeCommand(object sender, ExecutedRoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;

			e.Handled = true;
		}

		private void ExecuteToggleMaximizeNormalStateCommand(object sender, ExecutedRoutedEventArgs e)
		{
			ToggleMaximizeNormalState();

			e.Handled = true;
		}

		private void FocusInt()
		{
			Focus();
		}

		internal static Window GetWindowInternal(FrameworkElement element)
		{
#if SILVERLIGHT
			return element.GetAncestorsAndSelf<Window>(MixedTreeEnumerationStrategy.VisualThenLogicalInstance).FirstOrDefault();
#else
			return GetWindow(element);
#endif
		}

		private void MoveWindowToCenter(Size finalArrange)
		{
			if (IsManualLocation || WindowStartupLocation != WindowStartupLocation.CenterScreen)
				return;

			var clientBox = CalcClientBox(finalArrange);
			var alignBox = RectUtils.CalcAlignBox(Screen.FromElement(this).WorkingArea, clientBox, HorizontalAlignment.Center, VerticalAlignment.Center);

			this.SetLayoutBox(alignBox);

			// Resharper warning is not correct since IsManualLocation can be changed through event notification
			// ReSharper disable once RedundantAssignment
			IsManualLocation = false;
		}

		internal void MoveWindowToCenterInternal()
		{
			QueryWindowToCenter = true;

			InvalidateMeasure();
			InvalidateArrange();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			OnIsActiveChangedPrivate();
		}

		public sealed override void OnApplyTemplate()
		{
			OnPlatformBeforeApplyTemplate();
			OnTemplateDetach();

			foreach (var windowElement in EnumerateWindowElements())
				windowElement.Window = null;

			DetachWindowPresenter();

			base.OnApplyTemplate();

			AttachWindowPresenter();

			foreach (var windowElement in EnumerateWindowElements())
				windowElement.Window = this;

			OnPlatformAfterApplyTemplate();

			OnTemplateAttach();
		}

		internal virtual void OnBeginDragMove()
		{
		}

		internal void OnBeginResize()
		{
			foreach (var windowElement in EnumerateDescendantWindowElements().OfType<IWindowEventListener>())
				windowElement.OnResizeStarted();
		}

		protected override void OnDeactivated(EventArgs e)
		{
			base.OnDeactivated(e);

			OnIsActiveChangedPrivate();
		}

		internal virtual void OnDragMove()
		{
		}

		internal virtual void OnEndDragMove()
		{
		}

		protected virtual void OnExecutedCloseCommand(object commandParameter)
		{
			SetDialogResultAndClose(commandParameter as bool?);
		}

		internal virtual void OnFooterPresenterAttachedInternal(WindowFooterPresenter footerPresenter)
		{
		}

		private void OnFooterPresenterAttachedPrivate(WindowFooterPresenter footerPresenter)
		{
			OnFooterPresenterAttachedInternal(footerPresenter);
		}

		internal virtual void OnFooterPresenterDetachingInternal(WindowFooterPresenter footerPresenter)
		{
		}

		private void OnFooterPresenterDetachingPrivate(WindowFooterPresenter footerPresenter)
		{
			OnFooterPresenterDetachingInternal(footerPresenter);
		}

		private void OnHeaderButtonVisibilityRelatedPropertyChanged()
		{
			HeaderButtonVisibilityRelatedPropertyChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnHeaderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount == 2 && e.Handled == false)
				ToggleMaximizeNormalState();
		}

		internal virtual void OnHeaderPresenterAttachedInternal(WindowHeaderPresenter headerPresenter)
		{
		}

		private void OnHeaderPresenterAttachedPrivate(WindowHeaderPresenter headerPresenter)
		{
			OnHeaderPresenterAttachedInternal(headerPresenter);
		}

		internal virtual void OnHeaderPresenterDetachingInternal(WindowHeaderPresenter headerPresenter)
		{
		}

		private void OnHeaderPresenterDetachingPrivate(WindowHeaderPresenter headerPresenter)
		{
			OnHeaderPresenterDetachingInternal(headerPresenter);
		}

		protected virtual void OnIsActiveChanged()
		{
		}

		private void OnIsActiveChangedPrivate()
		{
			OnIsActiveChanged();

			IsActiveChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnIsDraggableChanged()
		{
			UpdateDraggableBehavior();

			OnHeaderButtonVisibilityRelatedPropertyChanged();

			IsDraggableChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnIsResizableChanged()
		{
			UpdateResizableBehavior();

			OnHeaderButtonVisibilityRelatedPropertyChanged();

			IsResizableChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (e.Handled)
				return;

			if (ReferenceEquals(this, FocusHelper.GetFocusedElement()) == false)
				return;

			if (e.Key == Key.Escape)
				CloseByEscape();
			else if (e.Key == Key.Enter)
				CloseByEnter();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			FocusInt();
		}

		protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseRightButtonDown(e);

			FocusInt();
		}

		internal void OnOnEndResize()
		{
			foreach (var windowElement in EnumerateDescendantWindowElements().OfType<IWindowEventListener>())
				windowElement.OnResizeFinished();
		}

		partial void OnPlatformAfterApplyTemplate();

		partial void OnPlatformBeforeApplyTemplate();

		protected virtual void OnPositionChanged()
		{
		}

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanging(string propertyName)
		{
			PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
		}

		protected virtual void OnTemplateAttach()
		{
		}

		protected virtual void OnTemplateDetach()
		{
		}

		private void OnWindowPresenterTemplateContractAttached(object sender, EventArgs eventArgs)
		{
			OnWindowPresenterTemplateContractAttachedPartial();
			OnWindowPresenterTemplateContractAttachedInternal();
		}

		internal virtual void OnWindowPresenterTemplateContractAttachedInternal()
		{
		}

		partial void OnWindowPresenterTemplateContractAttachedPartial();

		private void OnWindowPresenterTemplateContractDetaching(object sender, EventArgs eventArgs)
		{
			OnWindowPresenterTemplateContractDetachingInternal();
			OnWindowPresenterTemplateContractDetachingPartial();
		}

		internal virtual void OnWindowPresenterTemplateContractDetachingInternal()
		{
		}

		partial void OnWindowPresenterTemplateContractDetachingPartial();

		partial void PlatformCtor();

		internal void SetDialogResultAndClose(bool? dialogResult)
		{
			if (DialogResult != dialogResult)
				DialogResult = dialogResult;
			else
				Close();
		}

		internal virtual void ToggleMaximizeNormalState()
		{
			WindowState = WindowState == WindowState.Normal && IsResizable ? WindowState.Maximized : WindowState.Normal;
		}

		partial void UpdateDraggableBehavior();

		partial void UpdateResizableBehavior();

		FrameworkElement IImplementationRootProvider.ImplementationRoot => Content as FrameworkElement;
		public event PropertyChangedEventHandler PropertyChanged;
		public event PropertyChangingEventHandler PropertyChanging;

		event EventHandler IWindow.IsActiveChanged
		{
			add => IsActiveChanged += value;
			remove => IsActiveChanged -= value;
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition QueryWindowToCenter;
			public static readonly PackedBoolItemDefinition IsManualLocation;
			public static readonly PackedBoolItemDefinition IsManualSize;
			public static readonly PackedEnumItemDefinition<WindowStatus> Status;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				QueryWindowToCenter = allocator.AllocateBoolItem();
				IsManualLocation = allocator.AllocateBoolItem();
				IsManualSize = allocator.AllocateBoolItem();
				Status = allocator.AllocateEnumItem<WindowStatus>();
			}
		}
	}
}