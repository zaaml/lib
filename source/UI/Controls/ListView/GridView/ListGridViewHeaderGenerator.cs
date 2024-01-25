// <copyright file="ListGridViewHeaderGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.ListView
{
	[ContentProperty(nameof(HeaderTemplate))]
	public sealed class ListGridViewHeaderGenerator : ListGridViewHeaderGeneratorBase
	{
		public static readonly DependencyProperty HeaderTemplateProperty = DPM.Register<ListGridViewHeaderTemplate, ListGridViewHeaderGenerator>
			("HeaderTemplate", d => d.OnHeaderTemplatePropertyChangedPrivate);

		public ListGridViewHeaderGenerator()
		{
			Implementation = new TemplatedGridViewCellGenerator<ListGridViewHeader>(this);
		}

		public ListGridViewHeaderTemplate HeaderTemplate
		{
			get => (ListGridViewHeaderTemplate)GetValue(HeaderTemplateProperty);
			set => SetValue(HeaderTemplateProperty, value);
		}

		private TemplatedGridViewCellGenerator<ListGridViewHeader> Implementation { get; }

		protected override ListGridViewHeader CreateCell()
		{
			return Implementation.CreateCellCore();
		}

		protected override void DisposeCell(ListGridViewHeader header)
		{
			Implementation.DisposeCellCore(header);
		}

		private void OnHeaderTemplatePropertyChangedPrivate(ListGridViewHeaderTemplate oldValue, ListGridViewHeaderTemplate newValue)
		{
			Implementation.CellTemplate = newValue;
		}
	}
}