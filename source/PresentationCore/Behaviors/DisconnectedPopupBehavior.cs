using System;
using System.Windows;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.PropertyCore;
using Zaaml.PresentationCore.Utils;

namespace Zaaml.PresentationCore.Behaviors
{
	public sealed class DisconnectedPopupBehavior : BehaviorBase
	{
		public static readonly DependencyProperty OwnerProperty = DPM.Register<FrameworkElement, DisconnectedPopupBehavior>
			("Owner", d => d.OnOwnerChanged);

		private DependencyObject _root;

		public FrameworkElement Owner
		{
			get => (FrameworkElement) GetValue(OwnerProperty);
			set => SetValue(OwnerProperty, value);
		}

		private void OnOwnerChanged(FrameworkElement oldOwner, FrameworkElement newOwner)
		{
			UpdateDisconnectedParent(true);
		}

		private void UpdateDisconnectedParent(bool force)
		{
			var owner = Owner;
			DependencyObject root = null;

			if (FrameworkElement != null)
				foreach (var ancestor in FrameworkElement.GetAncestors(MixedTreeEnumerationStrategy.VisualThenLogicalInstance))
				{
					if (ReferenceEquals(owner, ancestor))
					{
						root = null;
						break;
					}

					root = ancestor;
				}

			if (ReferenceEquals(_root, root) && force == false)
				return;

			if (_root != null)
			{
				PresentationTreeUtils.SetDisconnectedParent(_root, null);
				_root = null;
			}

			_root = root;

			if (_root != null && ReferenceEquals(_root, owner) == false)
				PresentationTreeUtils.SetDisconnectedParent(_root, owner);
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			FrameworkElement.LayoutUpdated += FrameworkElementOnLayoutUpdated;
		}

		private void FrameworkElementOnLayoutUpdated(object sender, EventArgs e)
		{
			UpdateDisconnectedParent(false);
		}

		protected override void OnDetaching()
		{
			FrameworkElement.LayoutUpdated -= FrameworkElementOnLayoutUpdated;

			UpdateDisconnectedParent(true);

			base.OnDetaching();
		}
	}
}