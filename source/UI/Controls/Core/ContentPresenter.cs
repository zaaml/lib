// <copyright file="ContentPresenter.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.PresentationCore.Behaviors;
using Zaaml.PresentationCore.Interactivity;
using TriggerCollection = Zaaml.PresentationCore.Interactivity.TriggerCollection;

namespace Zaaml.UI.Controls.Core
{
#if SILVERLIGHT
  public partial class ContentPresenter : System.Windows.Controls.ContentPresenter
#else
	public class ContentPresenter : System.Windows.Controls.ContentPresenter
#endif
	{
		private SetterCollection _actualSetters;

		private TriggerCollection _actualTriggers;
		private FrameworkElement _actualVisualChild;
		private BehaviorCollection _contentBehaviorCollection;
		private SetterCollection _contentSetterCollection;
		private TriggerCollection _contentTriggerCollection;

		internal BehaviorCollection ContentBehaviorCollection
		{
			get => _contentBehaviorCollection;
			set
			{
				if (ReferenceEquals(_contentBehaviorCollection, value))
					return;

				DetachBehaviors(VisualChild, _contentBehaviorCollection);

				_contentBehaviorCollection = value;

				AttachBehaviors(VisualChild, _contentBehaviorCollection);
			}
		}

		internal SetterCollection ContentSetterCollection
		{
			get => _contentSetterCollection;
			set
			{
				if (ReferenceEquals(_contentSetterCollection, value))
					return;

				DetachSetters(VisualChild, _contentSetterCollection);

				_contentSetterCollection = value;

				AttachSetters(VisualChild, _contentSetterCollection);
			}
		}

		internal TriggerCollection ContentTriggerCollection
		{
			get => _contentTriggerCollection;
			set
			{
				if (ReferenceEquals(_contentTriggerCollection, value))
					return;

				DetachTriggers(VisualChild, _contentTriggerCollection);

				_contentTriggerCollection = value;

				AttachTriggers(VisualChild, _contentTriggerCollection);
			}
		}

		private FrameworkElement VisualChild
		{
			get => _actualVisualChild;
			set
			{
				if (ReferenceEquals(_actualVisualChild, value))
					return;

				DetachSetters(_actualVisualChild, ContentSetterCollection);
				DetachTriggers(_actualVisualChild, ContentTriggerCollection);
				DetachBehaviors(_actualVisualChild, ContentBehaviorCollection);

				_actualVisualChild = value;

				if (ContentBehaviorCollection != null)
					ContentBehaviorCollection.FrameworkElement = _actualVisualChild;

				AttachBehaviors(_actualVisualChild, ContentBehaviorCollection);
				AttachTriggers(_actualVisualChild, ContentTriggerCollection);
				AttachSetters(_actualVisualChild, ContentSetterCollection);
			}
		}

		private void AttachBehaviors(FrameworkElement element, BehaviorCollection behaviors)
		{
			if (element == null || behaviors == null)
				return;

			behaviors.FrameworkElement = element;
		}

		private void AttachSetters(FrameworkElement element, SetterCollection setters)
		{
			if (element == null || setters == null)
				return;

			_actualSetters = setters.DeepClone(element);
			_actualSetters.Load();
		}

		private void AttachTriggers(FrameworkElement element, TriggerCollection triggers)
		{
			if (element == null || triggers == null)
				return;

			_actualTriggers = triggers.DeepClone(element);
			_actualTriggers.Load();
		}

		private void DetachBehaviors(FrameworkElement element, BehaviorCollection behaviors)
		{
			if (element == null || behaviors == null)
				return;

			behaviors.FrameworkElement = null;
		}

		private void DetachSetters(FrameworkElement element, SetterCollection setters)
		{
			if (element == null || setters == null)
				return;

			_actualSetters.Unload();
			_actualSetters = null;
		}

		private void DetachTriggers(FrameworkElement element, TriggerCollection triggers)
		{
			if (element == null || triggers == null)
				return;

			_actualTriggers.Unload();
			_actualTriggers = null;
		}

		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);

			if (ReferenceEquals(VisualChild, visualRemoved))
				VisualChild = null;

			if (visualAdded != null)
				VisualChild = visualAdded as FrameworkElement;
		}
	}
}