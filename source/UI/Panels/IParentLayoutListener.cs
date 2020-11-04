// <copyright file="IParentLayoutListener.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Panels
{
  internal interface IParentLayoutListener
  {
    #region  Methods

    void EnterParentArrangePass(ParentLayoutPass layoutPass);

    void EnterParentMeasurePass(ParentLayoutPass layoutPass);

    void LeaveParentArrangePass(ref ParentLayoutPass layoutPass);

    void LeaveParentMeasurePass(ref ParentLayoutPass layoutPass);

    #endregion
  }
}