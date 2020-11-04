// <copyright file="ElementNameResolverService.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Windows;
using Zaaml.Core.Extensions;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Services;

namespace Zaaml.PresentationCore.Interactivity
{
	internal class ElementNameResolverService : ServiceBase<FrameworkElement>
	{
		#region Fields

		private readonly Dictionary<string, ElementNameResolver> _resolvers = new Dictionary<string, ElementNameResolver>();

		#endregion

		#region  Methods

		public static IElementNameResolver GetResolver(FrameworkElement dataObject, string elementName)
		{
			return dataObject.GetServiceOrCreate(() => new ElementNameResolverService()).GetResolverImpl(elementName);
		}

		private IElementNameResolver GetResolverImpl(string elementName)
		{
			return _resolvers.GetValueOrCreate(elementName, () => new ElementNameResolver(this, elementName)).AddRef();
		}

		protected override void OnAttach()
		{
			base.OnAttach();
			Target.LayoutUpdated += TargetOnLayoutUpdated;
		}

		protected override void OnDetach()
		{
			Target.LayoutUpdated -= TargetOnLayoutUpdated;
			base.OnDetach();
		}

		private void RemoveResolver(ElementNameResolver resolver)
		{
			_resolvers.Remove(resolver.ElementName);
			if (_resolvers.Count == 0)
				Target.RemoveService<ElementNameResolverService>();
		}

		private void TargetOnLayoutUpdated(object sender, EventArgs eventArgs)
		{
			foreach (var elementNameResolver in _resolvers.Values)
				elementNameResolver.TryResolveElement();
		}

		#endregion

		#region  Nested Types

		private class ElementNameResolver : IElementNameResolver
		{
			#region Fields

			private readonly ElementNameResolverService _service;
			private int _refCount;
			private WeakReference _weakElementReference;

			#endregion

			#region Ctors

			public ElementNameResolver(ElementNameResolverService service, string elementName)
			{
				_service = service;
				ElementName = elementName;
			}

			#endregion

			#region Properties

			private DependencyObject ActualElement => (DependencyObject) _weakElementReference.Target;

			public string ElementName { get; }

			#endregion

			#region  Methods

			public ElementNameResolver AddRef()
			{
				_refCount++;
				return this;
			}

			private void OnElementChanged()
			{
				ElementResolved?.Invoke(this, EventArgs.Empty);
			}

			private DependencyObject ResolveElement()
			{
				return _service.Target.FindName(ElementName) as DependencyObject;
			}

			public void TryResolveElement()
			{
				if (Element == null)
					Element = ResolveElement();
			}

			#endregion

			#region Interface Implementations

			#region IDisposable

			public void Dispose()
			{
				if (_refCount == 0)
					return;

				_refCount--;

				if (_refCount != 0) return;

				_service.RemoveResolver(this);
			}

			#endregion

			#region IElementNameResolver

			public event EventHandler ElementResolved;

			public DependencyObject Element
			{
				get
				{
					if (_weakElementReference == null)
						_weakElementReference = new WeakReference(ResolveElement());

					return ActualElement;
				}
				private set
				{
					if (ReferenceEquals(ActualElement, value))
						return;

					_weakElementReference = new WeakReference(value);
					OnElementChanged();
				}
			}

			#endregion

			#endregion
		}

		#endregion
	}
}