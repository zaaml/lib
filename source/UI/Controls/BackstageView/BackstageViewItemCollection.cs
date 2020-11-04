// <copyright file="BackstageViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.BackstageView
{
	public sealed class BackstageViewItemCollection : ItemCollectionBase<BackstageViewControl, BackstageViewItem>
	{
		#region Ctors

		internal BackstageViewItemCollection(BackstageViewControl backstageViewControl) : base(backstageViewControl)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<BackstageViewItem> DefaultGenerator { get; } = new BackstageViewItemGenerator();

		internal BackstageViewItemGeneratorBase Generator
		{
			get => (BackstageViewItemGeneratorBase) GeneratorCore;
			set => GeneratorCore = value;
		}

		#endregion
	}
}
