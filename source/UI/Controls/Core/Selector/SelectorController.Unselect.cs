// <copyright file="SelectorController.Unselect.cs" author="Dmitry Kravchenin" email="d.kravchenin@zaaml.com">
//   Copyright (c) Zaaml. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace Zaaml.UI.Controls.Core
{
	internal abstract partial class SelectorController<TItem>
	{
		public void UnselectAll()
		{
			using (SelectionHandlingScope)
			{
				UnselectAllSafe();
				EnsureSelection();
			}
		}

		private void UnselectAllSafe()
		{
			VerifySafe();

			foreach (var selection in CurrentSelectionCollection)
				SetItemSelected(selection.Item, false);

			CurrentSelectionCollection.Clear();
			ApplySelectionSafe(Selection<TItem>.Empty);

			CurrentSelectionCollection.IsInverted = false;
		}

		public bool UnselectIndex(int index)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectIndexSafe(index);
		}

		private bool UnselectIndexCore(int index, bool force)
		{
			CurrentSelectionCollection.UnselectIndex(index);

			if (SelectedIndex == index)
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectIndexSafe(-1);
			}

			EnsureSelection();

			return true;
		}

		private bool UnselectIndexSafe(int index)
		{
			VerifySafe();

			return UnselectIndexCore(index, false);
		}

		public bool UnselectItem(TItem item)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectItemSafe(item);
		}

		private bool UnselectItemCore(TItem item, bool force)
		{
			CurrentSelectionCollection.UnselectItem(item);

			if (EqualsItem(CurrentSelectedItem, item))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectItemSafe(null);
			}

			EnsureSelection();

			return true;
		}

		private bool UnselectItemSafe(TItem item)
		{
			VerifySafe();

			return UnselectItemCore(item, false);
		}

		public bool UnselectSource(object source)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectSourceSafe(source);
		}

		public void UnselectSourceCollection(IEnumerable<object> sourceCollection)
		{
			using (SelectionHandlingScope)
			{
				foreach (var source in sourceCollection)
				{
					if (CurrentSelectionCollection.FindBySource(source, out var selection))
					{
						CurrentSelectionCollection.Unselect(selection);

						if (Advisor.TryGetItemBySource(source, false, out var item)) 
							SetItemSelected(item, false);
					}
				}

				SelectFirst();
			}

			EnsureSelection();
		}

		private bool UnselectSourceCore(object source, bool force)
		{
			CurrentSelectionCollection.UnselectSource(source);

			if (EqualsSource(SelectedSource, source))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectSourceSafe(null);
			}

			EnsureSelection();

			return true;
		}

		private bool UnselectSourceSafe(object source)
		{
			VerifySafe();

			return UnselectSourceCore(source, false);
		}

		public bool UnselectValue(object value)
		{
			if (SelectionHandling)
				return false;

			using (SelectionHandlingScope)
				return UnselectValueSafe(value);
		}

		private bool UnselectValueCore(object value, bool force)
		{
			CurrentSelectionCollection.UnselectValue(value);

			if (EqualsValue(SelectedValue, value))
			{
				if (MultipleSelection)
				{
					SelectFirst();

					return true;
				}

				return SelectValueSafe(null);
			}

			EnsureSelection();

			return true;
		}

		private bool UnselectValueSafe(object value)
		{
			VerifySafe();

			return UnselectValueCore(value, false);
		}
	}
}