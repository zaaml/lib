// <copyright file="ListGridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(CellTemplate))]
	public sealed class ListGridViewCellGenerator : ListGridViewCellGeneratorBase
	{
		public static readonly DependencyProperty CellTemplateProperty = DPM.Register<ListGridViewCellTemplate, ListGridViewCellGenerator>
			("CellTemplate", d => d.OnCellTemplatePropertyChangedPrivate);

		public ListGridViewCellGenerator()
		{
			Implementation = new TemplatedGridViewCellGenerator<ListGridViewCell>(this);
		}

		public ListGridViewCellTemplate CellTemplate
		{
			get => (ListGridViewCellTemplate)GetValue(CellTemplateProperty);
			set => SetValue(CellTemplateProperty, value);
		}

		private TemplatedGridViewCellGenerator<ListGridViewCell> Implementation { get; }

		protected override ListGridViewCell CreateCell()
		{
			return Implementation.CreateCellCore();
		}

		protected override void DisposeCell(ListGridViewCell item)
		{
			Implementation.DisposeCellCore(item);
		}

		private void OnCellTemplatePropertyChangedPrivate(ListGridViewCellTemplate oldValue, ListGridViewCellTemplate newValue)
		{
			Implementation.CellTemplate = CellTemplate;
		}
	}
}