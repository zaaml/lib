// <copyright file="DockControlAutomation.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zaaml.PresentationCore.Automation;
using Zaaml.PresentationCore.Extensions;
using Zaaml.PresentationCore.Input;
using Zaaml.UI.Automation;

namespace Zaaml.UI.Controls.Docking.Automation
{
	internal class DockControlAutomation : AutomationBase
	{
		public DockControlAutomation(DockControl dockControl, MouseAutomation mouseAutomation)
		{
			DockControl = dockControl;
			MouseAutomation = mouseAutomation;
		}

		public DockControl DockControl { get; }

		private Point DragOutOffset { get; } = new(0, -50);

		public MouseAutomation MouseAutomation { get; }

		public async Task DragDropItemAsync(DockItem sourceItem, DropGuideAction guideAction)
		{
			Debug.Assert(ReferenceEquals(sourceItem.DockControl, DockControl));

			var headerBox = sourceItem.HeaderPresenter.GetScreenLogicalBox();
			var position = headerBox.GetCenter();

			await MouseAutomation.MoveAsync(position);
			await MouseAutomation.MouseDownAsync(MouseButtonKind.Left);
			await MouseAutomation.MoveAsync(position.WithOffset(DragOutOffset));

			DockControl.UpdateLayout();

			var templateContract = DockControl.TemplateContractInternal;
			var dropGuide = templateContract.GlobalCompass.DropGuides.Single(d => d.Action == guideAction);

			await MouseAutomation.MoveAsync(dropGuide.GetScreenLogicalBox().GetCenter());
			await MouseAutomation.MouseUpAsync(MouseButtonKind.Left);
		}

		public async Task DragDropItemAsync(DockItem sourceItem, DockItem targetItem, DropGuideAction guideAction)
		{
			Debug.Assert(ReferenceEquals(sourceItem.DockControl, DockControl));

			var headerBox = sourceItem.HeaderPresenter.GetScreenLogicalBox();
			var position = headerBox.GetCenter();

			await MouseAutomation.MoveAsync(position);
			await MouseAutomation.MouseDownAsync(MouseButtonKind.Left);
			await MouseAutomation.MoveAsync(position.WithOffset(DragOutOffset));

			DockControl.UpdateLayout();
			var targetCenter = targetItem.ContentPresenter.GetScreenLogicalBox().GetCenter();

			await MouseAutomation.MoveAsync(targetCenter);

			var templateContract = DockControl.TemplateContractInternal;
			var dropGuide = templateContract.LocalCompass.DropGuides.Single(d => d.Action == guideAction);

			await MouseAutomation.MoveAsync(dropGuide.GetScreenLogicalBox().GetCenter());
			await MouseAutomation.MouseUpAsync(MouseButtonKind.Left);
		}
	}
}