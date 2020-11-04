// <copyright file="LogService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Diagnostics;

namespace Zaaml.Core
{
  internal interface ILogger
  {
    #region  Methods

    void LogEntry(string logInformation, LogEntryKind entryKind);

    void LogError(Exception exception);

    #endregion
  }

  internal enum LogEntryKind
  {
    Error,
    Warning,
    Info
  }

  internal class SimpleDebuggerLogger : ILogger
  {
    #region Static Fields and Constants

    public static readonly ILogger Instance = new SimpleDebuggerLogger();

    #endregion

    #region Ctors

    private SimpleDebuggerLogger()
    {
    }

    #endregion

    #region  Methods

    private void OutputString(string str)
    {
      Debug.WriteLine(str);
    }

    #endregion

    #region Interface Implementations

    #region ILogger

    public void LogError(Exception exception)
    {
      OutputString(exception.ToString());
    }

    public void LogEntry(string logInformation, LogEntryKind entryKind)
    {
      OutputString($"{entryKind}: {logInformation}");
    }

    #endregion

    #endregion
  }

  internal static class LogService
  {
    #region Static Fields and Constants

    public static ILogger Logger = SimpleDebuggerLogger.Instance;

    #endregion

    #region  Methods

    public static void LogError(Exception exception)
    {
      Logger.LogError(exception);
    }

    public static void LogError(string logInformation)
    {
      Logger.LogEntry(logInformation, LogEntryKind.Error);
    }

    public static void LogInfo(string logInformation)
    {
      Logger.LogEntry(logInformation, LogEntryKind.Info);
    }

    public static void LogWarning(string logInformation)
    {
      Logger.LogEntry(logInformation, LogEntryKind.Warning);
    }

    #endregion
  }
}