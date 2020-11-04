// <copyright file="ResourceDictionaryTreeAdvisor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Trees;

namespace Zaaml.PresentationCore
{
  internal class ResourceDictionaryTreeAdvisor : ITreeEnumeratorAdvisor<ResourceDictionary>
  {
    #region Static Fields and Constants

    public static readonly ResourceDictionaryTreeAdvisor Instance = new ResourceDictionaryTreeAdvisor();

    #endregion

    #region Ctors

    private ResourceDictionaryTreeAdvisor()
    {
    }

    #endregion

    #region Interface Implementations

    #region ITreeEnumeratorAdvisor<ResourceDictionary>

    public IEnumerator<ResourceDictionary> GetChildren(ResourceDictionary node)
    {
      return node.MergedDictionaries.GetEnumerator();
    }

    #endregion

    #endregion
  }
}