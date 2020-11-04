// <copyright file="CompositeTransform.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Data;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore
{
  internal sealed class CompositeTransform : DependencyObject
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ScaleXProperty = DPM.Register<double, CompositeTransform>
      ("ScaleX", 1.0);

    public static readonly DependencyProperty ScaleYProperty = DPM.Register<double, CompositeTransform>
      ("ScaleY", 1.0);

    public static readonly DependencyProperty TranslateXProperty = DPM.Register<double, CompositeTransform>
      ("TranslateX", 0.0);

    public static readonly DependencyProperty TranslateYProperty = DPM.Register<double, CompositeTransform>
      ("TranslateY", 0.0);

    public static readonly DependencyProperty SkewXProperty = DPM.Register<double, CompositeTransform>
      ("SkewX", 0.0);

    public static readonly DependencyProperty SkewYProperty = DPM.Register<double, CompositeTransform>
      ("SkewY", 0.0);

    public static readonly DependencyProperty CenterXProperty = DPM.Register<double, CompositeTransform>
      ("CenterX", 0.0);

    public static readonly DependencyProperty CenterYProperty = DPM.Register<double, CompositeTransform>
      ("CenterY", 0.0);

    public static readonly DependencyProperty RotationProperty = DPM.Register<double, CompositeTransform>
      ("Rotation", 0.0);

    #endregion

    #region Fields

    private readonly TransformGroup _transform;

    #endregion

    #region Ctors

    public CompositeTransform()
    {
      var scaleTransform = new ScaleTransform();
      var rotateTransform = new RotateTransform();
      var translateTransform = new TranslateTransform();
      var skewTransform = new SkewTransform();

      scaleTransform.BindProperties(ScaleTransform.CenterXProperty, this, CenterXProperty);
      scaleTransform.BindProperties(ScaleTransform.CenterYProperty, this, CenterYProperty);
      scaleTransform.BindProperties(ScaleTransform.ScaleXProperty, this, ScaleXProperty);
      scaleTransform.BindProperties(ScaleTransform.ScaleYProperty, this, ScaleYProperty);

      rotateTransform.BindProperties(RotateTransform.CenterXProperty, this, CenterXProperty);
      rotateTransform.BindProperties(RotateTransform.CenterYProperty, this, CenterYProperty);
      rotateTransform.BindProperties(RotateTransform.AngleProperty, this, RotationProperty);

      translateTransform.BindProperties(TranslateTransform.XProperty, this, TranslateXProperty);
      translateTransform.BindProperties(TranslateTransform.YProperty, this, TranslateYProperty);

      skewTransform.BindProperties(SkewTransform.CenterXProperty, this, CenterXProperty);
      skewTransform.BindProperties(SkewTransform.CenterYProperty, this, CenterYProperty);
      skewTransform.BindProperties(SkewTransform.AngleXProperty, this, SkewXProperty);
      skewTransform.BindProperties(SkewTransform.AngleYProperty, this, SkewYProperty);

      _transform = new TransformGroup
      {
        Children =
        {
          scaleTransform,
          skewTransform,
          rotateTransform,
          translateTransform
        }
      };
    }

    #endregion

    #region Properties

    public double CenterX
    {
      get => (double) GetValue(CenterXProperty);
      set => SetValue(CenterXProperty, value);
    }

    public double CenterY
    {
      get => (double) GetValue(CenterYProperty);
      set => SetValue(CenterYProperty, value);
    }

    public double Rotation
    {
      get => (double) GetValue(RotationProperty);
      set => SetValue(RotationProperty, value);
    }

    public double ScaleX
    {
      get => (double) GetValue(ScaleXProperty);
      set => SetValue(ScaleXProperty, value);
    }

    public double ScaleY
    {
      get => (double) GetValue(ScaleYProperty);
      set => SetValue(ScaleYProperty, value);
    }

    public double SkewX
    {
      get => (double) GetValue(SkewXProperty);
      set => SetValue(SkewXProperty, value);
    }

    public double SkewY
    {
      get => (double) GetValue(SkewYProperty);
      set => SetValue(SkewYProperty, value);
    }

    public Transform Transform => _transform;

    public double TranslateX
    {
      get => (double) GetValue(TranslateXProperty);
      set => SetValue(TranslateXProperty, value);
    }

    public double TranslateY
    {
      get => (double) GetValue(TranslateYProperty);
      set => SetValue(TranslateYProperty, value);
    }

    #endregion
  }
}