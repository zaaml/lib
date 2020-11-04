// <copyright file="MouseButtonEventArgsExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Input;

namespace Zaaml.PresentationCore.Input
{
  internal static class MouseButtonEventArgsExtensions
  {
    #region  Methods

    public static MouseButtonEventArgsInt ToMouseButtonEventArgsInt(this MouseButtonEventArgs args, MouseButton button, MouseButtonState buttonState)
    {
      return new MouseButtonEventArgsInt(args, button, buttonState);
    }

#if !SILVERLIGHT
    public static MouseButtonEventArgsInt ToMouseButtonEventArgsInt(this MouseButtonEventArgs args)
    {
      return new MouseButtonEventArgsInt(args);
    }
#endif


    public static MouseEventArgsInt ToMouseEventArgsInt(this MouseEventArgs args)
    {
#if SILVERLIGHT
      return new MouseEventArgsInt(false, args.OriginalSource);
#else
      return new MouseEventArgsInt(args.Handled, args.OriginalSource);
#endif
    }

    public static MouseWheelEventArgsInt ToMouseWheelEventArgsInt(this MouseWheelEventArgs args)
    {
      return new MouseWheelEventArgsInt(args.Handled, args.OriginalSource, args.Delta);
    }

    #endregion
  }
}