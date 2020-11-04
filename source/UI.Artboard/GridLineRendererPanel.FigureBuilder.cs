// <copyright file="GridLineRendererPanel.FigureBuilder.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Zaaml.Core.Extensions;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Artboard
{
	public abstract partial class GridLineRendererPanel<TGridLineModel, TGridLineCollection, TGridLine>
	{
		private protected class FigureBuilder
		{
			private readonly GridLineRendererPanel<TGridLineModel, TGridLineCollection, TGridLine> _gridLineRenderer;
			private readonly List<PathGeometry> _pathFigures;
			private readonly List<Path> _pathList = new List<Path>();
			private readonly WeakReference<TGridLineModel> _weakModelReference;
			private Rect _finalRect;
			private int _lineCountX;
			private int _lineCountY;
			private Size _size;

			public FigureBuilder(GridLineRendererPanel<TGridLineModel, TGridLineCollection, TGridLine> gridLineRenderer)
			{
				var model = gridLineRenderer.Model;

				_gridLineRenderer = gridLineRenderer;
				_weakModelReference = new WeakReference<TGridLineModel>(model);

				Scale = gridLineRenderer.Scale;

				ModelVersion = model.Version;

				if (model.GridLines.Count == 0)
					return;

				GridStep = model.GridSize * Scale;

				if (GridStep < 1)
					return;

				MaxGridStep = CalcMaxGridStep(model, Scale, gridLineRenderer.SyncGridStep);

				_pathFigures = model.GridLines.SortedLines.Select(s => new PathGeometry
				{
					Figures = new PathFigureCollection()
				}).ToList();
			}

			public double GridStep { get; }

			public double MaxGridStep { get; }

			public TGridLineModel Model => _weakModelReference.TryGetTarget(out var target) ? target : null;

			public int ModelVersion { get; }

			public double Scale { get; }

			protected virtual void Arrange(bool invalidate)
			{
				foreach (var path in _pathList)
				{
					SetArrangeRect(path, _finalRect);

					path.Measure(_finalRect.Size);
				}
			}

			public virtual void Attach()
			{
				for (var index = 0; index < Model.GridLines.SortedLines.Count; index++)
				{
					var gridLine = Model.GridLines.SortedLines[index];
					var path = _gridLineRenderer.MountPath(gridLine);

					path.Data = _pathFigures[index];
					_pathList.Add(path);

					_gridLineRenderer.Children.Add(path);
				}

				Arrange(true);
			}

			private double CalcMaxGridStep(TGridLineModel model, double scale, double syncGridStep)
			{
				var gridSize = model.GridSize;
				var gridStep = gridSize * scale;

				if (gridStep < 1)
					return 0.0;

				var sortedDefinitions = model.GridLines.SortedLines;
				var stepCount = sortedDefinitions.Last().Step;
				var maxGridStep = stepCount * gridStep;

				if (syncGridStep > gridSize && (syncGridStep % gridSize).IsCloseTo(0.0))
				{
					if (Scale < 1 && (syncGridStep * Scale * stepCount % maxGridStep).IsCloseTo(0.0))
						maxGridStep *= syncGridStep / gridSize;
				}

				return maxGridStep;
			}

			protected virtual PathFigure CreateLineFigure(GridLine definition, Orientation orientation, double directOffset, double indirectOffset, double size)
			{
				var horizontal = orientation.IsHorizontal();
				var lineFigure = new PathFigure {StartPoint = new Point(horizontal ? directOffset : indirectOffset, horizontal ? indirectOffset : directOffset)};

				lineFigure.Segments.Add(new LineSegment {Point = new Point(horizontal ? directOffset : indirectOffset + size, horizontal ? indirectOffset + size : directOffset)});

				return lineFigure;
			}

			public virtual void Detach()
			{
				foreach (var path in _pathList)
				{
					path.Data = null;

					_gridLineRenderer.Children.Remove(path);
					_gridLineRenderer.ReleasePath(path);
				}

				_pathList.Clear();
			}

			public void EnsureSize(Size size)
			{
				var model = Model;

				if (model == null)
					return;

				if (model.GridLines.Count == 0 || GridStep < 1)
					return;

				size.Width = Math.Ceiling(size.Width / MaxGridStep) * MaxGridStep;
				size.Height = Math.Ceiling(size.Height / MaxGridStep) * MaxGridStep;

				if (_size.Width.IsGreaterThanOrClose(size.Width) && _size.Height.IsGreaterThanOrClose(size.Height))
				{
					Arrange(false);

					return;
				}

				var verticalLines = _gridLineRenderer.VerticalLines;
				var horizontalLines = _gridLineRenderer.HorizontalLines;

				if (verticalLines)
					EnsureSizePart(model, size, Orientation.Horizontal, horizontalLines);

				if (horizontalLines)
					EnsureSizePart(model, size, Orientation.Vertical, verticalLines);

				var finalSize = new Size(size.Width + (verticalLines ? MaxGridStep : 0), size.Height + (horizontalLines ? MaxGridStep : 0)).LayoutRound(RoundingMode.FromZero);
				var point = new Point(verticalLines ? -MaxGridStep : 0, horizontalLines ? -MaxGridStep : 0).LayoutRound(RoundingMode.MidPointFromZero);

				_finalRect = new Rect(point, finalSize);

				Arrange(true);

				_size = size;
			}

			private void EnsureSizePart(TGridLineModel model, Size availableSize, Orientation orientation, bool expandCross)
			{
				var horizontal = orientation.IsHorizontal();
				var orientedSize = new OrientedSize(orientation, availableSize);
				var currentOrientedSize = new OrientedSize(orientation, _size);
				var gridLines = model.GridLines;
				var sortedDefinitions = gridLines.SortedLines;
				var fillSize = orientedSize.Direct + MaxGridStep;
				var lineSize = orientedSize.Indirect + (expandCross ? MaxGridStep : 0);
				var lineCount = horizontal ? _lineCountX : _lineCountY;

				if (lineSize.IsGreaterThan(currentOrientedSize.Indirect))
				{
					for (var i = 0; i < sortedDefinitions.Count; i++)
					{
						var pathFigureCollection = _pathFigures[i].Figures;
						var iLine = 0;

						foreach (var figure in pathFigureCollection)
						{
							var line = (LineSegment) figure.Segments[0];
							var lineOrientation = figure.StartPoint.X.Equals(line.Point.X) ? Orientation.Horizontal : Orientation.Vertical;

							if (orientation != lineOrientation)
								continue;

							var orientedPoint = new OrientedPoint(orientation, figure.StartPoint);
							var iStep = gridLines.GetSortedDefinitionIndex(iLine);
							var lineDefinition = sortedDefinitions[iStep];

							UpdateLineFigure(figure, lineDefinition, orientation, orientedPoint.Direct, orientedPoint.Indirect, lineSize);

							iLine++;
						}
					}
				}

				for (var offset = lineCount * GridStep; offset < fillSize; offset += GridStep)
				{
					var snapOffset = offset.LayoutRound(orientation, RoundingMode.MidPointFromZero) + 0.5;
					var iStep = gridLines.GetSortedDefinitionIndex(lineCount);
					var lineDefinition = sortedDefinitions[iStep];
					var pathFigureCollection = _pathFigures[iStep].Figures;

					pathFigureCollection.Add(CreateLineFigure(lineDefinition, orientation, snapOffset, 0, lineSize));

					lineCount++;
				}

				if (horizontal)
					_lineCountX = lineCount;
				else
					_lineCountY = lineCount;
			}

			protected virtual void UpdateLineFigure(PathFigure figure, GridLine definition, Orientation orientation, double directOffset, double indirectOffset, double size)
			{
				var line = (LineSegment) figure.Segments[0];

				figure.StartPoint = orientation == Orientation.Horizontal ? new Point(directOffset, indirectOffset) : new Point(indirectOffset, directOffset);
				line.Point = orientation == Orientation.Horizontal ? new Point(directOffset, indirectOffset + size) : new Point(indirectOffset + size, directOffset);
			}
		}
	}
}