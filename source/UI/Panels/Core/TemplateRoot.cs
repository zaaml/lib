// <copyright file="TemplateRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.TemplateCore;

namespace Zaaml.UI.Panels.Core
{
  internal sealed class ControlRootStack
  {
    #region Fields

    private readonly Stack<ControlRootBase> _stack = new Stack<ControlRootBase>();

    #endregion

    #region  Methods

    public IEnumerable<ControlRootBase> Enumerate()
    {
      foreach (var rootBase in _stack)
        yield return rootBase;
    }

    public void Pop(ControlRootBase root)
    {
      if (ReferenceEquals(_stack.Peek(), root) == false)
        throw new InvalidOperationException();

      _stack.Pop();
    }

    public void Push(ControlRootBase root)
    {
      _stack.Push(root);
    }

    #endregion
  }

  public abstract class ControlRootBase : Panel
  {
    #region Static Fields and Constants

    internal static readonly ControlRootStack MeasureStack = new ControlRootStack();
    internal static readonly ControlRootStack ArrangeStack = new ControlRootStack();

    #endregion

    #region  Methods

    protected virtual Size ArrangeCoreOverride(Size finalSize)
    {
      return BaseArrange(finalSize);
    }

    protected sealed override Size ArrangeOverrideCore(Size finalSize)
    {
      try
      {
        ArrangeStack.Push(this);

        return ArrangeCoreOverride(finalSize);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }
      finally
      {
        ArrangeStack.Pop(this);
      }

      return finalSize;
    }

    protected virtual Size MeasureCoreOverride(Size availableSize)
    {
      return BaseMeasure(availableSize);
    }

    protected sealed override Size MeasureOverrideCore(Size availableSize)
    {
      try
      {
        MeasureStack.Push(this);

        ImplementationRootLoadedService.PulseImplementationRoot(this);

        return MeasureCoreOverride(availableSize);
      }
      catch (Exception e)
      {
        LogService.LogError(e);
      }
      finally
      {
        MeasureStack.Pop(this);
      }

      return XamlConstants.ZeroSize;
    }

    #endregion
  }

  public abstract class TemplateRoot : ControlRootBase
  {
    #region Ctors

    internal TemplateRoot()
    {
    }

    #endregion
  }

  public sealed class UserControlRoot : ControlRootBase
  {
  }

  public sealed class WindowTemplateRoot : TemplateRoot
  {
    #region Ctors

    public WindowTemplateRoot()
    {
#if !SILVERLIGHT
      SnapsToDevicePixels = true;
      UseLayoutRounding = false;
#endif
    }

    #endregion
  }
}