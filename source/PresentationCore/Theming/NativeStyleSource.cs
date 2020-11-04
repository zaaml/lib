// <copyright file="NativeStyleSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Windows.Data;
using Zaaml.Core;

namespace Zaaml.PresentationCore.Theming
{
  public class NativeStyleSource : INotifyPropertyChanged
  {
    #region Fields

    private readonly StyleService _styleService;

    #endregion

    #region Ctors

    internal NativeStyleSource(StyleService styleService)
    {
      _styleService = styleService;
      StyleBinding = new Binding(nameof(Style)) {Source = this};
    }

    #endregion

    #region Properties

    public System.Windows.Style Style => _styleService.NativeStyle;

    internal Binding StyleBinding { get; }

    #endregion

    #region  Methods

    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    internal void RaiseStyleChanged()
    {
      OnPropertyChanged(nameof(Style));
    }

    #endregion

    #region Interface Implementations

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #endregion
  }
}