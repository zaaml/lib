// <copyright file="DockPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using Panel = Zaaml.UI.Panels.Core.Panel;

namespace Zaaml.UI.Panels
{
	public sealed class DockPanel : Panel, IDockPanelAdvanced
	{
		public static readonly DependencyProperty DockProperty = DPM.RegisterAttached<Dock?, DockPanel>
			("Dock", null, OnDockChanged, CoerceDock);

		public static readonly DependencyProperty DockDistanceProperty = DPM.RegisterAttached<double, DockPanel>
			("DockDistance", 0, OnDockDistanceChanged, CoerceDockDistance);

		public static readonly DependencyProperty SpacingProperty = DPM.Register<double, DockPanel>
			("Spacing", 0, d => d.OnSpacingChanged, d => CoerceSpacing);

		public double Spacing
		{
			get => (double) GetValue(SpacingProperty);
			set => SetValue(SpacingProperty, value);
		}

		protected override Size ArrangeOverrideCore(Size finalSize)
		{
			return DockPanelLayout.Arrange(this, finalSize);
		}

		private static Dock? CoerceDock(DependencyObject dependencyObject, Dock? baseValue)
		{
			if (baseValue == null)
				return null;

			var dock = (Dock) baseValue;

			return dock == Dock.Left || dock == Dock.Top || dock == Dock.Right || dock == Dock.Bottom ? baseValue : null;
		}

		private static double CoerceDockDistance(DependencyObject dependencyObject, double dockDistance)
		{
			return dockDistance >= 0 ? dockDistance : 0;
		}

		private static double CoerceSpacing(double spacing)
		{
			return spacing >= 0 ? spacing : 0;
		}

		public static Dock? GetDock(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (Dock?) element.GetValue(DockProperty);
		}

		public static double GetDockDistance(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (double) element.GetValue(DockDistanceProperty);
		}

		protected override Size MeasureOverrideCore(Size availableSize)
		{
			return DockPanelLayout.Measure(this, availableSize);
		}

		private static void OnDockChanged(DependencyObject dependencyObject, Dock? oldDock, Dock? newDock)
		{
			OnLayoutPropertyChanged(dependencyObject);
		}

		private static void OnDockDistanceChanged(DependencyObject dependencyObject, double oldDistance, double newDistance)
		{
			OnLayoutPropertyChanged(dependencyObject);
		}

		private static void OnLayoutPropertyChanged(DependencyObject dependencyObject)
		{
			var uie = dependencyObject as UIElement;
			uie?.GetVisualParent<DockPanel>()?.InvalidateMeasure();
		}

		private void OnSpacingChanged(double oldValue, double newValue)
		{
			InvalidateMeasure();
		}

		public static void SetDock(UIElement element, Dock? dock)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(DockProperty, dock);
		}

		public static void SetDockDistance(UIElement element, double dockDistance)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(DockDistanceProperty, dockDistance);
		}

		double IDockPanel.GetDockDistance(UIElement element) => GetDockDistance(element);

		double IDockPanelAdvanced.Spacing => Spacing;

		Dock? IDockPanel.GetDock(UIElement element) => GetDock(element);
	}

	internal interface IDockPanel : IPanel
	{
		Dock? GetDock(UIElement element);

		double GetDockDistance(UIElement element);
	}

	internal interface IDockPanelAdvanced : IDockPanel
	{
		double Spacing { get; }
	}
	
	internal abstract class DockPanelLayout : PanelLayoutBase<IDockPanel>
	{
		private DockPanelLayout(IDockPanel dockPanel) : base(dockPanel)
		{
		}

		public static Size Arrange(IDockPanel panel, Size finalSize)
		{
			var children = panel.Elements;
			var count = children.Count;
			var spacing = panel is IDockPanelAdvanced dockPanelAdvanced ? dockPanelAdvanced.Spacing : 0.0;

			var left = .0;
			var top = .0;
			var right = .0;
			var bottom = .0;

			for (var i = 0; i < count; i++)
			{
				var child = children[i];

				if (child == null)
					continue;

				var dock = panel.GetDock(child);
				var dockDistance = child.IsLayoutVisible() ? panel.GetDockDistance(child) + spacing : 0;

				if (dock == null)
					continue;

				var childDesiredSize = child.DesiredSize;
				var rcChild = new Rect(left, top, Math.Max(0.0, finalSize.Width - (left + right)), Math.Max(0.0, finalSize.Height - (top + bottom)));

				switch (dock)
				{
					case Dock.Left:
						left += childDesiredSize.Width + dockDistance;
						rcChild.Width = childDesiredSize.Width;
						break;

					case Dock.Right:
						right += childDesiredSize.Width;
						rcChild.X = Math.Max(0.0, finalSize.Width - right);
						right += dockDistance;
						rcChild.Width = childDesiredSize.Width;
						break;

					case Dock.Top:
						top += childDesiredSize.Height + dockDistance;
						rcChild.Height = childDesiredSize.Height;
						break;

					case Dock.Bottom:
						bottom += childDesiredSize.Height;
						rcChild.Y = Math.Max(0.0, finalSize.Height - bottom);
						bottom += dockDistance;
						rcChild.Height = childDesiredSize.Height;
						break;
				}

				child.Arrange(rcChild);
			}

			var fillChild = new Rect(left, top, Math.Max(0.0, finalSize.Width - (left + right)), Math.Max(0.0, finalSize.Height - (top + bottom)));

			for (var i = 0; i < count; i++)
			{
				var child = children[i];

				if (child == null)
					continue;

				var dock = panel.GetDock(child);

				if (dock != null)
					continue;

				child.Arrange(fillChild);
			}

			return finalSize;
		}

		public static Size Measure(IDockPanel panel, Size constraint)
		{
			var children = panel.Elements;
			var spacing = panel is IDockPanelAdvanced dockPanelAdvanced ? dockPanelAdvanced.Spacing : 0.0;

			var resultWidth = .0;
			var resultHeight = .0;
			var width = .0;
			var height = .0;

			for (int i = 0, count = children.Count; i < count; ++i)
			{
				var child = children[i];

				if (child == null)
					continue;

				var dock = panel.GetDock(child);
				var dockDistance = child.IsLayoutVisible() ? panel.GetDockDistance(child) + spacing: 0;

				if (dock == null)
					continue;

				var childConstraint = new Size(Math.Max(0.0, constraint.Width - width), Math.Max(0.0, constraint.Height - height));

				child.Measure(childConstraint);

				var childDesiredSize = child.DesiredSize;

				switch (dock)
				{
					case Dock.Left:
					case Dock.Right:
						resultHeight = Math.Max(resultHeight, height + childDesiredSize.Height);
						width += childDesiredSize.Width + dockDistance;
						break;
					case Dock.Top:
					case Dock.Bottom:
						resultWidth = Math.Max(resultWidth, width + childDesiredSize.Width);
						height += childDesiredSize.Height + dockDistance;
						break;
				}
			}

			var fillConstraint = new Size(Math.Max(0.0, constraint.Width - width), Math.Max(0.0, constraint.Height - height));

			for (int i = 0, count = children.Count; i < count; ++i)
			{
				var child = children[i];

				if (child == null)
					continue;

				var dock = panel.GetDock(child);

				if (dock != null)
					continue;

				child.Measure(fillConstraint);

				var childDesiredSize = child.DesiredSize;

				resultWidth = Math.Max(resultWidth, width + childDesiredSize.Width);
				resultHeight = Math.Max(resultHeight, height + childDesiredSize.Height);
			}

			// Make sure the final accumulated size is reflected in parentSize.
			resultWidth = Math.Max(resultWidth, width);
			resultHeight = Math.Max(resultHeight, height);

			return new Size(resultWidth, resultHeight);
		}
	}
}