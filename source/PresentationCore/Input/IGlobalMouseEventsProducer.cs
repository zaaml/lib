// <copyright file="IGlobalMouseEventsProducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Input
{
  internal interface IGlobalMouseEventsProducer
  {
    #region Fields

    event MouseEventHandlerInt MouseMove;

    event MouseButtonEventHandlerInt MouseLeftButtonDown;
    event MouseButtonEventHandlerInt MouseLeftButtonUp;
    event MouseButtonEventHandlerInt MouseRightButtonDown;
    event MouseButtonEventHandlerInt MouseRightButtonUp;

    event MouseButtonEventHandlerInt PreviewMouseLeftButtonDown;
    event MouseButtonEventHandlerInt PreviewMouseLeftButtonUp;
    event MouseButtonEventHandlerInt PreviewMouseRightButtonDown;
    event MouseButtonEventHandlerInt PreviewMouseRightButtonUp;

    #endregion
  }
}