// <copyright file="CommandExtension.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows.Data;
using Zaaml.PresentationCore.MarkupExtensions;

namespace Zaaml.PresentationCore.CommandCore
{
  public class CommandExtension : MarkupExtensionBase
  {
    #region Properties

    public Binding Command { get; set; }
    public Binding CommandTarget { get; set; }

    #endregion

    #region  Methods

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      //var provideValueTarget = (IProvideValueTarget) serviceProvider.GetService(typeof (IProvideValueTarget));
      //return Command is RoutedCommand || Command is Binding ? new CommandWrapper(Command, (DependencyObject) provideValueTarget.TargetObject, CommandTarget) : Command;

      return null;
    }

    #endregion
  }
}