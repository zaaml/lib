// <copyright file="TableViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Markup;
using Zaaml.Core;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TableView
{
	[ContentProperty(nameof(Elements))]
	[TemplateContractType(typeof(TableViewItemTemplateContract))]
	public class TableViewItem : TemplateContractControl
	{
		private static readonly DependencyPropertyKey ElementsPropertyKey = DPM.RegisterReadOnly<TableViewItemElementsCollection, TableViewItem>
			("ElementsPrivate");

		private static readonly DependencyPropertyKey TableViewControlPropertyKey = DPM.RegisterReadOnly<TableViewControl, TableViewItem>
			("TableViewControl", default, d => d.OnTableViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty TableViewControlProperty = TableViewControlPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ElementsProperty = ElementsPropertyKey.DependencyProperty;

		static TableViewItem()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TableViewItem>();
		}

		public TableViewItemElementsCollection Elements => this.GetValueOrCreate(ElementsPropertyKey, CreateElementsCollection);

		protected override IEnumerator LogicalChildren => Elements.LogicalChildrenInternal;

		public TableViewControl TableViewControl
		{
			get => (TableViewControl)GetValue(TableViewControlProperty);
			internal set => this.SetReadOnlyValue(TableViewControlPropertyKey, value);
		}

		private TableViewItemTemplateContract TemplateContract => (TableViewItemTemplateContract)TemplateContractCore;

		private TableViewItemPanel TemplateRoot => TemplateContract.ItemPanel;

		private TableViewItemElementsCollection CreateElementsCollection()
		{
			return new TableViewItemElementsCollection(this);
		}

		internal void FixedMeasure(Size availableSize)
		{
			InvalidatePanelMeasure();

			Measure(availableSize);
		}

		protected virtual void OnTableViewControlChanged(TableViewControl oldTableView, TableViewControl newTableView)
		{
		}

		internal virtual void OnTableViewControlChangedInternal(TableViewControl oldTableView, TableViewControl newTableView)
		{
			OnTableViewControlChanged(oldTableView, newTableView);
		}

		private void OnTableViewControlPropertyChangedPrivate(TableViewControl oldTableView, TableViewControl newTableView)
		{
			OnTableViewControlChangedInternal(oldTableView, newTableView);
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			TemplateRoot.Item = this;
			Elements.ItemPanel = TemplateRoot;
		}

		protected override void OnTemplateContractDetaching()
		{
			Elements.ItemPanel = null;
			TemplateRoot.Item = null;

			base.OnTemplateContractDetaching();
		}

		private void InvalidatePanelMeasure()
		{
			TemplateRoot?.InvalidateAncestorsMeasure(this, true);
		}

		internal void StarMeasure(Size availableSize)
		{
			InvalidatePanelMeasure();

			Measure(availableSize);
		}
	}

	public class TableViewItemTemplateContract : TemplateContract
	{
		[TemplateContractPart(Required = true)]
		public TableViewItemPanel ItemPanel { get; [UsedImplicitly] private set; }
	}
}