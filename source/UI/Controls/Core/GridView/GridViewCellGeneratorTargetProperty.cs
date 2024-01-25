// <copyright file="GridCellGeneratorTargetProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core.GridView
{
	internal abstract class GridViewCellGeneratorTargetProperty<TGridCell, TProperty> : GridViewCellGeneratorTargetProperty<TGridCell>
		where TGridCell : GridViewCell
	{
		protected GridViewCellGeneratorTargetProperty(GridViewCellGenerator<TGridCell> generator) : base(generator)
		{
		}


		protected virtual TProperty ValueCore => default;
	}

	internal abstract class GridViewCellGeneratorTargetProperty<TGridCell> where TGridCell : GridViewCell
	{
		protected GridViewCellGeneratorTargetProperty(GridViewCellGenerator<TGridCell> generator)
		{
			Generator = generator;
		}

		public GridViewCellGenerator<TGridCell> Generator { get; }

		protected abstract void ClearPropertyValue(TGridCell gridCell);

		internal void ClearPropertyValueInternal(TGridCell gridCell)
		{
			ClearPropertyValue(gridCell);
		}

		protected virtual void OnValueChanged(GridViewCellGeneratorSourceProperty<TGridCell> sourceProperty)
		{
			Generator.OnGeneratorChangedInternal();
		}

		internal void OnValueChangedInternal(GridViewCellGeneratorSourceProperty<TGridCell> sourceProperty)
		{
			OnValueChanged(sourceProperty);
		}

		protected virtual void OnValueChanging(GridViewCellGeneratorSourceProperty<TGridCell> sourceProperty)
		{
			Generator.OnGeneratorChangingInternal();
		}

		internal void OnValueChangingInternal(GridViewCellGeneratorSourceProperty<TGridCell> sourceProperty)
		{
			OnValueChanging(sourceProperty);
		}

		protected abstract void SetPropertyValue(TGridCell gridCell);

		internal void SetPropertyValueInternal(TGridCell gridCell)
		{
			SetPropertyValue(gridCell);
		}
	}
}