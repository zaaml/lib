// <copyright file="GridCellGeneratorTargetDependencyProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal class GridViewCellGeneratorTargetDependencyProperty<TGridCell, TProperty> : GridViewCellGeneratorTargetDependencyPropertyBase<TGridCell, TProperty>
		where TGridCell : GridViewCell
	{
		public GridViewCellGeneratorTargetDependencyProperty(GridViewCellGenerator<TGridCell> generator, DependencyProperty targetProperty) : base(generator)
		{
			TargetProperty = targetProperty;
			SourceProperty = new GridViewCellGeneratorSourceProperty<TGridCell, TProperty>(this);
		}

		private GridViewCellGeneratorSourceProperty<TGridCell, TProperty> SourceProperty { get; }

		protected override DependencyProperty TargetProperty { get; }

		public TProperty Value
		{
			get => SourceProperty.Value;
			set => SourceProperty.Value = value;
		}

		protected override TProperty ValueCore => SourceProperty.Value;
	}
}