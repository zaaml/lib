// <copyright file="RangeControlBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.Core.Packed;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
	public abstract class RangeControlBase : TemplateContractControl
	{
		private static readonly PackedHierarchicalAllocator<RangeControlBase> TypeAllocator = new();

		public static readonly DependencyProperty MinimumProperty = DPM.Register<double, RangeControlBase>
			("Minimum", 0.0, r => r.OnMinimumChangedPrivate, r => r.CoerceMinimum);

		public static readonly DependencyProperty MaximumProperty = DPM.Register<double, RangeControlBase>
			("Maximum", 1.0, r => r.OnMaximumChangedPrivate, r => r.CoerceMaximum);

		private protected byte PackedValue;

		private protected bool Initializing
		{
			get => PackedDefinition.Initializing.GetValue(PackedValue);
			private set => PackedDefinition.Initializing.SetValue(ref PackedValue, value);
		}

		public double Maximum
		{
			get => (double)GetValue(MaximumProperty);
			set => SetValue(MaximumProperty, value);
		}

		public double Minimum
		{
			get => (double)GetValue(MinimumProperty);
			set => SetValue(MinimumProperty, value);
		}

		public bool Updating { get; private set; }

		public override void BeginInit()
		{
			base.BeginInit();

			Initializing = true;

			BeginInitCore();
		}

		protected virtual void BeginInitCore()
		{
		}

		public void BeginUpdate()
		{
			if (Updating)
				throw new InvalidOperationException("Already in update state");

			Updating = true;
		}

		protected virtual void BeginUpdateCore()
		{
		}

		private double CoerceMaximum(double value)
		{
			if (Updating || Initializing)
				return value;

			return value < Minimum ? Minimum : value;
		}

		private double CoerceMinimum(double value)
		{
			if (Updating || Initializing)
				return value;

			return value > Maximum ? Maximum : value;
		}


		public override void EndInit()
		{
			Initializing = false;

			EndInitCore();

			base.EndInit();
		}

		protected virtual void EndInitCore()
		{
		}

		public void EndUpdate()
		{
			if (Updating == false)
				throw new InvalidOperationException("Not in update state");

			Updating = false;

			EndUpdateCore();
		}

		protected virtual void EndUpdateCore()
		{
		}

		private protected static PackedValueAllocator GetAllocator<T>() where T : RangeControlBase
		{
			return TypeAllocator.GetAllocator<T>();
		}

		protected virtual void OnMaximumChanged(double oldValue, double newValue)
		{
		}

		private void OnMaximumChangedPrivate(double oldValue, double newValue)
		{
			OnMaximumChanged(oldValue, newValue);
		}

		protected virtual void OnMinimumChanged(double oldValue, double newValue)
		{
		}

		private void OnMinimumChangedPrivate(double oldValue, double newValue)
		{
			OnMinimumChanged(oldValue, newValue);
		}

		private static class PackedDefinition
		{
			public static readonly PackedBoolItemDefinition Initializing;

			static PackedDefinition()
			{
				var allocator = GetAllocator<RangeControlBase>();

				Initializing = allocator.AllocateBoolItem();
			}
		}
	}
}