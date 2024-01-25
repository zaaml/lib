// <copyright file="MenuBase.ItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Menu
{
	public abstract partial class MenuBase
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemStyleProperty = DPM.Register<Style, MenuBase>
			("ItemStyle");

		private static readonly DependencyPropertyKey ItemCollectionPropertyKey = DPM.RegisterReadOnly<MenuItemCollection, MenuBase>
			("ItemCollectionPrivate");

		public static readonly DependencyProperty ItemCollectionProperty = ItemCollectionPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, MenuBase>
			("SourceCollection", m => m.OnSourceCollectionChanged);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, MenuBase>
			("ItemGenerator", m => m.OnItemGeneratorChanged);

		#endregion

		#region Properties

		public MenuItemGeneratorBase ItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public MenuItemCollection ItemCollection => this.GetValueOrCreate(ItemCollectionPropertyKey, () => new MenuItemCollection(this));

		protected MenuItemsPresenter ItemsPresenter => TemplateContract.ItemsPresenter;

		public IEnumerable SourceCollection
		{
			get => (IEnumerable) GetValue(SourceCollectionProperty);
			set => SetValue(SourceCollectionProperty, value);
		}

		public Style ItemStyle
		{
			get => (Style) GetValue(ItemStyleProperty);
			set => SetValue(ItemStyleProperty, value);
		}

		protected override IEnumerator LogicalChildren => ItemCollection.LogicalChildren;

		private MenuBaseTemplateContract TemplateContract => (MenuBaseTemplateContract) TemplateContractCore;

		#endregion

		#region  Methods

		private void OnItemGeneratorChanged(MenuItemGeneratorBase oldGenerator, MenuItemGeneratorBase newGenerator)
		{
			ItemCollection.Generator = newGenerator;
		}

		private void OnSourceCollectionChanged(IEnumerable oldSource, IEnumerable newSource)
		{
			ItemCollection.SourceCollectionInternal = newSource;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.Items = ItemCollection;
			ItemsPresenter.ActualOrientation = Orientation;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.Items = null;

			base.OnTemplateContractDetaching();
		}

		#endregion
	}
}