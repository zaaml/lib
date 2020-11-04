// <copyright file="DelegateType.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;

namespace Zaaml.Core.Reflection
{
  internal class DelegateType<T> : Type
  {
    #region Static Fields and Constants

    private static readonly Type Type = typeof(T);

    #endregion

    #region Properties

    public override Assembly Assembly => Type.Assembly;

    public override string AssemblyQualifiedName => Type.AssemblyQualifiedName;

    public override Type BaseType => Type.BaseType;

    public override string FullName => Type.FullName;

    public override Guid GUID => Type.GUID;

    public override Module Module => Type.Module;

    public override string Name => Type.Name;

    public override string Namespace => Type.Namespace;

    public override Type UnderlyingSystemType => Type.UnderlyingSystemType;

    #endregion

    #region  Methods

    protected override TypeAttributes GetAttributeFlagsImpl()
    {
      return TypeAttributes.Class;
    }

    protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      return Type.GetConstructor(bindingAttr, binder, types, modifiers);
    }

    public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
    {
      return Type.GetConstructors(bindingAttr);
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
      return Type.GetCustomAttributes(inherit);
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
      return Type.GetCustomAttributes(attributeType, inherit);
    }

    public override Type GetElementType()
    {
      return Type.GetElementType();
    }

    public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
    {
      return Type.GetEvent(name, bindingAttr);
    }

    public override EventInfo[] GetEvents(BindingFlags bindingAttr)
    {
      return Type.GetEvents(bindingAttr);
    }

    public override FieldInfo GetField(string name, BindingFlags bindingAttr)
    {
      return Type.GetField(name, bindingAttr);
    }

    public override FieldInfo[] GetFields(BindingFlags bindingAttr)
    {
      return Type.GetFields(bindingAttr);
    }

    public override Type GetInterface(string name, bool ignoreCase)
    {
      return Type.GetInterface(name, ignoreCase);
    }

    public override Type[] GetInterfaces()
    {
      return Type.GetInterfaces();
    }

    public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
    {
      return Type.GetMembers(bindingAttr);
    }

    protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
    {
      return Type.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
    }

    public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
    {
      return Type.GetMethods(bindingAttr);
    }

    public override Type GetNestedType(string name, BindingFlags bindingAttr)
    {
      return Type.GetNestedType(name, bindingAttr);
    }

    public override Type[] GetNestedTypes(BindingFlags bindingAttr)
    {
      return Type.GetNestedTypes(bindingAttr);
    }

    public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
    {
      return Type.GetProperties(bindingAttr);
    }

    protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
    {
      return Type.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
    }

    protected override bool HasElementTypeImpl()
    {
      return false;
    }

    public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture,
      string[] namedParameters)
    {
      return Type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
    }

    protected override bool IsArrayImpl()
    {
      return Type.IsArray;
    }

    protected override bool IsByRefImpl()
    {
      return Type.IsByRef;
    }

    protected override bool IsCOMObjectImpl()
    {
      return Type.IsCOMObject;
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
      return Type.IsDefined(attributeType, inherit);
    }

    protected override bool IsPointerImpl()
    {
      return Type.IsPointer;
    }

    protected override bool IsPrimitiveImpl()
    {
      return Type.IsPrimitive;
    }

    #endregion
  }
}