// <copyright file="TransformUtil.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Zaaml.PresentationCore.Utils
{
  public static class TransformUtils
  {
    public static MatrixTransform CombineTransform(IEnumerable<Transform> transformCollection)
    {
      var transformGroup = new TransformGroup();

      foreach (var transform in transformCollection)
        transformGroup.Children.Add(transform);

      return new MatrixTransform { Matrix = transformGroup.Value };
    }

    public static MatrixTransform CombineTransform(params Transform[] transformCollection)
    {
      return CombineTransform(transformCollection.AsEnumerable());
    }
  }
}