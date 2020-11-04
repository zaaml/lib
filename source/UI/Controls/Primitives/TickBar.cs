// <copyright file="TickBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Zaaml.Core.Monads;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using Control = Zaaml.UI.Controls.Core.Control;
using Panel = Zaaml.UI.Panels.Core.Panel;

#pragma warning disable 67

namespace Zaaml.UI.Controls.Primitives
{
  public interface ITickBarProvider
  {
    #region Fields

    event EventHandler Changed;

    #endregion

    #region  Methods

    IEnumerable<TickMark> GetTickMarks(double availableSize);

    #endregion
  }

  public class SimpleTickBarProvider : ITickBarProvider
  {
    #region Fields

    private readonly List<TickMark> _tickMarks = new List<TickMark>();

    #endregion

    #region Properties

    public DataTemplate MajorTickMarkTemplate { get; set; }

    public DataTemplate MinorTickMarkTemplate { get; set; }

    public double Start { get; set; }

    public double Step { get; set; }

    public int SubDivisionsCount { get; set; }

    #endregion

    #region Interface Implementations

    #region ITickBarProvider

    public event EventHandler Changed;

    public IEnumerable<TickMark> GetTickMarks(double availableSize)
    {
      _tickMarks.Clear();

      var currentSize = 0.0;
      var currentSubDivision = 0;

      while (currentSize < availableSize)
      {
        if (currentSubDivision == 0)
          currentSubDivision = SubDivisionsCount;

        _tickMarks.Add(new TickMark
        {
          Label = (currentSize + Start).ToString(CultureInfo.InvariantCulture),
          Template = currentSubDivision == SubDivisionsCount ? MajorTickMarkTemplate : MinorTickMarkTemplate,
          Size = 1,
          Offset = currentSize
        });

        currentSubDivision--;
        currentSize += Step;
      }

      return _tickMarks;
    }

    #endregion

    #endregion
  }


  public class TickMarkLevel
  {
    #region Properties

    public DataTemplate TickMarkTemplate { get; set; }

    #endregion
  }

  public class TickMarkLevelCollection : ObservableCollection<TickMarkLevel>
  {
  }

  public class TickBarProvider : ITickBarProvider
  {
    #region Fields

    private readonly List<TickMark> _tickMarks = new List<TickMark>();
    private int _levelDivisionsCount;
    private double _start;
    private double _step;

    #endregion

    #region Ctors

    public TickBarProvider()
    {
      TickMarkLevels = new TickMarkLevelCollection();
      TickMarkLevels.CollectionChanged += (sender, args) => OnChanged();
    }

    public TickBarProvider(TickMarkLevelCollection tickMarkLevels)
    {
      if (tickMarkLevels == null)
        throw new ArgumentNullException(nameof(tickMarkLevels));

      TickMarkLevels = tickMarkLevels;
      TickMarkLevels.CollectionChanged += (sender, args) => OnChanged();
    }

    #endregion

    #region Properties

    public int LevelDivisionsCount
    {
      get => _levelDivisionsCount;
      set
      {
        _levelDivisionsCount = value;
        OnChanged();
      }
    }

    public double Start
    {
      get => _start;
      set
      {
        _start = value;
        OnChanged();
      }
    }

    public double Step
    {
      get => _step;
      set
      {
        _step = value;
        OnChanged();
      }
    }

    public TickMarkLevelCollection TickMarkLevels { get; }

    #endregion

    #region  Methods

    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region Interface Implementations

    #region ITickBarProvider

    public event EventHandler Changed;

    public IEnumerable<TickMark> GetTickMarks(double availableSize)
    {
      _tickMarks.Clear();

      if (DoubleUtils.IsZero(Step))
        return _tickMarks;

      var templates = TickMarkLevels.Select(l => l.TickMarkTemplate).Reverse().ToArray();

      var index = (int) Start / (int) Step;
      var div = LevelDivisionsCount;
      var ranks = Enumerable.Range(0, templates.Length).Select(i => (int) Math.Pow(div, i)).ToArray();

      var step = Step;
      var deltaOffset = Start % step;
      var current = 0.0;
      while (current + deltaOffset < availableSize)
      {
        for (var iBrush = templates.Length - 1; iBrush >= 0; iBrush--)
        {
          if (index % ranks[iBrush] != 0) continue;

          var tickMark = new TickMark
          {
            Label = (current + Start).ToString(CultureInfo.InvariantCulture),
            Template = templates[iBrush],
            Size = 1,
            Offset = current + deltaOffset
          };

          _tickMarks.Add(tickMark);

          break;
        }

        index++;
        current += step;
      }

      return _tickMarks;
    }

    #endregion

    #endregion
  }

  public class TickMark : DependencyObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty LabelProperty = DPM.Register<string, TickMark>
      ("Label");

    public static readonly DependencyProperty TemplateProperty = DPM.Register<DataTemplate, TickMark>
      ("Template");

    public static readonly DependencyProperty OffsetProperty = DPM.Register<double, TickMark>
      ("Offset");

    public static readonly DependencyProperty SizeProperty = DPM.Register<double, TickMark>
      ("Size");

    #endregion

    #region Properties

    public string Label
    {
      get => (string) GetValue(LabelProperty);
      set => SetValue(LabelProperty, value);
    }

    public double Offset
    {
      get => (double) GetValue(OffsetProperty);
      set => SetValue(OffsetProperty, value);
    }

    public double Size
    {
      get => (double) GetValue(SizeProperty);
      set => SetValue(SizeProperty, value);
    }

    public DataTemplate Template
    {
      get => (DataTemplate) GetValue(TemplateProperty);
      set => SetValue(TemplateProperty, value);
    }

    #endregion
  }

  public class TickBarPanel : Panel
  {
    #region Static Fields and Constants

    private static readonly DependencyProperty TickMarkProperty = DPM.RegisterAttached<TickMark, TickBarPanel>
      ("TickMark");

    #endregion

    #region Properties

    internal TickBar TickBar { get; set; }

    #endregion

    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      foreach (var tickMarkControl in Children.Cast<FrameworkElement>())
      {
        var tickMark = GetTickMark(tickMarkControl);
        var tickMarkRect = new Rect(tickMark.Offset, 0, tickMark.Size, finalSize.Height);

        tickMarkControl.Arrange(tickMarkRect);
      }

      return finalSize;
    }

    private static TickMark GetTickMark(DependencyObject element)
    {
      return (TickMark) element.GetValue(TickMarkProperty);
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var isHorizontal = TickBar.Return(t => t.Orientation.IsHorizontal());

      if (isHorizontal && double.IsInfinity(availableSize.Width))
        return XamlConstants.ZeroSize;

      if (!isHorizontal && double.IsInfinity(availableSize.Height))
        return XamlConstants.ZeroSize;

      var tickMarks = TickBar.With(t => t.Provider).Return(t => t.GetTickMarks(isHorizontal ? availableSize.Width : availableSize.Height).ToList());

      Children.Clear();

      if (tickMarks == null || tickMarks.Count == 0)
        return XamlConstants.ZeroSize;

      var size = XamlConstants.ZeroSize;

      foreach (var tickMark in tickMarks)
      {
        var tickMarkControl = tickMark.Template.Return(t => (FrameworkElement) t.LoadContent(), new Canvas());

        SetTickMark(tickMarkControl, tickMark);
        tickMarkControl.DataContext = tickMark;

        tickMarkControl.Measure(XamlConstants.InfiniteSize);

        if (isHorizontal)
        {
          size.Height = Math.Max(size.Height, tickMarkControl.DesiredSize.Height);
          size.Width += tickMark.Offset + tickMark.Size;
        }
        else
        {
          size.Width = Math.Max(size.Width, tickMarkControl.DesiredSize.Width);
          size.Height += tickMark.Offset + tickMark.Size;
        }

        Children.Add(tickMarkControl);
      }

      var lastTickMark = tickMarks.Last();

      if (isHorizontal)
        size.Width = lastTickMark.Offset + lastTickMark.Size;
      else
        size.Height = lastTickMark.Offset + lastTickMark.Size;

      return size;
    }

    private static void SetTickMark(DependencyObject element, TickMark value)
    {
      element.SetValue(TickMarkProperty, value);
    }

    #endregion
  }

  public class TickBar : Control
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty OrientationProperty = DPM.Register<Orientation, TickBar>
      ("Orientation");

    public static readonly DependencyProperty ProviderProperty = DPM.Register<ITickBarProvider, TickBar>
      ("Provider", t => t.OnProviderChanged);

    #endregion

    #region Fields

    private TickBarPanel _tickBarPanel;

    #endregion

    #region Ctors

    static TickBar()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<TickBar>();
    }

    public TickBar()
    {
      this.OverrideStyleKey<TickBar>();
    }

    #endregion

    #region Properties

    public Orientation Orientation
    {
      get => (Orientation) GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
    }

    public ITickBarProvider Provider
    {
      get => (ITickBarProvider) GetValue(ProviderProperty);
      set => SetValue(ProviderProperty, value);
    }

    #endregion

    #region  Methods

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      _tickBarPanel = (TickBarPanel) GetTemplateChild("TickBarPanel");

      _tickBarPanel.TickBar = this;
    }

    private void OnProviderChanged(ITickBarProvider oldProvider, ITickBarProvider newProvider)
    {
      if (oldProvider != null)
        oldProvider.Changed -= OnProviderStateChanged;

      if (newProvider != null)
        newProvider.Changed += OnProviderStateChanged;
    }

    private void OnProviderStateChanged(object sender, EventArgs eventArgs)
    {
      InvalidateMeasure();
    }

    #endregion
  }
}