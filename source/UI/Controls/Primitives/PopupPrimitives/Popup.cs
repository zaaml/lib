// <copyright file="Popup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.Core.Utils;
using Zaaml.Platform;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	[ContentProperty("Child")]
	public sealed partial class Popup : FixedTemplateControl<PopupRoot>, IPopup
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<UIElement, Popup>
			("Child", t => t.OnChildChanged);

		public static readonly DependencyProperty StaysOpenProperty = DPM.Register<bool, Popup>
			("StaysOpen", true);

		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, Popup>
			("IsOpen", t => t.OnIsOpenChanged, p => CoerceIsOpen);

		public static readonly DependencyProperty PlacementProperty = DPM.Register<PopupPlacement, Popup>
			("Placement", t => t.OnPlacementChanged);

		public static readonly DependencyProperty InflateProperty = DPM.RegisterAttached<Thickness, Popup>
			("Inflate", OnInflatePropertyChanged);

		public static readonly DependencyProperty PlacementOptionsProperty = DPM.Register<PopupPlacementOptions, Popup>
			("PlacementOptions", PopupPlacementOptions.None, t => t.OnOverflowOptionsChanged);

		public static readonly DependencyProperty IsHiddenProperty = DPM.Register<bool, Popup>
			("IsHidden", p => p.OnIsHiddenChanged);

		private static readonly DependencyProperty HitTestVisibleProperty = DPM.RegisterAttached<bool, Popup>
			("HitTestVisible", true);

		private DependencyObject _logicalChild;
		private PopupTreeMode _treeMode = PopupTreeMode.Visual;

		public event EventHandler Closed;
		public event EventHandler Opened;

		public Popup()
		{
			PopupSource = new PopupWndSource(this);

			Panel = new PopupPanel(this);

			PopupCloseController = new PopupCloseController(this);

			PlatformCtor();

			IsTabStop = false;
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		public bool IsHidden
		{
			get => (bool) GetValue(IsHiddenProperty);
			set => SetValue(IsHiddenProperty, value);
		}

		private bool IsOpenAttached => ReferenceEquals(PopupSource.ReadLocalBinding(PopupWndSource.IsOpenProperty)?.Source, this);

		internal DependencyObject LogicalChild
		{
			get => _logicalChild;
			set
			{
				if (ReferenceEquals(_logicalChild, value))
					return;

				if (_logicalChild != null)
					RemoveLogicalChild(_logicalChild);

				_logicalChild = value;

				if (_logicalChild != null)
					AddLogicalChild(_logicalChild);
			}
		}

		protected override IEnumerator LogicalChildren => _logicalChild != null ? EnumeratorUtils.Concat(_logicalChild, base.LogicalChildren) : base.LogicalChildren;

		internal IPopupOwner Owner { get; set; }

		internal PopupPanel Panel { get; }

		public PopupPlacementOptions PlacementOptions
		{
			get => (PopupPlacementOptions) GetValue(PlacementOptionsProperty);
			set => SetValue(PlacementOptionsProperty, value);
		}

		internal PopupCloseController PopupCloseController { get; }

		internal PopupWndSource PopupSource { get; }

		public bool StaysOpen
		{
			get => (bool) GetValue(StaysOpenProperty);
			set => SetValue(StaysOpenProperty, value);
		}

		internal PopupTreeMode TreeMode
		{
			get => _treeMode;
			set
			{
				if (_treeMode == value)
					return;

				if (TemplateRoot != null)
				{
					TemplateRoot.Popup = null;

					_treeMode = value;

					TemplateRoot.Popup = this;
				}
				else
					_treeMode = value;

				if (_treeMode == PopupTreeMode.Detached)
					Panel.SetBinding(DataContextProperty, new Binding {Path = new PropertyPath(DataContextProperty), Source = this});
				else
					Panel.ClearValue(DataContextProperty);
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Popup = this;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			return XamlConstants.ZeroSize;
		}

		internal void BringToFront()
		{
			if (PresentationCoreUtils.IsInDesignMode)
				return;

			PopupSource.ClearValue(PopupWndSource.IsOpenProperty);
			PopupSource.SetBinding(PopupWndSource.IsOpenProperty, new Binding {Path = new PropertyPath(IsOpenProperty), Source = this, Mode = BindingMode.TwoWay});
		}

		private static bool CoerceIsOpen(bool value)
		{
			return value;
		}

		public static Popup FromElement(DependencyObject dependencyObject)
		{
			return dependencyObject.GetAncestorsAndSelf(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).OfType<Popup>().FirstOrDefault();
		}

		public static bool GetHitTestVisible(DependencyObject element)
		{
			return (bool) element.GetValue(HitTestVisibleProperty);
		}

		public static Thickness GetInflate(DependencyObject element)
		{
			return (Thickness) element.GetValue(InflateProperty);
		}

		internal void InvalidatePlacement()
		{
			Panel?.InvalidatePlacement();
		}

		private void OnChildChanged(UIElement oldChild, UIElement newChild)
		{
			PlatformOnChildChanged(oldChild, newChild);

			if (oldChild is PopupContentPresenter oldPopupPresenter)
				oldPopupPresenter.Popup = null;

			if (newChild is PopupContentPresenter newPopupPresenter)
				newPopupPresenter.Popup = this;
		}

		private void OnClosed()
		{
			Closed?.Invoke(this, EventArgs.Empty);
			Placement?.OnPopupClosedInt();

			Panel.OnPopupClosed();

			InvalidatePlacement();
		}

		private static void OnInflatePropertyChanged(DependencyObject dependencyObject)
		{
		}

		private void OnIsHiddenChanged()
		{
			Panel?.OnIsHiddenChanged();
		}

		private void OnIsOpenChanged(bool oldValue, bool newValue)
		{
			if (IsOpen)
				OnOpened();
			else
				OnClosed();

			if (Child is IPopupChild popupChild)
				popupChild.IsOpen = IsOpen;

			if (IsOpen)
				PlatformOnOpened();

			IsOpenChanged?.Invoke(this, EventArgs.Empty);
		}

		private void OnOpened()
		{
			InvalidatePlacement();

			Placement?.OnPopupOpenedInt();

			Opened?.Invoke(this, EventArgs.Empty);
		}

		private void OnOverflowOptionsChanged()
		{
			InvalidatePlacement();
		}

		private void OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
			Panel?.InvalidatePlacement();

			if (oldPlacement != null)
				oldPlacement.Popup = null;

			if (newPlacement != null)
				newPlacement.Popup = this;

			PlacementChanged?.Invoke(this, EventArgs.Empty);
		}

		partial void PlatformCtor();

		partial void PlatformOnChildChanged(UIElement oldChild, UIElement newChild);

		partial void PlatformOnOpened();

		public static void SetHitTestVisible(DependencyObject element, bool value)
		{
			element.SetValue(HitTestVisibleProperty, value);
		}

		public static void SetInflate(DependencyObject element, Thickness value)
		{
			element.SetValue(InflateProperty, value);
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Popup = null;

			base.UndoTemplateOverride();
		}

		public event EventHandler IsOpenChanged;
		public event EventHandler PlacementChanged;

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public PopupPlacement Placement
		{
			get => (PopupPlacement) GetValue(PlacementProperty);
			set => SetValue(PlacementProperty, value);
		}
	}

	internal enum PopupTreeMode
	{
		Visual,
		Logical,
		Detached
	}

	internal interface IPopupOwner
	{
		bool CloseOnLostKeyboardFocus { get; }

		FrameworkElement FocusScopeElement { get; }
	}

#if true
	internal sealed class PopupWndSource : System.Windows.Controls.Primitives.Popup
	{
		public PopupWndSource(Popup popup)
		{
			Popup = popup;
		}
		
		public Popup Popup { get; }

		internal void SetEventTransparent(bool value)
		{
			var hwndSource = PresentationTreeUtils.GetPopupHwndSource(this);

			if (hwndSource == null)
				return;

			var exStyle = (WS_EX) NativeMethods.GetWindowLongPtr(hwndSource.Handle, GWL.EXSTYLE);
			var originalExStyle = exStyle;

			if (value)
				exStyle |= WS_EX.TRANSPARENT;
			else
				exStyle &= ~WS_EX.TRANSPARENT;

			if (exStyle != originalExStyle)
			{
				NativeMethods.SetWindowLongPtr(hwndSource.Handle, GWL.EXSTYLE, (IntPtr) exStyle);
			}
		}
	}
#else
	internal sealed class PopupWndSource : Window
	{
		public static readonly DependencyProperty IsOpenProperty = DPM.Register<bool, PopupWndSource>
			("IsOpen", t => t.OnIsOpenChanged);

		public PopupWndSource(Popup popup)
		{
			WindowStyle = WindowStyle.None;
			SizeToContent = SizeToContent.WidthAndHeight;
			AllowsTransparency = true;
			Background = Brushes.Transparent;
			Topmost = true;
			ShowActivated = true;
			ShowInTaskbar = false;
		}

		private void OnIsOpenChanged()
		{
			if (IsOpen)
				Open();
			else
				Hide();
		}

		public bool IsOpen
		{
			get => (bool) GetValue(IsOpenProperty);
			set => SetValue(IsOpenProperty, value);
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) Content;
			set => Content = value;
		}

		public PlacementMode Placement { get; set; }
		
		public event EventHandler Opened;

		public double HorizontalOffset
		{
			get => Left;
			set => Left = value;
		}

		public double VerticalOffset
		{
			get => Top;
			set => Top = value;
		}

		public void Open()
		{
			Show();
			
			Opened?.Invoke(this, EventArgs.Empty);
		}
	}
#endif
}