// <copyright file="BindingSource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Data;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Data
{
  internal struct BindingSource
  {
    #region Fields

    private ActualSource _actualSource;
    private object _sourceStore;

    #endregion

    #region Properties

    public string ElementName
    {
      get => _actualSource == ActualSource.ElementName ? (string) _sourceStore : null;
      set
      {
        _sourceStore = value;
        _actualSource = ActualSource.ElementName;
      }
    }

    public bool BindsDirectlyToSource { get; set; }

    public RelativeSource RelativeSource
    {
      get => _actualSource == ActualSource.RelativeSource ? (RelativeSource) _sourceStore : null;
      set
      {
        _sourceStore = value;
        _actualSource = ActualSource.RelativeSource;
      }
    }

    public object Source
    {
      get => _actualSource == ActualSource.Source ? _sourceStore : null;
      set
      {
        _sourceStore = value;
        _actualSource = ActualSource.Source;
      }
    }

    #endregion

    #region  Methods

    public void InitSource(NativeBinding binding)
    {
      switch (_actualSource)
      {
        case ActualSource.None:
          break;
        case ActualSource.RelativeSource:
          binding.RelativeSource = RelativeSource;
          break;
        case ActualSource.ElementName:
          binding.ElementName = ElementName;
          break;
        case ActualSource.Source:
          binding.Source = Source;
          break;
      }
    }

    #endregion

    #region  Nested Types

    private enum ActualSource
    {
      None,
      RelativeSource,
      ElementName,
      Source
    }

    #endregion
  }
}