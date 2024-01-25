// <copyright file="TreeGridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(CellTemplate))]
	public sealed class TreeGridViewCellGenerator : TreeGridViewCellGeneratorBase
	{
		public static readonly DependencyProperty CellTemplateProperty = DPM.Register<TreeGridViewCellTemplate, TreeGridViewCellGenerator>
			("CellTemplate", d => d.OnCellTemplatePropertyChangedPrivate);

		public TreeGridViewCellGenerator()
		{
			Implementation = new TemplatedGridViewCellGenerator<TreeGridViewCell>(this);
		}

		public TreeGridViewCellTemplate CellTemplate
		{
			get => (TreeGridViewCellTemplate)GetValue(CellTemplateProperty);
			set => SetValue(CellTemplateProperty, value);
		}

		private TemplatedGridViewCellGenerator<TreeGridViewCell> Implementation { get; }

		protected override TreeGridViewCell CreateCell()
		{
			return Implementation.CreateCellCore();
		}

		protected override void DisposeCell(TreeGridViewCell item)
		{
			Implementation.DisposeCellCore(item);
		}

		private void OnCellTemplatePropertyChangedPrivate(TreeGridViewCellTemplate oldValue, TreeGridViewCellTemplate newValue)
		{
			Implementation.CellTemplate = CellTemplate;
		}
	}
}