// <copyright file="SubjectResolver.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using System.Windows;
using Zaaml.PresentationCore.Data.MarkupExtensions;
using NativeBinding = System.Windows.Data.Binding;

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class SubjectResolver
	{
		#region  Methods

		public static void CopyFrom(IInteractivitySubject target, IInteractivitySubject source)
		{
			var sourceSubjectKind = source.SubjectKind & SubjectKind.Unspecified;
			
			if (sourceSubjectKind == 0)
				sourceSubjectKind = SubjectKind.Unspecified;

			switch (sourceSubjectKind)
			{
				case SubjectKind.Name:
					SetSubjectName(target, GetSubjectName(source));
					break;
				case SubjectKind.Explicit:
					SetExplicitSubject(target, GetExplicitSubject(source));
					break;
			}
		}

		public static object GetExplicitSubject(IInteractivitySubject interactivitySubject)
		{
			var subjectKind = interactivitySubject.SubjectKind;

			subjectKind &= SubjectKind.Unspecified;

			if (subjectKind == SubjectKind.Unspecified || subjectKind != SubjectKind.Explicit)
				return null;

			// Unwrap if resolved through Binding
			return (interactivitySubject.SubjectStore as BindingProxyBase)?.Binding ?? interactivitySubject.SubjectStore;
		}

		public static string GetSubjectName(IInteractivitySubject interactivitySubject)
		{
			var subjectKind = interactivitySubject.SubjectKind;

			subjectKind &= SubjectKind.Unspecified;

			if (subjectKind == SubjectKind.Unspecified || subjectKind != SubjectKind.Name)
				return null;

			// Unwrap if resolved through Binding
			return (interactivitySubject.SubjectStore as BindingProxyBase)?.Binding.ElementName ?? (string) interactivitySubject.SubjectStore;
		}

		private static DependencyObject InstallBinding(IInteractivitySubject interactivitySubject, NativeBinding binding)
		{
			var interactivityTarget = interactivitySubject.GetInteractivityObject().InteractivityTarget;
			
			if (interactivityTarget == null)
				return null;

			var subjectBindingProxy = new SubjectBindingProxy(binding, interactivitySubject);

			interactivitySubject.SubjectStore = subjectBindingProxy;
			interactivitySubject.SubjectKind |= SubjectKind.Resolved;

			return subjectBindingProxy.Value as DependencyObject;
		}

		public static bool IsResolved(IInteractivitySubject interactivitySubject)
		{
			return (interactivitySubject.SubjectKind & SubjectKind.Resolved) != 0;
		}

		public static bool IsSpecified(IInteractivitySubject interactivitySubject)
		{
			return (interactivitySubject.SubjectKind & SubjectKind.Unspecified) != SubjectKind.Unspecified;
		}

		public static DependencyObject ResolveSubject(IInteractivitySubject interactivitySubject)
		{
			if (IsResolved(interactivitySubject))
				return UnwrapResolved(interactivitySubject);

			var interactivityObject = interactivitySubject.GetInteractivityObject();

			if (interactivityObject.IsLoaded == false)
				return null;

			var interactivityRoot = interactivityObject.Root;

			// TODO Setters should apply values on Interactivity root only, however Triggers could interact with other controls on Page. Need to check setters behavior in future.
			if (interactivityRoot is StyleRoot && interactivityObject is SetterBase)
				return interactivityRoot.InteractivityTarget;

			var subjectKind = interactivitySubject.SubjectKind;
			subjectKind &= SubjectKind.Unspecified;

			var interactivityTarget = interactivityObject.InteractivityTarget;
			var subject = interactivitySubject.SubjectStore;

			// Explicit
			if (subjectKind == SubjectKind.Explicit)
			{
				var binding = subject as NativeBinding;

				if (subject is BindingBaseExtension bindingBase)
					binding = bindingBase.GetBinding(interactivityTarget, null);

				if (binding != null)
					return InstallBinding(interactivitySubject, binding);

				interactivitySubject.SubjectKind |= SubjectKind.Resolved;

				return subject as DependencyObject;
			}
			// Name
			if (subjectKind == SubjectKind.Name)
				return InstallBinding(interactivitySubject, new NativeBinding { ElementName = (string)subject, BindsDirectlyToSource = true });

			// InteractivityTarget
			if (interactivityTarget == null)
				return null;

			interactivitySubject.SubjectStore = interactivityTarget;
			interactivitySubject.SubjectKind = SubjectKind.Resolved;

			return interactivityTarget;
		}

		public static void SetExplicitSubject(IInteractivitySubject interactivitySubject, object value)
		{
			if (ReferenceEquals(interactivitySubject.SubjectStore, value))
				return;

			UnResolveSubject(interactivitySubject);

			interactivitySubject.SubjectStore = value;
			interactivitySubject.SubjectKind = SubjectKind.Explicit;
		}

		public static void SetSubjectName(IInteractivitySubject interactivitySubject, string value)
		{
			if (ReferenceEquals(interactivitySubject.SubjectStore, value))
				return;

			UnResolveSubject(interactivitySubject);

			interactivitySubject.SubjectStore = value;
			interactivitySubject.SubjectKind = SubjectKind.Name;
		}

		public static void UnResolveSubject(IInteractivitySubject interactivitySubject)
		{
			if (IsResolved(interactivitySubject) == false)
				return;

			if (IsSpecified(interactivitySubject) == false)
			{
				interactivitySubject.SubjectStore = null;
				
				return;
			}

			interactivitySubject.SubjectKind &= SubjectKind.Unspecified;
			switch (interactivitySubject.SubjectKind)
			{
				case SubjectKind.Explicit:
					if (interactivitySubject.SubjectStore is BindingProxyBase bindingProxy)
					{
						interactivitySubject.SubjectStore = bindingProxy.Binding;
						bindingProxy.Dispose();
					}
					break;
				
				case SubjectKind.Name:
					interactivitySubject.SubjectStore = GetSubjectName(interactivitySubject);
					break;
			}
		}

		private static DependencyObject UnwrapResolved(IInteractivitySubject interactivitySubject)
		{
			if (interactivitySubject.SubjectStore is BindingProxyBase bindingProxyBase)
				return bindingProxyBase.Value as DependencyObject;

			return (DependencyObject) interactivitySubject.SubjectStore;
		}

		#endregion

		#region  Nested Types

		private class SubjectBindingProxy : BindingProxyBase
		{
			#region Fields

			private readonly IInteractivitySubject _interactivityObject;

			#endregion

			#region Ctors

			public SubjectBindingProxy(NativeBinding binding, IInteractivitySubject interactivitySubject) : base(binding, interactivitySubject.GetInteractivityObject().InteractivityTarget)
			{
				_interactivityObject = interactivitySubject;
			}

			#endregion

			#region  Methods

			protected override void OnPropertyChanged(DependencyObject depObj, DependencyProperty dependencyProperty, object oldValue, object newValue)
			{
				var oldSubject = oldValue as DependencyObject;
				var newSubject = newValue as DependencyObject;
				
				if (ReferenceEquals(oldSubject, newSubject))
					return;

				_interactivityObject.OnSubjectChanged(oldSubject, newSubject);
			}

			#endregion
		}

		#endregion
	}


	[Flags]
	internal enum SubjectKind
	{
		Unspecified = Name | Explicit,

		Name = 1,
		Explicit = 2,
		Resolved = 4,

		Inherited = Resolved | Unspecified
	}


	internal interface IInteractivitySubject
	{
		#region Properties

		SubjectKind SubjectKind { get; set; }

		object SubjectStore { get; set; }

		#endregion

		#region  Methods

		void OnSubjectChanged(DependencyObject oldTargetSource, DependencyObject newTargetSource);

		#endregion
	}

	internal static class InteractivitySubjectExtensions
	{
		#region  Methods

		public static InteractivityObject GetInteractivityObject(this IInteractivitySubject interactivitySubject)
		{
			return (InteractivityObject) interactivitySubject;
		}

		#endregion
	}

	internal interface IInteractivityTargetSubject : IInteractivitySubject
	{
	}

	internal interface IInteractivitySourceSubject : IInteractivitySubject
	{
	}
}