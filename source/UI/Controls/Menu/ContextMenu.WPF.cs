// <copyright file="ContextMenu.WPF.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using Zaaml.PresentationCore;
using Zaaml.UI.Controls.Primitives.PopupPrimitives;

namespace Zaaml.UI.Controls.Menu
{
	public partial class ContextMenu
	{
		#region  Methods

		partial void PlatformOnIsSharedChanged()
		{
			if (IsShared)
				NameScope.SetNameScope(this, new ContextMenuNameScope(this));
			else
			{
				var nameScope = NameScope.GetNameScope(this) as ContextMenuNameScope;
				if (ReferenceEquals(this, nameScope?.ContextControl))
					ClearValue(NameScope.NameScopeProperty);
			}
		}

		#endregion

		#region  Nested Types

		private sealed class ContextMenuNameScope : ContextControlNameScope<ContextMenu>
		{
			#region Ctors

			public ContextMenuNameScope(ContextMenu contextControl) : base(contextControl)
			{
			}

			#endregion
		}

		#endregion
	}
}