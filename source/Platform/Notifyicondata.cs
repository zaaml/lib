// <copyright file="Point.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

#if COMPILE_EVER


using System;
using System.Runtime.InteropServices;

namespace Zaaml.Platform
{
  public enum NotifyIconMessage : uint
  {
    Add = 0x0,
    Modify = 0x1,
    Delete = 0x2,
    SetFocus = 0x3,
    SetVersion = 0x4,
  }

  [Flags]
  public enum NotifyIconFlags : uint
  {
    Message = 0x1,
    Icon = 0x2,
    Tip = 0x4,
    State = 0x8,
    Info = 0x10,
    Guid = 0x20,
    Realtime = 0x40,
    Showtip = 0x80
  }

  public enum NotifyIconState
  {
    Visible = 0x0,
    Hidden = 0x1,
    SharedIcon = 0x2
  }

  [Flags]
  public enum NotifyIconInfoFlags : uint
  {
    None = 0x0,
    Info = 0x1,
    Warning = 0x2,
    Error = 0x3,
    User = 0x4,
    IconMask = 0x0F,
    NoSound = 0x10,
    LargeIcon = 0x20,
    RespectQuietTime = 0x80
  }


  public enum NotifyIconVersion : uint
  {
    Win95 = 0x0,
    Win2000 = 0x3,
    Vista = 0x4
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct NotifyIconData
  {
    public uint Size;
    public IntPtr WindowHandle;
    public uint TaskbarIconId;
    public NotifyIconFlags Flags;
    public uint CallbackMessageId;
    public IntPtr IconHandle;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string Tip;
    public NotifyIconState State;
    public NotifyIconState StateMask;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string Info;
    public uint VersionOrTimeout;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)] public string InfoTitle;
    public NotifyIconInfoFlags InfoFlags;
    public Guid Guid;
    public IntPtr BalloonIconHandle;

    public static NotifyIconData CreateDefault(IntPtr hwnd, uint callbackMessageId)
    {
      var data = new NotifyIconData();

      if (Environment.OSVersion.Version.Major >= 6)
        data.Size = (uint) Marshal.SizeOf(data);
      else
        data.Size = 952;

      data.WindowHandle = hwnd;
      data.TaskbarIconId = 0x0;
      data.CallbackMessageId = callbackMessageId;
      data.VersionOrTimeout = (uint) NotifyIconVersion.Win95;
      data.IconHandle = IntPtr.Zero;
      data.State = NotifyIconState.Hidden;
      data.StateMask = NotifyIconState.Hidden;
      data.Flags = NotifyIconFlags.Message | NotifyIconFlags.Icon | NotifyIconFlags.Tip;
      data.Tip = "";
      data.Info = "";
      data.InfoTitle = "";

      return data;
    }
  }
}

#endif