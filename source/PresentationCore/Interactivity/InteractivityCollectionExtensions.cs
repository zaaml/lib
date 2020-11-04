// <copyright file="InteractivityCollectionExtensions.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

namespace Zaaml.PresentationCore.Interactivity
{
	internal static class InteractivityCollectionExtensions
	{
		#region  Methods

		public static T DeepCloneCollection<T, TI>(this T source, IInteractivityObject parent) where T : InteractivityCollection<TI> where TI : InteractivityObject
		{
			return (T) source.DeepClone(parent);
		}

		#endregion
	}
}