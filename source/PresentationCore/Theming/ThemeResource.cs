// <copyright file="ThemeResource.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.ComponentModel;
using Zaaml.PresentationCore.Extensions;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeResource : INotifyPropertyChanged
  {
    #region Fields

    private readonly SkinResourceManager _manager;
    private object _boundValue;
    private object _value;

    #endregion

    #region Ctors

    public ThemeResource()
    {
      Keyword = ThemeKeyword.Undefined;
    }

    internal ThemeResource(SkinResourceManager manager) : this()
    {
      _manager = manager;
    }

    #endregion

    #region Properties

    internal string ActualKey => ThemeManager.GetKeyFromKeyword(Keyword) ?? Key;

    public string Key { get; set; }

    public ThemeKeyword Keyword { get; set; }

    public object Value
    {
      get => _value;
      set
      {
        if (ReferenceEquals(_value, value))
          return;

        if (value is IResourceValue resourceValue)
          resourceValue.Key = ActualKey;

        _value = value.GetAsFrozen();

        OnPropertyChanged(nameof(Value));
      }
    }

    #endregion

    #region  Methods

    internal void BindValue(object value)
    {
      if (ReferenceEquals(_boundValue, value))
        return;

      if (_boundValue is ResourceSkinBase resourceSkin)
      {
        resourceSkin.ResourceChanged -= SkinResourceOnResourceChanged;
        resourceSkin.Manager = null;
      }

      _boundValue = value;

      resourceSkin = value as ResourceSkinBase;

      if (resourceSkin != null)
      {
        resourceSkin.Manager = _manager;
        resourceSkin.ResourceChanged += SkinResourceOnResourceChanged;
        Value = resourceSkin.FrozenValueInternal;
      }
      else
        Value = value;
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SkinResourceOnResourceChanged(object sender, EventArgs eventArgs)
    {
      Value = ((ResourceSkinBase) sender).FrozenValueInternal;
    }

    public override string ToString()
    {
      return ActualKey;
    }

    #endregion

    #region Interface Implementations

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #endregion
  }
}