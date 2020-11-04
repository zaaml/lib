// <copyright file="Popup.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using NativePopup = System.Windows.Controls.Primitives.Popup;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed partial class Popup
	{
		private static readonly Brush TransparentBrush = Colors.Transparent.ToSolidColorBrush().AsFrozen();

		private byte _packedValue;

		internal bool HandleKeyboardEvents
		{
			get => PackedDefinition.HandleKeyboardEvents.GetValue(_packedValue);
			set => PackedDefinition.HandleKeyboardEvents.SetValue(ref _packedValue, value);
		}

		internal bool HandleMouseEvents
		{
			get => PackedDefinition.HandleMouseEvents.GetValue(_packedValue);
			set => PackedDefinition.HandleMouseEvents.SetValue(ref _packedValue, value);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			return XamlConstants.ZeroSize;
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == IsHitTestVisibleProperty)
				UpdateHitTest();
		}

		partial void PlatformCtor()
		{
			PopupSource.Child = Panel;
			Panel.Background = TransparentBrush;
			PopupSource.Placement = PlacementMode.Absolute;
			PopupSource.AllowsTransparency = true;

			Focusable = false;
			Panel.Focusable = true;

			//HandleMouseEvents = true;
			//Panel.FocusVisualStyle = null;
			//FocusManager.SetIsFocusScope(Panel, true);
		}

		partial void PlatformOnChildChanged(UIElement oldChild, UIElement newChild)
		{
			Panel.Child = newChild;
		}

		partial void PlatformOnOpened()
		{
			if (IsOpenAttached)
				return;

			if (TemplateRoot == null)
				ApplyTemplate();

			BringToFront();
		}

		private static void UpdateHitTest()
		{
			//if (IsHitTestVisible == false)
			//{
			//  var popupHwndSource = PresentationSource.FromVisual(Panel) as HwndSource;
			//  if (popupHwndSource != null)
			//  {
			//    var extendedStyle = NativeMethodsSafe.GetWindowStyleEx(popupHwndSource.Handle);
			//    extendedStyle |= WS_EX.TRANSPARENT;
			//    NativeMethods.SetWindowLongPtr(popupHwndSource.Handle, GWL.EXSTYLE, new IntPtr((int) extendedStyle));
			//  }
			//  else
			//  {
			//    this.InvokeOnLayoutUpdate(UpdateHitTest);
			//  }
			//}
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition HandleKeyboardEvents;
			public static readonly PackedBoolItemDefinition HandleMouseEvents;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				HandleKeyboardEvents = allocator.AllocateBoolItem();
				HandleMouseEvents = allocator.AllocateBoolItem();
			}
		}
	}
}