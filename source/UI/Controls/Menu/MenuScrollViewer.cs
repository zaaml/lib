// <copyright file="MenuScrollViewer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zaaml.Core.Extensions;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using ZaamlContentControl = Zaaml.UI.Controls.Core.ContentControl;

#if SILVERLIGHT
using Zaaml.UI.Extensions;

#endif

namespace Zaaml.UI.Controls.Menu
{
	public sealed class MenuScrollViewer : ZaamlContentControl, INotifyPropertyChanged
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ScrollRepeatDelayProperty = DPM.Register<TimeSpan, MenuScrollViewer>
			("ScrollRepeatDelay", TimeSpan.FromMilliseconds(40), m => m.OnScrollRepeatDelayChanged);

		#endregion

		#region Fields

		private readonly DelayAction _scrollDelayAction;
		private byte _packedValue;

		private UIElement _scrollDownElement;
		private UIElement _scrollUpElement;
		private ScrollViewer _scrollViewer;

		#endregion

		#region Ctors

		static MenuScrollViewer()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<MenuScrollViewer>();
		}

		public MenuScrollViewer()
		{
			this.OverrideStyleKey<MenuScrollViewer>();

			_scrollDelayAction = new DelayAction(OnScrollCommand, ScrollRepeatDelay) { RepeatCount = int.MaxValue };
		}

		#endregion

		#region Properties

		public bool CanScrollDown
		{
			get => PackedDefinition.CanScrollDown.GetValue(_packedValue);
			private set
			{
				if (CanScrollDown == value)
					return;

				PackedDefinition.CanScrollDown.SetValue(ref _packedValue, value);

				OnPropertyChanged("CanScrollDown");
			}
		}

		public bool CanScrollUp
		{
			get => PackedDefinition.CanScrollUp.GetValue(_packedValue);
			private set
			{
				if (CanScrollUp == value)
					return;

				PackedDefinition.CanScrollUp.SetValue(ref _packedValue, value);

				OnPropertyChanged("CanScrollUp");
			}
		}

		private ScrollCommandKind ScrollCommand
		{
			get => PackedDefinition.ScrollCommand.GetValue(_packedValue);
			set
			{
				if (ScrollCommand == value)
					return;

				PackedDefinition.ScrollCommand.SetValue(ref _packedValue, value);

				if (value == ScrollCommandKind.None)
					_scrollDelayAction.Revoke();
				else
					_scrollDelayAction.Invoke();
			}
		}

		public TimeSpan ScrollRepeatDelay
		{
			get => (TimeSpan) GetValue(ScrollRepeatDelayProperty);
			set => SetValue(ScrollRepeatDelayProperty, value);
		}

		#endregion

		#region  Methods

		protected override Size MeasureOverride(Size availableSize)
		{
			var canScrollUp = CanScrollUp;
			var canScrollDown = CanScrollDown;

			var result = base.MeasureOverride(availableSize);

			bool newCanScrollUp;
			bool newCanScrollDown;
			UpdateScrollFlags(out newCanScrollUp, out newCanScrollDown);

			if (canScrollUp == newCanScrollUp && canScrollDown == newCanScrollDown || _scrollUpElement == null || _scrollDownElement == null)
				return result;

			CanScrollUp = newCanScrollUp;
			CanScrollDown = newCanScrollDown;

			return result;
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (_scrollUpElement != null)
			{
				_scrollUpElement.MouseEnter -= OnScrollButtonMouseEnter;
				_scrollUpElement.MouseLeave -= OnScrollButtonMouseLeave;
			}

			if (_scrollDownElement != null)
			{
				_scrollDownElement.MouseEnter -= OnScrollButtonMouseEnter;
				_scrollDownElement.MouseLeave -= OnScrollButtonMouseLeave;
			}

			_scrollUpElement = (UIElement) GetTemplateChild("PART_ScrollUpElement");
			_scrollDownElement = (UIElement) GetTemplateChild("PART_ScrollDownElement");
			_scrollViewer = (ScrollViewer) GetTemplateChild("PART_ScrollViewer");

			if (_scrollUpElement != null)
			{
				_scrollUpElement.MouseEnter += OnScrollButtonMouseEnter;
				_scrollUpElement.MouseLeave += OnScrollButtonMouseLeave;
			}

			if (_scrollDownElement != null)
			{
				_scrollDownElement.MouseEnter += OnScrollButtonMouseEnter;
				_scrollDownElement.MouseLeave += OnScrollButtonMouseLeave;
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnScrollButtonMouseEnter(object sender, MouseEventArgs e)
		{
			ScrollCommand = ReferenceEquals(sender, _scrollDownElement) ? ScrollCommandKind.ScrollDown : ScrollCommandKind.ScrollUp;
		}

		private void OnScrollButtonMouseLeave(object sender, MouseEventArgs e)
		{
			if (ReferenceEquals(sender, _scrollUpElement) && ScrollCommand == ScrollCommandKind.ScrollUp)
				ScrollCommand = ScrollCommandKind.None;

			if (ReferenceEquals(sender, _scrollDownElement) && ScrollCommand == ScrollCommandKind.ScrollDown)
				ScrollCommand = ScrollCommandKind.None;
		}

		private void OnScrollCommand()
		{
			switch (ScrollCommand)
			{
				case ScrollCommandKind.ScrollUp:
					_scrollViewer?.LineUp();
					break;
				case ScrollCommandKind.ScrollDown:
					_scrollViewer?.LineDown();
					break;
			}

			bool canScrollUp;
			bool canScrollDown;
			UpdateScrollFlags(out canScrollUp, out canScrollDown);

			if (ScrollCommand == ScrollCommandKind.ScrollUp && canScrollUp == false)
				ScrollCommand = ScrollCommandKind.None;

			if (ScrollCommand == ScrollCommandKind.ScrollDown && canScrollDown == false)
				ScrollCommand = ScrollCommandKind.None;

			CanScrollUp = canScrollUp;
			CanScrollDown = canScrollDown;
		}

		private void OnScrollRepeatDelayChanged()
		{
			_scrollDelayAction.Delay = ScrollRepeatDelay;
		}

		public void ResetScroll()
		{
			_scrollViewer?.ScrollToTop();
		}

		private void UpdateScrollFlags(out bool canScrollUp, out bool canScrollDown)
		{
			canScrollUp = false;
			canScrollDown = false;

			var scrollContentPresenter = _scrollViewer?.GetImplementationRoot() as ScrollContentPresenter;
			var scrollRequired = scrollContentPresenter?.CanVerticallyScroll == true;

			if (scrollRequired == false)
				return;

			if (scrollContentPresenter.ExtentHeight <= scrollContentPresenter.ViewportHeight)
				return;

			canScrollUp = scrollContentPresenter.VerticalOffset.IsCloseTo(0) == false;
			canScrollDown = scrollContentPresenter.VerticalOffset.IsCloseTo(scrollContentPresenter.ExtentHeight - scrollContentPresenter.ViewportHeight) == false;
		}

		#endregion

		#region Interface Implementations

		#region INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#endregion

		#region  Nested Types

		private enum ScrollCommandKind
		{
			None,
			ScrollDown,
			ScrollUp
		}

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition CanScrollUp;
			public static readonly PackedBoolItemDefinition CanScrollDown;
			public static readonly PackedEnumItemDefinition<ScrollCommandKind> ScrollCommand;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				CanScrollDown = allocator.AllocateBoolItem();
				CanScrollUp = allocator.AllocateBoolItem();
				ScrollCommand = allocator.AllocateEnumItem<ScrollCommandKind>();
			}

			#endregion
		}

		#endregion
	}
}