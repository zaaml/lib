// <copyright file="XamlConvertResult.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Converters
{
  internal struct XamlConvertResult
  {
    private readonly object _result;

    public XamlConvertResult(object result, XamlConvertException exception) : this()
    {
      _result = result;
      Exception = exception;
    }


    public XamlConvertException Exception { get; }

    public object Result
    {
      get
      {
        if (IsFailed)
          throw Exception;

        return _result;
      }
    }

    public bool IsValid => Exception == null;
    public bool IsFailed => Exception != null;
  }
}