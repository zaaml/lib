// <copyright file="ControlExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Windows.Controls;
using Zaaml.UI.Controls.VisualStates;

namespace Zaaml.UI.Extensions
{
	internal static class ControlExtensions
	{
		#region Static Fields and Constants

		private static readonly List<VisualGroup> GroupsList = new List<VisualGroup>
		{
			Common,
			Focus,
			Check,
			Selection,
			HasItemsGroup
		};

		#endregion

		#region Properties

		public static VisualGroup Check => CheckVisualGroup.Instance;

		public static VisualGroup Common => CommonVisualGroup.Instance;

		public static VisualGroup Focus => FocusVisualGroup.Instance;

		public static IEnumerable<VisualGroup> Groups => GroupsList;

		public static VisualGroup HasItemsGroup => HasItemsStatesGroup.Instance;

		public static VisualGroup Selection => SelectionStatesGroup.Instance;

		#endregion

		#region  Methods

		public static void UpdateVisualGroups(this Control control, bool useTransitions)
		{
			foreach (var visualGroup in GroupsList)
				visualGroup.UpdateState(control, useTransitions);
		}

		#endregion
	}
}