// <copyright file="SnapPlacement.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Snapping;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public sealed class SnapPlacement : RelativePlacementBase
	{
		private static readonly DependencyPropertyKey ActualSnapSidePropertyKey = DPM.RegisterAttachedReadOnly<SnapSide, SnapPlacement>
			("ActualSnapSide");

		private static readonly DependencyPropertyKey ActualSnapPointPropertyKey = DPM.RegisterAttachedReadOnly<SnapPoint, SnapPlacement>
			("ActualSnapPoint");

		public static readonly DependencyProperty SnapSideProperty = DPM.Register<SnapSide, SnapPlacement>
			("SnapSide", p => p.Invalidate);

		public static readonly DependencyProperty DefinitionProperty = DPM.Register<SnapDefinition, SnapPlacement>
			("Definition", p => p.Invalidate);

		public static readonly DependencyProperty SourceAdjustmentProperty = DPM.Register<SnapAdjustment, SnapPlacement>
			("SourceAdjustment");

		public static readonly DependencyProperty TargetAdjustmentProperty = DPM.Register<SnapAdjustment, SnapPlacement>
			("TargetAdjustment");

		public static readonly DependencyProperty AttachedDefinitionProperty = DPM.RegisterAttached<SnapDefinition, SnapPlacement>
			("AttachedDefinition");

		public static readonly DependencyProperty AttachedAdjustmentProperty = DPM.RegisterAttached<SnapAdjustment, SnapPlacement>
			("AttachedAdjustment");

		public static readonly DependencyProperty ActualSnapSideProperty = ActualSnapSidePropertyKey.DependencyProperty;
		public static readonly DependencyProperty ActualSnapPointProperty = ActualSnapPointPropertyKey.DependencyProperty;

		public SnapDefinition Definition
		{
			get => (SnapDefinition)GetValue(DefinitionProperty);
			set => SetValue(DefinitionProperty, value);
		}

		internal override bool ShouldConstraint => false;

		public SnapSide SnapSide
		{
			get => (SnapSide)GetValue(SnapSideProperty);
			set => SetValue(SnapSideProperty, value);
		}

		public SnapAdjustment SourceAdjustment
		{
			get => (SnapAdjustment)GetValue(SourceAdjustmentProperty);
			set => SetValue(SourceAdjustmentProperty, value);
		}

		public SnapAdjustment TargetAdjustment
		{
			get => (SnapAdjustment)GetValue(TargetAdjustmentProperty);
			set => SetValue(TargetAdjustmentProperty, value);
		}

		protected override Rect ArrangeOverride(Size desiredSize)
		{
			var snapSide = SnapSide;
			var sourceAdjustment = GetAttachedAdjustment(Popup) + GetAttachedAdjustment(Popup.Child) + SourceAdjustment;
			var targetAdjustment = GetAttachedAdjustment(ActualTarget) + TargetAdjustment;
			var snapOptions = ConvertOptions(Popup.PlacementOptions);
			var snapDefinition = Definition ?? GetAttachedDefinition(Popup) ?? SnapDefinition.Default;

			if (Popup.Panel.IsMouseVisited && (Popup.PlacementOptions & PopupPlacementOptions.PreservePosition) != 0)
			{
				snapSide = GetActualSnapSide(Popup);

				if ((snapOptions & SnapOptions.Fit) != 0)
					snapOptions ^= SnapOptions.Fit;
			}

			var rect = Snapper.Snap(ScreenBoundsCore, TargetScreenBox, desiredSize.Rect(), snapOptions, snapDefinition, targetAdjustment, sourceAdjustment, ref snapSide, out var snapPoint);

			SetActualSnapSide(Popup, snapSide);
			SetActualSnapPoint(Popup, snapPoint);

			return rect;
		}

		public static SnapPoint GetActualSnapPoint(DependencyObject element)
		{
			return element.GetReadOnlyValue<SnapPoint>(ActualSnapPointPropertyKey);
		}

		public static SnapSide GetActualSnapSide(DependencyObject element)
		{
			return element.GetReadOnlyValue<SnapSide>(ActualSnapSidePropertyKey);
		}

		public static SnapAdjustment GetAttachedAdjustment(DependencyObject element)
		{
			if (element == null)
				return new SnapAdjustment();

			return (SnapAdjustment)element.GetValue(AttachedAdjustmentProperty);
		}

		public static SnapDefinition GetAttachedDefinition(DependencyObject element)
		{
			return (SnapDefinition)element.GetValue(AttachedDefinitionProperty);
		}

		protected override void OnPopupAttached()
		{
			base.OnPopupAttached();

			SetActualSnapSide(Popup, SnapSide.Left);
			SetActualSnapPoint(Popup, SnapPoint.LeftBottom);
		}

		protected override void OnPopupDetaching()
		{
			SetActualSnapSide(Popup, SnapSide.Left);
			SetActualSnapPoint(Popup, SnapPoint.LeftBottom);

			base.OnPopupDetaching();
		}

		private static void SetActualSnapPoint(DependencyObject element, SnapPoint value)
		{
			element.SetReadOnlyValue(ActualSnapPointPropertyKey, value);
		}

		private static void SetActualSnapSide(DependencyObject element, SnapSide value)
		{
			element.SetReadOnlyValue(ActualSnapSidePropertyKey, value);
		}

		public static void SetAttachedAdjustment(DependencyObject element, SnapAdjustment value)
		{
			element.SetValue(AttachedAdjustmentProperty, value);
		}

		public static void SetAttachedDefinition(DependencyObject element, SnapDefinition value)
		{
			element.SetValue(AttachedDefinitionProperty, value);
		}
	}
}