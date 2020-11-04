// <copyright file="QueryAutoCompleteTextEventArgs.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;

namespace Zaaml.UI.Controls.Primitives.TextPrimitives
{
  public class QueryAutoCompleteTextEventArgs : EventArgs
  {
    #region Ctors

    public QueryAutoCompleteTextEventArgs(string typedText)
    {
      TypedText = typedText;
    }

    #endregion

    #region Properties

    public string AutoCompleteText { get; set; }

    public string TypedText { get; private set; }

    #endregion
  }
}