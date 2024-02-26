// <copyright file="InheritanceContextDependencyObjectCollection.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Data;

namespace Zaaml.PresentationCore
{
	public class InheritanceContextDependencyObjectCollection<T> : DependencyObjectCollectionBase<T> where T : InheritanceContextObject
	{
		private IInheritanceContext _inheritanceContext;

		internal IInheritanceContext InheritanceContext => _inheritanceContext ??= new InheritanceContext();

		internal DependencyObject Owner
		{
			get => _inheritanceContext?.Owner;
			set => InheritanceContext.Owner = value;
		}

		protected override void OnItemAdded(T obj)
		{
			if (obj is InheritanceContextObject inheritanceContextObject)
				inheritanceContextObject.InheritanceContext = InheritanceContext;

			base.OnItemAdded(obj);
		}

		protected override void OnItemRemoved(T obj)
		{
			base.OnItemRemoved(obj);

			if (obj is InheritanceContextObject inheritanceContextObject)
				inheritanceContextObject.InheritanceContext = null;
		}
	}
}