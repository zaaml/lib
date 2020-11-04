// <copyright file="DisableVeilControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Zaaml.Core.Packed;
using Zaaml.Core.Weak.Collections;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Interfaces;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.PropertyCore.Extensions;
using Zaaml.PresentationCore.Theming;
using Zaaml.PresentationCore.Utils;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Core
{
	public sealed class DisableVeilControl : Control, IPanelAdvisor
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty EnableVeilProperty = DPM.RegisterAttached<bool, DisableVeilControl>
			("EnableVeil", true);

		private static readonly Binding EnableVeilBinding = new Binding {Path = new PropertyPath(EnableVeilProperty), RelativeSource = XamlConstants.TemplatedParent, FallbackValue = true};
		private static readonly Binding IsParentEnabledBinding = new Binding {Path = new PropertyPath(IsEnabledProperty), RelativeSource = XamlConstants.TemplatedParent, FallbackValue = true};

		private static readonly DependencyPropertyKey ActualVeilBrushPropertyKey = DPM.RegisterAttachedReadOnly<Brush, DisableVeilControl>
			("ActualVeilBrush");

		public static readonly DependencyProperty ActualVeilBrushProperty = ActualVeilBrushPropertyKey.DependencyProperty;

		private static readonly DependencyProperty EnableVeilIntProperty = DPM.Register<bool, DisableVeilControl>
			("EnableVeilInt", true, v => v.OnVeilStateDirty);

		private static readonly DependencyProperty IsParentEnabledProperty = DPM.Register<bool, DisableVeilControl>
			("IsParentEnabled", true, v => v.OnVeilStateDirty);

		public static readonly DependencyProperty VeilBrushProperty = DPM.RegisterAttached<Brush, DisableVeilControl>
			("VeilBrush", OnVeilBrushChanged);

		private static readonly WeakLinkedList<DisableVeilControl> VisibleVeils = new WeakLinkedList<DisableVeilControl>();

#if SILVERLIGHT
		private static readonly DependencyProperty BackgroundIntProperty = DPM.Register<Brush, DisableVeilControl>
			("BackgroundInt", v => v.OnBackgroundChanged);
#endif

		public static readonly DependencyProperty FallbackVeilBrushProperty = DPM.Register<Brush, DisableVeilControl>
			("FallbackVeilBrush");

		#endregion

		#region Fields

		private byte _packedValue;
		private WeakLinkedNode<DisableVeilControl> _visibleNode;

		#endregion

		#region Ctors

		static DisableVeilControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<DisableVeilControl>();
		}

		public DisableVeilControl()
		{
			this.OverrideStyleKey<DisableVeilControl>();

#if SILVERLIGHT
			SetBinding(BackgroundIntProperty, new Binding { Path = new PropertyPath(BackgroundProperty), Source = this });
#endif

			IsHitTestVisible = false;
			Focusable = false;
			IsTabStop = false;

			Visibility = Visibility.Collapsed;
		}

		#endregion

		#region Properties

		private bool ActualShouldShowVeil => (IsEnabled == false || IsParentEnabled == false) && this.GetValue<bool>(EnableVeilIntProperty);

		private bool ActualShowVeil
		{
			get => ActualShowVeilField;
			set
			{
				if (ActualShowVeilField == value)
					return;

				if (_visibleNode != null)
					VisibleVeils.Remove(_visibleNode);

				_visibleNode = null;
				ActualShowVeilField = value;

				if (ActualShowVeilField)
					_visibleNode = VisibleVeils.Add(this);

				ActualVeilBrush = GetActualVeilBrush();

				Visibility = ActualShouldShowVeil.ToVisibility();
			}
		}

		private bool ActualShowVeilField
		{
			get => PackedDefinition.ActualShowVeil.GetValue(_packedValue);
			set => PackedDefinition.ActualShowVeil.SetValue(ref _packedValue, value);
		}

		public Brush ActualVeilBrush
		{
			get => (Brush) GetValue(ActualVeilBrushProperty);
			private set => this.SetReadOnlyValue(ActualVeilBrushPropertyKey, value);
		}

		public Brush FallbackVeilBrush
		{
			get => (Brush) GetValue(FallbackVeilBrushProperty);
			set => SetValue(FallbackVeilBrushProperty, value);
		}

		private bool IsMeasured
		{
			get => PackedDefinition.IsMeasured.GetValue(_packedValue);
			set => PackedDefinition.IsMeasured.SetValue(ref _packedValue, value);
		}

		private bool IsParentEnabled => this.GetValue<bool>(IsParentEnabledProperty);

		private static TreeEnumerationStrategy LookupBrushStrategy => MixedTreeEnumerationStrategy.DisconnectedThenVisualThenLogicalInstance;

		public Brush VeilBrush
		{
			get => (Brush) GetValue(VeilBrushProperty);
			set => SetValue(VeilBrushProperty, value);
		}

		#endregion

		#region  Methods

		protected override Size ArrangeOverride(Size finalSize)
		{
			UpdateVeil(true);

			return base.ArrangeOverride(finalSize);
		}

		private Brush GetActualVeilBrush()
		{
			var actualVeilBrush = this.GetAncestors(LookupBrushStrategy).Select(GetVeilBrush).FirstOrDefault(b => b != null);
			return actualVeilBrush ?? FallbackVeilBrush ?? Background;
		}

		public static bool GetEnableVeil(DependencyObject element)
		{
			return (bool) element.GetValue(EnableVeilProperty);
		}

		public static Brush GetVeilBrush(DependencyObject element)
		{
			return (Brush) element.GetValue(VeilBrushProperty);
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			IsMeasured = true;

			UpdateVeil(true);

			return base.MeasureOverride(availableSize);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			EnsureBindings();

			UpdateVeil(false);
		}

#if !SILVERLIGHT
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);

			if (e.Property == BackgroundProperty)
				OnBackgroundChanged();
		}
#endif

		private void OnBackgroundChanged()
		{
			ActualVeilBrush = GetActualVeilBrush();
		}

		protected override void OnIsEnabledChanged()
		{
			base.OnIsEnabledChanged();

			UpdateVeil(true);
		}

		protected override void OnLoaded()
		{
			base.OnLoaded();

			var actualShow = ActualShowVeil;

			UpdateVeil(true);

			if (ActualShowVeil == actualShow && actualShow)
				ActualVeilBrush = GetActualVeilBrush();
		}

		private static void OnVeilBrushChanged(DependencyObject dependencyObject)
		{
			var veilBrush = GetVeilBrush(dependencyObject);

			foreach (var veilControl in VisibleVeils)
				if (veilControl.IsDescendantOf(dependencyObject, LookupBrushStrategy))
					veilControl.UpdateVeilBrush(veilBrush);
		}

		private void OnVeilStateDirty()
		{
			UpdateVeil(true);

			if (IsMeasured) 
				return;

			InvalidateMeasure();
			(Parent as Panel)?.InvalidateMeasure();
		}

		public static void SetEnableVeil(DependencyObject element, bool value)
		{
			element.SetValue(EnableVeilProperty, value);
		}

		public static void SetVeilBrush(DependencyObject element, Brush value)
		{
			element.SetValue(VeilBrushProperty, value);
		}

		private void UpdateVeil(bool useTransitions)
		{
			ActualShowVeil = ActualShouldShowVeil;

			UpdateVisualState(useTransitions);
		}

		private void UpdateVeilBrush(Brush veilBrush)
		{
			ActualVeilBrush = veilBrush ?? FallbackVeilBrush ?? Background;
		}

		protected override void UpdateVisualState(bool useTransitions)
		{
			GotoVisualState(ActualShouldShowVeil ? "Disabled" : "Normal", useTransitions);
		}

		#endregion

		#region Interface Implementations

		#region IPanelAdvisor

		bool IPanelAdvisor.ShouldMeasure
		{
			get
			{
				EnsureBindings();

				return ActualShouldShowVeil;
			}
		}

		private void EnsureBindings()
		{
			if (this.HasLocalValue(EnableVeilIntProperty) == false)
				SetBinding(EnableVeilIntProperty, EnableVeilBinding);

			if (this.HasLocalValue(IsParentEnabledProperty) == false)
				SetBinding(IsParentEnabledProperty, IsParentEnabledBinding);
		}

		bool IPanelAdvisor.ShouldArrange => IsMeasured;

		#endregion

		#endregion

		#region  Nested Types

		private static class PackedDefinition
		{
			#region Static Fields and Constants

			public static readonly PackedBoolItemDefinition ActualShowVeil;
			public static readonly PackedBoolItemDefinition IsMeasured;

			#endregion

			#region Ctors

			static PackedDefinition()
			{
				var allocator = new PackedValueAllocator();

				ActualShowVeil = allocator.AllocateBoolItem();
				IsMeasured = allocator.AllocateBoolItem();
			}

			#endregion
		}

		#endregion
	}
}