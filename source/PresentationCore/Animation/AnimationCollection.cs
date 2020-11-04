// <copyright file="AnimationCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;

namespace Zaaml.PresentationCore.Animation
{
  public class AnimationCollection : InheritanceContextDependencyObjectCollection<AnimationBase>
  {
    #region Ctors

    public AnimationCollection()
    {
    }

    internal AnimationCollection(DependencyObject frameworkElement)
    {
      Owner = frameworkElement;
    }

    #endregion
  }
}