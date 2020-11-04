// <copyright file="PackedHierarchicalAllocator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Zaaml.Core.Extensions;

namespace Zaaml.Core.Packed
{
  internal class PackedHierarchicalAllocator<T> where T : class
  {
    #region Fields

    private readonly PackedValueAllocator _baseAllocator = new PackedValueAllocator();
    private readonly Dictionary<Type, PackedValueAllocator> _typePackedValueAllocators = new Dictionary<Type, PackedValueAllocator>();
    private bool _isBaseTypeInitialized;

    #endregion

    #region  Methods

    public PackedValueAllocator GetAllocator<TD>() where TD : T
    {
      return GetAllocatorImpl(typeof(TD));
    }

    private PackedValueAllocator GetAllocatorImpl(Type type)
    {
      if (type == typeof(T))
      {
        if (_isBaseTypeInitialized) 
	        return _baseAllocator;

        _isBaseTypeInitialized = true;

        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);

        return _baseAllocator;
      }

      var allocator = _typePackedValueAllocators.GetValueOrDefault(type);

      if (allocator == null)
      {
        // ReSharper disable once PossibleNullReferenceException
        RuntimeHelpers.RunClassConstructor(type.BaseType.TypeHandle);

        allocator = GetAllocatorImpl(type.BaseType).Clone();

        _typePackedValueAllocators[type] = allocator;
      }

      return allocator;
    }

    #endregion
  }
}