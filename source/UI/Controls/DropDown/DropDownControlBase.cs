// <copyright file="DropDownControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.DropDown
{
	public abstract class DropDownControlBase : TemplateContractControl
	{
		public static readonly DependencyProperty PopupMinHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMinHeight", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMinWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMinWidth", new PopupLength(0.0));

		public static readonly DependencyProperty PopupMaxHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMaxHeight", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupMaxWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupMaxWidth", new PopupLength(double.PositiveInfinity));

		public static readonly DependencyProperty PopupWidthProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupWidth", PopupLength.Auto);

		public static readonly DependencyProperty PopupHeightProperty = DPM.Register<PopupLength, DropDownControlBase>
			("PopupHeight", PopupLength.Auto);

		public static readonly DependencyProperty PlacementProperty = DPM.Register<Dock, DropDownControlBase>
			("Placement", Dock.Bottom);

		public static readonly DependencyProperty PlacementOptionsProperty = DPM.Register<PopupPlacementOptions, DropDownControlBase>
			("PlacementOptions", PopupPlacementOptions.PreservePosition);

		public static readonly DependencyProperty IsDropDownOpenProperty = DPM.Register<bool, DropDownControlBase>
			("IsDropDownOpen", d => d.OnIsDropDownOpenPropertyChangedPrivate, d => d.CoerceIsDropDownOpenPrivate);

		public event EventHandler DropDownClosed;
		public event EventHandler DropDownOpened;

		public bool IsDropDownOpen
		{
			get => (bool) GetValue(IsDropDownOpenProperty);
			set => SetValue(IsDropDownOpenProperty, value);
		}

		public Dock Placement
		{
			get => (Dock) GetValue(PlacementProperty);
			set => SetValue(PlacementProperty, value);
		}

		public PopupPlacementOptions PlacementOptions
		{
			get => (PopupPlacementOptions) GetValue(PlacementOptionsProperty);
			set => SetValue(PlacementOptionsProperty, value);
		}

		protected abstract PopupControlBase PopupControlCore { get; }

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupHeight
		{
			get => (PopupLength) GetValue(PopupHeightProperty);
			set => SetValue(PopupHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxHeight
		{
			get => (PopupLength) GetValue(PopupMaxHeightProperty);
			set => SetValue(PopupMaxHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMaxWidth
		{
			get => (PopupLength) GetValue(PopupMaxWidthProperty);
			set => SetValue(PopupMaxWidthProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinHeight
		{
			get => (PopupLength) GetValue(PopupMinHeightProperty);
			set => SetValue(PopupMinHeightProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupMinWidth
		{
			get => (PopupLength) GetValue(PopupMinWidthProperty);
			set => SetValue(PopupMinWidthProperty, value);
		}

		[TypeConverter(typeof(PopupLengthTypeConverter))]
		public PopupLength PopupWidth
		{
			get => (PopupLength) GetValue(PopupWidthProperty);
			set => SetValue(PopupWidthProperty, value);
		}

		public void CloseDropDown()
		{
			if (IsDropDownOpen == false)
				return;

			this.SetCurrentValueInternal(IsDropDownOpenProperty, false);

			CoerceIsDropDownOpen();
		}

		private void CoerceIsDropDownOpen()
		{
			var popupIsOpen = PopupControlCore?.PopupController?.Popup?.IsOpen;

			if (popupIsOpen == false)
				this.SetCurrentValueInternal(IsDropDownOpenProperty, false);
			else if (popupIsOpen == true)
				this.SetCurrentValueInternal(IsDropDownOpenProperty, true);
		}

		private protected virtual bool CoerceIsDropDownOpenCorePrivate(bool isDropDownOpen)
		{
			return isDropDownOpen;
		}

		private bool CoerceIsDropDownOpenPrivate(bool isDropDownOpen)
		{
			return CoerceIsDropDownOpenCorePrivate(isDropDownOpen);
		}

		protected virtual void OnClosed()
		{
			DropDownClosed?.Invoke(this, EventArgs.Empty);
		}

		private protected virtual void OnClosedCore()
		{
			OnClosed();
		}

		protected virtual void OnIsDropDownOpenChanged()
		{
		}

		private protected virtual void OnIsDropDownOpenChangedInternal()
		{
			OnIsDropDownOpenChanged();
		}

		private void OnIsDropDownOpenPropertyChangedPrivate(bool oldValue, bool newValue)
		{
			if (IsDropDownOpen)
				OnOpenedCore();
			else
				OnClosedCore();

			UpdateVisualState(true);

			OnIsDropDownOpenChangedInternal();
		}

		protected virtual void OnOpened()
		{
			DropDownOpened?.Invoke(this, EventArgs.Empty);
		}

		private protected virtual void OnOpenedCore()
		{
			OnOpened();
		}

		public void OpenDropDown()
		{
			if (IsDropDownOpen)
				return;

			OpenDropDownCore();
		}

		private protected virtual void OpenDropDownCore()
		{
			if (PopupControlCore == null)
				return;

			this.SetCurrentValueInternal(IsDropDownOpenProperty, true);

			CoerceIsDropDownOpen();
		}
	}
}