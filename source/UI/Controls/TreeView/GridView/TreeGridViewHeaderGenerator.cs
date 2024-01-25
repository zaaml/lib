// <copyright file="TreeGridViewHeaderGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TreeView
{
	[ContentProperty(nameof(HeaderTemplate))]
	public sealed class TreeGridViewHeaderGenerator : TreeGridViewHeaderGeneratorBase
	{
		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<TreeGridViewHeaderTemplate, TreeGridViewHeaderGenerator>
			("HeaderTemplate", d => d.OnHeaderTemplatePropertyChangedPrivate);

		public TreeGridViewHeaderGenerator()
		{
			Implementation = new TemplatedGridViewCellGenerator<TreeGridViewHeader>(this);
		}

		public TreeGridViewHeaderTemplate HeaderTemplate
		{
			get => (TreeGridViewHeaderTemplate)GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		private TemplatedGridViewCellGenerator<TreeGridViewHeader> Implementation { get; }

		protected override TreeGridViewHeader CreateCell()
		{
			return Implementation.CreateCellCore();
		}

		protected override void DisposeCell(TreeGridViewHeader header)
		{
			Implementation.DisposeCellCore(header);
		}

		private void OnHeaderTemplatePropertyChangedPrivate(TreeGridViewHeaderTemplate oldValue, TreeGridViewHeaderTemplate newValue)
		{
			Implementation.CellTemplate = newValue;
		}
	}
}