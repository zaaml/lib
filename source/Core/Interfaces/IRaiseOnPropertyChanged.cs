// <copyright file="IRaiseOnPropertyChanged.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

namespace Zaaml.Core.Interfaces
{
  internal interface IRaiseOnPropertyChanged
  {
    #region  Methods

    void OnPropertyChanged(string propertyName);

    #endregion
  }
}