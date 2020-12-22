// <copyright file="PopupMenu.ItemsControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Menu
{
	public partial class PopupMenu
	{
		#region Static Fields and Constants

		public static readonly DependencyProperty ItemStyleProperty = DPM.Register<Style, PopupMenu>
			("ItemStyle");

		private static readonly DependencyPropertyKey ItemsPropertyKey = DPM.RegisterReadOnly<MenuItemCollection, PopupMenu>
			("ItemsInt");

		public static readonly DependencyProperty ItemsProperty = ItemsPropertyKey.DependencyProperty;

		public static readonly DependencyProperty SourceCollectionProperty = DPM.Register<IEnumerable, PopupMenu>
			("SourceCollection", m => m.OnSourceCollectionChanged);

		public static readonly DependencyProperty ItemGeneratorProperty = DPM.Register<MenuItemGeneratorBase, PopupMenu>
			("ItemGenerator", m => m.OnItemGeneratorChanged);

		#endregion

		#region Properties

		public MenuItemGeneratorBase ItemGenerator
		{
			get => (MenuItemGeneratorBase) GetValue(ItemGeneratorProperty);
			set => SetValue(ItemGeneratorProperty, value);
		}

		public MenuItemCollection Items => this.GetValueOrCreate(ItemsPropertyKey, () => new MenuItemCollection(this));

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

		protected override IEnumerator LogicalChildren => Items.LogicalChildren;

		#endregion

		#region  Methods

		private void OnItemGeneratorChanged(MenuItemGeneratorBase oldGenerator, MenuItemGeneratorBase newGenerator)
		{
			Items.Generator = newGenerator;
		}

		private void OnSourceCollectionChanged(IEnumerable oldSource, IEnumerable newSource)
		{
			Items.SourceInternal = newSource;
		}

		protected override void OnTemplateContractAttached()
		{
			base.OnTemplateContractAttached();

			ItemsPresenter.Items = Items;
			ItemsPresenter.ActualOrientation = Orientation.Vertical;
		}

		protected override void OnTemplateContractDetaching()
		{
			ItemsPresenter.Items = null;

			base.OnTemplateContractDetaching();
		}

		#endregion
	}
}