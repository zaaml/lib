#if !SILVERLIGHT
using System;

namespace Zaaml.UI.Windows
{
	public partial class FloatingDialog
	{
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			_isClosed = true;
		}
	}
}

#endif