// <copyright file="DropGuide.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Theming;
using ZaamlContentControl = System.Windows.Controls.ContentControl;

namespace Zaaml.UI.Controls.Docking
{
  [Flags]
  public enum DropGuideAction
  {
    Undefined = 0,

    SplitLeft = 0x1,
    SplitTop = 0x2,
    SplitRight = 0x4,
    SplitBottom = 0x8,

    SplitAll = SplitLeft | SplitTop | SplitRight | SplitBottom,

    SplitDocumentLeft = 0x10,
    SplitDocumentTop = 0x20,
    SplitDocumentRight = 0x40,
    SplitDocumentBottom = 0x80,

    SplitDocumentHorizontal = SplitDocumentLeft | SplitDocumentRight,
    SplitDocumentVertical = SplitDocumentTop | SplitDocumentBottom,

    SplitDocumentAll = SplitDocumentLeft | SplitDocumentTop | SplitDocumentRight | SplitDocumentBottom,

    AutoHideLeft = 0x100,
    AutoHideTop = 0x200,
    AutoHideRight = 0x400,
    AutoHideBottom = 0x800,

    AutoHideAll = AutoHideLeft | AutoHideTop | AutoHideRight | AutoHideBottom,

    TabLeft = 0x1000,
    TabTop = 0x2000,
    TabRight = 0x4000,
    TabBottom = 0x8000,
    TabCenter = 0x10000,

    TabAll = TabLeft | TabTop | TabRight | TabBottom | TabCenter,

    DockLeft = 0x100000,
    DockTop = 0x200000,
    DockRight = 0x400000,
    DockBottom = 0x800000,

    DockAll = DockLeft | DockTop | DockRight | DockBottom,

    All = SplitAll | SplitDocumentAll | AutoHideAll | TabAll | DockAll
  }

  public enum DropGuideActionType
  {
    Undefined,
    Split,
    SplitDocument,
    AutoHide,
    Dock,
    Tab
  }

  public class DropGuide : ZaamlContentControl, INotifyPropertyChanged
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty ActionProperty = DPM.Register<DropGuideAction, DropGuide>
      ("Action");

    private static readonly DependencyPropertyKey IsAllowedPropertyKey = DPM.RegisterReadOnly<bool, DropGuide>
      ("IsAllowed");

    private static readonly DependencyPropertyKey IsActivePropertyKey = DPM.RegisterReadOnly<bool, DropGuide>
      ("IsActive");

    public static readonly DependencyProperty IsActiveProperty = IsActivePropertyKey.DependencyProperty;

    public static readonly DependencyProperty IsAllowedProperty = IsAllowedPropertyKey.DependencyProperty;

    #endregion

    #region Ctors

    static DropGuide()
    {
      DefaultStyleKeyHelper.OverrideStyleKey<DropGuide>();
    }

    public DropGuide()
    {
      this.OverrideStyleKey<DropGuide>();
    }

    #endregion

    #region Properties

    public DropGuideAction Action
    {
      get => (DropGuideAction) GetValue(ActionProperty);
      set => SetValue(ActionProperty, value);
    }

    internal DropCompass Compass { get; set; }

    public bool IsActive
    {
      get => (bool) GetValue(IsActiveProperty);
      internal set => this.SetReadOnlyValue(IsActivePropertyKey, value);
    }

    public bool IsAllowed
    {
      get => (bool) GetValue(IsAllowedProperty);
      internal set => this.SetReadOnlyValue(IsAllowedPropertyKey, value);
    }

    public Dock? Side => GetGuideSide(Action);

    #endregion

    #region  Methods

    public static DropGuideActionType GetActionType(DropGuideAction dropGuideAction)
    {
      switch (dropGuideAction)
      {
        case DropGuideAction.SplitLeft:
        case DropGuideAction.SplitTop:
        case DropGuideAction.SplitRight:
        case DropGuideAction.SplitBottom:
          return DropGuideActionType.Split;

        case DropGuideAction.SplitDocumentLeft:
        case DropGuideAction.SplitDocumentTop:
        case DropGuideAction.SplitDocumentRight:
        case DropGuideAction.SplitDocumentBottom:
          return DropGuideActionType.SplitDocument;

        case DropGuideAction.AutoHideLeft:
        case DropGuideAction.AutoHideTop:
        case DropGuideAction.AutoHideRight:
        case DropGuideAction.AutoHideBottom:
          return DropGuideActionType.AutoHide;

        case DropGuideAction.TabLeft:
        case DropGuideAction.TabTop:
        case DropGuideAction.TabRight:
        case DropGuideAction.TabBottom:
        case DropGuideAction.TabCenter:
          return DropGuideActionType.Tab;

        case DropGuideAction.DockLeft:
        case DropGuideAction.DockTop:
        case DropGuideAction.DockRight:
        case DropGuideAction.DockBottom:
          return DropGuideActionType.Dock;
      }

      return DropGuideActionType.Undefined;
    }

    public static Dock? GetGuideSide(DropGuideAction dropGuideAction)
    {
      switch (dropGuideAction)
      {
        case DropGuideAction.SplitLeft:
        case DropGuideAction.SplitDocumentLeft:
        case DropGuideAction.AutoHideLeft:
        case DropGuideAction.TabLeft:
        case DropGuideAction.DockLeft:
          return Dock.Left;

        case DropGuideAction.SplitTop:
        case DropGuideAction.SplitDocumentTop:
        case DropGuideAction.AutoHideTop:
        case DropGuideAction.TabTop:
        case DropGuideAction.DockTop:
          return Dock.Top;

        case DropGuideAction.SplitRight:
        case DropGuideAction.SplitDocumentRight:
        case DropGuideAction.AutoHideRight:
        case DropGuideAction.TabRight:
        case DropGuideAction.DockRight:
          return Dock.Right;

        case DropGuideAction.SplitBottom:
        case DropGuideAction.SplitDocumentBottom:
        case DropGuideAction.AutoHideBottom:
        case DropGuideAction.TabBottom:
        case DropGuideAction.DockBottom:
          return Dock.Bottom;

        case DropGuideAction.TabCenter:
          return null;
      }

      return null;
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Interface Implementations

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #endregion
  }
}