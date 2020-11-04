// <copyright file="TypeTree.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zaaml.Core.Reflection;
using Zaaml.Core.Trees;

namespace Zaaml.PresentationCore.Theming
{
  internal class TypeTree
  {
    #region Static Fields and Constants

    internal static readonly ITreeEnumeratorAdvisor<TypeTreeItem> TreeAdvisor = new DelegateTreeEnumeratorAdvisor<TypeTreeItem>(t => t.DerivedTypes.GetEnumerator());

    #endregion

    #region Fields

    private readonly Type _rootType;
    private readonly AssemblyName _rootTypeAssembly;
    private readonly Dictionary<Type, TypeTreeItem> _treeItemMap = new Dictionary<Type, TypeTreeItem>();

    #endregion

    #region Ctors

    public TypeTree(Type rootType)
    {
      _rootType = rootType;
      _rootTypeAssembly = new AssemblyName(_rootType.Assembly.FullName);
      _treeItemMap[_rootType] = new TypeTreeItem();
    }

    #endregion

    #region  Methods

    public void LookupTypes(IEnumerable<Assembly> assemblies)
    {
      foreach (var assembly in assemblies)
      {
#if !SILVERLIGHT
        var referencedAssemblies = assembly.GetReferencedAssemblies();
        if (ShouldProcess(assembly.GetName()) == false && referencedAssemblies.Any(ShouldProcess) == false)
          continue;
#endif
        var pathToRoot = new Stack<Type>();
        // ReSharper disable AssignNullToNotNullAttribute
        foreach (var type in assembly.GetLoadableTypes().Where(t => t.IsSubclassOf(_rootType)))
        {
          if (_treeItemMap.ContainsKey(type))
            continue;

          var current = type;

          pathToRoot.Push(current);

          while (true)
          {
            TypeTreeItem parentTreeItem;

            current = current.BaseType;

            if (_treeItemMap.TryGetValue(current, out parentTreeItem))
              break;

            pathToRoot.Push(current);
          }

          while (pathToRoot.Count > 0)
          {
            var peek = pathToRoot.Pop();
            _treeItemMap[peek] = new TypeTreeItem(peek, _treeItemMap[current]);
            current = peek;
          }
        }
        // ReSharper restore AssignNullToNotNullAttribute        
      }
    }

    private bool ShouldProcess(AssemblyName assembly)
    {
      return string.Equals(assembly.FullName, _rootTypeAssembly.FullName, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
  }

  internal class TypeTreeItem
  {
    #region Ctors

    public TypeTreeItem(Type type, TypeTreeItem baseType)
    {
      Type = type;
      BaseType = baseType;
      BaseType.DerivedTypesInt.Add(this);
    }

    public TypeTreeItem()
    {
      Type = typeof(object);
    }

    #endregion

    #region Properties

    public TypeTreeItem BaseType { get; }

    public IEnumerable<TypeTreeItem> DerivedTypes => DerivedTypesInt;

    private List<TypeTreeItem> DerivedTypesInt { get; } = new List<TypeTreeItem>();

    public Type Type { get; }

    #endregion

    #region  Methods

    public override string ToString()
    {
      return Type.Name;
    }

    #endregion
  }
}