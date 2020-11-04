// <copyright file="NotImplementedType.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;

namespace Zaaml.Core.Reflection
{
  internal class NotImplementedType : Type
  {
    #region Properties

    public override Assembly Assembly => throw new NotImplementedException();

    public override string AssemblyQualifiedName => throw new NotImplementedException();

    public override Type BaseType => throw new NotImplementedException();

    public override string FullName => throw new NotImplementedException();

    public override Guid GUID => throw new NotImplementedException();

    public override Module Module => throw new NotImplementedException();

    public override string Name => throw new NotImplementedException();

    public override string Namespace => throw new NotImplementedException();

    public override Type UnderlyingSystemType => throw new NotImplementedException();

    #endregion

    #region  Methods

    protected override TypeAttributes GetAttributeFlagsImpl()
    {
      throw new NotImplementedException();
    }

    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      throw new NotImplementedException();
    }

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override Type GetElementType()
    {
      throw new NotImplementedException();
    }

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override FieldInfo GetField(string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override Type GetInterface(string name, bool ignoreCase)
    {
      throw new NotImplementedException();
    }

    public override Type[] GetInterfaces()
    {
      throw new NotImplementedException();
    }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override Type GetNestedType(string name, BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
      throw new NotImplementedException();
    }

    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
    {
      throw new NotImplementedException();
    }

    protected override bool HasElementTypeImpl()
    {
      throw new NotImplementedException();
    }

    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture,
      string[] namedParameters)
    {
      throw new NotImplementedException();
    }

    protected override bool IsArrayImpl()
    {
      throw new NotImplementedException();
    }

    protected override bool IsByRefImpl()
    {
      throw new NotImplementedException();
    }

    protected override bool IsCOMObjectImpl()
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    protected override bool IsPointerImpl()
    {
      throw new NotImplementedException();
    }

    protected override bool IsPrimitiveImpl()
    {
      throw new NotImplementedException();
    }

    #endregion
  }

  internal class NotImplementedPropertyInfo : PropertyInfo
  {
    #region Properties

    public override PropertyAttributes Attributes => throw new NotImplementedException();

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override Type DeclaringType => throw new NotImplementedException();

    public override string Name => throw new NotImplementedException();

    public override Type PropertyType => throw new NotImplementedException();

    public override Type ReflectedType => throw new NotImplementedException();

    #endregion

    #region  Methods

    public override MethodInfo[] GetAccessors(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetGetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override ParameterInfo[] GetIndexParameters()
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetSetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }

  internal class CustomPropertyInfo : PropertyInfo
  {
    #region Properties

    public override PropertyAttributes Attributes { get; }
    public override bool CanRead { get; }
    public override bool CanWrite { get; }
    public override Type DeclaringType => DeclaringTypeEx;
    public Type DeclaringTypeEx { get; set; }

    public override string Name => NameEx;
    public string NameEx { get; set; }
    public override Type PropertyType { get; }
    public override Type ReflectedType { get; }

    #endregion

    #region  Methods

    public override MethodInfo[] GetAccessors(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetGetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override ParameterInfo[] GetIndexParameters()
    {
      throw new NotImplementedException();
    }

    public override MethodInfo GetSetMethod(bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}