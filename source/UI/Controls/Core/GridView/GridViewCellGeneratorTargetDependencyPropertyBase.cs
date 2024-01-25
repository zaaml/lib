// <copyright file="GridCellGeneratorTargetDependencyPropertyBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal abstract class GridViewCellGeneratorTargetDependencyPropertyBase<TGridCell, TProperty> : GridViewCellGeneratorTargetProperty<TGridCell, TProperty>
		where TGridCell : GridViewCell
	{
		protected GridViewCellGeneratorTargetDependencyPropertyBase(GridViewCellGenerator<TGridCell> generator) : base(generator)
		{
		}

		protected abstract DependencyProperty TargetProperty { get; }

		public GridViewCellGeneratorTargetDependencyPropertyBase<TGridCell, TProperty> BindSource(DependencyObject source, DependencyProperty sourceProperty)
		{
			return this;
		}

		protected override void ClearPropertyValue(TGridCell gridCell)
		{
			gridCell.ClearValue(TargetProperty);
		}

		protected override void SetPropertyValue(TGridCell gridCell)
		{
			gridCell.SetValue(TargetProperty, ValueCore);
		}
	}
}