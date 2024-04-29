// <copyright file="RadioMenuItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Menu
{
	public sealed class RadioMenuItemCollection : MenuItemCollectionBase<RadioMenuItem>
	{
		internal RadioMenuItemCollection(Control menuItemOwner) : base(menuItemOwner)
		{
		}

		protected override ItemGenerator<RadioMenuItem> DefaultGenerator => DefaultRadioGenerator;

		internal RadioMenuItemGenerator DefaultRadioGenerator { get; } = new();

		internal RadioMenuItemGeneratorBase Generator
		{
			get => (RadioMenuItemGeneratorBase)GeneratorCore;
			set => GeneratorCore = value;
		}
	}
}