// <copyright file="AccordionViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.AccordionView
{
	public sealed class AccordionViewItemCollection : ItemCollectionBase<AccordionViewControl, AccordionViewItem>
	{
		#region Ctors

		internal AccordionViewItemCollection(AccordionViewControl accordionView) : base(accordionView)
		{
		}

		#endregion

		#region Properties

		protected override ItemGenerator<AccordionViewItem> DefaultGenerator { get; } = new AccordionViewItemGenerator();

		internal AccordionViewItemGeneratorBase Generator
		{
			get => (AccordionViewItemGeneratorBase) GeneratorCore;
			set => GeneratorCore = value;
		}

		#endregion
	}
}