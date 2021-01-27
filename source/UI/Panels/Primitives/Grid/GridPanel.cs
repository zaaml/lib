// <copyright file="GridPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 

using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;
using Zaaml.UI.Panels.Flexible;

// ReSharper disable CommentTypo

namespace Zaaml.UI.Panels.Primitives
{
	[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
	public class GridPanel : Panel
	{
		private const double Epsilon = 1e-5; //  used in fp calculations
		private const int LayoutLoopMaxCount = 5; // 5 is an arbitrary constant chosen to end the measure loop
		private static readonly LocalDataStoreSlot TempDefinitionsDataSlot = Thread.AllocateDataSlot();
		private static readonly IComparer SpanPreferredDistributionOrderComparerInstance = new SpanPreferredDistributionOrderComparer();
		private static readonly IComparer SpanMaxDistributionOrderComparerInstance = new SpanMaxDistributionOrderComparer();
		private static readonly IComparer MinRatioComparerInstance = new MinRatioComparer();
		private static readonly IComparer MaxRatioComparerInstance = new MaxRatioComparer();
		private static readonly IComparer StarWeightComparerInstance = new StarWeightComparer();

		public static readonly DependencyProperty ColumnProperty = DPM.RegisterAttached<int, GridPanel>
			("Column", 0, OnCellAttachedPropertyChanged, IsIntValueNotNegative);

		public static readonly DependencyProperty RowProperty = DPM.RegisterAttached<int, GridPanel>
			("Row", 0, OnCellAttachedPropertyChanged, IsIntValueNotNegative);

		public static readonly DependencyProperty ColumnSpanProperty = DPM.RegisterAttached<int, GridPanel>
			("ColumnSpan", 1, OnCellAttachedPropertyChanged, IsIntValueGreaterThanZero);

		public static readonly DependencyProperty RowSpanProperty = DPM.RegisterAttached<int, GridPanel>
			("RowSpan", 1, OnCellAttachedPropertyChanged, IsIntValueGreaterThanZero);

		public static readonly DependencyProperty IsSharedSizeScopeProperty = DPM.RegisterAttached<bool, GridPanel>
			("IsSharedSizeScope", false, GridPanelDefinition.OnIsSharedSizeScopePropertyChanged);

		private GridColumnCollection _defaultColumns;
		private GridRowCollection _defaultRows;

		private int[] _definitionIndices;
		private Flags _flags;
		private double[] _roundingErrors;

		internal bool ArrangeOverrideInProgress
		{
			get => CheckFlagsAnd(Flags.ArrangeOverrideInProgress);
			set => SetFlags(value, Flags.ArrangeOverrideInProgress);
		}

		private bool CellsStructureDirty
		{
			get => !CheckFlagsAnd(Flags.ValidCellsStructure);
			set => SetFlags(!value, Flags.ValidCellsStructure);
		}

		internal bool ColumnDefinitionCollectionDirty
		{
			get => !CheckFlagsAnd(Flags.ValidDefinitionsUStructure);
			set => SetFlags(!value, Flags.ValidDefinitionsUStructure);
		}

		public GridColumnCollection Columns
		{
			get
			{
				ExtData ??= new ExtendedData();

				return ExtData.ColumnDefinitions ?? (ExtData.ColumnDefinitions = new GridColumnCollection(this));
			}
		}

		private GridColumnCollection DefaultColumns => _defaultColumns ??= new GridColumnCollection(this) {new GridPanelColumn()};

		private GridRowCollection DefaultRows => _defaultRows ??= new GridRowCollection(this) {new GridPanelRow()};

		private int[] DefinitionIndices
		{
			get
			{
				var requiredLength = Math.Max(Math.Max(DefinitionsU.Count, DefinitionsV.Count), 1) * 2;

				if (_definitionIndices == null || _definitionIndices.Length < requiredLength)
					_definitionIndices = new int[requiredLength];

				return _definitionIndices;
			}
		}

		private GridColumnCollection DefinitionsU => ExtData.DefinitionsU;

		private GridRowCollection DefinitionsV => ExtData.DefinitionsV;

		private ExtendedData ExtData { get; set; }

		private bool HasGroup3CellsInAutoRows
		{
			get => CheckFlagsAnd(Flags.HasGroup3CellsInAutoRows);
			set => SetFlags(value, Flags.HasGroup3CellsInAutoRows);
		}

		private bool HasStarCellsU
		{
			get => CheckFlagsAnd(Flags.HasStarCellsU);
			set => SetFlags(value, Flags.HasStarCellsU);
		}

		private bool HasStarCellsV
		{
			get => CheckFlagsAnd(Flags.HasStarCellsV);
			set => SetFlags(value, Flags.HasStarCellsV);
		}

		private bool ListenToNotifications
		{
			get => CheckFlagsAnd(Flags.ListenToNotifications);
			set => SetFlags(value, Flags.ListenToNotifications);
		}

		internal bool MeasureOverrideInProgress
		{
			get => CheckFlagsAnd(Flags.MeasureOverrideInProgress);
			set => SetFlags(value, Flags.MeasureOverrideInProgress);
		}

		private CellCache[] PrivateCells => ExtData.CellCachesCollection;

		private double[] RoundingErrors
		{
			get
			{
				var requiredLength = Math.Max(DefinitionsU.Count, DefinitionsV.Count);

				if (_roundingErrors == null && requiredLength == 0)
					_roundingErrors = new double[1];
				else if (_roundingErrors == null || _roundingErrors.Length < requiredLength)
					_roundingErrors = new double[requiredLength];

				return _roundingErrors;
			}
		}

		internal bool RowDefinitionCollectionDirty
		{
			get => !CheckFlagsAnd(Flags.ValidDefinitionsVStructure);
			set => SetFlags(!value, Flags.ValidDefinitionsVStructure);
		}

		public GridRowCollection Rows
		{
			get
			{
				ExtData ??= new ExtendedData();

				return ExtData.RowDefinitions ?? (ExtData.RowDefinitions = new GridRowCollection(this));
			}
		}

		private bool SizeToContentU
		{
			get => CheckFlagsAnd(Flags.SizeToContentU);
			set => SetFlags(value, Flags.SizeToContentU);
		}

		private bool SizeToContentV
		{
			get => CheckFlagsAnd(Flags.SizeToContentV);
			set => SetFlags(value, Flags.SizeToContentV);
		}

		private GridPanelDefinition[] TempDefinitions
		{
			get
			{
				var extData = ExtData;
				var requiredLength = Math.Max(DefinitionsU.Count, DefinitionsV.Count) * 2;

				if (extData.TempDefinitions != null && extData.TempDefinitions.Length >= requiredLength)
					return extData.TempDefinitions;

				var tempDefinitionsWeakRef = (WeakReference) Thread.GetData(TempDefinitionsDataSlot);

				if (tempDefinitionsWeakRef == null)
				{
					extData.TempDefinitions = new GridPanelDefinition[requiredLength];

					Thread.SetData(TempDefinitionsDataSlot, new WeakReference(extData.TempDefinitions));
				}
				else
				{
					extData.TempDefinitions = (GridPanelDefinition[]) tempDefinitionsWeakRef.Target;

					if (extData.TempDefinitions != null && extData.TempDefinitions.Length >= requiredLength)
						return extData.TempDefinitions;

					extData.TempDefinitions = new GridPanelDefinition[requiredLength];
					tempDefinitionsWeakRef.Target = extData.TempDefinitions;
				}

				return extData.TempDefinitions;
			}
		}

		private static bool _AreClose(double d1, double d2)
		{
			return Math.Abs(d1 - d2) < Epsilon;
		}

		private static bool _IsZero(double d)
		{
			return Math.Abs(d) < Epsilon;
		}

		private void ApplyCachedMinSizes(double[] minSizes, bool isRows)
		{
			for (var i = 0; i < minSizes.Length; i++)
			{
				if (DoubleUtils.GreaterThanOrClose(minSizes[i], 0) == false)
					continue;

				if (isRows)
					DefinitionsV[i].SetMinSize(minSizes[i]);
				else
					DefinitionsU[i].SetMinSize(minSizes[i]);
			}
		}

		protected override Size ArrangeOverrideCore(Size arrangeSize)
		{
			try
			{
				ArrangeOverrideInProgress = true;

				if (ExtData == null)
				{
					var children = InternalChildren;

					for (int i = 0, count = children.Count; i < count; ++i)
						children[i]?.Arrange(new Rect(arrangeSize));
				}
				else
				{
					Debug.Assert(DefinitionsU.Count > 0 && DefinitionsV.Count > 0);

					SetFinalSize(DefinitionsU, arrangeSize.Width, true);
					SetFinalSize(DefinitionsV, arrangeSize.Height, false);

					var children = InternalChildren;

					for (var currentCell = 0; currentCell < PrivateCells.Length; ++currentCell)
					{
						var cell = children[currentCell];

						if (cell == null)
							continue;

						var columnIndex = PrivateCells[currentCell].ColumnIndex;
						var rowIndex = PrivateCells[currentCell].RowIndex;
						var columnSpan = PrivateCells[currentCell].ColumnSpan;
						var rowSpan = PrivateCells[currentCell].RowSpan;

						var cellRect = new Rect(
							columnIndex == 0 ? 0.0 : DefinitionsU[columnIndex].FinalOffset,
							rowIndex == 0 ? 0.0 : DefinitionsV[rowIndex].FinalOffset,
							GetFinalSizeForRange(DefinitionsU, columnIndex, columnSpan),
							GetFinalSizeForRange(DefinitionsV, rowIndex, rowSpan));

						cell.Arrange(cellRect);
					}
				}
			}
			finally
			{
				SetValid();

				ArrangeOverrideInProgress = false;
			}

			return arrangeSize;
		}

		private double[] CacheMinSizes(int cellsHead, bool isRows)
		{
			var minSizes = isRows ? new double[DefinitionsV.Count] : new double[DefinitionsU.Count];

			for (var j = 0; j < minSizes.Length; j++)
			{
				minSizes[j] = -1;
			}

			var i = cellsHead;

			do
			{
				if (isRows)
					minSizes[PrivateCells[i].RowIndex] = DefinitionsV[PrivateCells[i].RowIndex].RawMinSize;
				else
					minSizes[PrivateCells[i].ColumnIndex] = DefinitionsU[PrivateCells[i].ColumnIndex].RawMinSize;

				i = PrivateCells[i].Next;
			} while (i < PrivateCells.Length);

			return minSizes;
		}

		private static double CalculateDesiredSize<TDefinition>(GridDefinitionCollection<TDefinition> definitions) where TDefinition : GridPanelDefinition
		{
			double desiredSize = 0;

			for (var i = 0; i < definitions.Count; ++i)
				desiredSize += definitions[i].MinSize;

			return desiredSize;
		}

		/// <summary>
		///   CheckFlagsAnd returns <c>true</c> if all the flags in the
		///   given bitmask are set on the object.
		/// </summary>
		private bool CheckFlagsAnd(Flags flags)
		{
			return (_flags & flags) == flags;
		}

		/// <summary>
		///   Choose the ratio with maximum discrepancy from the current proportion.
		///   Returns:
		///   true    if proportion fails a min constraint but not a max, or
		///   if the min constraint has higher discrepancy
		///   false   if proportion fails a max constraint but not a min, or
		///   if the max constraint has higher discrepancy
		///   null    if proportion doesn't fail a min or max constraint
		///   The discrepancy is the ratio of the proportion to the max- or min-ratio.
		///   When both ratios hit the constraint,  minRatio < proportion
		///   < maxRatio,
		///     and the minRatio has higher discrepancy if
		///     ( proportion / minRatio)>
		///     (maxRatio / proportion)
		/// </summary>
		private static bool? Choose(double minRatio, double maxRatio, double proportion)
		{
			if (minRatio < proportion)
			{
				if (!(maxRatio > proportion))
					return true;

				// compare proportion/minRatio : maxRatio/proportion, but
				// do it carefully to avoid floating-point overflow or underflow
				// and divide-by-0.
				var minPower = Math.Floor(Math.Log(minRatio, 2.0));
				var maxPower = Math.Floor(Math.Log(maxRatio, 2.0));
				var f = Math.Pow(2.0, Math.Floor((minPower + maxPower) / 2.0));

				return proportion / f * (proportion / f) > minRatio / f * (maxRatio / f);
			}

			if (maxRatio > proportion)
				return false;

			return null;
		}

		private static bool CompareNullRefs(object x, object y, out int result)
		{
			result = 2;

			if (x == null)
			{
				if (y == null)
					result = 0;
				else
					result = -1;
			}
			else
			{
				if (y == null)
					result = 1;
			}

			return result != 2;
		}

		private void EnsureMinSizeInDefinitionRange<TDefinition>(GridDefinitionCollection<TDefinition> definitions, int start, int count, double requestedSize, double percentReferenceSize) where TDefinition : GridPanelDefinition
		{
			Debug.Assert(1 < count && 0 <= start && start + count <= definitions.Count);

			//  avoid processing when asked to distribute "0"
			if (_IsZero(requestedSize))
				return;

			var tempDefinitions = TempDefinitions; //  temp array used to remember definitions for sorting
			var end = start + count;
			var autoDefinitionsCount = 0;
			double rangeMinSize = 0;
			double rangePreferredSize = 0;
			double rangeMaxSize = 0;
			double maxMaxSize = 0; //  maximum of maximum sizes

			//  first accumulate the necessary information:
			//  a) sum up the sizes in the range;
			//  b) count the number of auto definitions in the range;
			//  c) initialize temp array
			//  d) cache the maximum size into SizeCache
			//  e) accumulate max of max sizes
			for (var i = start; i < end; ++i)
			{
				var minSize = definitions[i].MinSize;
				var preferredSize = definitions[i].PreferredSize;
				var maxSize = Math.Max(definitions[i].UserMaxSize, minSize);

				rangeMinSize += minSize;
				rangePreferredSize += preferredSize;
				rangeMaxSize += maxSize;

				definitions[i].SizeCache = maxSize;

				//  sanity check: no matter what, but min size must always be the smaller;
				//  max size must be the biggest; and preferred should be in between
				Debug.Assert(minSize <= preferredSize
				             && preferredSize <= maxSize
				             && rangeMinSize <= rangePreferredSize
				             && rangePreferredSize <= rangeMaxSize);

				if (maxMaxSize < maxSize)
					maxMaxSize = maxSize;

				if (definitions[i].UserSize.IsAuto)
					autoDefinitionsCount++;

				tempDefinitions[i - start] = definitions[i];
			}

			//  avoid processing if the range already big enough
			if (requestedSize > rangeMinSize)
			{
				if (requestedSize <= rangePreferredSize)
				{
					//
					//  requestedSize fits into preferred size of the range.
					//  distribute according to the following logic:
					//  * do not distribute into auto definitions - they should continue to stay "tight";
					//  * for all non-auto definitions distribute to equi-size min sizes, without exceeding preferred size.
					//
					//  in order to achieve that, definitions are sorted in a way that all auto definitions
					//  are first, then definitions follow ascending order with PreferredSize as the key of sorting.
					//
					double sizeToDistribute;
					int i;

					Array.Sort(tempDefinitions, 0, count, SpanPreferredDistributionOrderComparerInstance);
					for (i = 0, sizeToDistribute = requestedSize; i < autoDefinitionsCount; ++i)
					{
						//  sanity check: only auto definitions allowed in this loop
						Debug.Assert(tempDefinitions[i].UserSize.IsAuto);

						//  adjust sizeToDistribute value by subtracting auto definition min size
						sizeToDistribute -= tempDefinitions[i].MinSize;
					}

					for (; i < count; ++i)
					{
						//  sanity check: no auto definitions allowed in this loop
						Debug.Assert(!tempDefinitions[i].UserSize.IsAuto);

						var newMinSize = Math.Min(sizeToDistribute / (count - i), tempDefinitions[i].PreferredSize);

						if (newMinSize > tempDefinitions[i].MinSize)
							tempDefinitions[i].UpdateMinSize(newMinSize);

						sizeToDistribute -= newMinSize;
					}

					//  sanity check: requested size must all be distributed
					Debug.Assert(_IsZero(sizeToDistribute));
				}
				else if (requestedSize <= rangeMaxSize)
				{
					//
					//  requestedSize bigger than preferred size, but fit into max size of the range.
					//  distribute according to the following logic:
					//  * do not distribute into auto definitions, if possible - they should continue to stay "tight";
					//  * for all non-auto definitions distribute to euqi-size min sizes, without exceeding max size.
					//
					//  in order to achieve that, definitions are sorted in a way that all non-auto definitions
					//  are last, then definitions follow ascending order with MaxSize as the key of sorting.
					//
					double sizeToDistribute;
					int i;

					Array.Sort(tempDefinitions, 0, count, SpanMaxDistributionOrderComparerInstance);
					for (i = 0, sizeToDistribute = requestedSize - rangePreferredSize; i < count - autoDefinitionsCount; ++i)
					{
						//  sanity check: no auto definitions allowed in this loop
						Debug.Assert(!tempDefinitions[i].UserSize.IsAuto);

						var preferredSize = tempDefinitions[i].PreferredSize;
						var newMinSize = preferredSize + sizeToDistribute / (count - autoDefinitionsCount - i);

						tempDefinitions[i].UpdateMinSize(Math.Min(newMinSize, tempDefinitions[i].SizeCache));
						sizeToDistribute -= tempDefinitions[i].MinSize - preferredSize;
					}

					for (; i < count; ++i)
					{
						//  sanity check: only auto definitions allowed in this loop
						Debug.Assert(tempDefinitions[i].UserSize.IsAuto);

						var preferredSize = tempDefinitions[i].MinSize;
						var newMinSize = preferredSize + sizeToDistribute / (count - i);

						tempDefinitions[i].UpdateMinSize(Math.Min(newMinSize, tempDefinitions[i].SizeCache));
						sizeToDistribute -= tempDefinitions[i].MinSize - preferredSize;
					}

					//  sanity check: requested size must all be distributed
					Debug.Assert(_IsZero(sizeToDistribute));
				}
				else
				{
					//
					//  requestedSize bigger than max size of the range.
					//  distribute according to the following logic:
					//  * for all definitions distribute to equi-size min sizes.
					//
					var equalSize = requestedSize / count;

					if (equalSize < maxMaxSize
					    && !_AreClose(equalSize, maxMaxSize))
					{
						//  equi-size is less than maximum of maxSizes.
						//  in this case distribute so that smaller definitions grow faster than
						//  bigger ones.
						var totalRemainingSize = maxMaxSize * count - rangeMaxSize;
						var sizeToDistribute = requestedSize - rangeMaxSize;

						//  sanity check: totalRemainingSize and sizeToDistribute must be real positive numbers
						Debug.Assert(!double.IsInfinity(totalRemainingSize)
						             && !DoubleUtils.IsNaN(totalRemainingSize)
						             && totalRemainingSize > 0
						             && !double.IsInfinity(sizeToDistribute)
						             && !DoubleUtils.IsNaN(sizeToDistribute)
						             && sizeToDistribute > 0);

						for (var i = 0; i < count; ++i)
						{
							var deltaSize = (maxMaxSize - tempDefinitions[i].SizeCache) * sizeToDistribute / totalRemainingSize;

							tempDefinitions[i].UpdateMinSize(tempDefinitions[i].SizeCache + deltaSize);
						}
					}
					else
					{
						//
						//  equi-size is greater or equal to maximum of max sizes.
						//  all definitions receive equalSize as their mim sizes.
						//
						for (var i = 0; i < count; ++i)
							tempDefinitions[i].UpdateMinSize(equalSize);
					}
				}
			}
		}

		public static int GetColumn(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (int) element.GetValue(ColumnProperty);
		}

		public static int GetColumnSpan(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (int) element.GetValue(ColumnSpanProperty);
		}

		internal double GetFinalColumnDefinitionWidth(int columnIndex)
		{
			var value = 0.0;

			Debug.Assert(ExtData != null);

			//  actual value calculations require structure to be up-to-date
			if (ColumnDefinitionCollectionDirty)
				return value;

			var definitions = DefinitionsU;

			value = definitions[(columnIndex + 1) % definitions.Count].FinalOffset;

			if (columnIndex != 0)
				value -= definitions[columnIndex].FinalOffset;

			return value;
		}

		internal double GetFinalRowDefinitionHeight(int rowIndex)
		{
			var value = 0.0;

			Debug.Assert(ExtData != null);

			//  actual value calculations require structure to be up-to-date
			if (RowDefinitionCollectionDirty)
				return value;

			var definitions = DefinitionsV;

			value = definitions[(rowIndex + 1) % definitions.Count].FinalOffset;

			if (rowIndex != 0)
				value -= definitions[rowIndex].FinalOffset;

			return value;
		}

		private static double GetFinalSizeForRange<TDefinition>(GridDefinitionCollection<TDefinition> definitions, int start, int count) where TDefinition : GridPanelDefinition
		{
			double size = 0;
			var i = start + count - 1;

			do
			{
				size += definitions[i].SizeCache;
			} while (--i >= start);

			return size;
		}

		public static bool GetIsSharedSizeScope(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (bool) element.GetValue(IsSharedSizeScopeProperty);
		}

		private LayoutTimeSizeType GetLengthTypeForRange<TDefinition>(GridDefinitionCollection<TDefinition> definitions, int start, int count) where TDefinition : GridPanelDefinition
		{
			Debug.Assert(0 < count && 0 <= start && start + count <= definitions.Count);

			var lengthType = LayoutTimeSizeType.None;
			var i = start + count - 1;

			do
			{
				lengthType |= definitions[i].SizeType;
			} while (--i >= start);

			return lengthType;
		}

		private static double GetMeasureSizeForRange<TDefinition>(GridDefinitionCollection<TDefinition> definitions, int start, int count) where TDefinition : GridPanelDefinition
		{
			Debug.Assert(0 < count && 0 <= start && start + count <= definitions.Count);

			double measureSize = 0;
			var i = start + count - 1;

			do
			{
				measureSize += definitions[i].SizeType == LayoutTimeSizeType.Auto ? definitions[i].MinSize : definitions[i].MeasureSize;
			} while (--i >= start);

			return measureSize;
		}

		[AttachedPropertyBrowsableForChildren]
		public static int GetRow(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (int) element.GetValue(RowProperty);
		}

		public static int GetRowSpan(UIElement element)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			return (int) element.GetValue(RowSpanProperty);
		}

		internal void Invalidate()
		{
			CellsStructureDirty = true;

			InvalidateMeasure();
		}

		private static object IsIntValueGreaterThanZero(DependencyObject d, object value)
		{
			return (int) value > 0 ? value : 1;
		}

		private static object IsIntValueNotNegative(DependencyObject d, object value)
		{
			return (int) value >= 0 ? value : 0;
		}

		private void MeasureCell(int cell, bool forceInfinityV)
		{
			double cellMeasureWidth;
			double cellMeasureHeight;

			if (PrivateCells[cell].IsAutoU
			    && !PrivateCells[cell].IsStarU)
			{
				//  if cell belongs to at least one Auto column and not a single Star column
				//  then it should be calculated "to content", thus it is possible to "shortcut"
				//  calculations and simply assign PositiveInfinity here.
				cellMeasureWidth = double.PositiveInfinity;
			}
			else
			{
				//  otherwise...
				cellMeasureWidth = GetMeasureSizeForRange(
					DefinitionsU,
					PrivateCells[cell].ColumnIndex,
					PrivateCells[cell].ColumnSpan);
			}

			if (forceInfinityV)
			{
				cellMeasureHeight = double.PositiveInfinity;
			}
			else if (PrivateCells[cell].IsAutoV
			         && !PrivateCells[cell].IsStarV)
			{
				//  if cell belongs to at least one Auto row and not a single Star row
				//  then it should be calculated "to content", thus it is possible to "shortcut"
				//  calculations and simply assign PositiveInfinity here.
				cellMeasureHeight = double.PositiveInfinity;
			}
			else
			{
				cellMeasureHeight = GetMeasureSizeForRange(
					DefinitionsV,
					PrivateCells[cell].RowIndex,
					PrivateCells[cell].RowSpan);
			}

			var child = InternalChildren[cell];

			if (child == null)
				return;

			var childConstraint = new Size(cellMeasureWidth, cellMeasureHeight);

			child.Measure(childConstraint);
		}

		private void MeasureCellsGroup(
			int cellsHead,
			Size referenceSize,
			bool ignoreDesiredSizeU,
			bool forceInfinityV)
		{
			MeasureCellsGroup(cellsHead, referenceSize, ignoreDesiredSizeU, forceInfinityV, out _);
		}

		/// <summary>
		///   Measures one group of cells.
		/// </summary>
		/// <param name="cellsHead">Head index of the cells chain.</param>
		/// <param name="referenceSize">
		///   Reference size for spanned cells
		///   calculations.
		/// </param>
		/// <param name="ignoreDesiredSizeU">
		///   When "true" cells' desired
		///   width is not registered in columns.
		/// </param>
		/// <param name="forceInfinityV">
		///   Passed through to MeasureCell.
		///   When "true" cells' desired height is not registered in rows.
		/// </param>
		private void MeasureCellsGroup(int cellsHead, Size referenceSize, bool ignoreDesiredSizeU, bool forceInfinityV, out bool hasDesiredSizeUChanged)
		{
			hasDesiredSizeUChanged = false;

			if (cellsHead >= PrivateCells.Length)
			{
				return;
			}

			var children = InternalChildren;
			var ignoreDesiredSizeV = forceInfinityV;
			Hashtable spanStore = null;

			var i = cellsHead;

			do
			{
				var oldWidth = children[i].DesiredSize.Width;

				MeasureCell(i, forceInfinityV);

				hasDesiredSizeUChanged |= !DoubleUtils.AreClose(oldWidth, children[i].DesiredSize.Width);

				if (!ignoreDesiredSizeU)
				{
					if (PrivateCells[i].ColumnSpan == 1)
						DefinitionsU[PrivateCells[i].ColumnIndex].UpdateMinSize(Math.Min(children[i].DesiredSize.Width, DefinitionsU[PrivateCells[i].ColumnIndex].UserMaxSize));
					else
					{
						RegisterSpan(
							ref spanStore,
							PrivateCells[i].ColumnIndex,
							PrivateCells[i].ColumnSpan,
							true,
							children[i].DesiredSize.Width);
					}
				}

				if (!ignoreDesiredSizeV)
				{
					if (PrivateCells[i].RowSpan == 1)
					{
						DefinitionsV[PrivateCells[i].RowIndex].UpdateMinSize(Math.Min(children[i].DesiredSize.Height, DefinitionsV[PrivateCells[i].RowIndex].UserMaxSize));
					}
					else
					{
						RegisterSpan(
							ref spanStore,
							PrivateCells[i].RowIndex,
							PrivateCells[i].RowSpan,
							false,
							children[i].DesiredSize.Height);
					}
				}

				i = PrivateCells[i].Next;
			} while (i < PrivateCells.Length);

			if (spanStore == null)
				return;

			foreach (DictionaryEntry e in spanStore)
			{
				var key = (SpanKey) e.Key;
				var requestedSize = (double) e.Value;

				if (key.U)
					EnsureMinSizeInDefinitionRange(DefinitionsU, key.Start, key.Count, requestedSize, referenceSize.Width);
				else
					EnsureMinSizeInDefinitionRange(DefinitionsV, key.Start, key.Count, requestedSize, referenceSize.Height);
			}
		}

		protected override Size MeasureOverrideCore(Size constraint)
		{
			Size gridDesiredSize;
			var extData = ExtData;

			try
			{
				ListenToNotifications = true;
				MeasureOverrideInProgress = true;

				if (extData == null)
				{
					gridDesiredSize = new Size();
					var children = InternalChildren;

					for (int i = 0, count = children.Count; i < count; ++i)
					{
						var child = children[i];

						if (child == null)
							continue;

						child.Measure(constraint);
						gridDesiredSize.Width = Math.Max(gridDesiredSize.Width, child.DesiredSize.Width);
						gridDesiredSize.Height = Math.Max(gridDesiredSize.Height, child.DesiredSize.Height);
					}
				}
				else
				{
					{
						var sizeToContentU = double.IsPositiveInfinity(constraint.Width);
						var sizeToContentV = double.IsPositiveInfinity(constraint.Height);

						// Clear index information and rounding errors
						if (RowDefinitionCollectionDirty || ColumnDefinitionCollectionDirty)
						{
							if (_definitionIndices != null)
							{
								Array.Clear(_definitionIndices, 0, _definitionIndices.Length);
								_definitionIndices = null;
							}

							if (UseLayoutRounding)
							{
								if (_roundingErrors != null)
								{
									Array.Clear(_roundingErrors, 0, _roundingErrors.Length);
									_roundingErrors = null;
								}
							}
						}

						ValidateDefinitionsUStructure();
						ValidateDefinitionsLayout(DefinitionsU, sizeToContentU);

						ValidateDefinitionsVStructure();
						ValidateDefinitionsLayout(DefinitionsV, sizeToContentV);

						CellsStructureDirty |= SizeToContentU != sizeToContentU || SizeToContentV != sizeToContentV;

						SizeToContentU = sizeToContentU;
						SizeToContentV = sizeToContentV;
					}

					ValidateCells();

					Debug.Assert(DefinitionsU.Count > 0 && DefinitionsV.Count > 0);

					//  Grid classifies cells into four groups depending on
					//  the column / row type a cell belongs to (number corresponds to
					//  group number):
					//
					//                   Px      Auto     Star
					//               +--------+--------+--------+
					//               |        |        |        |
					//            Px |    1   |    1   |    3   |
					//               |        |        |        |
					//               +--------+--------+--------+
					//               |        |        |        |
					//          Auto |    1   |    1   |    3   |
					//               |        |        |        |
					//               +--------+--------+--------+
					//               |        |        |        |
					//          Star |    4   |    2   |    4   |
					//               |        |        |        |
					//               +--------+--------+--------+
					//
					//  The group number indicates the order in which cells are measured.
					//  Certain order is necessary to be able to dynamically resolve star
					//  columns / rows sizes which are used as input for measuring of
					//  the cells belonging to them.
					//
					//  However, there are cases when topology of a grid causes cyclical
					//  size dependences. For example:
					//
					//
					//                         column width="Auto"      column width="*"
					//                      +----------------------+----------------------+
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//  row height="Auto"   |                      |      cell 1 2        |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      +----------------------+----------------------+
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//  row height="*"      |       cell 2 1       |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      |                      |                      |
					//                      +----------------------+----------------------+
					//
					//  In order to accurately calculate constraint width for "cell 1 2"
					//  (which is the remaining of grid's available width and calculated
					//  value of Auto column), "cell 2 1" needs to be calculated first,
					//  as it contributes to the Auto column's calculated value.
					//  At the same time in order to accurately calculate constraint
					//  height for "cell 2 1", "cell 1 2" needs to be calcualted first,
					//  as it contributes to Auto row height, which is used in the
					//  computation of Star row resolved height.
					//
					//  to "break" this cyclical dependency we are making (arbitrary)
					//  decision to treat cells like "cell 2 1" as if they appear in Auto
					//  rows. And then recalculate them one more time when star row
					//  heights are resolved.
					//
					//  (Or more strictly) the code below implement the following logic:
					//
					//                       +---------+
					//                       |  enter  |
					//                       +---------+
					//                            |
					//                            V
					//                    +----------------+
					//                    | Measure Group1 |
					//                    +----------------+
					//                            |
					//                            V
					//                          / - \
					//                        /       \
					//                  Y   /    Can    \    N
					//            +--------|   Resolve   |-----------+
					//            |         \  StarsV?  /            |
					//            |           \       /              |
					//            |             \ - /                |
					//            V                                  V
					//    +----------------+                       / - \
					//    | Resolve StarsV |                     /       \
					//    +----------------+               Y   /    Can    \    N
					//            |                      +----|   Resolve   |------+
					//            V                      |     \  StarsU?  /       |
					//    +----------------+             |       \       /         |
					//    | Measure Group2 |             |         \ - /           |
					//    +----------------+             |                         V
					//            |                      |                 +-----------------+
					//            V                      |                 | Measure Group2' |
					//    +----------------+             |                 +-----------------+
					//    | Resolve StarsU |             |                         |
					//    +----------------+             V                         V
					//            |              +----------------+        +----------------+
					//            V              | Resolve StarsU |        | Resolve StarsU |
					//    +----------------+     +----------------+        +----------------+
					//    | Measure Group3 |             |                         |
					//    +----------------+             V                         V
					//            |              +----------------+        +----------------+
					//            |              | Measure Group3 |        | Measure Group3 |
					//            |              +----------------+        +----------------+
					//            |                      |                         |
					//            |                      V                         V
					//            |              +----------------+        +----------------+
					//            |              | Resolve StarsV |        | Resolve StarsV |
					//            |              +----------------+        +----------------+
					//            |                      |                         |
					//            |                      |                         V
					//            |                      |                +------------------+
					//            |                      |                | Measure Group2'' |
					//            |                      |                +------------------+
					//            |                      |                         |
					//            +----------------------+-------------------------+
					//                                   |
					//                                   V
					//                           +----------------+
					//                           | Measure Group4 |
					//                           +----------------+
					//                                   |
					//                                   V
					//                               +--------+
					//                               |  exit  |
					//                               +--------+
					//
					//  where:
					//  *   all [Measure GroupN] - regular children measure process -
					//      each cell is measured given contraint size as an input
					//      and each cell's desired size is accumulated on the
					//      corresponding column / row;
					//  *   [Measure Group2'] - is when each cell is measured with
					//      infinit height as a constraint and a cell's desired
					//      height is ignored;
					//  *   [Measure Groups''] - is when each cell is measured (second
					//      time during single Grid.MeasureOverride) regularly but its
					//      returned width is ignored;
					//
					//  This algorithm is believed to be as close to ideal as possible.
					//  It has the following drawbacks:
					//  *   cells belonging to Group2 can be called to measure twice;
					//  *   iff during second measure a cell belonging to Group2 returns
					//      desired width greater than desired width returned the first
					//      time, such a cell is going to be clipped, even though it
					//      appears in Auto column.
					//

					MeasureCellsGroup(extData.CellGroup1, constraint, false, false);

					{
						//  after Group1 is measured,  only Group3 may have cells belonging to Auto rows.
						var canResolveStarsV = !HasGroup3CellsInAutoRows;

						if (canResolveStarsV)
						{
							if (HasStarCellsV)
							{
								ResolveStar(DefinitionsV, constraint.Height);
							}

							MeasureCellsGroup(extData.CellGroup2, constraint, false, false);
							if (HasStarCellsU)
							{
								ResolveStar(DefinitionsU, constraint.Width);
							}

							MeasureCellsGroup(extData.CellGroup3, constraint, false, false);
						}
						else
						{
							//  if at least one cell exists in Group2, it must be measured before
							//  StarsU can be resolved.
							var canResolveStarsU = extData.CellGroup2 > PrivateCells.Length;
							if (canResolveStarsU)
							{
								if (HasStarCellsU)
								{
									ResolveStar(DefinitionsU, constraint.Width);
								}

								MeasureCellsGroup(extData.CellGroup3, constraint, false, false);
								if (HasStarCellsV)
								{
									ResolveStar(DefinitionsV, constraint.Height);
								}
							}
							else
							{
								// This is a revision to the algorithm employed for the cyclic
								// dependency case described above. We now repeatedly
								// measure Group3 and Group2 until their sizes settle. We
								// also use a count heuristic to break a loop in case of one.

								var hasDesiredSizeUChanged = false;
								var cnt = 0;

								// Cache Group2MinWidths & Group3MinHeights
								var group2MinSizes = CacheMinSizes(extData.CellGroup2, false);
								var group3MinSizes = CacheMinSizes(extData.CellGroup3, true);

								MeasureCellsGroup(extData.CellGroup2, constraint, false, true);

								do
								{
									if (hasDesiredSizeUChanged)
									{
										// Reset cached Group3Heights
										ApplyCachedMinSizes(group3MinSizes, true);
									}

									if (HasStarCellsU)
									{
										ResolveStar(DefinitionsU, constraint.Width);
									}

									MeasureCellsGroup(extData.CellGroup3, constraint, false, false);

									// Reset cached Group2Widths
									ApplyCachedMinSizes(group2MinSizes, false);

									if (HasStarCellsV)
									{
										ResolveStar(DefinitionsV, constraint.Height);
									}

									MeasureCellsGroup(extData.CellGroup2, constraint, cnt == LayoutLoopMaxCount, false, out hasDesiredSizeUChanged);
								} while (hasDesiredSizeUChanged && ++cnt <= LayoutLoopMaxCount);
							}
						}
					}

					MeasureCellsGroup(extData.CellGroup4, constraint, false, false);

					gridDesiredSize = new Size(CalculateDesiredSize(DefinitionsU), CalculateDesiredSize(DefinitionsV));
				}
			}
			finally
			{
				MeasureOverrideInProgress = false;
			}

			return gridDesiredSize;
		}

		/// <summary>
		///   <see cref="PropertyMetadata.PropertyChangedCallback" />
		/// </summary>
		private static void OnCellAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Visual child)
			{
				if (VisualTreeHelper.GetParent(child) is GridPanel grid && grid.ExtData != null && grid.ListenToNotifications)
				{
					grid.CellsStructureDirty = true;
					grid.InvalidateMeasure();
				}
			}
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			CellsStructureDirty = true;

			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}

		private static void RegisterSpan(ref Hashtable store, int start, int count, bool u, double value)
		{
			store ??= new Hashtable();

			var key = new SpanKey(start, count, u);
			var o = store[key];

			if (o == null || value > (double) o)
				store[key] = value;
		}

		private void ResolveStar<TDefinition>(GridDefinitionCollection<TDefinition> definitions, double availableSize) where TDefinition : GridPanelDefinition
		{
			ResolveStarMaxDiscrepancy(definitions, availableSize);
		}

		// new implementation as of 4.7.  Several improvements:
		// 1. Allocate to *-defs hitting their min or max constraints, before allocating
		//      to other *-defs.  A def that hits its min uses more space than its
		//      proportional share, reducing the space available to everyone else.
		//      The legacy algorithm deducted this space only from defs processed
		//      after the min;  the new algorithm deducts it proportionally from all
		//      defs.   This avoids the "*-defs exceed available space" problem,
		//      and other related problems where *-defs don't receive proportional
		//      allocations even though no constraints are preventing it.
		// 2. When multiple defs hit min or max, resolve the one with maximum
		//      discrepancy (defined below).   This avoids discontinuities - small
		//      change in available space resulting in large change to one def's allocation.
		// 3. Correct handling of large *-values, including Infinity.
		private void ResolveStarMaxDiscrepancy<TDefinition>(GridDefinitionCollection<TDefinition> definitions, double availableSize) where TDefinition : GridPanelDefinition
		{
			var defCount = definitions.Count;
			var tempDefinitions = TempDefinitions;
			double takenSize = 0;
			double totalStarWeight;
			var starCount = 0; // number of unresolved *-definitions
			var scale = 1.0; // scale factor applied to each *-weight;  negative means "Infinity is present"

			// Phase 1.  Determine the maximum *-weight and prepare to adjust *-weights
			var maxStar = 0.0;
			for (var i = 0; i < defCount; ++i)
			{
				var def = definitions[i];

				if (def.SizeType == LayoutTimeSizeType.Star)
				{
					++starCount;
					def.MeasureSize = 1.0; // meaning "not yet resolved in phase 3"

					if (def.UserSize.Value > maxStar)
						maxStar = def.UserSize.Value;
				}
			}

			if (double.IsPositiveInfinity(maxStar))
			{
				// negative scale means one or more of the weights was Infinity
				scale = -1.0;
			}
			else if (starCount > 0)
			{
				// if maxStar * starCount > Double.Max, summing all the weights could cause
				// floating-point overflow.  To avoid that, scale the weights by a factor to keep
				// the sum within limits.  Choose a power of 2, to preserve precision.
				var power = Math.Floor(Math.Log(double.MaxValue / maxStar / starCount, 2.0));

				if (power < 0.0)
					scale = Math.Pow(2.0, power - 4.0); // -4 is just for paranoia
			}

			// normally Phases 2 and 3 execute only once.  But certain unusual combinations of weights
			// and constraints can defeat the algorithm, in which case we repeat Phases 2 and 3.
			// More explanation below...
			for (var runPhase2and3 = true; runPhase2and3;)
			{
				// Phase 2.   Compute total *-weight W and available space S.
				// For *-items that have Min or Max constraints, compute the ratios used to decide
				// whether proportional space is too big or too small and add the item to the
				// corresponding list.  (The "min" list is in the first half of tempDefinitions,
				// the "max" list in the second half.  TempDefinitions has capacity at least
				// 2*defCount, so there's room for both lists.)
				totalStarWeight = 0.0;
				takenSize = 0.0;
				int maxCount;
				var minCount = maxCount = 0;

				for (var i = 0; i < defCount; ++i)
				{
					var def = definitions[i];

					switch (def.SizeType)
					{
						case LayoutTimeSizeType.Auto:
							takenSize += definitions[i].MinSize;
							break;
						case LayoutTimeSizeType.Pixel:
							takenSize += def.MeasureSize;
							break;
						case LayoutTimeSizeType.Star:
							if (def.MeasureSize < 0.0)
							{
								takenSize += -def.MeasureSize; // already resolved
							}
							else
							{
								var starWeight = StarWeight(def, scale);
								totalStarWeight += starWeight;

								if (def.MinSize > 0.0)
								{
									// store ratio w/min in MeasureSize (for now)
									tempDefinitions[minCount++] = def;
									def.MeasureSize = starWeight / def.MinSize;
								}

								var effectiveMaxSize = Math.Max(def.MinSize, def.UserMaxSize);

								if (!double.IsPositiveInfinity(effectiveMaxSize))
								{
									// store ratio w/max in SizeCache (for now)
									tempDefinitions[defCount + maxCount++] = def;
									def.SizeCache = starWeight / effectiveMaxSize;
								}
							}

							break;
					}
				}

				// Phase 3.  Resolve *-items whose proportional sizes are too big or too small.
				int minCountPhase2 = minCount, maxCountPhase2 = maxCount;
				var takenStarWeight = 0.0;
				var remainingAvailableSize = availableSize - takenSize;
				var remainingStarWeight = totalStarWeight - takenStarWeight;
				Array.Sort(tempDefinitions, 0, minCount, MinRatioComparerInstance);
				Array.Sort(tempDefinitions, defCount, maxCount, MaxRatioComparerInstance);

				while (minCount + maxCount > 0 && remainingAvailableSize > 0.0)
				{
					// the calculation
					//            remainingStarWeight = totalStarWeight - takenStarWeight
					// is subject to catastrophic cancellation if the two terms are nearly equal,
					// which leads to meaningless results.   Check for that, and recompute from
					// the remaining definitions.   [This leads to quadratic behavior in really
					// pathological cases - but they'd never arise in practice.]
					const double starFactor = 1.0 / 256.0; // lose more than 8 bits of precision -> recalculate

					if (remainingStarWeight < totalStarWeight * starFactor)
					{
						takenStarWeight = 0.0;
						totalStarWeight = 0.0;

						for (var i = 0; i < defCount; ++i)
						{
							var def = definitions[i];

							if (def.SizeType == LayoutTimeSizeType.Star && def.MeasureSize > 0.0)
							{
								totalStarWeight += StarWeight(def, scale);
							}
						}

						remainingStarWeight = totalStarWeight - takenStarWeight;
					}

					var minRatio = minCount > 0 ? tempDefinitions[minCount - 1].MeasureSize : double.PositiveInfinity;
					var maxRatio = maxCount > 0 ? tempDefinitions[defCount + maxCount - 1].SizeCache : -1.0;

					// choose the def with larger ratio to the current proportion ("max discrepancy")
					var proportion = remainingStarWeight / remainingAvailableSize;
					var chooseMin = Choose(minRatio, maxRatio, proportion);

					// if no def was chosen, advance to phase 4;  the current proportion doesn't
					// conflict with any min or max values.
					if (!chooseMin.HasValue)
					{
						break;
					}

					// get the chosen definition and its resolved size
					GridPanelDefinition resolvedDef;
					double resolvedSize;

					if (chooseMin == true)
					{
						resolvedDef = tempDefinitions[minCount - 1];
						resolvedSize = resolvedDef.MinSize;
						--minCount;
					}
					else
					{
						resolvedDef = tempDefinitions[defCount + maxCount - 1];
						resolvedSize = Math.Max(resolvedDef.MinSize, resolvedDef.UserMaxSize);
						--maxCount;
					}

					// resolve the chosen def, deduct its contributions from W and S.
					// Defs resolved in phase 3 are marked by storing the negative of their resolved
					// size in MeasureSize, to distinguish them from a pending def.
					takenSize += resolvedSize;
					resolvedDef.MeasureSize = -resolvedSize;
					takenStarWeight += StarWeight(resolvedDef, scale);
					--starCount;

					remainingAvailableSize = availableSize - takenSize;
					remainingStarWeight = totalStarWeight - takenStarWeight;

					// advance to the next candidate defs, removing ones that have been resolved.
					// Both counts are advanced, as a def might appear in both lists.
					while (minCount > 0 && tempDefinitions[minCount - 1].MeasureSize < 0.0)
					{
						--minCount;
						tempDefinitions[minCount] = null;
					}

					while (maxCount > 0 && tempDefinitions[defCount + maxCount - 1].MeasureSize < 0.0)
					{
						--maxCount;
						tempDefinitions[defCount + maxCount] = null;
					}
				}

				// decide whether to run Phase2 and Phase3 again.  There are 3 cases:
				// 1. There is space available, and *-defs remaining.  This is the
				//      normal case - move on to Phase 4 to allocate the remaining
				//      space proportionally to the remaining *-defs.
				// 2. There is space available, but no *-defs.  This implies at least one
				//      def was resolved as 'max', taking less space than its proportion.
				//      If there are also 'min' defs, reconsider them - we can give
				//      them more space.   If not, all the *-defs are 'max', so there's
				//      no way to use all the available space.
				// 3. We allocated too much space.   This implies at least one def was
				//      resolved as 'min'.  If there are also 'max' defs, reconsider
				//      them, otherwise the over-allocation is an inevitable consequence
				//      of the given min constraints.
				// Note that if we return to Phase2, at least one *-def will have been
				// resolved.  This guarantees we don't run Phase2+3 infinitely often.
				runPhase2and3 = false;

				if (starCount == 0 && takenSize < availableSize)
				{
					// if no *-defs remain and we haven't allocated all the space, reconsider the defs
					// resolved as 'min'.   Their allocation can be increased to make up the gap.
					for (var i = minCount; i < minCountPhase2; ++i)
					{
						var def = tempDefinitions[i];

						if (def == null)
							continue;

						def.MeasureSize = 1.0; // mark as 'not yet resolved'
						++starCount;
						runPhase2and3 = true; // found a candidate, so re-run Phases 2 and 3
					}
				}

				if (takenSize > availableSize)
				{
					// if we've allocated too much space, reconsider the defs
					// resolved as 'max'.   Their allocation can be decreased to make up the gap.
					for (var i = maxCount; i < maxCountPhase2; ++i)
					{
						var def = tempDefinitions[defCount + i];

						if (def == null)
							continue;

						def.MeasureSize = 1.0; // mark as 'not yet resolved'
						++starCount;
						runPhase2and3 = true; // found a candidate, so re-run Phases 2 and 3
					}
				}
			}

			// Phase 4.  Resolve the remaining defs proportionally.
			starCount = 0;

			for (var i = 0; i < defCount; ++i)
			{
				var def = definitions[i];

				if (def.SizeType == LayoutTimeSizeType.Star)
				{
					if (def.MeasureSize < 0.0)
					{
						// this def was resolved in phase 3 - fix up its measure size
						def.MeasureSize = -def.MeasureSize;
					}
					else
					{
						// this def needs resolution, add it to the list, sorted by *-weight
						tempDefinitions[starCount++] = def;
						def.MeasureSize = StarWeight(def, scale);
					}
				}
			}

			if (starCount > 0)
			{
				Array.Sort(tempDefinitions, 0, starCount, StarWeightComparerInstance);

				// compute the partial sums of *-weight, in increasing order of weight
				// for minimal loss of precision.
				totalStarWeight = 0.0;

				for (var i = 0; i < starCount; ++i)
				{
					var def = tempDefinitions[i];

					totalStarWeight += def.MeasureSize;
					def.SizeCache = totalStarWeight;
				}

				// resolve the defs, in decreasing order of weight
				for (var i = starCount - 1; i >= 0; --i)
				{
					var def = tempDefinitions[i];
					var resolvedSize = def.MeasureSize > 0.0 ? Math.Max(availableSize - takenSize, 0.0) * (def.MeasureSize / def.SizeCache) : 0.0;

					// min and max should have no effect by now, but just in case...
					resolvedSize = Math.Min(resolvedSize, def.UserMaxSize);
					resolvedSize = Math.Max(def.MinSize, resolvedSize);

					def.MeasureSize = resolvedSize;
					takenSize += resolvedSize;
				}
			}
		}

		internal static double RoundLayoutValue(double value, double dpiScale)
		{
			double newValue;

			// If DPI == 1, don't use DPI-aware rounding.
			if (!DoubleUtils.AreClose(dpiScale, 1.0))
			{
				newValue = Math.Round(value * dpiScale) / dpiScale;

				// If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
				if (DoubleUtils.IsNaN(newValue) || double.IsInfinity(newValue) || DoubleUtils.AreClose(newValue, double.MaxValue))
					newValue = value;
			}
			else
				newValue = Math.Round(value);

			return newValue;
		}

		public static void SetColumn(UIElement element, int value)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(ColumnProperty, value);
		}

		public static void SetColumnSpan(UIElement element, int value)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(ColumnSpanProperty, value);
		}

		private void SetFinalSize<TDefinition>(GridDefinitionCollection<TDefinition> definitions, double finalSize, bool columns) where TDefinition : GridPanelDefinition
		{
			SetFinalSizeMaxDiscrepancy(definitions, finalSize, columns);
		}

		// new implementation, as of 4.7.  This incorporates the same algorithm
		// as in ResolveStarMaxDiscrepancy.  It differs in the same way that SetFinalSizeLegacy
		// differs from ResolveStarLegacy, namely (a) leaves results in def.SizeCache
		// instead of def.MeasureSize, (b) implements LayoutRounding if requested,
		// (c) stores intermediate results differently.
		// The LayoutRounding logic is improved:
		// 1. Use pre-rounded values during proportional allocation.  This avoids the
		//      same kind of problems arising from interaction with min/max that
		//      motivated the new algorithm in the first place.
		// 2. Use correct "nudge" amount when distributing roundoff space.   This
		//      comes into play at high DPI - greater than 134.
		// 3. Applies rounding only to real pixel values (not to ratios)
		private void SetFinalSizeMaxDiscrepancy<TDefinition>(GridDefinitionCollection<TDefinition> definitions, double finalSize, bool columns) where TDefinition : GridPanelDefinition
		{
			var defCount = definitions.Count;
			var definitionIndices = DefinitionIndices;
			var takenSize = 0.0;
			double totalStarWeight;
			var starCount = 0; // number of unresolved *-definitions
			var scale = 1.0; // scale factor applied to each *-weight;  negative means "Infinity is present"

			// Phase 1.  Determine the maximum *-weight and prepare to adjust *-weights
			var maxStar = 0.0;

			for (var i = 0; i < defCount; ++i)
			{
				var def = definitions[i];

				if (def.UserSize.IsStar)
				{
					++starCount;
					def.MeasureSize = 1.0; // meaning "not yet resolved in phase 3"

					if (def.UserSize.Value > maxStar)
						maxStar = def.UserSize.Value;
				}
			}

			if (double.IsPositiveInfinity(maxStar))
			{
				// negative scale means one or more of the weights was Infinity
				scale = -1.0;
			}
			else if (starCount > 0)
			{
				// if maxStar * starCount > Double.Max, summing all the weights could cause
				// floating-point overflow.  To avoid that, scale the weights by a factor to keep
				// the sum within limits.  Choose a power of 2, to preserve precision.
				var power = Math.Floor(Math.Log(double.MaxValue / maxStar / starCount, 2.0));

				if (power < 0.0)
				{
					scale = Math.Pow(2.0, power - 4.0); // -4 is just for paranoia
				}
			}

			// normally Phases 2 and 3 execute only once.  But certain unusual combinations of weights
			// and constraints can defeat the algorithm, in which case we repeat Phases 2 and 3.
			// More explanation below...
			for (var runPhase2and3 = true; runPhase2and3;)
			{
				// Phase 2.   Compute total *-weight W and available space S.
				// For *-items that have Min or Max constraints, compute the ratios used to decide
				// whether proportional space is too big or too small and add the item to the
				// corresponding list.  (The "min" list is in the first half of definitionIndices,
				// the "max" list in the second half.  DefinitionIndices has capacity at least
				// 2*defCount, so there's room for both lists.)
				totalStarWeight = 0.0;
				takenSize = 0.0;

				int maxCount;
				var minCount = maxCount = 0;

				for (var i = 0; i < defCount; ++i)
				{
					var def = definitions[i];

					if (def.UserSize.IsStar)
					{
						Debug.Assert(!def.IsShared, "*-defs cannot be shared");

						if (def.MeasureSize < 0.0)
						{
							takenSize += -def.MeasureSize; // already resolved
						}
						else
						{
							var starWeight = StarWeight(def, scale);

							totalStarWeight += starWeight;

							if (def.MinSizeForArrange > 0.0)
							{
								// store ratio w/min in MeasureSize (for now)
								definitionIndices[minCount++] = i;
								def.MeasureSize = starWeight / def.MinSizeForArrange;
							}

							var effectiveMaxSize = Math.Max(def.MinSizeForArrange, def.UserMaxSize);

							if (!double.IsPositiveInfinity(effectiveMaxSize))
							{
								// store ratio w/max in SizeCache (for now)
								definitionIndices[defCount + maxCount++] = i;
								def.SizeCache = starWeight / effectiveMaxSize;
							}
						}
					}
					else
					{
						double userSize = 0;

						switch (def.UserSize.UnitType)
						{
							case FlexLengthUnitType.Pixel:
								userSize = def.UserSize.Value;
								break;

							case FlexLengthUnitType.Auto:
								userSize = def.MinSizeForArrange;
								break;
						}

						double userMaxSize;

						if (def.IsShared)
						{
							//  overriding userMaxSize effectively prevents squishy-ness.
							//  this is a "solution" to avoid shared definitions from been sized to
							//  different final size at arrange time, if / when different grids receive
							//  different final sizes.
							userMaxSize = userSize;
						}
						else
						{
							userMaxSize = def.UserMaxSize;
						}

						def.SizeCache = Math.Max(def.MinSizeForArrange, Math.Min(userSize, userMaxSize));
						takenSize += def.SizeCache;
					}
				}

				// Phase 3.  Resolve *-items whose proportional sizes are too big or too small.
				int minCountPhase2 = minCount, maxCountPhase2 = maxCount;
				var takenStarWeight = 0.0;
				var remainingAvailableSize = finalSize - takenSize;
				var remainingStarWeight = totalStarWeight - takenStarWeight;

				var minRatioIndexComparer = new MinRatioIndexComparer<TDefinition>(definitions);
				Array.Sort(definitionIndices, 0, minCount, minRatioIndexComparer);
				var maxRatioIndexComparer = new MaxRatioIndexComparer<TDefinition>(definitions);
				Array.Sort(definitionIndices, defCount, maxCount, maxRatioIndexComparer);

				while (minCount + maxCount > 0 && remainingAvailableSize > 0.0)
				{
					// the calculation
					//            remainingStarWeight = totalStarWeight - takenStarWeight
					// is subject to catastrophic cancellation if the two terms are nearly equal,
					// which leads to meaningless results.   Check for that, and recompute from
					// the remaining definitions.   [This leads to quadratic behavior in really
					// pathological cases - but they'd never arise in practice.]
					const double starFactor = 1.0 / 256.0; // lose more than 8 bits of precision -> recalculate

					if (remainingStarWeight < totalStarWeight * starFactor)
					{
						takenStarWeight = 0.0;
						totalStarWeight = 0.0;

						for (var i = 0; i < defCount; ++i)
						{
							var def = definitions[i];

							if (def.UserSize.IsStar && def.MeasureSize > 0.0)
							{
								totalStarWeight += StarWeight(def, scale);
							}
						}

						remainingStarWeight = totalStarWeight - takenStarWeight;
					}

					var minRatio = minCount > 0 ? definitions[definitionIndices[minCount - 1]].MeasureSize : double.PositiveInfinity;
					var maxRatio = maxCount > 0 ? definitions[definitionIndices[defCount + maxCount - 1]].SizeCache : -1.0;

					// choose the def with larger ratio to the current proportion ("max discrepancy")
					var proportion = remainingStarWeight / remainingAvailableSize;
					var chooseMin = Choose(minRatio, maxRatio, proportion);

					// if no def was chosen, advance to phase 4;  the current proportion doesn't
					// conflict with any min or max values.
					if (chooseMin.HasValue == false)
						break;

					// get the chosen definition and its resolved size
					int resolvedIndex;
					GridPanelDefinition resolvedDef;
					double resolvedSize;

					if (chooseMin == true)
					{
						resolvedIndex = definitionIndices[minCount - 1];
						resolvedDef = definitions[resolvedIndex];
						resolvedSize = resolvedDef.MinSizeForArrange;
						--minCount;
					}
					else
					{
						resolvedIndex = definitionIndices[defCount + maxCount - 1];
						resolvedDef = definitions[resolvedIndex];
						resolvedSize = Math.Max(resolvedDef.MinSizeForArrange, resolvedDef.UserMaxSize);
						--maxCount;
					}

					// resolve the chosen def, deduct its contributions from W and S.
					// Defs resolved in phase 3 are marked by storing the negative of their resolved
					// size in MeasureSize, to distinguish them from a pending def.
					takenSize += resolvedSize;
					resolvedDef.MeasureSize = -resolvedSize;
					takenStarWeight += StarWeight(resolvedDef, scale);
					--starCount;

					remainingAvailableSize = finalSize - takenSize;
					remainingStarWeight = totalStarWeight - takenStarWeight;

					// advance to the next candidate defs, removing ones that have been resolved.
					// Both counts are advanced, as a def might appear in both lists.
					while (minCount > 0 && definitions[definitionIndices[minCount - 1]].MeasureSize < 0.0)
					{
						--minCount;
						definitionIndices[minCount] = -1;
					}

					while (maxCount > 0 && definitions[definitionIndices[defCount + maxCount - 1]].MeasureSize < 0.0)
					{
						--maxCount;
						definitionIndices[defCount + maxCount] = -1;
					}
				}

				// decide whether to run Phase2 and Phase3 again.  There are 3 cases:
				// 1. There is space available, and *-defs remaining.  This is the
				//      normal case - move on to Phase 4 to allocate the remaining
				//      space proportionally to the remaining *-defs.
				// 2. There is space available, but no *-defs.  This implies at least one
				//      def was resolved as 'max', taking less space than its proportion.
				//      If there are also 'min' defs, reconsider them - we can give
				//      them more space.   If not, all the *-defs are 'max', so there's
				//      no way to use all the available space.
				// 3. We allocated too much space.   This implies at least one def was
				//      resolved as 'min'.  If there are also 'max' defs, reconsider
				//      them, otherwise the over-allocation is an inevitable consequence
				//      of the given min constraints.
				// Note that if we return to Phase2, at least one *-def will have been
				// resolved.  This guarantees we don't run Phase2+3 infinitely often.
				runPhase2and3 = false;
				if (starCount == 0 && takenSize < finalSize)
				{
					// if no *-defs remain and we haven't allocated all the space, reconsider the defs
					// resolved as 'min'.   Their allocation can be increased to make up the gap.
					for (var i = minCount; i < minCountPhase2; ++i)
					{
						if (definitionIndices[i] < 0)
							continue;

						var def = definitions[definitionIndices[i]];

						def.MeasureSize = 1.0; // mark as 'not yet resolved'
						++starCount;
						runPhase2and3 = true; // found a candidate, so re-run Phases 2 and 3
					}
				}

				if (takenSize > finalSize)
				{
					// if we've allocated too much space, reconsider the defs
					// resolved as 'max'.   Their allocation can be decreased to make up the gap.
					for (var i = maxCount; i < maxCountPhase2; ++i)
					{
						if (definitionIndices[defCount + i] < 0)
							continue;

						var def = definitions[definitionIndices[defCount + i]];

						def.MeasureSize = 1.0; // mark as 'not yet resolved'
						++starCount;
						runPhase2and3 = true; // found a candidate, so re-run Phases 2 and 3
					}
				}
			}

			// Phase 4.  Resolve the remaining defs proportionally.
			starCount = 0;

			for (var i = 0; i < defCount; ++i)
			{
				var def = definitions[i];

				if (def.UserSize.IsStar == false)
					continue;

				if (def.MeasureSize < 0.0)
				{
					// this def was resolved in phase 3 - fix up its size
					def.SizeCache = -def.MeasureSize;
				}
				else
				{
					// this def needs resolution, add it to the list, sorted by *-weight
					definitionIndices[starCount++] = i;
					def.MeasureSize = StarWeight(def, scale);
				}
			}

			if (starCount > 0)
			{
				var starWeightIndexComparer = new StarWeightIndexComparer<TDefinition>(definitions);

				Array.Sort(definitionIndices, 0, starCount, starWeightIndexComparer);

				// compute the partial sums of *-weight, in increasing order of weight
				// for minimal loss of precision.
				totalStarWeight = 0.0;

				for (var i = 0; i < starCount; ++i)
				{
					var def = definitions[definitionIndices[i]];

					totalStarWeight += def.MeasureSize;
					def.SizeCache = totalStarWeight;
				}

				// resolve the defs, in decreasing order of weight.
				for (var i = starCount - 1; i >= 0; --i)
				{
					var def = definitions[definitionIndices[i]];
					var resolvedSize = def.MeasureSize > 0.0 ? Math.Max(finalSize - takenSize, 0.0) * (def.MeasureSize / def.SizeCache) : 0.0;

					// min and max should have no effect by now, but just in case...
					resolvedSize = Math.Min(resolvedSize, def.UserMaxSize);
					resolvedSize = Math.Max(def.MinSizeForArrange, resolvedSize);

					// Use the raw (unrounded) sizes to update takenSize, so that
					// proportions are computed in the same terms as in phase 3;
					// this avoids errors arising from min/max constraints.
					takenSize += resolvedSize;
					def.SizeCache = resolvedSize;
				}
			}

			// Phase 5.  Apply layout rounding.  We do this after fully allocating
			// unrounded sizes, to avoid breaking assumptions in the previous phases
			if (UseLayoutRounding)
			{
				DpiScale dpiScale = VisualTreeHelper.GetDpi(this);

				var dpi = columns ? dpiScale.DpiScaleX : dpiScale.DpiScaleY;
				var roundingErrors = RoundingErrors;
				var roundedTakenSize = 0.0;

				// round each of the allocated sizes, keeping track of the deltas
				for (var i = 0; i < definitions.Count; ++i)
				{
					var def = definitions[i];
					var roundedSize = RoundLayoutValue(def.SizeCache, dpi);

					roundingErrors[i] = roundedSize - def.SizeCache;
					def.SizeCache = roundedSize;
					roundedTakenSize += roundedSize;
				}

				// The total allocation might differ from finalSize due to rounding
				// effects.  Tweak the allocations accordingly.

				// Theoretical and historical note.  The problem at hand - allocating
				// space to columns (or rows) with *-weights, min and max constraints,
				// and layout rounding - has a long history.  Especially the special
				// case of 50 columns with min=1 and available space=435 - allocating
				// seats in the U.S. House of Representatives to the 50 states in
				// proportion to their population.  There are numerous algorithms
				// and papers dating back to the 1700's, including the book:
				// Balinski, M. and H. Young, Fair Representation, Yale University Press, New Haven, 1982.
				//
				// One surprising result of all this research is that *any* algorithm
				// will suffer from one or more undesirable features such as the
				// "population paradox" or the "Alabama paradox", where (to use our terminology)
				// increasing the available space by one pixel might actually decrease
				// the space allocated to a given column, or increasing the weight of
				// a column might decrease its allocation.   This is worth knowing
				// in case someone complains about this behavior;  it's not a bug so
				// much as something inherent to the problem.  Cite the book mentioned
				// above or one of the 100s of references, and resolve as WontFix.
				//
				// Fortunately, our scenarios tend to have a small number of columns (~10 or fewer)
				// each being allocated a large number of pixels (~50 or greater), and
				// people don't even notice the kind of 1-pixel anomolies that are
				// theoretically inevitable, or don't care if they do.  At least they shouldn't
				// care - no one should be using the results WPF's grid layout to make
				// quantitative decisions; its job is to produce a reasonable display, not
				// to allocate seats in Congress.
				//
				// Our algorithm is more susceptible to paradox than the one currently
				// used for Congressional allocation ("Huntington-Hill" algorithm), but
				// it is faster to run:  O(N log N) vs. O(S * N), where N=number of
				// definitions, S = number of available pixels.  And it produces
				// adequate results in practice, as mentioned above.
				//
				// To reiterate one point:  all this only applies when layout rounding
				// is in effect.  When fractional sizes are allowed, the algorithm
				// behaves as well as possible, subject to the min/max constraints
				// and precision of floating-point computation.  (However, the resulting
				// display is subject to anti-aliasing problems.   TANSTAAFL.)

				if (_AreClose(roundedTakenSize, finalSize) == false)
				{
					// Compute deltas
					for (var i = 0; i < definitions.Count; ++i)
						definitionIndices[i] = i;

					// Sort rounding errors
					var roundingErrorIndexComparer = new RoundingErrorIndexComparer(roundingErrors);
					Array.Sort(definitionIndices, 0, definitions.Count, roundingErrorIndexComparer);
					var adjustedSize = roundedTakenSize;
					var dpiIncrement = 1.0 / dpi;

					if (roundedTakenSize > finalSize)
					{
						var i = definitions.Count - 1;

						while (adjustedSize > finalSize && !_AreClose(adjustedSize, finalSize) && i >= 0)
						{
							var definition = definitions[definitionIndices[i]];
							var final = definition.SizeCache - dpiIncrement;
							final = Math.Max(final, definition.MinSizeForArrange);

							if (final < definition.SizeCache)
							{
								adjustedSize -= dpiIncrement;
							}

							definition.SizeCache = final;
							i--;
						}
					}
					else if (roundedTakenSize < finalSize)
					{
						var i = 0;
						while (adjustedSize < finalSize && !_AreClose(adjustedSize, finalSize) && i < definitions.Count)
						{
							var definition = definitions[definitionIndices[i]];
							var final = definition.SizeCache + dpiIncrement;
							final = Math.Max(final, definition.MinSizeForArrange);
							if (final > definition.SizeCache)
							{
								adjustedSize += dpiIncrement;
							}

							definition.SizeCache = final;
							i++;
						}
					}
				}
			}

			// Phase 6.  Compute final offsets
			definitions[0].FinalOffset = 0.0;

			for (var i = 0; i < definitions.Count; ++i)
				definitions[(i + 1) % definitions.Count].FinalOffset = definitions[i].FinalOffset + definitions[i].SizeCache;
		}

		private void SetFlags(bool value, Flags flags)
		{
			_flags = value ? _flags | flags : _flags & ~flags;
		}

		public static void SetIsSharedSizeScope(UIElement element, bool value)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(IsSharedSizeScopeProperty, value);
		}

		public static void SetRow(UIElement element, int value)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(RowProperty, value);
		}

		public static void SetRowSpan(UIElement element, int value)
		{
			if (element == null)
				throw new ArgumentNullException(nameof(element));

			element.SetValue(RowSpanProperty, value);
		}

		private void SetValid()
		{
			var extData = ExtData;

			if (extData?.TempDefinitions == null)
				return;

			//  TempDefinitions has to be cleared to avoid "memory leaks"
			Array.Clear(extData.TempDefinitions, 0, Math.Max(DefinitionsU.Count, DefinitionsV.Count));

			extData.TempDefinitions = null;
		}

		public bool ShouldSerializeColumnDefinitions()
		{
			var extData = ExtData;

			return extData?.ColumnDefinitions != null && extData.ColumnDefinitions.Count > 0;
		}

		public bool ShouldSerializeRowDefinitions()
		{
			var extData = ExtData;

			return extData?.RowDefinitions != null && extData.RowDefinitions.Count > 0;
		}

		private static double StarWeight(GridPanelDefinition def, double scale)
		{
			if (scale < 0.0)
			{
				// if one of the *-weights is Infinity, adjust the weights by mapping
				// Infinty to 1.0 and everything else to 0.0:  the infinite items share the
				// available space equally, everyone else gets nothing.
				return double.IsPositiveInfinity(def.UserSize.Value) ? 1.0 : 0.0;
			}

			return def.UserSize.Value * scale;
		}

		private void ValidateCells()
		{
			if (CellsStructureDirty == false)
				return;

			ValidateCellsCore();
			CellsStructureDirty = false;
		}

		private void ValidateCellsCore()
		{
			var children = InternalChildren;
			var extData = ExtData;

			extData.CellCachesCollection = new CellCache[children.Count];
			extData.CellGroup1 = int.MaxValue;
			extData.CellGroup2 = int.MaxValue;
			extData.CellGroup3 = int.MaxValue;
			extData.CellGroup4 = int.MaxValue;

			var hasStarCellsU = false;
			var hasStarCellsV = false;
			var hasGroup3CellsInAutoRows = false;

			for (var i = PrivateCells.Length - 1; i >= 0; --i)
			{
				var child = children[i];
				if (child == null)
				{
					continue;
				}

				var cell = new CellCache();

				//
				//  read and cache child positioning properties
				//

				//  read indices from the corresponding properties
				//      clamp to value < number_of_columns
				//      column >= 0 is guaranteed by property value validation callback
				cell.ColumnIndex = Math.Min(GetColumn(child), DefinitionsU.Count - 1);
				//      clamp to value < number_of_rows
				//      row >= 0 is guaranteed by property value validation callback
				cell.RowIndex = Math.Min(GetRow(child), DefinitionsV.Count - 1);

				//  read span properties
				//      clamp to not exceed beyond right side of the grid
				//      column_span > 0 is guaranteed by property value validation callback
				cell.ColumnSpan = Math.Min(GetColumnSpan(child), DefinitionsU.Count - cell.ColumnIndex);

				//      clamp to not exceed beyond bottom side of the grid
				//      row_span > 0 is guaranteed by property value validation callback
				cell.RowSpan = Math.Min(GetRowSpan(child), DefinitionsV.Count - cell.RowIndex);

				Debug.Assert(0 <= cell.ColumnIndex && cell.ColumnIndex < DefinitionsU.Count);
				Debug.Assert(0 <= cell.RowIndex && cell.RowIndex < DefinitionsV.Count);

				//
				//  calculate and cache length types for the child
				//

				cell.SizeTypeU = GetLengthTypeForRange(DefinitionsU, cell.ColumnIndex, cell.ColumnSpan);
				cell.SizeTypeV = GetLengthTypeForRange(DefinitionsV, cell.RowIndex, cell.RowSpan);

				hasStarCellsU |= cell.IsStarU;
				hasStarCellsV |= cell.IsStarV;

				//
				//  distribute cells into four groups.
				//

				if (!cell.IsStarV)
				{
					if (!cell.IsStarU)
					{
						cell.Next = extData.CellGroup1;
						extData.CellGroup1 = i;
					}
					else
					{
						cell.Next = extData.CellGroup3;
						extData.CellGroup3 = i;

						//  remember if this cell belongs to auto row
						hasGroup3CellsInAutoRows |= cell.IsAutoV;
					}
				}
				else
				{
					if (cell.IsAutoU
					    //  note below: if spans through Star column it is NOT Auto
					    && !cell.IsStarU)
					{
						cell.Next = extData.CellGroup2;
						extData.CellGroup2 = i;
					}
					else
					{
						cell.Next = extData.CellGroup4;
						extData.CellGroup4 = i;
					}
				}

				PrivateCells[i] = cell;
			}

			HasStarCellsU = hasStarCellsU;
			HasStarCellsV = hasStarCellsV;
			HasGroup3CellsInAutoRows = hasGroup3CellsInAutoRows;
		}

		private void ValidateDefinitionsLayout<TDefinition>(GridDefinitionCollection<TDefinition> definitions, bool treatStarAsAuto) where TDefinition : GridPanelDefinition
		{
			for (var i = 0; i < definitions.Count; ++i)
			{
				definitions[i].OnBeforeLayout(this);

				var userMinSize = definitions[i].UserMinSize;
				var userMaxSize = definitions[i].UserMaxSize;
				double userSize = 0;

				switch (definitions[i].UserSize.UnitType)
				{
					case FlexLengthUnitType.Pixel:
						definitions[i].SizeType = LayoutTimeSizeType.Pixel;
						userSize = definitions[i].UserSize.Value;
						// this was brought with NewLayout and defeats squishy behavior
						userMinSize = Math.Max(userMinSize, Math.Min(userSize, userMaxSize));
						break;
					case FlexLengthUnitType.Auto:
						definitions[i].SizeType = LayoutTimeSizeType.Auto;
						userSize = double.PositiveInfinity;
						break;
					case FlexLengthUnitType.Star:
						if (treatStarAsAuto)
						{
							definitions[i].SizeType = LayoutTimeSizeType.Auto;
							userSize = double.PositiveInfinity;
						}
						else
						{
							definitions[i].SizeType = LayoutTimeSizeType.Star;
							userSize = double.PositiveInfinity;
						}

						break;
					default:
						Debug.Assert(false);
						break;
				}

				definitions[i].UpdateMinSize(userMinSize);
				definitions[i].MeasureSize = Math.Max(userMinSize, Math.Min(userSize, userMaxSize));
			}
		}

		/// <summary>
		///   Initializes DefinitionsU member either to user supplied ColumnDefinitions collection
		///   or to a default single element collection. DefinitionsU gets trimmed to size.
		/// </summary>
		/// <remarks>
		///   This is one of two methods, where ColumnDefinitions and DefinitionsU are directly accessed.
		///   All the rest measure / arrange / render code must use DefinitionsU.
		/// </remarks>
		private void ValidateDefinitionsUStructure()
		{
			if (ColumnDefinitionCollectionDirty)
			{
				var extData = ExtData;

				if (extData.ColumnDefinitions == null)
					extData.DefinitionsU ??= DefaultColumns;
				else
					extData.DefinitionsU = extData.ColumnDefinitions.Count == 0 ? DefaultColumns : extData.ColumnDefinitions;

				ColumnDefinitionCollectionDirty = false;
			}

			Debug.Assert(ExtData.DefinitionsU != null && ExtData.DefinitionsU.Count > 0);
		}

		/// <summary>
		///   Initializes DefinitionsV memeber either to user supplied RowDefinitions collection
		///   or to a default single element collection. DefinitionsV gets trimmed to size.
		/// </summary>
		/// <remarks>
		///   This is one of two methods, where RowDefinitions and DefinitionsV are directly accessed.
		///   All the rest measure / arrange / render code must use DefinitionsV.
		/// </remarks>
		private void ValidateDefinitionsVStructure()
		{
			if (RowDefinitionCollectionDirty)
			{
				var extData = ExtData;

				if (extData.RowDefinitions == null)
					extData.DefinitionsV ??= DefaultRows;
				else
					extData.DefinitionsV = extData.RowDefinitions.Count == 0 ? DefaultRows : extData.RowDefinitions;

				RowDefinitionCollectionDirty = false;
			}

			Debug.Assert(ExtData.DefinitionsV != null && ExtData.DefinitionsV.Count > 0);
		}

		private class ExtendedData
		{
			internal CellCache[] CellCachesCollection; //  backing store for logical children
			internal int CellGroup1; //  index of the first cell in first cell group
			internal int CellGroup2; //  index of the first cell in second cell group
			internal int CellGroup3; //  index of the first cell in third cell group
			internal int CellGroup4; //  index of the first cell in forth cell group
			internal GridColumnCollection ColumnDefinitions; //  collection of column definitions (logical tree support)
			internal GridColumnCollection DefinitionsU; //  collection of column definitions used during calc
			internal GridRowCollection DefinitionsV; //  collection of row definitions used during calc
			internal GridRowCollection RowDefinitions; //  collection of row definitions (logical tree support)

			internal GridPanelDefinition[] TempDefinitions; //  temporary array used during layout for various purposes
			//  TempDefinitions.Length == Max(definitionsU.Length, definitionsV.Length)
		}

		[Flags]
		private enum Flags
		{
			//
			//  the following flags let grid tracking dirtiness in more granular manner:
			//  * Valid???Structure flags indicate that elements were added or removed.
			//  * Valid???Layout flags indicate that layout time portion of the information
			//    stored on the objects should be updated.
			//
			ValidDefinitionsUStructure = 0x00000001,
			ValidDefinitionsVStructure = 0x00000002,
			ValidCellsStructure = 0x00000004,

			//
			//  boolean flags
			//
			ListenToNotifications = 0x00001000, //  "0" when all notifications are ignored
			SizeToContentU = 0x00002000, //  "1" if calculating to content in U direction
			SizeToContentV = 0x00004000, //  "1" if calculating to content in V direction
			HasStarCellsU = 0x00008000, //  "1" if at least one cell belongs to a Star column
			HasStarCellsV = 0x00010000, //  "1" if at least one cell belongs to a Star row
			HasGroup3CellsInAutoRows = 0x00020000, //  "1" if at least one cell of group 3 belongs to an Auto row
			MeasureOverrideInProgress = 0x00040000, //  "1" while in the context of Grid.MeasureOverride
			ArrangeOverrideInProgress = 0x00080000, //  "1" while in the context of Grid.ArrangeOverride
		}

		[Flags]
		internal enum LayoutTimeSizeType : byte
		{
			None = 0x00,
			Pixel = 0x01,
			Auto = 0x02,
			Star = 0x04,
		}

		private struct CellCache
		{
			internal int ColumnIndex;
			internal int RowIndex;
			internal int ColumnSpan;
			internal int RowSpan;
			internal LayoutTimeSizeType SizeTypeU;
			internal LayoutTimeSizeType SizeTypeV;
			internal int Next;
			internal bool IsStarU => (SizeTypeU & LayoutTimeSizeType.Star) != 0;
			internal bool IsAutoU => (SizeTypeU & LayoutTimeSizeType.Auto) != 0;
			internal bool IsStarV => (SizeTypeV & LayoutTimeSizeType.Star) != 0;
			internal bool IsAutoV => (SizeTypeV & LayoutTimeSizeType.Auto) != 0;
		}

		private sealed class SpanKey
		{
			internal SpanKey(int start, int count, bool u)
			{
				Start = start;
				Count = count;
				U = u;
			}

			internal int Count { get; }

			internal int Start { get; }

			internal bool U { get; }

			public override bool Equals(object obj)
			{
				return obj is SpanKey sk
				       && sk.Start == Start
				       && sk.Count == Count
				       && sk.U == U;
			}

			public override int GetHashCode()
			{
				var hash = Start ^ (Count << 2);

				if (U)
					hash &= 0x7ffffff;
				else
					hash |= 0x8000000;

				return hash;
			}
		}

		private sealed class SpanPreferredDistributionOrderComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var definitionX = x as GridPanelDefinition;
				var definitionY = y as GridPanelDefinition;

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				if (definitionX.UserSize.IsAuto)
				{
					if (definitionY.UserSize.IsAuto)
						result = definitionX.MinSize.CompareTo(definitionY.MinSize);
					else
						result = -1;
				}
				else
				{
					if (definitionY.UserSize.IsAuto)
						result = +1;
					else
						result = definitionX.PreferredSize.CompareTo(definitionY.PreferredSize);
				}

				return result;
			}
		}

		private sealed class SpanMaxDistributionOrderComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var definitionX = x as GridPanelDefinition;
				var definitionY = y as GridPanelDefinition;

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				if (definitionX.UserSize.IsAuto)
				{
					if (definitionY.UserSize.IsAuto)
						result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
					else
						result = +1;
				}
				else
				{
					if (definitionY.UserSize.IsAuto)
						result = -1;
					else
						result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);
				}

				return result;
			}
		}

		private sealed class RoundingErrorIndexComparer : IComparer
		{
			private readonly double[] _errors;

			internal RoundingErrorIndexComparer(double[] errors)
			{
				Debug.Assert(errors != null);

				_errors = errors;
			}

			public int Compare(object x, object y)
			{
				var indexX = x as int?;
				var indexY = y as int?;

				if (CompareNullRefs(indexX, indexY, out var result))
					return result;

				var errorX = _errors[indexX.Value];
				var errorY = _errors[indexY.Value];

				result = errorX.CompareTo(errorY);

				return result;
			}
		}

		private sealed class MinRatioComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var definitionX = x as GridPanelDefinition;
				var definitionY = y as GridPanelDefinition;

				if (CompareNullRefs(definitionY, definitionX, out var result))
					return result;

				result = definitionY.MeasureSize.CompareTo(definitionX.MeasureSize);

				return result;
			}
		}

		private sealed class MaxRatioComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var definitionX = x as GridPanelDefinition;
				var definitionY = y as GridPanelDefinition;

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);

				return result;
			}
		}

		private sealed class StarWeightComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				var definitionX = x as GridPanelDefinition;
				var definitionY = y as GridPanelDefinition;

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				result = definitionX.MeasureSize.CompareTo(definitionY.MeasureSize);

				return result;
			}
		}

		private sealed class MinRatioIndexComparer<TDefinition> : IComparer where TDefinition : GridPanelDefinition
		{
			private readonly GridDefinitionCollection<TDefinition> _definitions;

			internal MinRatioIndexComparer(GridDefinitionCollection<TDefinition> definitions)
			{
				Debug.Assert(definitions != null);

				_definitions = definitions;
			}

			public int Compare(object x, object y)
			{
				var indexX = x as int?;
				var indexY = y as int?;

				GridPanelDefinition definitionX = null;
				GridPanelDefinition definitionY = null;

				if (indexX != null)
					definitionX = _definitions[indexX.Value];

				if (indexY != null)
					definitionY = _definitions[indexY.Value];

				if (CompareNullRefs(definitionY, definitionX, out var result))
					return result;

				result = definitionY.MeasureSize.CompareTo(definitionX.MeasureSize);

				return result;
			}
		}

		private sealed class MaxRatioIndexComparer<TDefinition> : IComparer where TDefinition : GridPanelDefinition
		{
			private readonly GridDefinitionCollection<TDefinition> _definitions;

			internal MaxRatioIndexComparer(GridDefinitionCollection<TDefinition> definitions)
			{
				Debug.Assert(definitions != null);

				_definitions = definitions;
			}

			public int Compare(object x, object y)
			{
				var indexX = x as int?;
				var indexY = y as int?;

				GridPanelDefinition definitionX = null;
				GridPanelDefinition definitionY = null;

				if (indexX != null)
					definitionX = _definitions[indexX.Value];

				if (indexY != null)
					definitionY = _definitions[indexY.Value];

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				result = definitionX.SizeCache.CompareTo(definitionY.SizeCache);

				return result;
			}
		}

		private sealed class StarWeightIndexComparer<TDefinition> : IComparer where TDefinition : GridPanelDefinition
		{
			private readonly GridDefinitionCollection<TDefinition> _definitions;

			internal StarWeightIndexComparer(GridDefinitionCollection<TDefinition> definitions)
			{
				Debug.Assert(definitions != null);

				_definitions = definitions;
			}

			public int Compare(object x, object y)
			{
				var indexX = x as int?;
				var indexY = y as int?;

				GridPanelDefinition definitionX = null;
				GridPanelDefinition definitionY = null;

				if (indexX != null)
					definitionX = _definitions[indexX.Value];

				if (indexY != null)
					definitionY = _definitions[indexY.Value];

				if (CompareNullRefs(definitionX, definitionY, out var result))
					return result;

				result = definitionX.MeasureSize.CompareTo(definitionY.MeasureSize);

				return result;
			}
		}
	}
}