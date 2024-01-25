// <copyright file="GridViewCellGenerator.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System;
using Zaaml.PresentationCore;

namespace Zaaml.UI.Controls.Core.GridView
{
	public abstract class GridViewCellGenerator<TGridCell> : InheritanceContextObject where TGridCell : GridViewCell
	{
		internal event EventHandler GeneratorChangedCore;
		internal event EventHandler GeneratorChangingCore;

		protected virtual bool SupportsRecycling => false;

		internal bool SupportsRecyclingInternal => SupportsRecycling;

		protected abstract TGridCell CreateCell();

		internal virtual TGridCell CreateCellCore()
		{
			return CreateCell();
		}

		protected abstract void DisposeCell(TGridCell item);

		internal virtual void DisposeCellCore(TGridCell item)
		{
			DisposeCell(item);
		}

		protected virtual void OnGeneratorChanged()
		{
			GeneratorChangedCore?.Invoke(this, EventArgs.Empty);
		}

		internal void OnGeneratorChangedInternal()
		{
			OnGeneratorChanged();
		}

		protected virtual void OnGeneratorChanging()
		{
			GeneratorChangingCore?.Invoke(this, EventArgs.Empty);
		}

		internal void OnGeneratorChangingInternal()
		{
			OnGeneratorChanging();
		}
	}
}