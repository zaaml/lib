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
		#region Fields

		event EventHandler<ScrollInfoChangedEventArgs> ScrollInfoChanged;
		event EventHandler<OffsetChangedEventArgs> OffsetChanged;

		#endregion

		#region Properties

		bool CanHorizontallyScroll { get; set; }

		bool CanVerticallyScroll { get; set; }

		Size Extent { get; }

		Vector Offset { get; set; }

		Size Viewport { get; }

		#endregion

		#region  Methods

		void ExecuteScrollCommand(ScrollCommandKind command);

		#endregion
	}

	internal abstract class ScrollPanelLayoutBase<TPanel> : PanelLayoutBase<TPanel>, IScrollViewPanelLayout where TPanel : IPanel, IScrollViewPanel
	{
		private Vector _offset;
		private byte _packedValue;
		private ScrollInfo _scrollInfo;
		private event EventHandler<OffsetChangedEventArgs> OffsetChanged;
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
	
		protected virtual Size Extent => ScrollInfo.Extent;

		protected Vector Offset
		{
			get => _offset;
			set
			{
				if (_offset.Equals(value))
					return;

				var offsetChangedEventArgs = new OffsetChangedEventArgs(_offset, value);

				_offset = value;

				OnOffsetChanged(offsetChangedEventArgs);
			}
		}

		protected ScrollInfo ScrollInfo
		{
			get => _scrollInfo;
			set
			{
				var oldScrollInfo = ScrollInfo;

				if (oldScrollInfo.Equals(value))
					return;

				_scrollInfo = value;

				OnScrollInfoChanged(new ScrollInfoChangedEventArgs(oldScrollInfo, _scrollInfo));
			}
		}

		protected virtual Size Viewport => ScrollInfo.Viewport;

		protected virtual void ExecuteScrollCommand(ScrollCommandKind command)
		{
		}

		private void OnCanScrollChanged()
		{
		}

		protected virtual void OnOffsetChanged(OffsetChangedEventArgs eventArgs)
		{
			OffsetChanged?.Invoke(this, eventArgs);
		}

		protected void OnScrollInfoChanged(ScrollInfoChangedEventArgs eventArgs)
		{
			ScrollInfoChanged?.Invoke(this, eventArgs);
		}

		event EventHandler<ScrollInfoChangedEventArgs> IScrollViewPanelLayout.ScrollInfoChanged
		{
			add => ScrollInfoChanged += value;
			remove => ScrollInfoChanged -= value;
		}

		event EventHandler<OffsetChangedEventArgs> IScrollViewPanelLayout.OffsetChanged
		{
			add => OffsetChanged += value;
			remove => OffsetChanged -= value;
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
			set => Offset = value;
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