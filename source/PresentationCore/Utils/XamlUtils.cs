// <copyright file="XamlUtils.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.IO;
using System.Text;
using System.Windows.Markup;

namespace Zaaml.PresentationCore.Utils
{
  internal static class XamlUtils
  {
    #region  Methods

    public static object Load(string xaml)
    {
#if SILVERLIGHT
        return XamlReader.Load(xaml);
#else
      using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xaml)))
        return XamlReader.Load(stream);
#endif
    }

    public static T Load<T>(string xaml) where T : class
    {
      return Load(xaml) as T;
    }

    #endregion
  }
}