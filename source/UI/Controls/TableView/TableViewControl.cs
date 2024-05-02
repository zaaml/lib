// <copyright file="TableViewControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Controls.Core;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Controls.TableView
{
	[TemplateContractType(typeof(TableViewControlTemplateContract))]
	public class TableViewControl : ItemsControlBase<TableViewControl, TableViewItem, TableViewItemCollection, TableViewItemsPresenter, TableViewPanel>
	{
		public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, TableViewControl>
			("Orientation", Orientation.Vertical, d => d.OnOrientationPropertyChangedPrivate);

		private static readonly DependencyPropertyKey DefinitionPropertyKey = DPM.RegisterReadOnly<TableViewDefinitionCollection, TableViewControl>
			("DefinitionPrivate");

		public static readonly DependencyProperty ItemSpacingProperty = DPM.Register<double, TableViewControl>
			("ItemSpacing", default, d => d.OnItemSpacingPropertyChangedPrivate);

		public static readonly DependencyProperty ElementSpacingProperty = DPM.Register<double, TableViewControl>
			("ElementSpacing", default, d => d.OnElementSpacingPropertyChangedPrivate);

		public static readonly DependencyProperty DefinitionLengthProperty = DPM.Register<FlexLength, TableViewControl>
			("DefinitionLength", FlexLength.Star, d => d.OnDefinitionLengthPropertyChangedPrivate);

		public static readonly DependencyProperty HeaderItemProperty = DPM.Register<TableViewItem, TableViewControl>
			("HeaderItem", default, d => d.OnHeaderItemPropertyChangedPrivate);

		public static readonly DependencyProperty FooterItemProperty = DPM.Register<TableViewItem, TableViewControl>
			("FooterItem", default, d => d.OnFooterItemPropertyChangedPrivate);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<TableViewItemGenerator, TableViewControl>
			("ItemGenerator", default, d => d.OnItemGeneratorPropertyChangedPrivate);

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, TableViewControl>
			("SourceCollection", d => d.OnSourceCollectionPropertyChangedPrivate);

		public static readonly DependencyProperty DefinitionsProperty = DefinitionPropertyKey.DependencyProperty;

		static TableViewControl()
		{
			DefaultStyleKeyHelper.OverrideStyleKey<TableViewControl>();
		}

		public TableViewControl()
		{
			this.OverrideStyleKey<TableViewControl>();
		}

		[TypeConverter(typeof(FlexLengthTypeConverter))]
		public FlexLength DefinitionLength
		{
			get => (FlexLength)GetValue(DefinitionLengthProperty);
			set => SetValue(DefinitionLengthProperty, value);
		}

		public TableViewDefinitionCollection Definitions => this.GetValueOrCreate(DefinitionPropertyKey, CreateTableViewDefinitionCollection);

		public double ElementSpacing
		{
			get => (double)GetValue(ElementSpacingProperty);
			set => SetValue(ElementSpacingProperty, value);
		}

		public TableViewItem FooterItem
		{
			get => (TableViewItem)GetValue(FooterItemProperty);
			set => SetValue(FooterItemProperty, value);
		}

		public TableViewItem HeaderItem
		{
			get => (TableViewItem)GetValue(HeaderItemProperty);
			set => SetValue(HeaderItemProperty, value);
		}

		public TableViewItemGenerator ItemGenerator
		{
			get => (TableViewItemGenerator)GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public double ItemSpacing
		{
			get => (double)GetValue(ItemSpacingProperty);
			set => SetValue(ItemSpacingProperty, value);
		}

		public Orientation Orientation
		{
			get => (Orientation)GetValue(OrientationProperty);
			set => SetValue(OrientationProperty, value);
		}

		public IEnumerable SourceCollection
		{
			get => (IEnumerable)GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		protected override TableViewItemCollection CreateItemCollection()
		{
			return new TableViewItemCollection(this);
		}

		private TableViewDefinitionCollection CreateTableViewDefinitionCollection()
		{
			return new TableViewDefinitionCollection(this);
		}

		private void InvalidatePanel()
		{
			InvalidateMeasure();
			ItemsPresenter?.InvalidatePanel();
		}

		private void OnDefinitionLengthPropertyChangedPrivate(FlexLength oldValue, FlexLength newValue)
		{
			InvalidatePanel();
		}

		internal void OnDefinitionsChanged()
		{
			InvalidatePanel();
		}

		private void OnElementSpacingPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidatePanel();
		}

		private void OnFooterItemPropertyChangedPrivate(TableViewItem oldValue, TableViewItem newValue)
		{
			LogicalChildMentor.OnLogicalChildPropertyChanged(oldValue, newValue);

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.TableViewControl, this) == false)
					throw new InvalidOperationException();

				oldValue.TableViewControl = null;
			}

			if (newValue != null)
			{
				if (newValue.TableViewControl != null)
					throw new InvalidOperationException("TableViewItem already has parent");

				newValue.TableViewControl = this;
			}

			if (ItemsPresenter == null)
				return;

			ItemsPresenter.FooterItem = newValue;
		}

		private void OnHeaderItemPropertyChangedPrivate(TableViewItem oldValue, TableViewItem newValue)
		{
			LogicalChildMentor.OnLogicalChildPropertyChanged(oldValue, newValue);

			if (oldValue != null)
			{
				if (ReferenceEquals(oldValue.TableViewControl, this) == false)
					throw new InvalidOperationException();

				oldValue.TableViewControl = null;
			}

			if (newValue != null)
			{
				if (newValue.TableViewControl != null)
					throw new InvalidOperationException("TableViewItem already has parent");

				newValue.TableViewControl = this;
			}

			InvalidatePanel();

			if (ItemsPresenter == null)
				return;

			ItemsPresenter.HeaderItem = newValue;
		}

		internal override void OnItemAttachedInternal(TableViewItem item)
		{
			item.TableViewControl = this;

			base.OnItemAttachedInternal(item);
		}

		internal override void OnItemDetachedInternal(TableViewItem item)
		{
			base.OnItemDetachedInternal(item);

			item.TableViewControl = null;
		}

		private void OnItemGeneratorPropertyChangedPrivate(TableViewItemGenerator oldValue, TableViewItemGenerator newValue)
		{
			ItemCollection.Generator = newValue;
		}

		private void OnItemSpacingPropertyChangedPrivate(double oldValue, double newValue)
		{
			InvalidatePanel();
		}

		private void OnOrientationPropertyChangedPrivate(Orientation oldValue, Orientation newValue)
		{
			InvalidatePanel();
		}

		private void OnSourceCollectionPropertyChangedPrivate(IEnumerable oldValue, IEnumerable newValue)
		{
			SourceCollectionCore = newValue;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.TableViewControl = this;
			ItemsPresenter.HeaderItem = HeaderItem;
			ItemsPresenter.FooterItem = FooterItem;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.FooterItem = null;
			ItemsPresenter.HeaderItem = null;
			ItemsPresenter.TableViewControl = null;

			base.OnTemplateContractDetaching();
		}
	}
}