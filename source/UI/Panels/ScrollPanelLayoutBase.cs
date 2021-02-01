// <copyright file="ScrollPanelLayoutBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.UI.Controls.ScrollView;
using Zaaml.UI.Panels.Core;

// ReSharper disable StaticMemberInGenericType

namespace Zaaml.UI.Panels
{
	internal interface IScrollViewPanelLayout
	{
		event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;

		bool CanHorizontallyScroll { get; set; }

		bool CanVerticallyScroll { get; set; }

		Size Extent { get; }

		Vector Offset { get; set; }

		Size Viewport { get; }

		void ExecuteScrollCommand(ScrollCommandKind command);
	}

	internal abstract class ScrollPanelLayoutBase<TPanel> : PanelLayoutBase<TPanel>, IScrollViewPanelLayout where TPanel : IPanel, IScrollViewPanel
	{
		private byte _packedValue;
		private ScrollInfo _scrollInfo;
		private event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;

		protected ScrollPanelLayoutBase(TPanel panel) : base(panel)
		{
		}

		protected bool CanHorizontallyScroll
		{
			get => PackedDefinition.CanHorizontallyScroll.GetValue(_packedValue);
			set
			{
				if (CanHorizontallyScroll == value)
					return;

				PackedDefinition.CanHorizontallyScroll.SetValue(ref _packedValue, value);

				OnCanScrollChanged();
			}
		}

		protected bool CanVerticallyScroll
		{
			get => PackedDefinition.CanVerticallyScroll.GetValue(_packedValue);
			set
			{
				if (CanVerticallyScroll == value)
					return;

				PackedDefinition.CanVerticallyScroll.SetValue(ref _packedValue, value);

				OnCanScrollChanged();
			}
		}

		protected Size Extent => ScrollInfo.Extent;

		protected Vector Offset
		{
			get => ScrollInfo.Offset;
			set => ScrollInfo = ScrollInfo.WithOffset(value, true);
		}

		protected ScrollInfo ScrollInfo
		{
			get => _scrollInfo;
			set
			{
				var oldScrollInfo = _scrollInfo;

				if (oldScrollInfo.Equals(value))
					return;

				_scrollInfo = value;

				OnScrollInfoChanged(new ScrollInfoChangedEventArgs(oldScrollInfo, _scrollInfo));
			}
		}

		protected Size Viewport => ScrollInfo.Viewport;

		protected virtual void ExecuteScrollCommand(ScrollCommandKind command)
		{
		}

		private void OnCanScrollChanged()
		{
		}

		private protected virtual void OnOffsetChanged()
		{
		}

		protected virtual void OnScrollInfoChanged(ScrollInfoChangedEventArgs eventArgs)
		{
			ScrollInfoChanged?.Invoke(this, eventArgs);
		}

		event EventHandler<ScrollInfoChangedEventArgs> IScrollViewPanelLayout.ScrollInfoChanged
		{
			add => ScrollInfoChanged += value;
			remove => ScrollInfoChanged -= value;
		}

		bool IScrollViewPanelLayout.CanHorizontallyScroll
		{
			get => CanHorizontallyScroll;
			set => CanHorizontallyScroll = value;
		}

		bool IScrollViewPanelLayout.CanVerticallyScroll
		{
			get => CanVerticallyScroll;
			set => CanVerticallyScroll = value;
		}

		Size IScrollViewPanelLayout.Extent => Extent;

		Vector IScrollViewPanelLayout.Offset
		{
			get => Offset;
			set
			{
				if (Offset.Equals(value))
					return;

				Offset = value;

				OnOffsetChanged();
			}
		}

		Size IScrollViewPanelLayout.Viewport => Viewport;

		void IScrollViewPanelLayout.ExecuteScrollCommand(ScrollCommandKind command)
		{
			ExecuteScrollCommand(command);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition CanHorizontallyScroll;
			public static readonly PackedBoolItemDefinition CanVerticallyScroll;

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				CanHorizontallyScroll = allocator.AllocateBoolItem();
				CanVerticallyScroll = allocator.AllocateBoolItem();
			}
		}
	}
}