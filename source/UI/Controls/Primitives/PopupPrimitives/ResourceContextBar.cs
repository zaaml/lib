// <copyright file="ResourceContextBar.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Markup;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Primitives.PopupPrimitives
{
	public class ResourceContextBar : ContextBar, INameScope
	{
		private readonly NameScope _nameScope = new NameScope();

		internal override bool OwnerAttachSelector => false;

		void INameScope.RegisterName(string name, object scopedElement)
		{
			_nameScope.RegisterName(name, scopedElement);
		}

		void INameScope.UnregisterName(string name)
		{
			_nameScope.UnregisterName(name);
		}

		object INameScope.FindName(string name)
		{
			return _nameScope.FindName(name) ?? NameScope.FindName(this, name);
		}
	}
}