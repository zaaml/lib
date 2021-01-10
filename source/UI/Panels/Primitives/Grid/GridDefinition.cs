// <copyright file="GridDefinition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Panels.Primitives
{
	public abstract class GridDefinition : InheritanceContextObject
	{
		internal const bool ThisIsColumnDefinition = true;
		internal const bool ThisIsRowDefinition = false;

		internal static readonly DependencyProperty PrivateSharedSizeScopeProperty = DPM.RegisterAttached<SharedSizeScope, GridDefinition>
			("PrivateSharedSizeScope", new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static readonly DependencyProperty SharedSizeGroupProperty = DPM.Register<string, GridDefinition>
			("SharedSizeGroup", OnSharedSizeGroupPropertyChanged, SharedSizeGroupPropertyValueValid);

		private readonly bool _isColumnDefinition;
		private Flags _flags;

		private SharedSizeState _sharedState;

		private GridPanel.LayoutTimeSizeType _sizeType;

		static GridDefinition()
		{
			PrivateSharedSizeScopeProperty.OverrideMetadata(typeof(GridDefinition), new FrameworkPropertyMetadata(OnPrivateSharedSizeScopePropertyChanged));
		}

		internal GridDefinition(bool isColumnDefinition)
		{
			_isColumnDefinition = isColumnDefinition;
			Index = -1;
		}

		internal double FinalOffset { get; set; }

		internal GridPanel GridPanel { get; set; }

		internal int Index { get; set; }

		internal bool InParentLogicalTree => Index != -1;

		internal bool IsShared => _sharedState != null;

		private bool LayoutWasUpdated
		{
			get => CheckFlagsAnd(Flags.LayoutWasUpdated);
			set => SetFlags(value, Flags.LayoutWasUpdated);
		}

		internal double MeasureSize { get; set; }

		internal double MinSize
		{
			get
			{
				var minSize = RawMinSize;

				if (UseSharedMinimum && _sharedState != null && minSize < _sharedState.MinSize)
					minSize = _sharedState.MinSize;

				return minSize;
			}
		}

		internal double MinSizeForArrange
		{
			get
			{
				var minSize = RawMinSize;

				if (_sharedState != null && (UseSharedMinimum || !LayoutWasUpdated) && minSize < _sharedState.MinSize)
					minSize = _sharedState.MinSize;

				return minSize;
			}
		}

		internal double PreferredSize
		{
			get
			{
				var preferredSize = MinSize;

				if (_sizeType != GridPanel.LayoutTimeSizeType.Auto && preferredSize < MeasureSize)
					preferredSize = MeasureSize;

				return preferredSize;
			}
		}

		private SharedSizeScope PrivateSharedSizeScope => (SharedSizeScope) GetValue(PrivateSharedSizeScopeProperty);


		internal double RawMinSize { get; private set; }

		public string SharedSizeGroup
		{
			get => (string) GetValue(SharedSizeGroupProperty);
			set => SetValue(SharedSizeGroupProperty, value);
		}

		internal double SizeCache { get; set; }

		internal GridPanel.LayoutTimeSizeType SizeType
		{
			get => _sizeType;
			set => _sizeType = value;
		}

		internal double UserMaxSize => UserMaxSizeValueCache;

		internal double UserMaxSizeValueCache => (double) GetValue(_isColumnDefinition ? GridColumn.MaxWidthProperty : GridRow.MaxHeightProperty);

		internal double UserMinSize => UserMinSizeValueCache;

		internal double UserMinSizeValueCache => (double) GetValue(_isColumnDefinition ? GridColumn.MinWidthProperty : GridRow.MinHeightProperty);

		internal FlexLength UserSize => _sharedState?.UserSize ?? UserSizeValueCache;

		internal FlexLength UserSizeValueCache => (FlexLength) GetValue(_isColumnDefinition ? GridColumn.WidthProperty : GridRow.HeightProperty);

		private bool UseSharedMinimum
		{
			get => CheckFlagsAnd(Flags.UseSharedMinimum);
			set => SetFlags(value, Flags.UseSharedMinimum);
		}

		private bool CheckFlagsAnd(Flags flags)
		{
			return (_flags & flags) == flags;
		}

		protected void Invalidate()
		{
			GridPanel?.Invalidate();
		}

		private static void InvalidateGridPanelMeasure(DependencyObject d)
		{
			var definition = (GridDefinition) d;

			if (definition.InParentLogicalTree == false)
				return;

			definition.GridPanel?.InvalidateMeasure();
		}

		internal static bool IsUserMaxSizePropertyValueValid(object value)
		{
			var v = (double) value;

			return !DoubleUtils.IsNaN(v) && v >= 0.0d;
		}

		internal static bool IsUserMinSizePropertyValueValid(object value)
		{
			var v = (double) value;

			return !DoubleUtils.IsNaN(v) && v >= 0.0d && !double.IsPositiveInfinity(v);
		}

		internal static bool IsUserSizePropertyValueValid(object value)
		{
			return ((FlexLength) value).Value >= 0;
		}

		internal void OnBeforeLayout(GridPanel grid)
		{
			RawMinSize = 0;
			LayoutWasUpdated = true;

			_sharedState?.EnsureDeferredValidation(grid);
		}

		internal void OnEnterParentTree()
		{
			if (_sharedState != null)
				return;

			var sharedSizeGroupId = SharedSizeGroup;

			if (sharedSizeGroupId == null)
				return;

			var privateSharedSizeScope = PrivateSharedSizeScope;

			if (privateSharedSizeScope == null)
				return;

			_sharedState = privateSharedSizeScope.EnsureSharedState(sharedSizeGroupId);
			_sharedState.AddMember(this);
		}

		internal void OnExitParentTree()
		{
			FinalOffset = 0;

			if (_sharedState == null)
				return;

			_sharedState.RemoveMember(this);
			_sharedState = null;
		}

		internal static void OnIsSharedSizeScopePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if ((bool) e.NewValue)
			{
				var sharedStatesCollection = new SharedSizeScope();

				d.SetValue(PrivateSharedSizeScopeProperty, sharedStatesCollection);
			}
			else
				d.ClearValue(PrivateSharedSizeScopeProperty);
		}

		private static void OnPrivateSharedSizeScopePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var definition = (GridDefinition) d;

			if (definition.InParentLogicalTree == false)
				return;

			var privateSharedSizeScope = (SharedSizeScope) e.NewValue;

			if (definition._sharedState != null)
			{
				//  if definition is already registered And shared size scope is changing,
				//  then un-register the definition from the current shared size state object.
				definition._sharedState.RemoveMember(definition);
				definition._sharedState = null;
			}

			if (definition._sharedState != null || privateSharedSizeScope == null)
				return;

			var sharedSizeGroup = definition.SharedSizeGroup;

			if (sharedSizeGroup == null)
				return;

			//  if definition is not registered and both: shared size group id AND private shared scope
			//  are available, then register definition.
			definition._sharedState = privateSharedSizeScope.EnsureSharedState(definition.SharedSizeGroup);
			definition._sharedState.AddMember(definition);
		}

		private static void OnSharedSizeGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var definition = (GridDefinition) d;

			if (definition.InParentLogicalTree == false)
				return;

			var sharedSizeGroupId = (string) e.NewValue;

			if (definition._sharedState != null)
			{
				//  if definition is already registered AND shared size group id is changing,
				//  then un-register the definition from the current shared size state object.
				definition._sharedState.RemoveMember(definition);
				definition._sharedState = null;
			}

			if (definition._sharedState != null || sharedSizeGroupId == null)
				return;

			var privateSharedSizeScope = definition.PrivateSharedSizeScope;

			if (privateSharedSizeScope == null)
				return;

			//  if definition is not registered and both: shared size group id AND private shared scope
			//  are available, then register definition.
			definition._sharedState = privateSharedSizeScope.EnsureSharedState(sharedSizeGroupId);
			definition._sharedState.AddMember(definition);
		}

		internal static void OnUserMaxSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InvalidateGridPanelMeasure(d);
		}

		internal static void OnUserMinSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			InvalidateGridPanelMeasure(d);
		}

		internal static void OnUserSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var definition = (GridDefinition) d;

			if (definition.InParentLogicalTree == false)
				return;

			if (definition._sharedState != null)
				definition._sharedState.Invalidate();
			else
			{
				var parentGrid = definition.GridPanel;

				if (((FlexLength) e.OldValue).UnitType != ((FlexLength) e.NewValue).UnitType)
					parentGrid.Invalidate();
				else
					parentGrid.InvalidateMeasure();
			}
		}

		private void SetFlags(bool value, Flags flags)
		{
			_flags = value ? _flags | flags : _flags & ~flags;
		}

		internal void SetMinSize(double minSize)
		{
			RawMinSize = minSize;
		}

		private static object SharedSizeGroupPropertyValueValid(DependencyObject dependencyObject, object value)
		{
			if (value == null)
				return true;

			var id = (string) value;

			if (id == string.Empty)
				return false;

			var i = -1;

			while (++i < id.Length)
			{
				var isDigit = char.IsDigit(id[i]);

				if (i == 0 && isDigit || !(isDigit || char.IsLetter(id[i]) || '_' == id[i]))
					break;
			}

			if (i != id.Length)
				throw new InvalidOperationException("Invalid value");

			return value;
		}

		internal void UpdateMinSize(double minSize)
		{
			RawMinSize = Math.Max(RawMinSize, minSize);
		}

		[Flags]
		private enum Flags : byte
		{
			UseSharedMinimum = 0x00000020, //  when "1", definition will take into account shared state's minimum
			LayoutWasUpdated = 0x00000040, //  set to "1" every time the parent grid is measured
		}

		private class SharedSizeScope
		{
			private readonly Hashtable _registry = new Hashtable();

			internal SharedSizeState EnsureSharedState(string sharedSizeGroup)
			{
				Debug.Assert(sharedSizeGroup != null);

				if (_registry[sharedSizeGroup] is SharedSizeState sharedState)
					return sharedState;

				sharedState = new SharedSizeState(this, sharedSizeGroup);
				_registry[sharedSizeGroup] = sharedState;

				return sharedState;
			}

			internal void Remove(object key)
			{
				Debug.Assert(_registry.Contains(key));

				_registry.Remove(key);
			}
		}

		private class SharedSizeState
		{
			private readonly EventHandler _layoutUpdated; //  instance event handler for layout updated event
			private readonly List<GridDefinition> _registry; //  registry of participating definitions
			private readonly string _sharedSizeGroupId; //  Id of the shared size group this object is servicing

			private readonly SharedSizeScope _sharedSizeScope; //  the scope this state belongs to
			private bool _broadcastInvalidation; //  "true" when broadcasting of invalidation is needed
			private UIElement _layoutUpdatedHost; //  UIElement for which layout updated event handler is registered
			private double _minSize; //  shared state
			private FlexLength _userSize; //  shared state
			private bool _userSizeValid; //  "true" when _userSize is up to date

			internal SharedSizeState(SharedSizeScope sharedSizeScope, string sharedSizeGroupId)
			{
				Debug.Assert(sharedSizeScope != null && sharedSizeGroupId != null);
				_sharedSizeScope = sharedSizeScope;
				_sharedSizeGroupId = sharedSizeGroupId;
				_registry = new List<GridDefinition>();
				_layoutUpdated = OnLayoutUpdated;
				_broadcastInvalidation = true;
			}

			internal double MinSize
			{
				get
				{
					if (_userSizeValid == false)
						EnsureUserSizeValid();

					return _minSize;
				}
			}

			internal FlexLength UserSize
			{
				get
				{
					if (!_userSizeValid)
						EnsureUserSizeValid();

					return _userSize;
				}
			}

			internal void AddMember(GridDefinition member)
			{
				Debug.Assert(!_registry.Contains(member));

				_registry.Add(member);

				Invalidate();
			}


			internal void EnsureDeferredValidation(UIElement layoutUpdatedHost)
			{
				if (_layoutUpdatedHost != null)
					return;

				_layoutUpdatedHost = layoutUpdatedHost;

				_layoutUpdatedHost.LayoutUpdated += _layoutUpdated;
			}

			private void EnsureUserSizeValid()
			{
				_userSize = new FlexLength(1, FlexLengthUnitType.Auto);

				for (int i = 0, count = _registry.Count; i < count; ++i)
				{
					Debug.Assert(_userSize.UnitType == FlexLengthUnitType.Auto || _userSize.UnitType == FlexLengthUnitType.Pixel);

					var currentLength = _registry[i].UserSizeValueCache;

					if (currentLength.UnitType != FlexLengthUnitType.Pixel)
						continue;

					if (_userSize.UnitType == FlexLengthUnitType.Auto)
						_userSize = currentLength;
					else if (_userSize.Value < currentLength.Value)
						_userSize = currentLength;
				}

				//  taking maximum with user size effectively prevents squishy-ness.
				//  this is a "solution" to avoid shared definitions from been sized to
				//  different final size at arrange time, if / when different grids receive
				//  different final sizes.
				_minSize = _userSize.IsAbsolute ? _userSize.Value : 0.0;

				_userSizeValid = true;
			}

			internal void Invalidate()
			{
				_userSizeValid = false;

				if (_broadcastInvalidation == false)
					return;

				for (int i = 0, count = _registry.Count; i < count; ++i)
				{
					var parentGrid = _registry[i].GridPanel;

					parentGrid.Invalidate();
				}

				_broadcastInvalidation = false;
			}

			private void OnLayoutUpdated(object sender, EventArgs e)
			{
				double sharedMinSize = 0;

				//  accumulate min size of all participating definitions
				for (int i = 0, count = _registry.Count; i < count; ++i)
					sharedMinSize = Math.Max(sharedMinSize, _registry[i].RawMinSize);

				var sharedMinSizeChanged = !DoubleUtils.AreClose(_minSize, sharedMinSize);

				//  compare accumulated min size with min sizes of the individual definitions
				for (int i = 0, count = _registry.Count; i < count; ++i)
				{
					var definitionBase = _registry[i];

					// we'll set d.UseSharedMinimum to maintain the invariant:
					//      d.UseSharedMinimum iff d._minSize < this.MinSize
					// i.e. iff d is not a "long-pole" definition.
					//
					// Measure/Arrange of d's Grid uses d._minSize for long-pole
					// definitions, and max(d._minSize, shared size) for
					// short-pole definitions.  This distinction allows us to react
					// to changes in "long-pole-ness" more efficiently and correctly,
					// by avoiding remeasures when a long-pole definition changes.
					var useSharedMinimum = !DoubleUtils.AreClose(definitionBase.RawMinSize, sharedMinSize);

					// before doing that, determine whether d's Grid needs to be remeasured.
					// It's important _not_ to remeasure if the last measure is still
					// valid, otherwise infinite loops are possible
					bool measureIsValid;

					if (definitionBase.UseSharedMinimum == false)
					{
						// d was a long-pole.  measure is valid iff it's still a long-pole,
						// since previous measure didn't use shared size.
						measureIsValid = !useSharedMinimum;
					}
					else if (useSharedMinimum)
					{
						// d was a short-pole, and still is.  measure is valid
						// iff the shared size didn't change
						measureIsValid = !sharedMinSizeChanged;
					}
					else
					{
						// d was a short-pole, but is now a long-pole.  This can
						// happen in several ways:
						//  a. d's minSize increased to or past the old shared size
						//  b. other long-pole definitions decreased, leaving
						//      d as the new winner
						// In the former case, the measure is valid - it used
						// d's new larger minSize.  In the latter case, the
						// measure is invalid - it used the old shared size,
						// which is larger than d's (possibly changed) minSize
						measureIsValid = definitionBase.LayoutWasUpdated &&
						                 DoubleUtils.GreaterThanOrClose(definitionBase.RawMinSize, MinSize);
					}

					if (!measureIsValid)
					{
						var parentGrid = definitionBase.GridPanel;

						parentGrid.InvalidateMeasure();
					}
					else if (!DoubleUtils.AreClose(sharedMinSize, definitionBase.SizeCache))
					{
						//  if measure is valid then also need to check arrange.
						//  Note: definitionBase.SizeCache is volatile but at this point
						//  it contains up-to-date final size
						var parentGrid = definitionBase.GridPanel;

						parentGrid.InvalidateArrange();
					}

					// now we can restore the invariant, and clear the layout flag
					definitionBase.UseSharedMinimum = useSharedMinimum;
					definitionBase.LayoutWasUpdated = false;
				}

				_minSize = sharedMinSize;

				_layoutUpdatedHost.LayoutUpdated -= _layoutUpdated;
				_layoutUpdatedHost = null;

				_broadcastInvalidation = true;
			}

			internal void RemoveMember(GridDefinition member)
			{
				Invalidate();
				_registry.Remove(member);

				if (_registry.Count == 0)
					_sharedSizeScope.Remove(_sharedSizeGroupId);
			}
		}
	}
}