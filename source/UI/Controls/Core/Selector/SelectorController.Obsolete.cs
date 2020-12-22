// <copyright file="SelectorController.Obsolete.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		private SelectNextMode SelectNextMode { get; set; }

		private void SelectNext(bool force = false)
		{
			SelectIndexCore(SelectNext(SelectedIndex, SelectNextMode), force);
		}

		private int SelectNext(int index, SelectNextMode mode)
		{
			if (Count == 0)
				index = -1;
			else if (Count == 1)
				index = 0;
			else
			{
				switch (mode)
				{
					case SelectNextMode.First:

						index = 0;

						break;

					case SelectNextMode.PrevOrNearest:

						index = index > 0 ? index - 1 : index + 1;

						break;

					case SelectNextMode.Prev:

						index = index == 0 ? Count - 1 : index - 1;

						break;

					case SelectNextMode.Next:

						index = index == Count - 1 ? 0 : index + 1;

						break;

					case SelectNextMode.NextOrNearest:

						index = index < Count - 1 ? index + 1 : index - 1;

						break;

					case SelectNextMode.Last:

						index = Count - 1;

						break;
				}
			}

			return index;
		}
	}
}