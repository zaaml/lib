// <copyright file="GridCellGeneratorSourceProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal class GridViewCellGeneratorSourceProperty<TGridCell, TProperty> : GridViewCellGeneratorSourceProperty<TGridCell>
		where TGridCell : GridViewCell
	{
		private TProperty _value;

		public GridViewCellGeneratorSourceProperty(GridViewCellGeneratorTargetProperty<TGridCell> generatorProperty) : this(generatorProperty, null)
		{
		}

		public GridViewCellGeneratorSourceProperty(GridViewCellGeneratorTargetProperty<TGridCell> generatorProperty, IEqualityComparer<TProperty> equalityComparer) : base(generatorProperty)
		{
			Comparer = equalityComparer ?? EqualityComparer<TProperty>.Default;
		}

		protected virtual IEqualityComparer<TProperty> Comparer { get; }

		private GridViewCellGeneratorTargetProperty<TGridCell, TProperty> ConcreteProperty => (GridViewCellGeneratorTargetProperty<TGridCell, TProperty>)GeneratorProperty;

		public TProperty Value
		{
			get => _value;
			set
			{
				if (Comparer.Equals(_value, value))
					return;

				var oldValue = _value;

				OnValueChanging(oldValue, value);

				_value = value;

				OnValueChanged(oldValue, value);
			}
		}

		protected virtual void OnValueChanged(TProperty oldValue, TProperty newValue)
		{
			GeneratorProperty.OnValueChangedInternal(this);
		}

		protected virtual void OnValueChanging(TProperty oldValue, TProperty newValue)
		{
			GeneratorProperty.OnValueChangingInternal(this);
		}
	}

	internal abstract class GridViewCellGeneratorSourceProperty<TGridCell> where TGridCell : GridViewCell
	{
		protected GridViewCellGeneratorSourceProperty(GridViewCellGeneratorTargetProperty<TGridCell> generatorProperty)
		{
			GeneratorProperty = generatorProperty;
		}

		public GridViewCellGeneratorTargetProperty<TGridCell> GeneratorProperty { get; }
	}
}