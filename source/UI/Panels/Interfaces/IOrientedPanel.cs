// <copyright file="IOrientedPanel.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Windows.Controls;
using Zaaml.PresentationCore.Interfaces;

namespace Zaaml.UI.Panels.Interfaces
{
	internal interface IOrientedPanel : IPanel
	{
		Orientation Orientation { get; }
	}
}