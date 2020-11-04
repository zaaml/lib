// <copyright file="IconPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using System.Windows.Markup;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.UI.Panels.Core;

namespace Zaaml.UI.Controls.Primitives.ContentPrimitives
{
  public abstract class IconPresenterBase : Panel
  {
    #region Static Fields and Constants

    public static readonly DependencyProperty IconProperty = DPM.Register<IconBase, IconPresenterBase>
      ("Icon", i => i.OnIconChanged);

    #endregion

    #region Fields

    private IconBase _actualIcon;

    #endregion

    #region Ctors

    internal IconPresenterBase()
    {
#if !SILVERLIGHT
      Focusable = false;
#endif
    }

    #endregion

    #region Properties

    protected IconBase ActualIcon
    {
      get => _actualIcon;
      private set
      {
        if (ReferenceEquals(_actualIcon, value))
          return;

        if (_actualIcon != null)
          _actualIcon.Presenter = null;

        IconBase.UseIcon(ref _actualIcon, value, this);

        if (_actualIcon != null)
          _actualIcon.Presenter = this;

        OnActualIconChanged();
      }
    }

    internal IconBase ActualIconInternal => ActualIcon;

    public IconBase Icon
    {
      get => (IconBase) GetValue(IconProperty);
      set => SetValue(IconProperty, value);
    }

    #endregion

    #region  Methods

    protected virtual void OnActualIconChanged()
    {
      InvalidateMeasure();
    }

    private void OnIconChanged(IconBase oldIcon, IconBase newIcon)
    {
      ActualIcon = newIcon?.SharedResource == true ? newIcon.Clone(false) : newIcon;
    }

    #endregion
  }

  [ContentProperty(nameof(Icon))]
  public sealed class IconPresenter : IconPresenterBase
  {
    #region  Methods

    protected override Size ArrangeOverrideCore(Size finalSize)
    {
      var icon = ActualIcon;

      if (icon == null)
        return finalSize;

      icon.Arrange(finalSize.Rect());

      return finalSize;
    }

    protected override Size MeasureOverrideCore(Size availableSize)
    {
      var icon = ActualIcon;

      if (icon == null)
        return XamlConstants.ZeroSize;

      icon.Measure(availableSize);

      return icon.DesiredSize;
    }

    #endregion
  }

  internal interface IIconPresenter
  {
		IconBase Icon { get; }
  }
}