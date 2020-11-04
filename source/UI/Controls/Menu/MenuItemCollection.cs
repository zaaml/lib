// <copyright file="MenuItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;
using Control = System.Windows.Controls.Control;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class MenuItemCollection : MenuItemCollectionBase<MenuItemBase>
	{
		#region Static Fields and Constants

		internal static readonly MenuItemCollection Empty = new MenuItemCollection(new Control());

		#endregion

		#region Ctors

		internal MenuItemCollection(Control menuItemOwner) : base(menuItemOwner)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<MenuItemBase> DefaultGenerator { get; } = new MenuItemGenerator();

		internal MenuItemGeneratorBase Generator
		{
			get => (MenuItemGeneratorBase) GeneratorCore;
			set => GeneratorCore = value;
		}

		#endregion
	}
}