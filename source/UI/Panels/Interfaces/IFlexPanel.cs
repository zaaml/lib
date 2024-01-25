// <copyright file="IFlexPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows;
using Zaaml.UI.Panels.Flexible;

namespace Zaaml.UI.Panels.Interfaces
{
	internal interface IFlexPanel : IOrientedPanel
	{
		IFlexDistributor Distributor { get; }

		bool HasHiddenChildren { get; set; }

		double Spacing { get; }

		FlexStretch Stretch { get; }

		FlexElement GetFlexElement(UIElement child);

		bool GetIsHidden(UIElement child);

		void SetIsHidden(UIElement child, bool value);
	}
}