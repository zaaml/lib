// <copyright file="IInteractivityVisitor.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal interface IInteractivityVisitor
	{
		void Visit(InteractivityObject interactivityObject);
	}
}