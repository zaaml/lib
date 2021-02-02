// <copyright file="FixedTemplateContentControl.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Zaaml.Core.Utils;
using Zaaml.PresentationCore;
using Zaaml.PresentationCore.PropertyCore;

namespace Zaaml.UI.Controls.Core
{
	internal sealed class FixedTemplateContentControlController<TPanel, TChild>
		where TPanel : Panel
		where TChild : UIElement
	{
		private TChild _child;

		public FixedTemplateContentControlController(IFixedTemplateContentControl<TPanel> control)
		{
			Control = control;
		}

		public TChild Child
		{
			get => _child;
			set
			{
				if (ReferenceEquals(_child, value))
					return;

				var templateRoot = TemplateRoot;

				if (_child != null)
				{
					if (templateRoot != null)
						templateRoot.Children.Remove(_child);
					else
						RemoveLogicalChild(_child);
				}

				_child = value;

				if (_child != null)
				{
					if (templateRoot != null)
						templateRoot.Children.Add(_child);
					else
						AddLogicalChild(_child);
				}
			}
		}

		public IFixedTemplateContentControl<TPanel> Control { get; }

		public IEnumerator LogicalChildren => Child != null ? EnumeratorUtils.Concat(Child, Control.BaseLogicalChildren) : Control.BaseLogicalChildren;

		private Panel TemplateRoot => Control.Panel;

		private void AddLogicalChild(TChild child)
		{
			if (Control.IsLogicalParent)
				Control.AddLogicalChild(child);
		}

		public void AttachChild()
		{
			if (Child == null)
				return;

			RemoveLogicalChild(Child);

			TemplateRoot.Children.Add(Child);
		}

		public void DetachChild()
		{
			if (Child == null)
				return;

			Control.Panel.Children.Remove(Child);

			AddLogicalChild(Child);
		}

		private void RemoveLogicalChild(TChild child)
		{
			if (Control.IsLogicalParent)
				Control.RemoveLogicalChild(child);
		}
	}

	internal interface IFixedTemplateContentControl<out TPanel> : ILogicalOwner
		where TPanel : Panel
	{
		bool IsLogicalParent { get; }

		TPanel Panel { get; }
	}


	public abstract class FixedTemplateContentControlBase<TPanel, TChild> : FixedTemplateControl<TPanel>, IFixedTemplateContentControl<TPanel>
		where TPanel : Panel
		where TChild : FrameworkElement
	{
		protected FixedTemplateContentControlBase()
		{
			Controller = new FixedTemplateContentControlController<TPanel, TChild>(this);
		}

		protected TChild ChildCore
		{
			get => Controller.Child;
			set => Controller.Child = value;
		}

		private FixedTemplateContentControlController<TPanel, TChild> Controller { get; }

		protected virtual bool IsLogicalParent => true;

		protected override IEnumerator LogicalChildren => Controller.LogicalChildren;

		protected override void ApplyTemplateOverride()
		{
			base.ApplyTemplateOverride();

			Controller.AttachChild();
		}

		protected override void UndoTemplateOverride()
		{
			Controller.DetachChild();

			base.UndoTemplateOverride();
		}

		TPanel IFixedTemplateContentControl<TPanel>.Panel => TemplateRoot;

		bool IFixedTemplateContentControl<TPanel>.IsLogicalParent => IsLogicalParent;
	}

	[ContentProperty(nameof(Child))]
	public abstract class FixedTemplateContentControl<TPanel, TChild> : FixedTemplateContentControlBase<TPanel, TChild>
		where TPanel : Panel
		where TChild : FrameworkElement
	{
		public static readonly DependencyProperty ChildProperty = DPM.Register<TChild, FixedTemplateContentControl<TPanel, TChild>>
			("Child", d => d.OnChildPropertyChangedPrivate);

		public TChild Child
		{
			get => (TChild) GetValue(ChildProperty);
			set => SetValue(ChildProperty, value);
		}

		private void OnChildPropertyChangedPrivate()
		{
			ChildCore = Child;
		}
	}
}