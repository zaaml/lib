// <copyright file="AnimationTrigger.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Interactivity
{
  [ContentProperty("KeyTriggers")]
  public sealed class AnimationTrigger : TriggerBase
  {
    #region Fields

    private AnimationKeyTriggerCollection _keyTriggers;

    #endregion

    #region Properties

    private IEnumerable<AnimationKeyFrameTriggerBase> ActualKeyTriggers => _keyTriggers ?? Enumerable.Empty<AnimationKeyFrameTriggerBase>();

    internal override IEnumerable<InteractivityObject> Children => base.Children.Concat(ActualKeyTriggers);

    public AnimationKeyTriggerCollection KeyTriggers => _keyTriggers ?? (_keyTriggers = new AnimationKeyTriggerCollection(this));

    #endregion

    #region  Methods

    protected override InteractivityObject CreateInstance()
    {
      return new AnimationTrigger();
    }

    #endregion
  }

  public abstract class AnimationKeyFrameTriggerBase : TriggerBase
  {
    #region Properties

    public TimeSpan? Offset { get; set; }

    #endregion
  }

  public sealed class KeyFrameTrigger : AnimationKeyFrameTriggerBase
  {
    #region  Methods

    protected override InteractivityObject CreateInstance()
    {
      return new KeyFrameTrigger();
    }

    #endregion
  }

  public sealed class SpanFrameTrigger : AnimationKeyFrameTriggerBase
  {
    #region Properties

    public Duration? Duration { get; set; }

    #endregion

    #region  Methods

    protected override InteractivityObject CreateInstance()
    {
      return new SpanFrameTrigger();
    }

    #endregion
  }

  public class AnimationKeyTriggerCollection : InteractivityCollection<AnimationKeyFrameTriggerBase>
  {
    #region Ctors

    internal AnimationKeyTriggerCollection(IInteractivityObject parent) : base(parent)
    {
    }

    #endregion

    #region  Methods

    internal override InteractivityCollection<AnimationKeyFrameTriggerBase> CreateInstance(IInteractivityObject parent)
    {
      return new AnimationKeyTriggerCollection(parent);
    }

    #endregion
  }
}