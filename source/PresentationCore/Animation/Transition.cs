// <copyright file="Transition.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.Animation
{
  [TypeConverter(typeof(TransitionTypeConverter))]
  [ContentProperty(nameof(EasingFunction))]
  public class Transition
  {
    #region Properties

    public TimeSpan BeginTime { get; set; }

    public Duration Duration { get; set; }

    public IEasingFunction EasingFunction { get; set; }

    #endregion

    #region  Methods

    internal static bool TryParse(string strValue, out Transition transition)
    {
      transition = null;

      TimeSpan timeSpan;
      if (TimeSpan.TryParse(strValue, CultureInfo.InvariantCulture, out timeSpan))
      {
        transition = new Transition
        {
          Duration = timeSpan
        };

        return true;
      }
      return false;
    }

    #endregion
  }

  public sealed class TransitionExtension : MarkupExtensionBase
  {
    #region Fields

    private Transition _transition;

    #endregion

    #region Properties

    private Transition ActualTransition => _transition ?? (_transition = new Transition());

    public TimeSpan BeginTime
    {
      get => _transition?.BeginTime ?? default(TimeSpan);
      set => ActualTransition.BeginTime = value;
    }

    public Duration Duration
    {
      get => _transition?.Duration ?? default(Duration);

      set => ActualTransition.Duration = value;
    }

    public IEasingFunction EasingFunction
    {
      get => _transition?.EasingFunction;
      set => ActualTransition.EasingFunction = value;
    }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return _transition;
    }

    #endregion
  }

  internal sealed class TransitionTypeConverter : TypeConverter
  {
    #region  Methods

    public override bool CanConvertFrom(ITypeDescriptorContext td, Type t)
    {
      return t == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var strValue = value as string;

      if (strValue == null)
        return base.ConvertFrom(context, culture, value);

      Transition transition;

      if (Transition.TryParse(strValue, out transition))
        return transition;

      return base.ConvertFrom(context, culture, value);
    }

    #endregion
  }
}