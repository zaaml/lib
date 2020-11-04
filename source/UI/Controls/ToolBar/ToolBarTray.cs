// <copyright file="ToolBarTray.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Behaviors.Draggable;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ToolBar
{
	[ContentProperty("ToolBars")]
	[TemplateContractType(typeof(ToolBarTrayTemplateContract))]
	public sealed class ToolBarTray : TemplateContractControl
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, ToolBarTray>
			("Orientation", Orientation.Horizontal, t => t.OnOrientationChanged);

		private static readonly DependencyPropertyKey ToolBarsPropertyKey = DPM.RegisterReadOnly<ToolBarCollection, ToolBarTray>
			("ToolBarsPrivate");

		public static readonly DependencyProperty IsLockedProperty = DPM.Register<bool, ToolBarTray>
			("IsLocked", t => t.OnIsLockedChanged);

		public static readonly DependencyProperty ToolBarsProperty = ToolBarsPropertyKey.DependencyProperty;

		private ToolBarControl _dragToolBar;
		private Point _relativeMouseLocation;
		private bool _waitCurrentBand;

		static ToolBarTray()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<ToolBarTray>();
		}

		public ToolBarTray()
		{
			this.OverrideStyleKey<ToolBarTray>();

			DraggableBehavior.AddDragStartedHandler(this, OnDragStarted);
			DraggableBehavior.AddDragEndedHandler(this, OnDragEnded);
			DraggableBehavior.AddDragMoveHandler(this, OnDragMove);
		}

		private void OnDragMove(object sender, DragMoveRoutedEventArgs e)
		{
			if (IsDragging == false)
				return;

			var currentBand = ToolBarHost.Bands.Single(b => b.ToolBars.Contains(_dragToolBar));
			var prevToolBar = currentBand.ToolBars.LeftOfFirstOrDefault(_dragToolBar);
			var nextToolBar = currentBand.ToolBars.RightOfFirstOrDefault(_dragToolBar);
			var orientation = Orientation;
			var rotatedOrientation = orientation.Rotate();
			var position = MouseInternal.ScreenPosition;
			var positionPart = position.GetPart(orientation);
			var rotatedPositionPart = position.GetPart(rotatedOrientation);
			var relativeMouseLocationPart = _relativeMouseLocation.GetPart(orientation);

			try
			{
				var currentToolBarHostBox = _dragToolBar.GetScreenBox();
				var overCurrentBand = currentToolBarHostBox.GetRange(rotatedOrientation)
					.WithExpand(currentToolBarHostBox.GetSize(rotatedOrientation) / 4)
					.Contains(rotatedPositionPart);

				if (_waitCurrentBand || overCurrentBand)
				{
					if (prevToolBar != null && positionPart < prevToolBar.GetScreenBox().GetMinPart(orientation))
						SwapToolBarsHorizontally(currentBand, _dragToolBar, prevToolBar);
					else if (nextToolBar != null && positionPart > nextToolBar.GetScreenBox().GetMinPart(orientation))
						SwapToolBarsHorizontally(currentBand, _dragToolBar, nextToolBar);

					_waitCurrentBand = overCurrentBand == false;
				}
				else
				{
					var mouseBand = ToolBarHost.Bands.FirstOrDefault(b => b.GetHostBox().GetRange(rotatedOrientation).Contains(rotatedPositionPart));

					if (mouseBand == null)
					{
						if (currentBand.ToolBars.Count == 1)
							return;

						var newBand = new Band();

						ToolBarHost.Bands.Insert(rotatedPositionPart < currentToolBarHostBox.GetMinPart(rotatedOrientation) ? 0 : ToolBarHost.Bands.Count, newBand);
						ChangeToolBarBand(_dragToolBar, currentBand, newBand, 0);

						_waitCurrentBand = true;
					}
					else
					{
						int newBandIndex;
						var mouseBandBox = mouseBand.GetHostBox();

						if (positionPart <= mouseBandBox.GetMinPart(orientation))
							newBandIndex = 0;
						else if (positionPart >= mouseBandBox.GetMaxPart(orientation))
							newBandIndex = mouseBand.ToolBars.Count;
						else
						{
							var toolBar = mouseBand.ToolBars.First(t => t.GetScreenBox().GetRange(orientation).Contains(positionPart));

							newBandIndex = mouseBand.ToolBars.IndexOf(toolBar) + 1;
						}

						ChangeToolBarBand(_dragToolBar, currentBand, mouseBand, newBandIndex);
						_waitCurrentBand = true;
					}
				}
			}
			finally
			{
				currentBand = ToolBarHost.Bands.Single(b => b.ToolBars.Contains(_dragToolBar));
				prevToolBar = currentBand.ToolBars.LeftOfFirstOrDefault(_dragToolBar);

				if (prevToolBar != null)
					SetToolBarLength(prevToolBar, positionPart - prevToolBar.GetScreenBox().GetMinPart(orientation) - relativeMouseLocationPart);
			}
		}

		private void OnDragEnded(object sender, DragEndedRoutedEventArgs e)
		{
			if (IsLocked || IsDragging == false)
				return;

			IsDragging = false;

			ToolBarHost.NormalizeBands();

			InvalidateBands();
		}

		private void OnDragStarted(object sender, DragStartedRoutedEventArgs e)
		{
			if (IsLocked)
				return;

			var draggableBehavior = e.Behavior;

			if (draggableBehavior.FrameworkElement is ToolBarControl toolBarControl)
			{
				IsDragging = true;
				_waitCurrentBand = false;

				_dragToolBar = toolBarControl;
				_relativeMouseLocation = MouseInternal.GetPosition(_dragToolBar);
			}
		}

		internal bool IsDragging { get; private set; }

		public bool IsLocked
		{
			get => (bool) GetValue(IsLockedProperty);
			set => SetValue(IsLockedProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation) GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		private ToolBarTrayTemplateContract TemplateContract => (ToolBarTrayTemplateContract) TemplateContractInternal;

		internal ToolBarTrayPanel ToolBarHost => TemplateContract.ToolBarHost;

		public ToolBarCollection ToolBars => this.GetValueOrCreate(ToolBarsPropertyKey, CreateToolBarCollection);

		private void AddToolBarImpl(ToolBarControl toolBar)
		{
			ToolBarHost.Children.Add(toolBar);
		}

		private void ChangeToolBarBand(ToolBarControl toolBar, Band currentBand, Band newBand, int newBandIndex)
		{
			var prevToolBar = currentBand.ToolBars.LeftOfFirstOrDefault(toolBar);

			if (prevToolBar != null)
				SetToolBarLength(prevToolBar, double.PositiveInfinity);

			currentBand.ToolBars.Remove(toolBar);
			newBand.ToolBars.Insert(newBandIndex, toolBar);

			SetToolBarLength(newBand.ToolBars.Last(), double.PositiveInfinity);

			if (currentBand.ToolBars.Any())
				SetToolBarLength(currentBand.ToolBars.Last(), double.PositiveInfinity);
			else
				ToolBarHost.Bands.Remove(currentBand);

			InvalidateBands();
			UpdateLayout();
		}

		private ToolBarCollection CreateToolBarCollection()
		{
			return new ToolBarCollection(OnToolBarAdded, OnToolBarRemoved);
		}

		internal void InvalidateBands()
		{
			InvalidateMeasure();
			ToolBarHost?.InvalidateMeasure();
		}

		private void OnIsLockedChanged()
		{
			if (IsDragging)
				_dragToolBar.DraggableBehavior.StopDrag();

			foreach (var toolBar in ToolBars)
				toolBar.UpdateActualDragHandleVisibility();
		}

		private void OnOrientationChanged()
		{
			UpdateOrientation();
			InvalidateBands();
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			foreach (var toolBar in ToolBars)
				AddToolBarImpl(toolBar);

			ToolBarHost.ToolBarTray = this;
		}

		protected override void OnTemplateContractDetaching()
		{
			foreach (var toolBar in ToolBars)
				RemoveToolBarImpl(toolBar);

			ToolBarHost.ToolBarTray = null;

			base.OnTemplateContractDetaching();
		}

		private void OnToolBarAdded(ToolBarControl toolBar)
		{
			toolBar.Tray = this;

			if (IsTemplateAttached == false)
				return;

			AddToolBarImpl(toolBar);

			InvalidateBands();
		}


		private void OnToolBarRemoved(ToolBarControl toolBar)
		{
			toolBar.Tray = null;

			if (IsTemplateAttached == false)
				return;

			RemoveToolBarImpl(toolBar);

			InvalidateBands();
		}

		private void RemoveToolBarImpl(ToolBarControl toolBar)
		{
			ToolBarHost.Children.Remove(toolBar);
		}

		private static void SetToolBarLength(ToolBarControl toolBar, double length)
		{
			toolBar.Length = length.Clamp(50, double.PositiveInfinity);
		}

		private void SwapToolBarsHorizontally(Band band, ToolBarControl first, ToolBarControl second)
		{
			var firstIndex = band.ToolBars.IndexOf(first);
			var secondIndex = band.ToolBars.IndexOf(second);
			var prev = Math.Min(firstIndex, secondIndex);

			SetToolBarLength(band.ToolBars[prev], double.PositiveInfinity);
			SetToolBarLength(band.ToolBars.Last(), double.PositiveInfinity);

			band.ToolBars.Swap(firstIndex, secondIndex);

			InvalidateBands();
			UpdateLayout();
		}

		private void UpdateOrientation()
		{
			foreach (var toolBar in ToolBars)
				toolBar.UpdateActualOrientation();
		}
	}

	public sealed class ToolBarTrayTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = false)]
		public ToolBarTrayPanel ToolBarHost { get; [UsedImplicitly] private set; }
	}
}