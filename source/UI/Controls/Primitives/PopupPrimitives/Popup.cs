// <copyright file="Popup.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Threading;
using Zaaml.Core.Utils;
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
			("IsOpen", false, t => t.OnIsOpenChanged, p => p.CoerceIsOpen);

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
			PopupSource = new PopupSource(this);

			Panel = new PopupPanel(this)
			{
				Visibility = Visibility.Collapsed
			};

			PopupCloseController = new PopupCloseController(this);

			PlatformCtor();

			IsTabStop = false;
		}

		public FrameworkElement Child
		{
			get => (FrameworkElement) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private bool DispatcherClosing { get; set; }

		public bool IsHidden
		{
			get => (bool) GetValue(IsHiddenProperty);
			set => SetValue(IsHiddenProperty, value);
		}

		private bool IsOpenAttached => ReferenceEquals(PopupSource.ReadLocalBinding(PopupSource.IsOpenProperty)?.Source, this);

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

		private bool OpeningLocked { get; set; }

		internal IPopupOwner Owner { get; set; }

		internal PopupPanel Panel { get; }

		public PopupPlacementOptions PlacementOptions
		{
			get => (PopupPlacementOptions) GetValue(PlacementOptionsProperty);
			set => SetValue(PlacementOptionsProperty, value);
		}

		internal PopupCloseController PopupCloseController { get; }

		internal PopupSource PopupSource { get; }

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

			PopupSource.ClearValue(PopupSource.IsOpenProperty);
			PopupSource.SetBinding(PopupSource.IsOpenProperty, new Binding {Path = new PropertyPath(IsOpenProperty), Source = this, Mode = BindingMode.TwoWay});
		}

		public void Close()
		{
			if (DispatcherClosing)
				return;

			IsOpen = false;
		}

		internal void CloseDispatcher()
		{
			if (DispatcherClosing)
				return;

			try
			{
				DispatcherClosing = false;

				PreIsOpenChange(false);
				Dispatcher.Invoke(Close, DispatcherPriority.Input);
			}
			finally
			{
				DispatcherClosing = false;
			}
		}

		private bool CoerceIsOpen(bool value)
		{
			if (OpeningLocked)
				return false;

			if (value && DispatcherClosing)
				return false;

			PreIsOpenChange(value);

			return value;
		}

		internal void DisableOpenUntilNextLayoutUpdate()
		{
			OpeningLocked = true;

			this.InvokeOnLayoutUpdate(() => OpeningLocked = false);
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

		internal static bool IsMouseEventSourceHitTestVisible(DependencyObject mouseEventSource)
		{
			return mouseEventSource.GetAncestorsAndSelf(VisualTreeEnumerationStrategy.Instance).OfType<FrameworkElement>().Any(f => GetHitTestVisible(f) == false) == false;
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

		public void Open()
		{
			if (DispatcherClosing)
				return;

			IsOpen = true;
		}

		partial void PlatformCtor();

		partial void PlatformOnChildChanged(UIElement oldChild, UIElement newChild);

		partial void PlatformOnOpened();

		private void PreIsOpenChange(bool value)
		{
			Panel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
		}

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

	internal sealed class PopupSource : System.Windows.Controls.Primitives.Popup
	{
		public PopupSource(Popup popup)
		{
			Popup = popup;
		}

		public Popup Popup { get; }
	}
}