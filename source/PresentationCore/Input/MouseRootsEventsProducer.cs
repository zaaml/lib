// <copyright file="MouseRootsEventsProducer.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Input
{
  internal sealed partial class MouseRootsEventsProducer : GlobalMouseEventsProducerBase
  {
    #region Static Fields and Constants

    public static readonly IGlobalMouseEventsProducer Instance = new MouseRootsEventsProducer();

    #endregion

    #region Ctors

    static MouseRootsEventsProducer()
    {
      CompositionTarget.Rendering += CompositionTargetOnRendering;
    }

    private MouseRootsEventsProducer()
    {
    }

    #endregion

    #region Properties

    private static MouseRootsEventsProducer InstanceCore => (MouseRootsEventsProducer) Instance;

    #endregion

    #region  Methods

    private static void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
    {
      foreach (var visualRoot in PresentationTreeUtils.EnumerateVisualRoots().OfType<UIElement>())
        EnsureService(visualRoot);
    }

    private static void EnsureService(UIElement visualRoot)
    {
      if (visualRoot.Dispatcher.CheckAccess())
        visualRoot.GetServiceOrCreate(() => new MouseServiceInt());
      else
        visualRoot.Dispatcher.BeginInvoke(() => EnsureService(visualRoot));
    }

    #endregion
  }
}