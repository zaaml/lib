// <copyright file="LinkedListEnumerable.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace Zaaml.Core
{
  internal struct LinkedListEnumerable<TNode>
  {
    #region Fields

    private readonly Func<TNode, bool> _getIsEnd;
    private readonly Func<TNode, TNode> _getNext;
    private readonly Func<TNode, TNode> _getPrev;

    #endregion

    #region Ctors

    public LinkedListEnumerable(Func<TNode, TNode> getPrev, Func<TNode, TNode> getNext, Func<TNode, bool> getIsEnd)
    {
      _getPrev = getPrev;
      _getNext = getNext;
      _getIsEnd = getIsEnd;
    }

    #endregion

    #region Methods

    public IEnumerable<TNode> Enumerate(TNode node, Direction direction, bool includeStart)
    {
      return includeStart ? EnumerateIncludeStart(node, direction) : EnumerateIncludeStart(node, direction).Skip(1);
    }

    private IEnumerable<TNode> EnumerateIncludeStart(TNode node, Direction direction)
    {
      var current = node;
      do
      {
        yield return current;
        switch (direction)
        {
          case Direction.Forward:
            current = _getNext(current);
            break;
          case Direction.Backward:
            current = _getPrev(current);
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(direction));
        }
      } while (_getIsEnd(current) == false);
    }

    #endregion
  }
}