// <copyright file="TabViewItemCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.TabView
{
  public sealed class TabViewItemCollection : ItemCollectionBase<TabViewControl, TabViewItem>
  {
    #region Ctors

    internal TabViewItemCollection(TabViewControl tabViewControl):base(tabViewControl)
    {
    }

    #endregion

    #region Properties

    protected override ItemGenerator<TabViewItem> DefaultGenerator { get; } = new TabViewItemGenerator();

    internal TabViewItemGeneratorBase Generator
    {
      get => (TabViewItemGeneratorBase) GeneratorCore;
      set => GeneratorCore = value;
    }

    #endregion
  }
}