// <copyright file="TableViewItem.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TableView
{
	[ContentProperty(nameof(Elements))]
	public class TableViewItem : FixedTemplateControl<TableViewItemPanel>
	{
		private static readonly DependencyPropertyKey ElementsPropertyKey = DPM.RegisterReadOnly<TableViewItemElementsCollection, TableViewItem>
			("ElementsPrivate");

		private static readonly DependencyPropertyKey TableViewControlPropertyKey = DPM.RegisterReadOnly<TableViewControl, TableViewItem>
			("TableViewControl", default, d => d.OnTableViewControlPropertyChangedPrivate);

		public static readonly DependencyProperty TableViewControlProperty = TableViewControlPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ElementsProperty = ElementsPropertyKey.DependencyProperty;
		private static readonly PropertyPath BackgroundPropertyPath = new PropertyPath(BackgroundProperty);

		public TableViewItemElementsCollection Elements => this.GetValueOrCreate(ElementsPropertyKey, CreateElementsCollection);

		protected override IEnumerator LogicalChildren => Elements.LogicalChildrenInternal;

		public TableViewControl TableViewControl
		{
			get => (TableViewControl) GetValue(TableViewControlProperty);
			internal set => this.SetReadOnlyValue(TableViewControlPropertyKey, value);
		}

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			TemplateRoot.Item = this;
			Elements.ItemPanel = TemplateRoot;

			TemplateRoot.SetBinding(Panel.BackgroundProperty, new Binding {Path = BackgroundPropertyPath, Source = this});
		}

		private TableViewItemElementsCollection CreateElementsCollection()
		{
			return new TableViewItemElementsCollection(this);
		}

		internal void FixedMeasure(Size availableSize)
		{
			TemplateRoot?.InvalidateMeasure();
			InvalidateMeasure();

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

		internal void StarMeasure(Size availableSize)
		{
			TemplateRoot?.InvalidateMeasure();
			InvalidateMeasure();

			Measure(availableSize);
		}

		protected override void UndoTemplateOverride()
		{
			Elements.ItemPanel = null;
			TemplateRoot.Item = null;
			TemplateRoot.ClearValue(Panel.BackgroundProperty);

			base.UndoTemplateOverride();
		}
	}
}