// <copyright file="Error.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zaaml.Core
{
  internal static class Error
  {
    #region Properties

    public static RefactoringException Refactoring => new RefactoringException();

    #endregion

    #region  Methods

    internal static string GetArgumentName(ExceptionArgument argument)
    {
      switch (argument)
      {
        case ExceptionArgument.obj:
          return "obj";
        case ExceptionArgument.dictionary:
          return "dictionary";
        case ExceptionArgument.array:
          return "array";
        case ExceptionArgument.info:
          return "info";
        case ExceptionArgument.key:
          return "key";
        case ExceptionArgument.collection:
          return "collection";
        case ExceptionArgument.match:
          return "match";
        case ExceptionArgument.converter:
          return "converter";
        case ExceptionArgument.queue:
          return "queue";
        case ExceptionArgument.stack:
          return "stack";
        case ExceptionArgument.capacity:
          return "capacity";
        case ExceptionArgument.index:
          return "index";
        case ExceptionArgument.startIndex:
          return "startIndex";
        case ExceptionArgument.value:
          return "value";
        case ExceptionArgument.count:
          return "count";
        case ExceptionArgument.arrayIndex:
          return "arrayIndex";
        case ExceptionArgument.item:
          return "item";
        default:
          return string.Empty;
      }
    }

    internal static string GetResourceName(ExceptionResource resource)
    {
      switch (resource)
      {
        case ExceptionResource.Argument_ImplementIComparable:
          return "Argument_ImplementIComparable";
        case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
          return "ArgumentOutOfRange_NeedNonNegNum";
        case ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired:
          return "ArgumentOutOfRange_NeedNonNegNumRequired";
        case ExceptionResource.Arg_ArrayPlusOffTooSmall:
          return "Arg_ArrayPlusOffTooSmall";
        case ExceptionResource.Argument_AddingDuplicate:
          return "Argument_AddingDuplicate";
        case ExceptionResource.Serialization_InvalidOnDeser:
          return "Serialization_InvalidOnDeser";
        case ExceptionResource.Serialization_MismatchedCount:
          return "Serialization_MismatchedCount";
        case ExceptionResource.Serialization_MissingValues:
          return "Serialization_MissingValues";
        case ExceptionResource.Arg_RankMultiDimNotSupported:
          return "Arg_MultiRank";
        case ExceptionResource.Arg_NonZeroLowerBound:
          return "Arg_NonZeroLowerBound";
        case ExceptionResource.Argument_InvalidArrayType:
          return "Invalid_Array_Type";
        case ExceptionResource.NotSupported_KeyCollectionSet:
          return "NotSupported_KeyCollectionSet";
        case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
          return "ArgumentOutOfRange_SmallCapacity";
        case ExceptionResource.ArgumentOutOfRange_Index:
          return "ArgumentOutOfRange_Index";
        case ExceptionResource.Argument_InvalidOffLen:
          return "Argument_InvalidOffLen";
        case ExceptionResource.InvalidOperation_CannotRemoveFromStackOrQueue:
          return "InvalidOperation_CannotRemoveFromStackOrQueue";
        case ExceptionResource.InvalidOperation_EmptyCollection:
          return "InvalidOperation_EmptyCollection";
        case ExceptionResource.InvalidOperation_EmptyQueue:
          return "InvalidOperation_EmptyQueue";
        case ExceptionResource.InvalidOperation_EnumOpCantHappen:
          return "InvalidOperation_EnumOpCantHappen";
        case ExceptionResource.InvalidOperation_EnumFailedVersion:
          return "InvalidOperation_EnumFailedVersion";
        case ExceptionResource.InvalidOperation_EmptyStack:
          return "InvalidOperation_EmptyStack";
        case ExceptionResource.InvalidOperation_EnumNotStarted:
          return "InvalidOperation_EnumNotStarted";
        case ExceptionResource.InvalidOperation_EnumEnded:
          return "InvalidOperation_EnumEnded";
        case ExceptionResource.NotSupported_SortedListNestedWrite:
          return "NotSupported_SortedListNestedWrite";
        case ExceptionResource.NotSupported_ValueCollectionSet:
          return "NotSupported_ValueCollectionSet";
        default:
          return string.Empty;
      }
    }

    internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, ExceptionArgument argName)
    {
      if (value != null || (object) default(T) == null)
        return;
      ThrowArgumentNullException(argName);
    }

    public static void NotSupported()
    {
      throw new NotSupportedException();
    }

    public static void OutOfRange(string paramName = null, string message = null)
    {
      throw new ArgumentOutOfRangeException(paramName, message);
    }

    internal static void ThrowArgumentOutOfRangeException()
    {
      ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
    }

    internal static void ThrowArgumentException(ExceptionResource resource)
    {
      throw new ArgumentException(SR.GetString(GetResourceName(resource)));
    }

    internal static void ThrowArgumentNullException(ExceptionArgument argument)
    {
      throw new ArgumentNullException(GetArgumentName(argument));
    }

    internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
    {
      throw new ArgumentOutOfRangeException(GetArgumentName(argument));
    }

    internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
    {
      throw new ArgumentOutOfRangeException(GetArgumentName(argument), SR.GetString(GetResourceName(resource)));
    }

    internal static void ThrowInvalidOperationException(ExceptionResource resource)
    {
      throw new InvalidOperationException(SR.GetString(GetResourceName(resource)));
    }

    internal static void ThrowKeyNotFoundException()
    {
      throw new KeyNotFoundException();
    }

    internal static void ThrowNotSupportedException(ExceptionResource resource)
    {
      throw new NotSupportedException(SR.GetString(GetResourceName(resource)));
    }

    public static void ThrowRefactoring(string message = null)
    {
      throw new RefactoringException(message);
    }

    internal static void ThrowSerializationException(ExceptionResource resource)
    {
      throw new SerializationException(SR.GetString(GetResourceName(resource)));
    }

    internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
    {
      throw new ArgumentException(SR.GetString("Arg_WrongType", key, (object) targetType), nameof(key));
    }

    internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
    {
      throw new ArgumentException(SR.GetString("Arg_WrongType", value, (object) targetType), nameof(value));
    }

    #endregion
  }

  internal enum ExceptionArgument
  {
    obj,
    dictionary,
    dictionaryCreationThreshold,
    array,
    info,
    key,
    collection,
    list,
    match,
    converter,
    queue,
    stack,
    capacity,
    index,
    startIndex,
    value,
    count,
    arrayIndex,
    name,
    mode,
    item,
    options,
    view,
    sourceBytesToCopy,
  }

  internal enum ExceptionResource
  {
    ArgumentOutOfRange_NeedNonNegNumRequired,
    Serialization_MismatchedCount,
    Serialization_MissingValues,
    InvalidOperation_EmptyCollection,
    Argument_ImplementIComparable,
    Argument_InvalidType,
    Argument_InvalidArgumentForComparison,
    Argument_InvalidRegistryKeyPermissionCheck,
    ArgumentOutOfRange_NeedNonNegNum,
    Arg_ArrayPlusOffTooSmall,
    Arg_NonZeroLowerBound,
    Arg_RankMultiDimNotSupported,
    Arg_RegKeyDelHive,
    Arg_RegKeyStrLenBug,
    Arg_RegSetStrArrNull,
    Arg_RegSetMismatchedKind,
    Arg_RegSubKeyAbsent,
    Arg_RegSubKeyValueAbsent,
    Argument_AddingDuplicate,
    Serialization_InvalidOnDeser,
    Serialization_MissingKeys,
    Serialization_NullKey,
    Argument_InvalidArrayType,
    NotSupported_KeyCollectionSet,
    NotSupported_ValueCollectionSet,
    ArgumentOutOfRange_SmallCapacity,
    ArgumentOutOfRange_Index,
    Argument_InvalidOffLen,
    Argument_ItemNotExist,
    ArgumentOutOfRange_Count,
    ArgumentOutOfRange_InvalidThreshold,
    ArgumentOutOfRange_ListInsert,
    NotSupported_ReadOnlyCollection,
    InvalidOperation_CannotRemoveFromStackOrQueue,
    InvalidOperation_EmptyQueue,
    InvalidOperation_EnumOpCantHappen,
    InvalidOperation_EnumFailedVersion,
    InvalidOperation_EmptyStack,
    ArgumentOutOfRange_BiggerThanCollection,
    InvalidOperation_EnumNotStarted,
    InvalidOperation_EnumEnded,
    NotSupported_SortedListNestedWrite,
    InvalidOperation_NoValue,
    InvalidOperation_RegRemoveSubKey,
    Security_RegistryPermission,
    UnauthorizedAccess_RegistryNoWrite,
    ObjectDisposed_RegKeyClosed,
    NotSupported_InComparableType,
    Argument_InvalidRegistryOptionsCheck,
    Argument_InvalidRegistryViewCheck,
  }
}