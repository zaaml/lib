// <copyright file="Panel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Packed;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Panels.Core
{
	public partial class Panel : System.Windows.Controls.Panel, IPanel
	{
		#region Fields

		private WeakLinkedList<IPanelLayoutListener> _layoutListeners;

#if DEBUG
		private byte _packedValue;
#endif

		#endregion

		#region Ctors

		public Panel()
		{
			ReadOnlyChildren = new ReadOnlyUIElementCollection(this);

			PlatformCtor();
		}

		#endregion

		#region Properties

#if DEBUG
		public bool Debug
		{
			get => PackedDefinition.Debug.GetValue(_packedValue);
			set => PackedDefinition.Debug.SetValue(ref _packedValue, value);
		}
#endif

		private WeakLinkedList<IPanelLayoutListener> EnsureLayoutListeners => _layoutListeners ??= new WeakLinkedList<IPanelLayoutListener>();

		internal bool HasLogicalOrientationInt => HasLogicalOrientation;

		internal Orientation LogicalOrientationInt => LogicalOrientation;

		internal ReadOnlyUIElementCollection ReadOnlyChildren { get; }

		#endregion

		#region  Methods

		protected sealed override Size ArrangeOverride(Size finalSize)
		{
			if (_layoutListeners == null)
				return ArrangeOverrideCore(finalSize);

			foreach (var layoutListener in EnsureLayoutListeners)
				layoutListener.Arranging(finalSize);

			var size = ArrangeOverrideCore(finalSize);

			foreach (var layoutListener in EnsureLayoutListeners)
				layoutListener.Arranged(finalSize, size);

			return size;
		}

		protected virtual Size ArrangeOverrideCore(Size finalSize)
		{
			return BaseArrange(finalSize);
		}

		protected Size BaseArrange(Size finalSize)
		{
			var finalRect = new Rect(new Point(), finalSize);
			var count = Children.Count;

			for (var i = 0; i < count; i++)
			{
				var child = Children[i];
				var panelAdvisor = child as IPanelAdvisor;

				if (panelAdvisor?.ShouldArrange == false)
					continue;

				child.Arrange(finalRect);
			}

			return finalSize;
		}

		protected Size BaseMeasure(Size availableSize)
		{
			return this.OnMeasureOverride(MeasureImpl, availableSize);
		}

		private Size MeasureImpl(Size availableSize)
		{
			var size = new Size();
			var count = Children.Count;

			for (var i = 0; i < count; i++)
			{
				var child = Children[i];
				var panelAdvisor = child as IPanelAdvisor;

				if (panelAdvisor?.ShouldMeasure == false)
					continue;

				child.Measure(availableSize);
				size = size.ExpandTo(child.DesiredSize);
			}

			return size;
		}

		protected sealed override Size MeasureOverride(Size availableSize)
		{
			if (_layoutListeners == null)
				return MeasureOverrideCore(availableSize);

			foreach (var layoutListener in EnsureLayoutListeners)
				layoutListener.Measuring(availableSize);

			var size = MeasureOverrideCore(availableSize);

			foreach (var layoutListener in EnsureLayoutListeners)
				layoutListener.Measured(availableSize, size);

			return size;
		}

		protected virtual Size MeasureOverrideCore(Size availableSize)
		{
			return BaseMeasure(availableSize);
		}

		partial void PlatformCtor();

		#endregion

		#region Interface Implementations

		#region IPanel

		void IPanel.AttachLayoutListener(IPanelLayoutListener layoutListener)
		{
			EnsureLayoutListeners.Add(layoutListener);
		}

		void IPanel.DetachLayoutListener(IPanelLayoutListener layoutListener)
		{
			EnsureLayoutListeners.Remove(layoutListener);
		}

		IReadOnlyList<UIElement> IPanel.Elements => ReadOnlyChildren;

		#endregion

		#endregion

		#region  Nested Types

#if DEBUG
		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition Debug;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				Debug = allocator.AllocateBoolItem();
			}

			#endregion
		}
#endif

		#endregion
	}

#if COMPILE_EVER
	internal class ElementLayoutDefinition
	{
		public FrameworkElement Target { get; set; }
		public string TargetName { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public Thickness Margin { get; set; }
		public Thickness Padding { get; set; }
		public HorizontalAlignment HorizontalAlignment { get; set; }
		public VerticalAlignment VerticalAlignment { get; set; }
		public double MinWidth { get; set; }
		public double MaxWidth { get; set; }
		public double MinHeight { get; set; }
		public double MaxnHeight { get; set; }
	}

	internal class ElementGridLayoutDefinitin : ElementLayoutDefinition
	{
		public int Row { get; set; }
		public int Column { get; set; }
	}
#endif
}