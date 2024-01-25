// <copyright file="GridCellGeneratorBindingTargetProperty.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Data;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.UI.Controls.Core.GridView
{
	internal abstract class GridViewCellGeneratorBindingTargetProperty<TGridCell, TProperty> : GridViewCellGeneratorTargetDependencyPropertyBase<TGridCell, TProperty>
		where TGridCell : GridViewCell
	{
		private static readonly Binding NullBinding = new() { Source = default(TProperty), BindsDirectlyToSource = true, Mode = BindingMode.OneTime };

		protected GridViewCellGeneratorBindingTargetProperty(GridViewCellGenerator<TGridCell> generator) : base(generator)
		{
		}

		protected abstract Binding BindingCore { get; }

		protected override void ClearPropertyValue(TGridCell gridCell)
		{
			var binding = BindingCore ?? NullBinding;
			var target = GetTarget(gridCell);
			var currentBinding = target.ReadLocalBinding(TargetProperty);

			if (ReferenceEquals(currentBinding, binding))
				target.ClearValue(TargetProperty);
		}

		protected virtual DependencyObject GetTarget(TGridCell gridCell)
		{
			return gridCell;
		}

		protected override void SetPropertyValue(TGridCell gridCell)
		{
			var binding = BindingCore ?? NullBinding;
			var target = GetTarget(gridCell);
			var currentBinding = target.ReadLocalBinding(TargetProperty);

			if (ReferenceEquals(currentBinding, binding) == false)
				target.SetBinding(TargetProperty, binding);
		}
	}
}