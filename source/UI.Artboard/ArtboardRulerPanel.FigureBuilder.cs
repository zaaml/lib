// <copyright file="ArtboardRulerPanel.FigureBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zaaml.Core.Extensions;
using Zaaml.Core.Monads;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	public partial class ArtboardRulerPanel
	{
		private sealed class RulerFigureBuilder : FigureBuilder
		{
			private const int TextDistance = 3;
			private readonly ArtboardRulerPanel _rulerPanel;
			private readonly List<Rect> _textBlockRects = new List<Rect>();
			private readonly List<TextBlock> _textBlocks = new List<TextBlock>();
			private int _firstTickMark;
			private bool _isAttached;

			public RulerFigureBuilder(ArtboardRulerPanel rulerPanel) : base(rulerPanel)
			{
				var artboardRulerModel = Model;

				_rulerPanel = rulerPanel;

				MinLabelStepCount = artboardRulerModel.GridLines.Where(t => t.ShowLabel).FirstMinElementOrDefault(t => t.Step).Return(t => t.Step);
				MaxTickMarkSize = artboardRulerModel.GridLines.Max(t => t.Size);
			}

			public double MaxTickMarkSize { get; }

			private int MinLabelStepCount { get; }

			protected override void Arrange(bool invalidate)
			{
				base.Arrange(invalidate);

				UpdateLabels(invalidate);
			}

			public override void Attach()
			{
				base.Attach();

				_isAttached = true;

				foreach (var textBlockRect in _textBlockRects)
					AttachTextBlock(textBlockRect);

				UpdateLabels(true);
			}

			private void AttachTextBlock(Rect textBlockRect)
			{
				var textBlock = _rulerPanel.MountTextBlock();

				SetArrangeRect(textBlock, textBlockRect);

				_textBlocks.Add(textBlock);
				_rulerPanel.Children.Add(textBlock);
			}

			protected override PathFigure CreateLineFigure(GridLine definition, Orientation orientation, double directOffset, double indirectOffset, double size)
			{
				var tickMarkDefinition = (TickMarkDefinition) definition;
				var tickMarkSize = tickMarkDefinition.Size;

				if (tickMarkDefinition.ShowLabel)
				{
					var textDistance = orientation == Orientation.Horizontal ? TextDistance : -TextDistance;
					var textBlockOffset = directOffset + textDistance - MaxGridStep;
					var rect = new Rect(new Point(orientation.IsHorizontal() ? textBlockOffset : 0, orientation.IsVertical() ? textBlockOffset : 0), XamlConstants.ZeroSize);

					if (_isAttached)
						AttachTextBlock(rect);

					_textBlockRects.Add(rect);
				}

				return base.CreateLineFigure(definition, orientation, directOffset, MaxTickMarkSize - tickMarkSize, tickMarkSize);
			}

			public override void Detach()
			{
				base.Detach();

				_isAttached = false;

				foreach (var textBlock in _textBlocks)
				{
					_rulerPanel.Children.Remove(textBlock);
					_rulerPanel.ReleaseTextBlock(textBlock);
				}

				_textBlocks.Clear();
			}

			private string FormatLabel(int iLine)
			{
				return (iLine * Model.GridSize).ToString(CultureInfo.InvariantCulture);
			}

			public void UpdateLabels(bool force = false)
			{
				if (MinLabelStepCount == 0)
					return;

				var actualGridStep = GridStep;
				var gridLengthOffset = _rulerPanel._artboardRuler.Orientation == Orientation.Horizontal ? _rulerPanel.OffsetX : _rulerPanel.OffsetY;
				var pixelOffset = gridLengthOffset.Unit == GridLineStepUnit.Step ? gridLengthOffset.Value * actualGridStep : gridLengthOffset.Value;

				var pixelTransform = -pixelOffset % MaxGridStep;

				if (pixelTransform < 0)
					pixelTransform += MaxGridStep;

				var offset = -pixelOffset - pixelTransform + MaxGridStep;
				var step = MinLabelStepCount * GridStep;
				var firstTickMark = -((int) offset / (int) step);

				if (firstTickMark == _firstTickMark && force == false)
					return;

				var iLine = 0;

				foreach (var label in _textBlocks)
				{
					label.Text = FormatLabel((firstTickMark + iLine) * MinLabelStepCount);
					label.InvalidateMeasure();
					label.Measure(XamlConstants.InfiniteSize);

					var rect = new Rect(GetArrangeRect(label).GetTopLeft(), label.DesiredSize);

					SetArrangeRect(label, rect);

					label.Arrange(rect);

					iLine++;
				}

				_firstTickMark = firstTickMark;
			}

			protected override void UpdateLineFigure(PathFigure figure, GridLine definition, Orientation orientation, double directOffset, double indirectOffset, double size)
			{
				var tickMarkDefinition = (TickMarkDefinition) definition;
				var tickMarkSize = tickMarkDefinition.Size;

				base.UpdateLineFigure(figure, definition, orientation, directOffset, MaxTickMarkSize - tickMarkSize, tickMarkSize);
			}
		}
	}
}