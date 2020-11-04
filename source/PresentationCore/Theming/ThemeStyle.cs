// <copyright file="ThemeStyle.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class ThemeStyle : ThemeStyleBase
  {
    #region Fields

    private bool _isDeferred;

    #endregion

    #region Properties

    public StyleBase BasedOn
    {
      get => BasedOnCore;
      set => BasedOnCore = value;
    }

    //protected override StyleBase BasedOnCore
    //{
	   // get => IsDeferred ? null : base.BasedOnCore;
	   // set => base.BasedOnCore = value;
    //}

    private bool IsActuallyDeferred { get; set; }

    internal bool IsDeferred
    {
      get => _isDeferred;
      set
      {
        _isDeferred = value;
        IsActuallyDeferred |= value;
      }
    }

    internal Theme Owner { get; set; }

    internal override StyleService StyleService => BasedOnCore?.StyleService ?? base.StyleService;

    public override Type TargetType
    {
      get => base.TargetType;
      set
      {
        if (ReferenceEquals(TargetType, value))
          return;

        if (TargetType != null)
          throw new InvalidOperationException("TargetType is sealed and can not be changed");

        base.TargetType = value;

        Initialize();
      }
    }

    internal Assembly Assembly { get; set; }

    #endregion

    #region  Methods

    //private bool _onStyleInheritanceChanged;

    //protected override void OnStyleInheritanceChanged()
    //{
	   // if (_onStyleInheritanceChanged)
		  //  return;

	   // _onStyleInheritanceChanged = true;

	   // base.OnStyleInheritanceChanged();

	   // _onStyleInheritanceChanged = false;
    //}

    protected override void Initialize()
    {
      if (BasedOnCore == null && TargetType != null)
        BasedOnCore = ThemeManager.GetThemeStyle(TargetType);
    }

    public override string ToString()
    {
      return IsActuallyDeferred ? base.ToString() + " Deferred" : base.ToString();
    }

    #endregion
  }
}