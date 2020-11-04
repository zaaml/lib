// <copyright file="MouseButtonEventArgsInt.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Input;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Input
{
  internal class MouseButtonEventArgsInt : MouseEventArgsInt
  {
	  private Point? _screenPosition;

	  #region Ctors

    internal MouseButtonEventArgsInt(MouseButtonEventArgs args, MouseButton button, MouseButtonState buttonState)
      : base(args.Handled, args.OriginalSource)
    {
      OriginalArgs = args;
      Button = button;
      ButtonState = buttonState;
      ClickCount = args.ClickCount;


    }

    internal MouseButtonEventArgs OriginalArgs { get; }

#if !SILVERLIGHT
    internal MouseButtonEventArgsInt(MouseButtonEventArgs args) : base(args.Handled, args.OriginalSource)
    {
      OriginalArgs = args;
      Button = args.ChangedButton;
      ButtonState = args.ButtonState;
      ClickCount = args.ClickCount;
    }
#endif

    public override bool Handled
    {
      get => OriginalArgs.Handled;
      set => OriginalArgs.Handled = value;
    }

    internal MouseButtonEventArgsInt(MouseButton button, MouseButtonState buttonState, int clickCount, bool handled, object originalSource) : base(handled, originalSource)
    {
      Button = button;
      ButtonState = buttonState;
      ClickCount = clickCount;
    }

    #endregion

    #region Properties

	  public Point ScreenPosition
	  {
		  get
		  {
			  if (_screenPosition == null)
			  {
				  var uie = PresentationTreeUtils.GetUIElementEventSource(OriginalArgs.OriginalSource);

				  _screenPosition = uie != null ? UIElementTransformUtils.TransformToScreen(uie).Transform(OriginalArgs.GetPosition(uie)) : new Point();
			  }

			  return _screenPosition.Value;
		  }
	  }

	  public MouseButton Button { get; }

    public MouseButtonState ButtonState { get; }

    public int ClickCount { get; }

    #endregion
  }
}