// <copyright file="StyleRoot.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Zaaml.PresentationCore.Theming;

namespace Zaaml.PresentationCore.Interactivity
{
	internal sealed class StyleRoot : InteractivityRoot, IRuntimeSetterFactory
	{
		public StyleRoot(InteractivityService service) : base(service)
		{
		}

		protected override void EnsureVisualStateObserver()
		{
			RealVisualStateObserver = InteractivityTarget.GetInteractivityService();
		}

		protected override void OnDescendantApiPropertyChanged(Stack<InteractivityObject> descendants, string propertyName)
		{
		}

		public override void UpdateClass()
		{
			var service = StyleService.GetRuntimeService(InteractivityTarget);

			if (service == null)
				return;

			UpdateClass(service.Setters);
			UpdateClass(service.Triggers);
		}

		public override void UpdateSkin(SkinBase skin)
		{
			var service = StyleService.GetRuntimeService(InteractivityTarget);

			if (service == null)
				return;

			UpdateSkin(service.Setters, skin);
			UpdateSkin(service.Triggers, skin);
		}

		public override void UpdateThemeResources()
		{
			var service = StyleService.GetRuntimeService(InteractivityTarget);

			if (service == null)
				return;

			UpdateThemeResources(service.Setters);
			UpdateThemeResources(service.Triggers);
		}

		RuntimeSetter IRuntimeSetterFactory.CreateSetter()
		{
			return new StyleRuntimeSetter();
		}
	}
}