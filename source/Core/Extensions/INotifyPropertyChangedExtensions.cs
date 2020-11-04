// <copyright file="INotifyPropertyChangedExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using Zaaml.Core.Disposable;
using Zaaml.Core.Monads;

namespace Zaaml.Core.Extensions
{
// ReSharper disable once InconsistentNaming
  internal static class INotifyPropertyChangedExtensions
  {
    #region  Methods

    public static void InvokeOnPropertyChanged<T>(this T target, string propertyName, Action<T> action)
      where T : INotifyPropertyChanged
    {
      target.Listener(propertyName, disposable =>
      {
        action(target);
        disposable.Dispose();
      });
    }

    public static void InvokeOnPropertyChanged<T>(this T target, string propertyName, Action action)
      where T : INotifyPropertyChanged
    {
      target.Listener(propertyName, disposable =>
      {
        action();
        disposable.Dispose();
      });
    }

    public static void InvokeOnPropertyChanged<T>(this T target, string propertyName, Func<T, bool> action)
      where T : INotifyPropertyChanged
    {
      target.Listener(propertyName, disposable => action(target).Do(disposable.Dispose));
    }

    public static void InvokeOnPropertyChanged<T>(this T target, string propertyName, Func<bool> action)
      where T : INotifyPropertyChanged
    {
      target.Listener(propertyName, disposable => action().Do(disposable.Dispose));
    }

    public static IDisposable Listener(this INotifyPropertyChanged target, string propertyName, Action action)
    {
      PropertyChangedEventHandler handler = (sender, args) => args.PropertyName.If(p => string.Equals(propertyName, p)).Do(action);
      target.PropertyChanged += handler;
      return new DelegateDisposable(() => target.PropertyChanged -= handler);
    }

    public static IDisposable Listener(this INotifyPropertyChanged target, string propertyName, Action<IDisposable> action)
    {
      IDisposable disposable = null;
      disposable = target.Listener(propertyName, () => action(disposable));
      return disposable;
    }

    #endregion
  }
}