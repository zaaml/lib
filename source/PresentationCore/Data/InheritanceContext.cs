// <copyright file="InheritanceContext.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.PresentationCore.Data
{
	internal class InheritanceContext : IInheritanceContext
	{
		private static readonly DependencyProperty ContextProperty = DPM.RegisterAttached<DependencyObject, InheritanceContext>
			("Context");

		private readonly DependencyObjectCollectionBase<DependencyObject> _contextObjects = [];
		private DependencyObject _owner;

		public DependencyObject Owner
		{
			get => _owner;
			set
			{
				if (ReferenceEquals(_owner, value))
					return;

				_owner?.ClearValue(ContextProperty);

				_owner = value;

				_owner?.SetValue(ContextProperty, _contextObjects);
			}
		}

		public void Attach(DependencyObject depObj)
		{
			_contextObjects.Add(depObj);
		}

		public void Detach(DependencyObject depObj)
		{
			_contextObjects.Remove(depObj);
		}
	}
}