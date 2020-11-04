// <copyright file="VisualCloneControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.TemplateCore;
using Zaaml.UI.Controls.Core;

namespace Zaaml.UI.Controls.Primitives
{
  public sealed class VisualCloneControl : FixedTemplateControlBase
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty SourceProperty = DPM.Register<FrameworkElement, VisualCloneControl>
      ("Source", c => c.OnSourceChanged);


    public static readonly DependencyProperty ModeProperty = DPM.Register<VisualBrushMode, VisualCloneControl>
      ("Mode", VisualBrushMode.RefreshOnRender);

    #endregion

    #region Fields

    [UsedImplicitly] private readonly BindActualSizeBehavior _bindActualSizeBehavior;
    private readonly VisualBrushBehavior _visualBrushBehavior;
    private Border _border;

    #endregion

    #region Ctors

    public VisualCloneControl()
    {
      TemplateInt = GenericControlTemplate.BorderTemplateInstance;

      _visualBrushBehavior = new VisualBrushBehavior();
      _visualBrushBehavior.BindProperties(VisualBrushBehavior.ModeProperty, this, ModeProperty);
      _visualBrushBehavior.BindProperties(VisualBrushBehavior.SourceProperty, this, SourceProperty);

      _bindActualSizeBehavior = new BindActualSizeBehavior();

      this.AddBehavior(_visualBrushBehavior);
      this.AddBehavior(_bindActualSizeBehavior);
    }

    #endregion

    #region Properties

    private Border Border
    {
      set
      {
        if (_border != null)
          _border.Background = null;

        _border = value;

        _border?.SetBinding(Border.BackgroundProperty, new Binding
        {
          Path = new PropertyPath(VisualBrushBehavior.BrushProperty),
          Source = _visualBrushBehavior
        });
      }
    }

    public VisualBrushMode Mode
    {
      get => (VisualBrushMode) GetValue(ModeProperty);
      set => SetValue(ModeProperty, value);
    }

    public FrameworkElement Source
    {
      get => (FrameworkElement) GetValue(SourceProperty);
      set => SetValue(SourceProperty, value);
    }

    #endregion

    #region  Methods

    protected override void ApplyTemplateOverride()
    {
      base.ApplyTemplateOverride();
      Border = this.GetImplementationRoot() as Border;
    }

    private void OnSourceChanged(FrameworkElement oldSource, FrameworkElement newSource)
    {
      //if (oldSource != null)
      //{
      //  ClearValue(HorizontalAlignmentProperty);
      //  ClearValue(VerticalAlignmentProperty);
      //}

      //if (newSource != null)
      //{
      //  this.BindProperties(HorizontalAlignmentProperty, newSource, HorizontalAlignmentProperty);
      //  this.BindProperties(VerticalAlignmentProperty, newSource, VerticalAlignmentProperty);
      //}

      _bindActualSizeBehavior.Source = Source;
    }

    #endregion
  }
}