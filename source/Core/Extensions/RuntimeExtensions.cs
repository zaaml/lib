using System.Reflection;
using Zaaml.Core.Utils;

namespace Zaaml.Core.Extensions
{
  internal static class RuntimeExtensions
  {
    #region  Methods

    public static FieldInfo TransformToDeclaringType(this FieldInfo fieldInfo)
    {
      return RuntimeUtils.TransformToDeclaringTypeFieldInfo(fieldInfo);
    }

    public static PropertyInfo TransformToDeclaringType(this PropertyInfo propertyInfo)
    {
      return RuntimeUtils.TransformToDeclaringTypePropertyInfo(propertyInfo);
    }

    #endregion
  }
}