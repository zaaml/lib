// <copyright file="SplitLayoutView.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Zaaml.Core;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.PresentationCore.Theming;
using Zaaml.UI.Extensions;

namespace Zaaml.UI.Controls.Docking
{
  public sealed class SplitLayoutView : BaseLayoutView<SplitLayout>
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, SplitLayoutView>
      ("Orientation", s => s.OnOrientationChanged);

    #endregion

    #region Fields

    private readonly ConstraintGridSplitterDelta _splitterConstraintHelper;
    private readonly Dictionary<DockItem, DockGridSplitter> _splitters = new Dictionary<DockItem, DockGridSplitter>();
    private bool _arrangeValid;

    #endregion

    #region Ctors

    static SplitLayoutView()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<SplitLayoutView>();
    }

    public SplitLayoutView()
    {
      this.OverrideStyleKey<SplitLayoutView>();

      _splitterConstraintHelper = new ConstraintGridSplitterDelta(new Size(40, 40));

      SizeChanged += OnSizeChanged;
    }

    #endregion

    #region Properties

    private Grid ItemsHost => TemplateContract.ItemsHost;

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    private SplitLayoutTemplateContract TemplateContract => (SplitLayoutTemplateContract) TemplateContractInternal;

    #endregion

    #region  Methods

    private void AddGridDefinitions()
    {
      if (TemplateContract.IsAttached == false)
        return;

      switch (Orientation)
      {
        case Orientation.Horizontal:

          ItemsHost.ColumnDefinitions.Add(new ColumnDefinition());
          ItemsHost.ColumnDefinitions.Add(new ColumnDefinition());

          break;

        case Orientation.Vertical:

          ItemsHost.RowDefinitions.Add(new RowDefinition());
          ItemsHost.RowDefinitions.Add(new RowDefinition());

          break;
      }
    }

    private void ApplySize(Size size, bool equalizeSize = false)
    {
      if (_arrangeValid == false || Items.Count == 0)
        return;

      var equalSize = new Size(size.Width / Items.Count, size.Height / Items.Count);

      foreach (var item in Items)
      {
	      var resSize = equalizeSize ? equalSize : new Size(SplitLayout.GetSplitWidth(item), SplitLayout.GetSplitHeight(item));

	      switch (Orientation)
	      {
		      case Orientation.Horizontal:

			      var columnDefinition = ItemsHost.ColumnDefinitions[Grid.GetColumn(item)];

			      columnDefinition.Width = new GridLength(resSize.Width, GridUnitType.Star);

			      break;

		      case Orientation.Vertical:

			      var rowDefinition = ItemsHost.RowDefinitions[Grid.GetRow(item)];

			      rowDefinition.Height = new GridLength(resSize.Height, GridUnitType.Star);

			      break;
	      }
      }
    }

    protected internal override void ArrangeItems()
    {
      if (TemplateContract.IsAttached == false)
        return;

      var iDefinition = 0;

      ItemsHost.Children.Clear();

      DockGridSplitter splitter = null;

      foreach (var item in OrderedItems)
      {
        splitter = _splitters[item];
        splitter.Visibility = Visibility.Visible;

        ItemsHost.Children.Add(item);
        ItemsHost.Children.Add(splitter);

        item.ApplyGridPosition();
        splitter.ApplyGridPosition();

        switch (Orientation)
        {
          case Orientation.Horizontal:

            Grid.SetColumn(item, iDefinition);
            Grid.SetColumn(splitter, iDefinition + 1);
            ItemsHost.ColumnDefinitions[iDefinition + 1].Width = GridLength.Auto;
            DockLayoutView.SetResizeDirection(splitter, GridResizeDirection.Columns);

            break;

          case Orientation.Vertical:

            Grid.SetRow(item, iDefinition);
            Grid.SetRow(splitter, iDefinition + 1);
            ItemsHost.RowDefinitions[iDefinition + 1].Height = GridLength.Auto;
            DockLayoutView.SetResizeDirection(splitter, GridResizeDirection.Rows);

            break;
        }

        iDefinition += 2;
      }

      if (splitter != null)
        splitter.Visibility = Visibility.Collapsed;

      _arrangeValid = true;

      ApplySize(RenderSize);
    }

    protected override TemplateContract CreateTemplateContract()
    {
      return new SplitLayoutTemplateContract();
    }

    private void FinalizeSplitting()
    {
      foreach (var item in Items)
      {
        switch (Orientation)
        {
          case Orientation.Horizontal:
            SplitLayout.SetSplitWidth(item, GetColumnWidth(item));
            break;
          case Orientation.Vertical:
            SplitLayout.SetSplitHeight(item, GetRowHeight(item));
            break;
        }
      }
    }

    private double GetColumnWidth(DockItem item)
    {
      return ItemsHost.ColumnDefinitions[Grid.GetColumn(item)].Width.Value;
    }

    private double GetRowHeight(DockItem item)
    {
      return ItemsHost.RowDefinitions[Grid.GetRow(item)].Height.Value;
    }

    protected override void OnItemAdded(DockItem item)
    {
      var splitter = new DockGridSplitter();

      splitter.LostMouseCapture += OnSplitterLostMouseCapture;
      splitter.MouseLeftButtonDown += OnSplitterMouseLeftButtonDown;

      splitter.AddHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnSplitterMouseLeftButtonDown, true);

      _splitterConstraintHelper.AddSplitter(splitter);
      _splitters[item] = splitter;

      AddGridDefinitions();

      InvalidateItemsArrange();
    }

    protected override void OnItemRemoved(DockItem item)
    {
      var splitter = _splitters[item];

      splitter.LostMouseCapture -= OnSplitterLostMouseCapture;
      splitter.RemoveHandler(MouseLeftButtonDownEvent, (MouseButtonEventHandler) OnSplitterMouseLeftButtonDown);

      _splitterConstraintHelper.RemoveSplitter(splitter);

      RemoveGridDefinitions();

      ItemsHost?.Children.Remove(item);

      InvalidateItemsArrange();
    }

    protected override void OnLayoutAttached()
    {
      base.OnLayoutAttached();

      SetBinding(OrientationProperty, new Binding {Path = new PropertyPath(SplitLayout.OrientationProperty), Source = Layout});
    }

    protected override void OnLayoutDetaching()
    {
      ClearValue(OrientationProperty);

      base.OnLayoutDetaching();
    }

    private void OnOrientationChanged()
    {
      if (ItemsHost == null)
        return;

      ItemsHost.ColumnDefinitions.Clear();
      ItemsHost.RowDefinitions.Clear();

      for (var index = 0; index < Items.Count; index++)
        AddGridDefinitions();

      InvalidateItemsArrange();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
      ApplySize(args.NewSize);
    }

    private void OnSplitterLostMouseCapture(object sender, MouseEventArgs mouseEventArgs)
    {
      FinalizeSplitting();
    }

    private void OnSplitterMouseLeftButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
    {
      if (_arrangeValid && mouseButtonEventArgs.ClickCount == 2)
        ApplySize(RenderSize, true);
    }

    protected override void OnTemplateContractAttached()
    {
      base.OnTemplateContractAttached();

      _splitterConstraintHelper.Grid = ItemsHost;

      for (var index = 0; index < Items.Count; index++)
        AddGridDefinitions();
    }

    protected override void OnTemplateContractDetaching()
    {
      ItemsHost.ColumnDefinitions.Clear();
      ItemsHost.RowDefinitions.Clear();

      ItemsHost.Children.Clear();
      _splitterConstraintHelper.Grid = null;

      base.OnTemplateContractDetaching();
    }

    private void RemoveGridDefinitions()
    {
      if (TemplateContract.IsAttached == false)
        return;

      switch (Orientation)
      {
        case Orientation.Horizontal:

          ItemsHost.ColumnDefinitions.RemoveAt(0);
          ItemsHost.ColumnDefinitions.RemoveAt(0);

          break;

        case Orientation.Vertical:

          ItemsHost.RowDefinitions.RemoveAt(0);
          ItemsHost.RowDefinitions.RemoveAt(0);

          break;
      }
    }

    #endregion
  }

  internal sealed class SplitLayoutTemplateContract : TemplateContract
  {
    #region Properties

    [TemplateContractPart(Required = true)]
    public Grid ItemsHost { get; [UsedImplicitly] private set; }

    #endregion
  }
}