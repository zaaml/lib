// <copyright file="UIElementCollectionBase.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Zaaml.PresentationCore
{
	public class UIElementCollectionBase<T> : DependencyObjectCollectionBase<T> where T : UIElement
	{
		private Panel _elementsHost;
		private ILogicalOwner _logicalHost;

		protected Panel ElementsHost
		{
			get => _elementsHost;
			set
			{
				if (ReferenceEquals(_elementsHost, value))
					return;

				if (_elementsHost != null && value != null)
				{
					VisualDetachElements();

					_elementsHost = value;

					VisualAttachElements();
				}
				else
				{
					if (_elementsHost != null)
					{
						VisualDetachElements();
						LogicalAttachElements();
					}

					_elementsHost = value;

					if (_elementsHost != null)
					{
						LogicalDetachElements();
						VisualAttachElements();
					}
				}
			}
		}

		protected IEnumerator LogicalChildren
		{
			get
			{
				if (ElementsHost != null || LogicalHost == null)
					return Enumerable.Empty<object>().GetEnumerator();

				return GetEnumerator();
			}
		}

		internal IEnumerator LogicalChildrenInternal => LogicalChildren;

		private protected ILogicalOwner LogicalHost
		{
			get => _logicalHost;
			set
			{
				if (ReferenceEquals(_logicalHost, value))
					return;

				if (_logicalHost != null && ElementsHost == null)
					LogicalDetachElements();

				_logicalHost = value;

				if (_logicalHost != null && ElementsHost == null)
					LogicalAttachElements();
			}
		}

		private void LogicalAttachElement(T element)
		{
			LogicalHost?.AddLogicalChild(element);
		}

		private void LogicalAttachElements()
		{
			foreach (var element in this)
				LogicalAttachElement(element);
		}

		private void LogicalDetachElement(T element)
		{
			LogicalHost?.RemoveLogicalChild(element);
		}

		private void LogicalDetachElements()
		{
			foreach (var element in this)
				LogicalDetachElement(element);
		}

		protected override void OnItemAdded(T element)
		{
			base.OnItemAdded(element);

			if (ElementsHost != null)
				VisualAttachElement(element);
			else if (LogicalHost != null)
				LogicalAttachElement(element);
		}

		protected override void OnItemRemoved(T element)
		{
			if (ElementsHost != null)
				VisualDetachElement(element);
			else if (LogicalHost != null)
				LogicalDetachElement(element);

			base.OnItemRemoved(element);
		}

		private void VisualAttachElement(T element)
		{
			ElementsHost.Children.Add(element);
		}

		private void VisualAttachElements()
		{
			foreach (var element in this)
				VisualAttachElement(element);
		}

		private void VisualDetachElement(T element)
		{
			ElementsHost.Children.Remove(element);
		}

		private void VisualDetachElements()
		{
			foreach (var element in this)
				VisualDetachElement(element);
		}
	}
}