// <copyright file="PopupControlController.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Disposable;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Utils;
#if !SILVERLIGHT
using Zaaml.PresentationCore.Input;
using System.Windows.Input;

#endif

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	internal abstract class PopupControlController : DependencyObject, IPopupControllerSelector, IPopupOwner
	{
		private static readonly List<PopupControlController> OpenControllers = new List<PopupControlController>();

		internal static readonly DependencyProperty PlacementDummyProperty = DPM.RegisterAttached<PopupPlacement, PopupControlController>
			("Placement");

		internal static readonly DependencyProperty IsOpenDummyProperty = DPM.RegisterAttached<bool, PopupControlController>
			("IsOpen");

		private static readonly DependencyPropertyKey OwnerDummyPropertyKey = DPM.RegisterAttachedReadOnly<FrameworkElement, PopupControlController>
			("Owner");

		private readonly Binding _controllerSelectorBinding;

#pragma warning disable 649
		private bool _captureMouse;
#pragma warning restore 649

		private IDisposable _globalTreeDetacher;

#if !SILVERLIGHT
		private MouseCaptureListener _mouseCaptureListener;
#endif
		private byte _packedValue;
		private Popup _popup;

		public event EventHandler<QueryCloseOnClickEventArgs> QueryCloseOnClick;

		static PopupControlController()
		{
			CompositionTarget.Rendering += OnRendering;
		}

		protected PopupControlController()
		{
			_controllerSelectorBinding = new Binding { Source = this, BindsDirectlyToSource = true, Mode = BindingMode.OneTime };
		}

		private bool ActualAttachToGlobalPopup
		{
			get => PackedDefinition.ActualAttachToGlobalPopup.GetValue(_packedValue);
			set => PackedDefinition.ActualAttachToGlobalPopup.SetValue(ref _packedValue, value);
		}

		private bool AttachToGlobalPopup
		{
			set
			{
				if (ActualAttachToGlobalPopup == value)
					return;

				if (ActualAttachToGlobalPopup)
					DetachFromGlobalTree();

				ActualAttachToGlobalPopup = value;

				if (ActualAttachToGlobalPopup)
					AttachToGlobalTree();
			}
		}

		private bool CaptureMouse
		{
			get { return _captureMouse; }
			set
			{
#if !SILVERLIGHT
				if (_captureMouse == value)
					return;

				_mouseCaptureListener = _mouseCaptureListener.DisposeExchange();

				_captureMouse = value;

				if (Popup == null)
					return;

				if (value)
					_mouseCaptureListener = new MouseCaptureListener(Popup.TreeMode == PopupTreeMode.Visual ? FrameworkElement : Popup.Panel, CaptureMode.SubTree);
#endif
			}
		}

		public bool CloseOnLostKeyboardFocus
		{
			get => PackedDefinition.CloseOnLostKeyboardFocus.GetValue(_packedValue);
			set => PackedDefinition.CloseOnLostKeyboardFocus.SetValue(ref _packedValue, value);
		}

		protected abstract IManagedPopupControl ControllablePopup { get; }

		protected abstract FrameworkElement FrameworkElement { get; }

		public bool IsModalMenu { get; set; }

		public bool IsOpen
		{
			get => FrameworkElement.GetValue<bool>(IsOpenProperty);
			set => FrameworkElement.SetValue<bool>(IsOpenProperty, value);
		}

		internal DependencyProperty IsOpenProperty => ControllablePopup.IsOpenProperty ?? IsOpenDummyProperty;

		public FrameworkElement Owner
		{
			get => FrameworkElement.GetValue<FrameworkElement>(OwnerPropertyKey);
			set => FrameworkElement.SetReadOnlyValue(OwnerPropertyKey, value);
		}

		private DependencyPropertyKey OwnerPropertyKey => ControllablePopup.OwnerPropertyKey ?? OwnerDummyPropertyKey;

		public PopupPlacement Placement
		{
			get => FrameworkElement.GetValue<PopupPlacement>(PlacementProperty);
			set => FrameworkElement.SetValue<PopupPlacement>(PlacementProperty, value);
		}

		private DependencyProperty PlacementProperty => ControllablePopup.PlacementProperty ?? PlacementDummyProperty;

		public Popup Popup
		{
			get => _popup;
			set
			{
				if (ReferenceEquals(_popup, value))
					return;

				if (_popup != null)
				{
					CaptureMouse = false;

					DetachPopup(_popup);
				}

				_popup = value;

				if (_popup != null)
				{
					AttachPopup(_popup);

					if (IsOpen)
						CaptureMouse = ShouldCaptureMouse;
				}
			}
		}

		private bool ShouldAttachSelector => !(ControllablePopup is IContextPopupControlInternal ctx) || ctx.OwnerAttachSelector;

		private bool ShouldCaptureMouse => IsModalMenu && Popup?.StaysOpen != true;

		private bool SuspendIsOpenChangedHandler
		{
			get => PackedDefinition.SuspendIsOpenChangedHandler.GetValue(_packedValue);
			set => PackedDefinition.SuspendIsOpenChangedHandler.SetValue(ref _packedValue, value);
		}

		public int Version { get; private set; }

		private void AttachPopup(Popup popup)
		{
			popup.Owner = this;
			popup.PopupCloseController.Closing += OnPopupControllerClosing;
			popup.PopupCloseController.QueryCloseOnClick += OnQueryCloseOnClick;

			popup.PopupSource.Opened += OnPopupSourceOpened;
			popup.PopupSource.Closed += OnPopupSourceClosed;

			popup.BindProperties(Popup.PlacementProperty, FrameworkElement, PlacementProperty, converter: PopupPlacementWrapperConverter.Instance);
			popup.BindProperties(Popup.IsOpenProperty, FrameworkElement, IsOpenProperty, BindingMode.TwoWay);
		}

		private void AttachToGlobalTree()
		{
			var control = FrameworkElement;

			if (control != null && control.IsInLiveTree() == false)
				_globalTreeDetacher = _globalTreeDetacher.DisposeExchange(DelegateDisposable.Create(() => GlobalPopup.Instance.Attach(control), () => GlobalPopup.Instance.Detach(control)));
		}

		private void ClosePopup()
		{
			try
			{
				SuspendIsOpenChangedHandler = true;

				if (OnClosing() == false)
				{
					IsOpen = true;

					return;
				}

				if (IsOpen == false)
					IsOpen = false;

				OnClosed();
			}
			finally
			{
				OpenControllers.Remove(this);
				AttachToGlobalPopup = false;
				SuspendIsOpenChangedHandler = false;

				if (IsOpen)
					OpenPopup();
			}
		}

		private void DetachFromGlobalTree()
		{
			_globalTreeDetacher = _globalTreeDetacher.DisposeExchange();
		}

		private void DetachPopup(Popup popup)
		{
			popup.Owner = null;
			popup.PopupCloseController.Closing -= OnPopupControllerClosing;
			popup.PopupCloseController.QueryCloseOnClick -= OnQueryCloseOnClick;

			popup.ClearValue(Popup.IsOpenProperty);
			popup.ClearValue(Popup.PlacementProperty);

			popup.PopupSource.Opened -= OnPopupSourceOpened;
			popup.PopupSource.Closed -= OnPopupSourceClosed;
		}

		private void EnsureContextOpen(int version)
		{
			if (version == Version)
				OpenPopup();
		}

		private void OnClosed()
		{
			CaptureMouse = false;

			Version++;

			ControllablePopup.OnClosed();
		}

		private bool OnClosing()
		{
			var cancelEventArgs = new PopupCancelEventArgs(ExplicitCloseReason.Instance, false);

			ControllablePopup.OnClosing(cancelEventArgs);

			return cancelEventArgs.Cancel == false;
		}

		internal void OnIsOpenChanged()
		{
			if (SuspendIsOpenChangedHandler)
			{
				ControllablePopup.OnIsOpenChanged();

				return;
			}

			if (IsOpen)
				OpenPopup();
			else
				ClosePopup();

			ControllablePopup.OnIsOpenChanged();
		}

		private void OnOpened()
		{
			ControllablePopup.OnOpened();
		}

		private bool OnOpening()
		{
			var cancelEventArgs = new PopupCancelEventArgs(null, false);

			ControllablePopup.OnOpening(cancelEventArgs);

			if (cancelEventArgs.Cancel == false)
				Version++;

			return cancelEventArgs.Cancel == false;
		}

		internal void OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			// ResourceContextMenu & ResourceContextBar should not change popup selector on owner change.
			if (ShouldAttachSelector)
			{
				if (oldOwner != null && ReferenceEquals(newOwner.ReadLocalBinding(PopupControlService.ControllerSelectorProperty), _controllerSelectorBinding))
					PopupControlService.ClearControllerSelector(oldOwner);

				if (newOwner != null && newOwner.HasLocalValue(PopupControlService.ControllerSelectorProperty) == false)
					newOwner.SetBinding(PopupControlService.ControllerSelectorProperty, _controllerSelectorBinding);
			}

			ControllablePopup.OnOwnerChanged(oldOwner, newOwner);
		}

		public void OnPlacementChanged(PopupPlacement oldPlacement, PopupPlacement newPlacement)
		{
			ControllablePopup.OnPlacementChanged(oldPlacement, newPlacement);
		}

		private void OnPopupControllerClosing(object sender, PopupCancelEventArgs e)
		{
			ControllablePopup.OnClosing(e);

			CaptureMouse = false;
		}

		private void OnPopupSourceClosed(object sender, EventArgs eventArgs)
		{
			CaptureMouse = false;
		}

		private void OnPopupSourceOpened(object sender, EventArgs eventArgs)
		{
			CaptureMouse = ShouldCaptureMouse;
		}

		public void OnPopupTreeModeChanged()
		{
			if (!CaptureMouse)
				return;

			CaptureMouse = false;
			CaptureMouse = true;
		}

		private void OnQueryCloseOnClick(object sender, QueryCloseOnClickEventArgs e)
		{
			QueryCloseOnClick?.Invoke(this, e);
		}

		private static void OnRendering(object sender, EventArgs eventArgs)
		{
			foreach (var popupController in OpenControllers.ToList())
				popupController.TryLoad();

			OpenControllers.Clear();
		}

		public void OpenContextControl(FrameworkElement owner, DependencyObject target)
		{
			if (target != null && target.GetAncestors(MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance).Any(a => ReferenceEquals(a, FrameworkElement)))
				return;

			if (ControllablePopup is IContextPopupControlInternal ctxControl)
			{
				ctxControl.Owner = owner;
				ctxControl.Target = target;

				if (owner is IContextPopupTarget contextPopupTarget)
					contextPopupTarget.OnContextPopupControlOpened(ctxControl);
			}

			var version = Version;

			Dispatcher.BeginInvoke(() => EnsureContextOpen(version));
		}

		protected void OpenPopup()
		{
			try
			{
				SuspendIsOpenChangedHandler = true;

				if (OnOpening() == false)
				{
					IsOpen = false;

					return;
				}

				if (IsOpen == false)
					IsOpen = true;

				OnOpened();
			}
			finally
			{
				if (IsOpen)
					OpenControllers.Add(this);

				SuspendIsOpenChangedHandler = false;
			}
		}

		private void TryLoad()
		{
			var visualParent = FrameworkElement.GetVisualParent();
			var logicalParent = FrameworkElement.GetLogicalParent();

			if (visualParent == null && logicalParent == null)
				AttachToGlobalPopup = true;
		}

		PopupControlController IPopupControllerSelector.SelectController(FrameworkElement frameworkElement, DependencyObject eventSource)
		{
			return this;
		}

		bool IPopupOwner.CloseOnLostKeyboardFocus => CloseOnLostKeyboardFocus;

		FrameworkElement IPopupOwner.FocusScopeElement => FrameworkElement;

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition ActualAttachToGlobalPopup;
			public static readonly PackedBoolItemDefinition SuspendIsOpenChangedHandler;
			public static readonly PackedBoolItemDefinition CloseOnLostKeyboardFocus;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				SuspendIsOpenChangedHandler = allocator.AllocateBoolItem();
				ActualAttachToGlobalPopup = allocator.AllocateBoolItem();
				CloseOnLostKeyboardFocus = allocator.AllocateBoolItem();
			}
		}
	}

	internal sealed class PopupControlController<T> : PopupControlController where T : Control, IManagedPopupControl
	{
		public PopupControlController(T control)
		{
			Control = control;
		}

		public T Control { get; }

		protected override IManagedPopupControl ControllablePopup => Control;

		protected override FrameworkElement FrameworkElement => Control;
	}
}