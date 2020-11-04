// <copyright file="SnapSideCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.ComponentModel;

namespace Zaaml.PresentationCore.Snapping
{
  [TypeConverter(typeof(SnapSideCollectionTypeConverter))]
  public class SnapSideCollection : List<SnapSide>
  {
  }
}