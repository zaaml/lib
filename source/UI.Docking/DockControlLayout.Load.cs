// <copyright file="DockControlLayout.Normalize.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Docking
{
	public sealed partial class DockControlLayout
	{
		public void Reset()
		{
			Items.Clear();	
		}
		
		internal void Load(DockItemCollection items)
		{
			Reset();
			
			
		}
	}
}