// <copyright file="Skin.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Zaaml.Core.Extensions;

namespace Zaaml.PresentationCore.Theming
{
  public sealed class Skin : SkinBase
  {
    #region Fields

    private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

    #endregion

    #region Ctors

    internal Skin()
    {
    }

    #endregion

    #region Properties

    internal override IEnumerable<KeyValuePair<string, object>> Resources => _dictionary;

    internal string ActualKey { get; set; }

    #endregion

    #region  Methods

    protected override object GetValue(string key)
    {
      return _dictionary.GetValueOrDefault(key);
    }

    internal void AddValueInternal(string key, object value)
    {
      _dictionary[key] = value;
    }

    internal void Flatten()
    {
      foreach (var kv in Resources.Where(r => r.Value is Skin).ToList())
      {
        var key = kv.Key;
        var childSkin = (Skin) kv.Value;

        childSkin.Flatten();

        foreach (var keyValuePair in childSkin.Resources)
        {
          var actual = keyValuePair.WithParentKey(key).Key;

          _dictionary[actual] = keyValuePair.Value;
        }
      }
    }

    public override string ToString()
    {
      return ActualKey;
    }

    #endregion
  }
}