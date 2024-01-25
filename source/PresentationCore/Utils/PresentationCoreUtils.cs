// <copyright file="PresentationCoreUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
#if SILVERLIGHT
#else
using System.Reflection;
using System;
using Zaaml.Platform;

#endif

namespace Zaaml.PresentationCore.Utils
{
  internal static class PresentationCoreUtils
  {
    #region Static Fields and Constants

#if !SILVERLIGHT
    private const int LastParameterCache = 1;
    private const int WindowResizeBorderThicknessCache = 0;
    private static readonly Version PresentationFrameworkVersion = Assembly.GetAssembly(typeof(Window)).GetName().Version;
    private static readonly bool[] ParamsValidCache = new bool[LastParameterCache];
    private static Thickness _windowResizeBorderThickness;
#endif

    #endregion

    #region Properties

#if SILVERLIGHT
    public static bool IsPresentationFrameworkVersionLessThan4 => false;
#else
    public static bool IsPresentationFrameworkVersionLessThan4 => PresentationFrameworkVersion < new Version(4, 0);
#endif

    public static Thickness WindowResizeBorderThickness
    {
      get
      {
#if SILVERLIGHT
        return new Thickness(0);
#else
        lock (ParamsValidCache)
        {
          if (ParamsValidCache[WindowResizeBorderThicknessCache])
	          return _windowResizeBorderThickness;

          var frameSize = DpiUtils.DeviceSizeToLogical(new Size(NativeMethods.GetSystemMetrics(SM.CXFRAME), NativeMethods.GetSystemMetrics(SM.CYFRAME)));
          _windowResizeBorderThickness = new Thickness(frameSize.Width, frameSize.Height, frameSize.Width, frameSize.Height);
          ParamsValidCache[WindowResizeBorderThicknessCache] = true;
        }
        return _windowResizeBorderThickness;
#endif
      }
    }

    public static bool IsInDesignMode
    {
      get
      {
#if SILVERLIGHT
        return DesignerProperties.IsInDesignTool;
#else
        return (bool) DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue;
#endif
      }
    }

    public static void LaunchDebugger()
    {
#if DEBUG 
      if (IsInDesignMode == false)
        return;

#if !SILVERLIGHT
      if (Debugger.IsAttached == false && Keyboard.GetKeyStates(Key.Scroll) == KeyStates.Toggled)
        Debugger.Launch();
#endif

      if (Debugger.IsAttached == false && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
        Debugger.Launch();
#endif
    }

    // ReSharper disable once MemberCanBeMadeStatic.Local
    public static object FreezeValue(object value)
    {
	    if (value is Freezable { IsFrozen: false, CanFreeze: true } freezable)
        freezable.Freeze();

      return value;
    }

    #endregion
  }
}