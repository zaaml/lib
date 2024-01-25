// <copyright file="GridViewCellPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core.GridView
{
	public class GridViewCellPresenter : FixedTemplateControl<Border>
	{
		private static readonly DependencyPropertySync<GridViewCell, Border> BorderPropertySync = new DependencyPropertySync<GridViewCell, Border>()
			.AddProperty(BackgroundProperty, Border.BackgroundProperty)
			.AddProperty(BorderBrushProperty, Border.BorderBrushProperty)
			.AddProperty(BorderThicknessProperty, Border.BorderThicknessProperty);

		private UIElement _child;
		private GridViewCell _gridCell;

		internal UIElement Child
		{
			get => _child;
			set
			{
				if (ReferenceEquals(_child, value))
					return;

				_child = value;

				if (TemplateRoot != null)
					TemplateRoot.Child = value;
			}
		}

		internal GridViewCell GridCell
		{
			get => _gridCell;
			set
			{
				if (ReferenceEquals(_gridCell, value))
					return;

				_gridCell = value;

				SyncBorder();
			}
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			SyncBorder();

			TemplateRoot.Child = Child;
		}

		internal void OnBorderPropertyChanged(DependencyPropertyChangedEventArgs e)
		{
			if (TemplateRoot == null || GridCell == null)
				return;

			BorderPropertySync.Sync(e.Property, GridCell, TemplateRoot);
		}

		private void SyncBorder()
		{
			if (GridCell == null || TemplateRoot == null)
				return;

			BorderPropertySync.Sync(GridCell, TemplateRoot);
		}

		protected override void UndoTemplateOverride()
		{
			TemplateRoot.Child = null;

			base.UndoTemplateOverride();
		}
	}
}